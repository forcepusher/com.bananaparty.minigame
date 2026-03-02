using UnityEngine;

namespace BananaParty.Minigame.Sample
{
    public class PlaceholderGameCanvas : MonoBehaviour
    {
        private ClickerMinigame _clickerMinigame;

        public void OnStartGameButtonClick()
        {
            _clickerMinigame = new ClickerMinigame();
            _clickerMinigame.StartMinigame(Camera.main);
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
