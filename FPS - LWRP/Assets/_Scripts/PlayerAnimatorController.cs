using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoomerFPSController
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        public Animator animator;
        public FPSController fpsController;

        private void Update()
        {
            animator.SetFloat("Horizontal", fpsController.axisInput.x);
            animator.SetFloat("Vertical", fpsController.axisInput.y);
            
        }
    }
}