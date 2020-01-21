using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

namespace GoomerFPSController
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class FPSController : MonoBehaviourPunCallbacks
    {
        public static FPSController Instance;

        #region Serialized Variables

        [Header("SpeedConfig")]
        [Range(1f, 25)] [SerializeField] private float WalkForwardSpeed = 4;
        [Range(1f, 25)] [SerializeField] private float StrafeSpeed = 3;
        [Range(1f, 25)] [SerializeField] private float WalkBackwardSpeed = 3;
        [Range(1f,5)] [SerializeField] private float SprintMultiplier = 1.4f;

        [Header("JumpConfig")]
        [Range(1,25)] [SerializeField] private float maxJumpHeight = 7f;
        [Range(1,10)] [SerializeField] private float FallMultiplier = 2.5f;
        [Range(1,10)] [SerializeField] private float LowJumpMultiplier = 2f;

        [Header("LeanConfig")]
        [SerializeField] private bool leanEnabled;
        [Range(1, 200)] [SerializeField] private float LeanSmooth = 35;
        [Range(1, 50)] [SerializeField] private float LeanAmount = 10;

        [Header("CrouchConfig")]
        [Range(.2f, 2f)] [SerializeField] private float CrouchHeight = 1f;
        [Range(.01f, 10f)] [SerializeField] private float CrouchSmooth = 1f;
        [Range(.1f, 15)] [SerializeField] private float CrouchSpeed = .5f;

        [Header("MouseConfig")]
        [Range(1, 1000)] public float MouseSensetivity = 40f;
        [Range(1, 50)] [SerializeField] private float MouseSmooth = 1f;

        [Header("MaskConfig")]
        [SerializeField] private LayerMask GroundMask;

        [Space]
        [Header("Debug/Cache")]
        public Rigidbody playerRigidBody;
        [SerializeField] private bool DrawDebugRays;
        [SerializeField] private Transform LookAtTransform;
        [SerializeField] private Transform CameraPivot;
        [SerializeField] private Transform WeaponHolder;
        [SerializeField] private Animator mainAnimator;
        [SerializeField] private Animator secondaryAnimator;

        #endregion

        #region Private/Debug Variables
        [Space]
        [Header("Private var")]
        public bool IsWalking;
        public bool IsSprinting;
        [SerializeField] private bool IsGrounded;
        public bool MidAir, IsCrouching;
        public bool cursorIsLocked;
        public bool cameraLocked;
        [SerializeField] private float moveSpeedOutput;
        [SerializeField] public Vector2 axisInput;
        [SerializeField] private Vector2 mouseInput;
        [SerializeField] private float sphereSize;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));

        private CapsuleCollider capsuleCollider;
        private Vector3 colliderBottom;
        private Vector3 targetVelocity;
        private Vector3 groundNormal;
        [HideInInspector] public float desiredPitch;
        [HideInInspector] public float desiredYaw;
        private float altYaw;
        private float startColliderHeight;
        private float _leanAmount;

        #endregion

        private void Init()
        {
            if (Instance == null)
                Instance = this;

            GetComponents();
            startColliderHeight = capsuleCollider.height;
        }
        private void GetComponents()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }
        private void OnEnable()
        {
            Init();
        }
        private void Update()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) //checks if we are connected to a photon server and own this photonview
            {
                PlayerSelection.Instance.Cam.enabled = false;
                PlayerSelection.Instance.Cam.gameObject.GetComponent<AudioListener>().enabled = false;
                return;
            }
            
            IsGrounded = ControllerInput.IsGrounded(capsuleCollider, GroundMask);
            MidAir = !IsGrounded;

            InternalLockUpdate();

        }
        private void FixedUpdate()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) //checks if we are connected to a photon server and own this photonview
            {
                return;
            }

            //GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

            GetGroundNormal();
            AltLook();
            axisInput = ControllerInput.GetAxisInputs();
            mouseInput = ControllerInput.GetMouseInputs();
            UpdateDesiredTargetSpeed(axisInput);
            UpdateControllerInputs();
            RotatePlayer();
            PitchCamera();
        }

        private void RotatePlayer() //left and right camera movement, rotates the capsule
        {
            if (Input.GetKey(KeyCode.LeftAlt) || cameraLocked) return;

            desiredYaw += mouseInput.x * MouseSensetivity * Time.unscaledDeltaTime;

            transform.rotation = Quaternion.Euler(new Vector3(0, desiredYaw, 0));
        }
        private void PitchCamera() //up and down camera movement
        {
            if (cameraLocked) return;
            
            desiredPitch -= mouseInput.y * MouseSensetivity * Time.unscaledDeltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, -90, 90);

            PlayerSelection.Instance.Cam.transform.localRotation = Quaternion.Euler(new Vector3(desiredPitch, 0, 0));
        }
        private void Jump(float jumpHeight)
        {
            if (playerRigidBody.velocity.y < 0)
            {
                playerRigidBody.velocity += Vector3.up * (Physics.gravity.y * (FallMultiplier - 1) * Time.deltaTime);
            }
            else if (playerRigidBody.velocity.y > 0 && !Input.GetKeyDown(KeyCode.Space))
            {
                playerRigidBody.velocity += Vector3.up * (Physics.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && !IsCrouching)
            {
                // if(mainAnimator )//move
                //     mainAnimator.SetTrigger("Jump");

                var jumpVelocity = new Vector3(0, jumpHeight, 0);
                jumpVelocity.y = Mathf.Clamp(jumpVelocity.y, -jumpHeight, jumpHeight);

                playerRigidBody.velocity = jumpVelocity;
            }

            CheckWall();
        }
        private void Crouch()
        {
            var isPressingCrouchKey = Input.GetKey(InputManager.Instance.InputKeyManager.CrouchKey);

            if (isPressingCrouchKey)
            {
                capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, CrouchHeight, Time.deltaTime * CrouchSmooth);
                moveSpeedOutput *= CrouchSpeed;
                IsCrouching = true;
            }
            else
            {
                capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, startColliderHeight, Time.deltaTime * CrouchSmooth);
                IsCrouching = false;
            }
        }
        private void AltLook()
        {
            var origYRot = 0f;

            if (Input.GetKey(KeyCode.LeftAlt) && !cameraLocked)
            {
                altYaw += mouseInput.x * MouseSensetivity * Time.fixedDeltaTime;
                altYaw = Mathf.Clamp(altYaw, -110, 110);
                CameraPivot.localRotation = Quaternion.Euler(new Vector3(0, altYaw, 0));
                WeaponHolder.localRotation = Quaternion.Euler(new Vector3(0, -altYaw, 0));
            }
            else
            {
                WeaponHolder.localRotation = Quaternion.Euler(new Vector3(0, origYRot, 0));
                CameraPivot.localRotation = Quaternion.Euler(new Vector3(0, origYRot, 0));
                altYaw = origYRot; //find a way to lerp 
            }
        }
        private void Lean(float leanAmount, float leanSmooth)
        {
            if (!leanEnabled) return;

            _leanAmount = Mathf.Clamp(_leanAmount, -leanAmount, leanAmount);

            if (Input.GetKey(InputManager.Instance.InputKeyManager.LeanLeftKey)) //left lean
            {
                _leanAmount += Time.deltaTime * leanSmooth;
                CameraPivot.localRotation *= Quaternion.Euler(new Vector3(0, 0, _leanAmount));
            }
            else if (Input.GetKey(InputManager.Instance.InputKeyManager.LeanRightKey)) //right lean
            {
                _leanAmount -= Time.deltaTime * leanSmooth;
                CameraPivot.localRotation *= Quaternion.Euler(new Vector3(0, 0, _leanAmount));
            }
            else
            {
                _leanAmount = Mathf.Lerp(_leanAmount, 0 , Time.deltaTime * leanSmooth);
                CameraPivot.localRotation = Quaternion.Euler(new Vector3(0,0, _leanAmount));
            }
        }

        private void UpdateDesiredTargetSpeed(Vector2 input)
        {
            IsSprinting = false;

            if (input == Vector2.zero) return;

            moveSpeedOutput = Mathf.Clamp(moveSpeedOutput, 0, 10);

            if (input.x > 0 || input.x < 0)
            {
                //strafe
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, StrafeSpeed, 1);
            }
            else if (input.y < 0)
            {
                //backwards
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, WalkBackwardSpeed, 1);
            }
            else if (input.y > 0 && input.x == 0)
            {
                //forwards
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, WalkForwardSpeed, 1);
            }
            else
            {
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, 0, Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftShift) && !IsCrouching)
            {
                moveSpeedOutput *= SprintMultiplier;
                IsSprinting = true;
            }
        }
        private void UpdateControllerInputs()
        {
            Lean(LeanAmount, LeanSmooth);
            Jump(maxJumpHeight);
            Crouch();

            IsWalking = ControllerInput.WalkOutput();
            if (IsSprinting || IsCrouching) IsWalking = false;

            targetVelocity = new Vector3(axisInput.x * moveSpeedOutput, playerRigidBody.velocity.y, axisInput.y * moveSpeedOutput);
            targetVelocity = transform.TransformDirection(targetVelocity);

            playerRigidBody.velocity = targetVelocity;

            if (mainAnimator) //move
            {
                mainAnimator.SetFloat("Horizontal", axisInput.x * moveSpeedOutput);
                mainAnimator.SetFloat("Vertical", axisInput.y * moveSpeedOutput);
                if (secondaryAnimator)
                {
                    secondaryAnimator.SetFloat("Horizontal", axisInput.x * moveSpeedOutput); //for the body legs
                    secondaryAnimator.SetFloat("Vertical", axisInput.y * moveSpeedOutput);
                }

                mainAnimator.GetBoneTransform(HumanBodyBones.Head).LookAt(LookAtTransform); //controls the head look
            }
        }
        private void CheckWall()
        {
            var horizontalMove = playerRigidBody.velocity;
            horizontalMove.y = 0;
            var distance = horizontalMove.magnitude;
            horizontalMove.Normalize();

            if (playerRigidBody.SweepTest(horizontalMove, out var hit, distance))
            {
                Debug.DrawLine(transform.position, hit.point, Color.yellow);
            }
        }
        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(groundNormal, Vector3.up);
            return SlopeCurveModifier.Evaluate(angle);
        }
        private void GetGroundNormal()
        {
            Debug.DrawRay(colliderBottom + new Vector3(0, 0.1f, 0), Vector3.down, Color.red);

            groundNormal = Physics.Raycast(colliderBottom + new Vector3(0, 0.1f, 0), Vector3.down, out var hitInfo, 0.2f, GroundMask) ? hitInfo.normal : Vector3.up;
        }

        #region Debug

        private void OnDrawGizmos()
        {
            if (!DrawDebugRays || !capsuleCollider) return;

            var groundCheckRayDir = new Vector3(0, -0.05f, 0);
            var bounds = capsuleCollider.bounds;
            colliderBottom = IsCrouching ? bounds.center - new Vector3(0, .5f, 0) : bounds.center - new Vector3(0, 0.999f, 0);

            //axis input debug ray at the bottom of collider
            Debug.DrawRay(colliderBottom, targetVelocity, Color.blue);

            //ground check ray at the bottom of collider
            if (IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(colliderBottom, 0.2f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(colliderBottom, 0.2f);
            }
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                cursorIsLocked = false;
                cameraLocked = true;
            }
            else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                cursorIsLocked = true;
                cameraLocked = false;
            }

            if (cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        #endregion
    }
}