using UnityEngine;

namespace BananaParty.Minigame
{
    public class MinigameAsyncOperation : CustomYieldInstruction
    {
        public bool IsComplete { get; private set; } = false;

        public override bool keepWaiting => !IsComplete;

        public void Complete()
        {
            IsComplete = true;
        }
    }
}
