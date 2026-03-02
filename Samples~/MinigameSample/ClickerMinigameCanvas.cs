using UnityEngine;

namespace BananaParty.Minigame.Sample
{
    public class ClickerMinigameCanvas : MonoBehaviour
    {
        private const int ClicksToWin = 5;

        public int ClickCount { get; private set; } = 0;
        public bool IsGameFinished { get; private set; } = false;

        public void OnClickerButtonClick()
        {
            ClickCount += 1;

            if (ClickCount >= ClicksToWin)
                IsGameFinished = true;
        }
    }
}
