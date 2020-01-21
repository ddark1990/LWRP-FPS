using UnityEngine;
using GoomerFPSController;

public class WeaponSway : MonoBehaviour
{
    #region Variables
    [Header("MovementSway")]
    public bool ifMoveSway;
    [Range(0, 1f)]
    [SerializeField] private float verticalSwayAmount = 0.5f;
    [Range(0, 1f)]
    [SerializeField]private float horiztonalSwayAmount = 1f;
    [Range(0, 15f)]
    [SerializeField]private float swaySpeed = 4f;
    public float rotateGunForward = 15f;
    public float moveFracSpeed = 1f;

    [Header("MouseSway")]
    public bool ifMouseSway;
    public float mosueSwaySpeed;
    public float maxMouseSwayAmount;
    public float smoothAmount;
    [Range(0, 15f)] public float mouseFracSpeed = 1f;

    [Header("JumpSway")]
    public bool enableJumpSway;
    public float jumpFracSpeed = 1f;
    public float jumpSwaySpeed = 2f;
    public AnimationCurve jumpSwayCurve;

    [Space]

    [HideInInspector] public Vector3 origPos;
    [HideInInspector] public Vector3 origRot;
    private Vector3 HandleRighttorigRot;
    private Transform HandleRight;

    [Header("Cache")]
    [SerializeField] private WeaponManager WeaponManager;
    #endregion

    private void OnEnable()
    {
        SetSwayTransforms();
    }
    private void OnDisable()
    {
        ResetSwayTransforms();
    }
    private void FixedUpdate()
    {
        JumpSway(WeaponManager.ActiveRig);
        MouseSway(HandleRight);
        MovementSway(WeaponManager.ActiveRig);
    }

    private void MouseSway(Transform handle)
    {
        if (!handle) return; //return if no handle on rig

        float mouseFracComplete = Time.deltaTime * mouseFracSpeed;

        float mouseX = -Input.GetAxis("Mouse X") * mosueSwaySpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mosueSwaySpeed;
        mouseX = Mathf.Clamp(mouseX, -maxMouseSwayAmount, maxMouseSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxMouseSwayAmount, maxMouseSwayAmount);

        if (ifMouseSway && !InventoryManager.Instance.MenuOpen)
        {
            if (WeaponManager.lookingDownSights) maxMouseSwayAmount = .5f; //controls the speed when looking down sights, seperate into a function which controls the sways based on players movement state
            else maxMouseSwayAmount = 4;

            if (mouseX > 0 || mouseX < 0 || mouseY > 0 || mouseY < 0) handle.localRotation = Quaternion.Slerp(handle.localRotation, Quaternion.Euler(HandleRighttorigRot.x - mouseY, HandleRighttorigRot.y + mouseX, HandleRighttorigRot.z), mouseFracComplete);
            else handle.localRotation = Quaternion.Slerp(handle.localRotation, Quaternion.Euler(HandleRighttorigRot), mouseFracComplete);
        }
    }
    private void MovementSway(Transform activeRig)
    {
        float x = 0, y = 0;
        float moveFracComplete = Time.deltaTime * moveFracSpeed;

        if (ifMoveSway && !WeaponManager.lookingDownSights && !FPSController.Instance.MidAir)
        {
            y -= verticalSwayAmount * Mathf.Sin((swaySpeed * 2) * Time.time);
            x += horiztonalSwayAmount * Mathf.Sin(swaySpeed * Time.time);

            if (FPSController.Instance.IsWalking) //movement sway
            {
                activeRig.localPosition = new Vector3(activeRig.localPosition.x + x, activeRig.localPosition.y + y, activeRig.localPosition.z);
            }
            else if (FPSController.Instance.IsSprinting)
            {
                activeRig.localPosition = new Vector3(activeRig.localPosition.x + x, activeRig.localPosition.y + y, activeRig.localPosition.z);
                activeRig.localRotation = Quaternion.Slerp(activeRig.localRotation, Quaternion.Euler(activeRig.localRotation.x + rotateGunForward, origRot.y, origRot.z), moveFracComplete);
            }
            else
            {
                ResetRigTransform(moveFracComplete, activeRig);
            }
        }
    }
    private void JumpSway(Transform activeRig)
    {
        if (!FPSController.Instance.MidAir) return;

        var jumpVelocityNormalized = FPSController.Instance.playerRigidBody.velocity.normalized.y;
        var jumpFracComplete = Time.deltaTime * jumpFracSpeed;

        if (enableJumpSway)
        {            
            var f = jumpVelocityNormalized / 10 * jumpSwayCurve.Evaluate((jumpSwaySpeed * 2) * Time.time);
            var b = new Vector3(activeRig.localPosition.x, activeRig.localPosition.y + f, activeRig.localPosition.z);

            if (jumpVelocityNormalized > 0 || jumpVelocityNormalized < 0 && !FPSController.Instance.IsCrouching)
            {
                activeRig.localPosition = b;
            }
        }
    }

    public void SetSwayTransforms()
    {
        var rigData = WeaponManager.ActiveRig.GetComponent<RigData>();

        origPos = rigData.OrigPosRef.localPosition;
        origRot = rigData.OrigPosRef.localRotation.eulerAngles;

        HandleRight = WeaponManager.ActiveRig.GetComponent<RigData>().RightHandSwayHandle;
        HandleRighttorigRot = HandleRight.transform.localRotation.eulerAngles;

        Debug.Log("Setting Transforms");
    }
    public void ResetSwayTransforms()
    {
        origPos = Vector3.zero;
        origRot = Vector3.zero;

        HandleRight = null;
        HandleRighttorigRot = Vector3.zero;

        Debug.Log("Resetting Transforms");
    }
    private void ResetRigTransform(float fracComplete, Transform activeRig)
    {
        activeRig.localPosition = Vector3.Slerp(activeRig.localPosition, origPos, fracComplete);
        activeRig.localRotation = Quaternion.Slerp(activeRig.localRotation, Quaternion.Euler(origRot.x, origRot.y, origRot.z), fracComplete);
    }
}