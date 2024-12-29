using System;
using UnityEngine;
using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public interface IGameOverModalView
    {
        // Observables for UI events
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }
        IObservable<Unit> OnDisplayScore { get; }

        // Methods related to game state and UI updates
        void ShowPanel(int score, RenderTexture screenShot, ScoreContainer scoreContainer);
        void ShowPanelWihtoutData();
        void HidePanel();
    }
}
