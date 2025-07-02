using System;
using System.Collections.Generic;
using UseCase.DTO;
using UniRx;

namespace Presentation.DTO
{
    public class TitleSceneViewStateData : IDisposable
    {
        public ScoreDataDto? ScoreContainer { get; set; }
        public IReadOnlyList<LicenseDto> Licenses { get; set; }
        public float BgmVolume { get; set; }
        public float SeVolume { get; set; }
        public ReactiveProperty<string> UserName { get; set; }
        private readonly CompositeDisposable _disposables = new();

        public TitleSceneViewStateData(
            ScoreDataDto? scoreContainer,
            IReadOnlyList<LicenseDto> licenses)
        {
            ScoreContainer = scoreContainer;
            Licenses = licenses;
            UserName = new ReactiveProperty<string>(scoreContainer?.Score.UserName ?? string.Empty);

            UserName.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
