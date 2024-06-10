using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using Zenject;

namespace WatermelonGameClone
{
    public class TitleScenePresenter : IInitializable, IDisposable
    {
        // View Elements
        private ITitleSceneView _titleSceneView;

        private CompositeDisposable _disposables;

        [Inject]
        public void Construct(ITitleSceneView titleSceneView)
        {
            _titleSceneView = titleSceneView;
            _disposables = new CompositeDisposable();
        }

        void IInitializable.Initialize()
        {
            SubscribeToGameView(_titleSceneView);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SubscribeToGameView(ITitleSceneView titleSceneView)
        {
            titleSceneView
                .GameStartRequested
                .Subscribe(_ =>
                {
                    HandleGameStart();
                })
                .AddTo(_disposables);
        }

        private void HandleGameStart()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
