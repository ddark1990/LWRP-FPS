using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoomerFPSController
{
    public static class ControllerInput
    {
        public static float accelerationFloat;
        public static bool walkOutput, sprintOutput;

        public static Vector2 GetAxisInputs()
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        public static Vector2 GetMouseInputs()
        {
            return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        public static bool WalkOutput()
        {
            return walkOutput = Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 ||
                                Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0;
        }

        public static bool IsGrounded(CapsuleCollider capsuleCollider, LayerMask groundMask)
        {
            var bounds = capsuleCollider.bounds;
            var colliderBottom = FPSController.Instance.IsCrouching ? bounds.center - new Vector3(0, .5f, 0) : bounds.center - new Vector3(0, 1f, 0);
            
            //anim.applyRootMotion = Physics.CheckSphere(colliderBottom, 0.25f, groundMask);
            
            // if(anim) //move
            //     anim.SetBool("MidAir" ,midAir);

            return Physics.CheckSphere(colliderBottom, 0.2f, groundMask);
        }

    }
}