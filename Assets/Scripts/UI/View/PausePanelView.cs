using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class PausePanelView : MonoBehaviour, IPausePanelView
    {
        // Button
        [SerializeField] Button _buttonBackToTitle;
        [SerializeField] Button _buttonRestart;
        [SerializeField] Button _buttonBackToGame;

        // Objects
        private Transform _canvasTransform;

        // Subjects
        private Subject<Unit> _onRestart = new Subject<Unit>();
        private Subject<Unit> _onBackToTitle = new Subject<Unit>();
        private Subject<Unit> _onBackToGame = new Subject<Unit>();

        // Observables
        public IObservable<Unit> OnRestart => _buttonRestart.OnClickAsObservable();
        public IObservable<Unit> OnBackToTitle => _buttonBackToTitle.OnClickAsObservable();
        public IObservable<Unit> OnBackToGame => _buttonBackToGame.OnClickAsObservable();

        private IUIAnimator _uiAnimator;


        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,
            [Inject(Id = "CanvasTransform")] Transform canvasTransform)
        {
            _uiAnimator = uiAnimator;
            _canvasTransform = canvasTransform;

            _onRestart.AddTo(this);
            _onBackToTitle.AddTo(this);
            _onBackToGame.AddTo(this);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {

        }

        public void ShowPanel()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
