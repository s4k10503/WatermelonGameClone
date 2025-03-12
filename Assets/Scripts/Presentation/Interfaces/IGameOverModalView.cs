using Presentation.DTO;

using System;
using UniRx;
using UnityEngine;

namespace Presentation.Interfaces
{
    public interface IGameOverModalView
    {
        // Observables for UI events
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }
        IObservable<Unit> OnDisplayScore { get; }

        // Methods related to game state and UI updates
        void ShowModal(int score, RenderTexture screenShot, ScoreContainerDto scoreContainer);
        void ShowModalWithoutData();
        void HideModal();
    }
}
