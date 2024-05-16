using System;
using UniRx;
using UnityEngine;

namespace WatermelonGameClone
{
    public interface IPausePanelView
    {
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }
        IObservable<Unit> OnBackToGame { get; }

        void ShowPanel();
        void HidePanel();
    }
}