using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace WatermelonGameClone
{
    public class TitlePanelView : MonoBehaviour, ITitlePanelView
    {
        // Button
        [SerializeField] Button _buttonGameStart;

        // Observables
        public IObservable<Unit> OnGameStart => _buttonGameStart.OnClickAsObservable();

    }
}
