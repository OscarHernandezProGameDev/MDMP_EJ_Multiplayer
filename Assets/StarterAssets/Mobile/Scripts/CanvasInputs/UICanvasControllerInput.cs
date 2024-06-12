using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {
        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        [SerializeField] private bool leaderboardStatus = false;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualAimInput(bool virtualSprintState)
        {
            starterAssetsInputs.AimInput(virtualSprintState);
        }

        public void VirtualFirePrimaryInput(bool virtualSprintState)
        {
            starterAssetsInputs.FirePrimaryInput(virtualSprintState);
        }

        public void VirtualLeaderboardInput(bool virtualSprintState)
        {
            leaderboardStatus = !leaderboardStatus;
            starterAssetsInputs.LeaderboardInput(virtualSprintState);
        }
    }

}
