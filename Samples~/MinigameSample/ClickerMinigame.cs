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
        private Scene? _minigameScene = null;

        public bool IsMinigameFinished => _clickerMinigameCanvas ? _clickerMinigameCanvas.IsGameFinished : false;

        public int MinigamePlayResult => _clickerMinigameCanvas ? _clickerMinigameCanvas.ClickCount : 0;

        public MinigameAsyncOperation StartMinigame()
        {
            MinigameAsyncOperation startAsyncOperation = new();
            StartMinigameAsync(startAsyncOperation);
            return startAsyncOperation;
        }

        private async void StartMinigameAsync(MinigameAsyncOperation startAsyncOperation)
        {
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            
            while (!loadingOperation.isDone)
                await Task.Yield();
        
            // Custom startup code should be here if needed
            _clickerMinigameCanvas = Object.FindAnyObjectByType<ClickerMinigameCanvas>();
            //

            _minigameScene = SceneManager.GetSceneByName(SceneName);

            SetSoundVolume(_volume);
            SetLanguage(_languageCode);

            startAsyncOperation.Complete();
        }

        public MinigameAsyncOperation StopMinigame()
        {
            MinigameAsyncOperation stopAsyncOperation = new();
            StopMinigameAsync(stopAsyncOperation);
            return stopAsyncOperation;
        }

        private async void StopMinigameAsync(MinigameAsyncOperation stopAsyncOperation)
        {
            AsyncOperation unloadingOperation = SceneManager.UnloadSceneAsync(SceneName);
            _minigameScene = null;
            
            // Custom cleanup code should be here if needed

            //
            
            while (!unloadingOperation.isDone)
                await Task.Yield();
            
            stopAsyncOperation.Complete();
        }

        public void SetSoundVolume(float volume)
        {
            _volume = volume;

            if (_minigameScene == null)
                return;

            foreach (GameObject rootGameObject in _minigameScene.Value.GetRootGameObjects())
                foreach (AudioSource audioSource in rootGameObject.GetComponentsInChildren<AudioSource>(true))
                    audioSource.volume = volume;
        }

        public void SetLanguage(string languageCode)
        {
            _languageCode = languageCode;

            if (_minigameScene == null)
                return;

            _clickerMinigameCanvas.SetLanguage(_languageCode);
        }
    }
}
