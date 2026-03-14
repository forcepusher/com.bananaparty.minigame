using UnityEngine;

namespace BananaParty.Minigame.Sample
{
    public class PlaceholderGameCanvas : MonoBehaviour
    {
        private ClickerMinigame _clickerMinigame;

        public void OnStartGameButtonClick()
        {
            _clickerMinigame = new ClickerMinigame();
            _clickerMinigame.SetLanguage("ru");
            _clickerMinigame.SetSoundVolume(0.25f);
            _clickerMinigame.StartMinigame();

            // Volume should be able to change while the game is running
            _clickerMinigame.SetSoundVolume(0.75f);
        }

        public void OnEndGameButtonClick()
        {
            if (_clickerMinigame == null)
                return;

            Debug.Log("Minigame result = " + _clickerMinigame.MinigamePlayResult);

            _clickerMinigame.EndMinigame();
            _clickerMinigame = null;
        }

        public void Update()
        {
            if (_clickerMinigame == null)
                return;

            if (_clickerMinigame.IsMinigameFinished)
            {
                Debug.Log("Minigame result = " + _clickerMinigame.MinigamePlayResult);

                _clickerMinigame.EndMinigame();
                _clickerMinigame = null;
            }
        }
    }
}
