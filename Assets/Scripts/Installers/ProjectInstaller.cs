using Zenject;
using UnityEngine;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using WatermelonGameClone.Presentation;
using WatermelonGameClone.Infrastructure;

namespace WatermelonGameClone.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] GameRuleSettings _gameRuleSettings;
        [SerializeField] SoundSettings _soundSettings;
        [SerializeField] ScoreTableSettings _scoreTableSettings;
        [SerializeField] AudioSource _audioSourceBGM;
        [SerializeField] AudioSource _audioSourceSE;
        [SerializeField] GameObject _inputEventProvider;

        public override void InstallBindings()
        {
            // Commom Services
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
                .Bind<GameRuleSettings>()
                .FromInstance(_gameRuleSettings)
                .AsSingle();

            Container
                .Bind<IGameRuleSettingsRepository>()
                .To<GameRuleSettingsRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<SoundSettings>()
                .FromInstance(_soundSettings)
                .AsSingle();

            Container
                .Bind<ISoundEffectsRepository>()
                .To<SoundEffectsRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<ScoreTableSettings>()
                .FromInstance(_scoreTableSettings)
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
