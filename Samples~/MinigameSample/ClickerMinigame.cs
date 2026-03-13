using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BananaParty.Minigame.Sample
{
    public class ClickerMinigame : IMinigame<int>
    {
        private const string SceneName = "ClickerMinigameScene";

        private ClickerMinigameCanvas _clickerMinigameCanvas;

        public bool IsMinigameFinished => _clickerMinigameCanvas ? _clickerMinigameCanvas.IsGameFinished : false;

        public int MinigamePlayResult => _clickerMinigameCanvas ? _clickerMinigameCanvas.ClickCount : 0;

        public AsyncOperation StartMinigame()
        {
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            GatherSceneReferencesAfterLoad(loadingOperation);
            return loadingOperation;
        }

        private async void GatherSceneReferencesAfterLoad(AsyncOperation loadingOperation)
        {
            while (!loadingOperation.isDone)
                await Task.Yield();

            _clickerMinigameCanvas = Object.FindAnyObjectByType<ClickerMinigameCanvas>();
        }

        public AsyncOperation EndMinigame()
        {
            return SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}
