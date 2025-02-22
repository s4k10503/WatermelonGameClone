using Domain.Interfaces;
using Domain.Services;
using UseCase.Interfaces;
using UseCase.UseCases.Common;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Presentation.Interfaces;
using Presentation.Presenter.Common;
using Presentation.View.Common;

using UnityEngine;
using Zenject;

namespace Installers
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        [SerializeField] AudioSource _audioSourceBGM;
        [SerializeField] AudioSource _audioSourceSE;
        [SerializeField] GameObject _inputEventProvider;

        public override void InstallBindings()
        {
            // Common Services
            Container
                .Bind<IScoreRankingService>()
                .To<ScoreRankingService>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IScoreResetService>()
                .To<ScoreResetService>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IExceptionHandler>()
                .To<ExceptionHandler>()
                .FromNew()
                .AsSingle();

            // Common Repositories
            Container
                .Bind<IGameRuleSettingsRepository>()
                .To<GameRuleSettingsRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<ISoundEffectsRepository>()
                .To<SoundEffectsRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IScoreTableRepository>()
                .To<ScoreTableRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IScoreRepository>()
                .To<JsonScoreRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<ISoundVolumeRepository>()
                .To<JsonSoundVolumeRepository>()
                .FromNew()
                .AsSingle();

            // Common UseCases
            Container
                .BindInterfacesTo<GameStateUseCase>()
                .FromNew()
                .AsSingle();

            Container
                .BindInterfacesTo<ScoreUseCase>()
                .FromNew()
                .AsSingle();

            Container
                .BindInterfacesTo<SoundUseCase>()
                .FromNew()
                .AsSingle();

            Container
                .BindInterfacesTo<ExceptionHandlingUseCase>()
                .FromNew()
                .AsSingle();

            // Common View
            Container
                .Bind<AudioSource>()
                .WithId("BGM")
                .FromComponentInNewPrefab(_audioSourceBGM)
                .AsCached();

            Container
                .Bind<AudioSource>()
                .WithId("SE")
                .FromComponentInNewPrefab(_audioSourceSE)
                .AsCached();

            Container
                .Bind<IInputEventProvider>()
                .To<InputEventProvider>()
                .FromComponentInNewPrefab(_inputEventProvider)
                .AsSingle();

            Container
                .Bind<IUIHelper>()
                .To<UIHelper>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IUIAnimator>()
                .To<UIAnimator>()
                .FromNew()
                .AsTransient();

            Container
                .Bind<ISceneLoaderUseCase>()
                .To<SceneLoaderUseCase>()
                .FromNew()
                .AsTransient();

            // Common Presenter
            Container
                .BindInterfacesTo<ProjectPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}
