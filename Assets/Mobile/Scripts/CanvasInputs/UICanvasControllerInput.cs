using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public PlayerCharacterInputs playerCharacterInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            playerCharacterInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            playerCharacterInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            playerCharacterInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            playerCharacterInputs.SprintInput(virtualSprintState);
        }
        
    }

}
