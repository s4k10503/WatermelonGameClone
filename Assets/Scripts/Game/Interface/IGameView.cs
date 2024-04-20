using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IGameView
    {
        // Observables for UI events
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }

        // Methods related to game state and UI updates
        void UpdateCurrentScore(int currentScore);
        void UpdateBestScore(int bestScore);
        void UpdateNextSphereImages(int nextSphereIndex);
        void MoveUI();
        void ShowGameOverPopup(int score);
        void DisplayTodayTopScores(List<int> scores);
    }
}
