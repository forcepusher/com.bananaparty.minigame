using UnityEngine;

namespace BananaParty.Minigame
{
    public interface IMinigame<TPlayResult>
    {
        public AsyncOperation StartMinigame(Camera mainSceneCamera);

        public AsyncOperation EndMinigame();

        public bool IsMinigameFinished { get; }

        public TPlayResult MinigamePlayResult { get; }
    }
}
