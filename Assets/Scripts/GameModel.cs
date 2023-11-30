using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public class GameModel
    {
        public enum GameState
        {
            Initializing,
            SphereMoving,
            SphereDropping,
            CheckingCollision,
            Merging,
            GameOver
        }

        public enum SoundEffect
        {
            Drop,
            Merge
        }

        private ReactiveProperty<GameState> _currentState = new ReactiveProperty<GameState>(GameState.Initializing);
        private ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        private ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);

        private Dictionary<SoundEffect, AudioClip> soundEffects = new Dictionary<SoundEffect, AudioClip>();

        private static readonly int s_scoreCoefficient = 10;
        private float _soundVolume = 1.0f;

        public ReactiveProperty<GameState> CurrentState
        {
            get { return _currentState; }
            private set { _currentState = value; }
        }

        public ReactiveProperty<int> CurrentScore
        {
            get { return _currentScore; }
            private set { _currentScore = value; }
        }

        public ReactiveProperty<int> BestScore
        {
            get { return _bestScore; }
            private set { _bestScore = value; }
        }

        public void ChangeState(GameState newState)
        {
            CurrentState.Value = newState;
        }

        public void CalcScore(int sphereNo)
        {
            int scoreToAdd = (sphereNo + 1) * s_scoreCoefficient;
            CurrentScore.Value += scoreToAdd;
        }

        public void SaveBestScore(int currentScore)
        {
            BestScore.Value = currentScore;
            PlayerPrefs.SetInt("BestScore", BestScore.Value);
        }

        public void SetSoundEffect()
        {
            LoadSoundEffect(SoundEffect.Drop, "Drop");
            LoadSoundEffect(SoundEffect.Merge, "Merge");
        }

        public void SetSoundVolume(float volume)
        {
            _soundVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        }

        private void LoadSoundEffect(SoundEffect effect, string resourcePath)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip != null)
            {
                soundEffects[effect] = clip;
            }
            else
            {
                Debug.LogError("Sound effect not found at path: " + resourcePath);
            }
        }

        public void PlaySoundEffect(SoundEffect effect, AudioSource source)
        {
            if (soundEffects.TryGetValue(effect, out AudioClip clip))
            {
                source.clip = clip;
                source.volume = _soundVolume;
                source.Play();
            }
        }
    }
}
