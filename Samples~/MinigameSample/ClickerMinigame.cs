using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BananaParty.Minigame.Sample
{
    public class ClickerMinigame : IMinigame<int>
    {
        private const string SceneName = "ClickerMinigameScene";

        private string _languageCode = "en";
        private float _volume = 1f;

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
            SetSoundVolume(_volume);
            _clickerMinigameCanvas.SetLanguage(_languageCode);
        }

        public AsyncOperation EndMinigame()
        {
            return SceneManager.UnloadSceneAsync(SceneName);
        }

        public void SetSoundVolume(float volume)
        {
            _volume = volume;

            foreach (AudioSource audioSource in Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
                audioSource.volume = volume;
        }

        public void SetLanguage(string languageCode)
        {
            _languageCode = languageCode;
        }
    }
}
