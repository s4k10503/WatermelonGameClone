using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class TitleSceneView : MonoBehaviour, ITitleSceneView
    {
        public ITitlePanelView TitlePanellView { get; private set; }

        // Observables
        public IObservable<Unit> GameStartRequested => TitlePanellView.OnGameStart;

        [Inject]
        public void Construct(ITitlePanelView titlePanellView)
        {
            TitlePanellView = titlePanellView;
        }

        private void OnDestroy()
        {

        }
    }
}
