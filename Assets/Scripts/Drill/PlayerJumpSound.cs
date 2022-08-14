using Licht.Impl.Events;
using Licht.Unity.CharacterControllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Drill
{
    public class PlayerJumpSound: MonoBehaviour
    {
        public AudioSource JumpSound;
        private void OnEnable()
        {
            this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>
                (LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnJumpStart);
        }

        private void OnJumpStart(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
        {
            JumpSound.pitch = 0.95f + Random.value * 0.1f;
            JumpSound.Play();
        }

        private void OnDisable()
        {
            this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>
                (LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnJumpStart);
        }
    }
}
