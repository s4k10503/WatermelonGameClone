using System;
using System.Collections.Generic;
using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public class TitleSceneViewStateData : IDisposable
    {
        public ScoreContainer ScoreContainer { get; set; }
        public IReadOnlyList<License> Licenses { get; set; }
        public float BgmVolume { get; set; }
        public float SeVolume { get; set; }
        public ReactiveProperty<string> UserName { get; set; }
        private readonly CompositeDisposable _disposables = new();

        public TitleSceneViewStateData(
            ScoreContainer scoreContainer,
            IReadOnlyList<License> licenses)
        {
            ScoreContainer = scoreContainer;
            Licenses = licenses;
            UserName = new ReactiveProperty<string>(scoreContainer.Data.Score.UserName);

            UserName.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
