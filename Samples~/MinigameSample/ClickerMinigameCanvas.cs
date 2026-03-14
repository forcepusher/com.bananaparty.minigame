using UnityEngine;
using UnityEngine.UI;

namespace BananaParty.Minigame.Sample
{
    public class ClickerMinigameCanvas : MonoBehaviour
    {
        private const int ClicksToWin = 5;

        [SerializeField]
        public Text _buttonText;

        public int ClickCount { get; private set; } = 0;
        public bool IsGameFinished { get; private set; } = false;

        public void OnClickerButtonClick()
        {
            ClickCount += 1;

            if (ClickCount >= ClicksToWin)
                IsGameFinished = true;
        }

        public void SetLanguage(string languageCode)
        {
            _buttonText.text = languageCode switch
            {
                "ru" => "Кликни 5 раз",
                "tr" => "5 kere tıkla",
                _ => "Click 5 times",
            };
        }
    }
}
