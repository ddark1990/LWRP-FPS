using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[Header("Character Settings")]
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		//[SerializeField] float m_JumpPower = 12f;
		//[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;
		[Header("Animator Settings")]
		//[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] private float m_MoveSpeedMultiplier = 1f;
		[SerializeField] private float m_AnimSpeedMultiplier = 1f;
		[SerializeField] private float animationBlendDamp = .5f;
		[Header("Walk Back Animation Settings")]
		[Tooltip("")]
		[SerializeField] private float backThreshold = .05f;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		//const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;
        NavMeshAgent agent;
        private AiStateController _stateController;
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int VerticalDamp = Animator.StringToHash("VerticalDamp");
        private static readonly int TurnDamp = Animator.StringToHash("TurnDamp");
        private static readonly int AgentVelocity = Animator.StringToHash("AgentVelocity");
        
        private Animator m_LastAnimatorCache; 
        private readonly Dictionary<string,int> m_AnimatorParamCache = new Dictionary<string,int>( ); 
        private int _hash;
        void Start()
		{
            agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			_stateController = GetComponent<AiStateController>();
			//m_Capsule = GetComponent<CapsuleCollider>();
			//m_CapsuleHeight = m_Capsule.height;
			//m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}

        private void Update()
        {
            SetRootMotion();
            //Debug.Log("Apply | Has RootMotion: " + m_Animator.applyRootMotion + " | " + m_Animator.hasRootMotion);
        }

        /*private void SetRootMotion()
        {
            if (!agent || !SetRoot) return;
            
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                Move(agent.desiredVelocity, false, false);
            }
            else
            {
                Move(Vector3.zero ,false,false);
            }
        }*/
        private void SetRootMotion()
        {
	        //if (!agent || !SetRoot) return;

	        Move(agent.remainingDistance > agent.stoppingDistance ? agent.desiredVelocity : Vector3.zero, false, false);
        }
        

        public void Move(Vector3 move, bool crouch, bool jump)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			
			////checks weather the transform is directly behind its destination vector, and if it is, set the horizontal animator float to 0
			if (_stateController.target)
			{
				var destinationForward = (agent.destination - _stateController.target.position).normalized;
				var negativeForward = -transform.forward.normalized;
        
				var destX = Mathf.Round(destinationForward.x * 10f) / 10f;
				var negForwardX = Mathf.Round(negativeForward.x * 10f) / 10f;
				var destZ = Mathf.Round(destinationForward.z * 10f) / 10f;
				var negForwardZ = Mathf.Round(negativeForward.z * 10f) / 10f;

				var minX = 0f;
				var maxX = 0f;
				var minZ = 0f;
				var maxZ = 0f;
			
				if (destX > 0)
				{
					minX = destX - backThreshold;
					maxX = destX + backThreshold;
				}
				else
				{
					minX = destX + backThreshold;
					maxX = destX - backThreshold;
				}
			
				if (destZ > 0)
				{
					minZ = destZ - backThreshold;
					maxZ = destZ + backThreshold;
				}
				else
				{
					minZ = destZ + backThreshold;
					maxZ = destZ - backThreshold;
				}
        
				if (_stateController.hasTargetFocus &&  backThreshold > 0 && _stateController && AiStateController.IsBetween(negForwardX, minX, maxX) && AiStateController.IsBetween(negForwardZ, minZ, maxZ) /*Mathf.Approximately(negForwardX, destX) && Mathf.Approximately(negForwardZ, destZ)*/)
				{
					m_TurnAmount = 0;
					m_ForwardAmount = -1;
					Debug.Log("TURNED BUTTERS");
				}
				else m_TurnAmount = Mathf.Atan2(move.x, move.z);

				Debug.DrawRay(m_Animator.rootPosition + Vector3.up, destinationForward);
				Debug.DrawRay(m_Animator.rootPosition + new Vector3(0,.5f,0), -transform.forward, Color.red);
			}
			////
			
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			//if (m_IsGrounded)
			//{
			//	HandleGroundedMovement(crouch, jump);
			//}
			//else
			//{
			//	HandleAirborneMovement();
			//}

			//ScaleCapsuleForCrouching(crouch);
			//PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}


		/*
		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * (m_Capsule.radius * k_Half), Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}
		*/
		
		private bool TryGetAnimatorParam( Animator animator, string paramName, out int hash ) //caches and resolves the params with no GC allocation per param
		{
			if( (m_LastAnimatorCache == null || m_LastAnimatorCache != animator) && animator != null ) // Rebuild cache
			{
				m_LastAnimatorCache = animator;
				m_AnimatorParamCache.Clear( );
				foreach( var param in animator.parameters )
				{
					int paramHash = Animator.StringToHash( param.name ); // could use param.nameHash property but this is clearer
					m_AnimatorParamCache.Add( param.name, paramHash );
				}
			}

			if( m_AnimatorParamCache != null && m_AnimatorParamCache.TryGetValue( paramName, out hash ) )
			{
				return true;
			}
			else
			{
				hash = 0;
				return false;
			}
		}

		private void UpdateAnimator(Vector3 move)
		{
			if( TryGetAnimatorParam( m_Animator, "Vertical", out _hash ) )
			{
				m_Animator.SetFloat(Vertical, m_ForwardAmount);
			}
			if( TryGetAnimatorParam( m_Animator, "Turn", out _hash ) )
			{
				m_Animator.SetFloat(Turn, m_TurnAmount);
			}
			if( TryGetAnimatorParam( m_Animator, "VerticalDamp", out _hash ) )
			{
				m_Animator.SetFloat(VerticalDamp, m_ForwardAmount, animationBlendDamp, Time.deltaTime);
			}
			if( TryGetAnimatorParam( m_Animator, "TurnDamp", out _hash ) )
			{
				m_Animator.SetFloat(TurnDamp, m_TurnAmount, animationBlendDamp, Time.deltaTime);
			}
			if( TryGetAnimatorParam( m_Animator, "AgentVelocity", out _hash ) )
			{
				m_Animator.SetFloat(AgentVelocity, agent.velocity.magnitude, 0.1f, Time.deltaTime);
			}

			// update the animator parameters
			//m_Animator.SetBool("Crouch", m_Crouching);
			//m_Animator.SetBool("OnGround", m_IsGrounded);
			//if (!m_IsGrounded)
			//{
			//	m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			//}

			//// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			//// (This code is reliant on the specific run cycle offset in our animations,
			//// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			//float runCycle =
			//	Mathf.Repeat(
			//		m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			//float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			//if (m_IsGrounded)
			//{
			//	m_Animator.SetFloat("JumpLeg", jumpLeg);
			//}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		// void HandleAirborneMovement()
		// {
		// 	// apply extra gravity from multiplier:
		// 	Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		// 	m_Rigidbody.AddForce(extraGravityForce);
		//
		// 	m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		// }


		/*
		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}
		*/

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            //if (m_IsGrounded && Time.deltaTime > 0)
            //{
            //	Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            //	// we preserve the existing y part of the current velocity.
            //	v.y = m_Rigidbody.velocity.y;
            //	m_Rigidbody.velocity = v;
            //}
            agent.velocity = m_Animator.deltaPosition * m_MoveSpeedMultiplier / Time.deltaTime;
        }


		private void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
