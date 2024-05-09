using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IGameOverPanelView
    {
        // Observables for UI events
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }

        // Methods related to game state and UI updates
        void ShowGameOverPopup(int score);
    }
}
