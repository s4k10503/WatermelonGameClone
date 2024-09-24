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
        [SerializeField] TimeSettings _timeSettings;
        [SerializeField] SoundSettings _soundSettings;
        [SerializeField] ScoreTableSettings _scoreTableSettings;
        [SerializeField] AudioSource _audioSourceBGM;
        [SerializeField] AudioSource _audioSourceSE;
        [SerializeField] GameObject _inputEventProvider;

        public override void InstallBindings()
        {
            // Common Model
            Container
                .Bind<TimeSettings>()
                .FromInstance(_timeSettings)
                .AsSingle();

            Container
                .Bind<ITimeSettingsRepository>()
                .To<TimeSettingsRepository>()
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
                .Bind<IGameStateUseCase>()
                .To<GameStateUseCase>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IScoreUseCase>()
                .To<ScoreUseCase>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<ISoundUseCase>()
                .To<SoundUseCase>()
                .FromNew()
                .AsSingle();

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
                .Bind<IScoreRepository>()
                .To<JsonScoreRepository>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<ISoundVolumeRepository>()
                .To<JsonSoundVolumeRepository>()
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
