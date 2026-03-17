using UnityEngine;

namespace BananaParty.Minigame
{
    public interface IMinigame<TPlayResult>
    {
        public MinigameAsyncOperation StartMinigame();

        public bool IsMinigameFinished { get; }

        public MinigameAsyncOperation StopMinigame();

        public TPlayResult MinigamePlayResult { get; }

        /// <summary>
        /// Use this layer as your default minigame layer to avoid conflicts with the main game scene.<br/>
        /// Camera and Light Culling Masks must be enabled only for <see cref="MainMinigameLayer"/> and <see cref="AdditionalMinigameLayer"/> exclusively.
        /// </summary>
        public int MainMinigameLayer { get => 30; }

        /// <summary>
        /// Use this layer for your custom minigame logic that might need an extra layer.
        /// </summary>
        public int AdditionalMinigameLayer { get => 31; }

        /// <summary>
        /// Change the minigame language.
        /// </summary>
        /// <param name="languageCode">ISO 639 Set 1 language code (e.g. "en", "ru", "tr")</param>
        public void SetLanguage(string languageCode) {}

        /// <summary>
        /// Sets master volume of the entire minigame.
        /// </summary>
        public void SetSoundVolume(float volume) {}
    }
}
