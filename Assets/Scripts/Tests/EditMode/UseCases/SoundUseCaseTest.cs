using Domain.Interfaces;
using Domain.ValueObject;
using UseCase.UseCases.Common;

using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.UseCases
{
    public sealed class SoundUseCaseTest
    {
        private SoundUseCase _soundUseCase;
        private AudioSource _audioSourceBgm;
        private AudioSource _audioSourceSe;
        private ISoundVolumeRepository _mockSoundVolumeRepository;
        private ISoundEffectsRepository _mockSoundEffectsRepository;
        private GameObject _gameObject;

        [SetUp]
        public void SetUp()
        {
            // Attach the actual AudioSource instance to GameObject
            _gameObject = new GameObject();
            _audioSourceBgm = _gameObject.AddComponent<AudioSource>();
            _audioSourceSe = _gameObject.AddComponent<AudioSource>();

            // Sound volume repository mock
            _mockSoundVolumeRepository = Substitute.For<ISoundVolumeRepository>();

            // Mock of sound effect lodge
            _mockSoundEffectsRepository = Substitute.For<ISoundEffectsRepository>();

            // Initialize the SoundUseCase to be tested
            _soundUseCase = new SoundUseCase(
                _audioSourceBgm,
                _audioSourceSe,
                _mockSoundVolumeRepository,
                _mockSoundEffectsRepository
            );
        }

        [UnityTest]
        public IEnumerator InitializeAsync_ShouldLoadSoundSettingsAndSetVolumes() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: Set Mock LoadSoundSettingsAsync
            _mockSoundVolumeRepository.LoadSoundSettingsAsync(Arg.Any<CancellationToken>())
                .Returns(UniTask.FromResult((0.5f, 0.7f)));

            // Use the actual AudioClip
            var dropClip = AudioClip.Create("DropSound", 44100, 1, 44100, false);
            var mergeClip = AudioClip.Create("MergeSound", 44100, 1, 44100, false);

            _mockSoundEffectsRepository.GetClip(SoundEffect.Drop).Returns(dropClip);
            _mockSoundEffectsRepository.GetClip(SoundEffect.Merge).Returns(mergeClip);

            // Act: Call the initialization method
            await _soundUseCase.InitializeAsync(CancellationToken.None);

            // Assert: Check that the sound volume is correctly set
            Assert.AreEqual(0.5f, _audioSourceBgm.volume);
            Assert.AreEqual(0.7f, _audioSourceSe.volume);

            // Check if the sound effect is read correctly
            _mockSoundEffectsRepository.Received(1).GetClip(SoundEffect.Drop);
            _mockSoundEffectsRepository.Received(1).GetClip(SoundEffect.Merge);
        });

        [Test]
        public void SetBGMVolume_ShouldUpdateBGMVolume()
        {
            // Act: Set the BGM volume
            _soundUseCase.SetBGMVolume(0.8f);

            // Assert: Check if the BGM volume is set correctly
            Assert.AreEqual(0.8f, _audioSourceBgm.volume);
        }

        [Test]
        public void SetSEVolume_ShouldUpdateSEVolume()
        {
            // Act: Set SE volume
            _soundUseCase.SetSeVolume(0.9f);

            // Assert: Check if the volume of SE is set correctly
            Assert.AreEqual(0.9f, _audioSourceSe.volume);
        }

        [UnityTest]
        public IEnumerator PlayBGM_ShouldCallPlayOnAudioSource()
        {
            // Arrange: Set AudioClip
            var bgmClip = AudioClip.Create("BGM", 44100, 1, 44100, false);
            _audioSourceBgm.clip = bgmClip;

            // Act: Call BGM playback
            _soundUseCase.PlayBGM();

            // Proceed with the frame and check the playback status
            yield return null;

            // Assert: Check if it is played with BGM AudioSource
            Assert.IsTrue(_audioSourceBgm.isPlaying);
        }

        [UnityTest]
        public IEnumerator SaveVolume_ShouldSaveCurrentVolumes() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            _audioSourceBgm.volume = 0.6f;
            _audioSourceSe.volume = 0.4f;

            // Act: Save sound settings
            await _soundUseCase.SaveVolume(CancellationToken.None);

            // Assert: Check if the volume is stored correctly
            await _mockSoundVolumeRepository.Received(1).SaveSoundSettingsAsync(0.6f, 0.4f, Arg.Any<CancellationToken>());
        });

        [UnityTest]
        public IEnumerator PlaySoundEffect_ShouldPlayCorrectSoundEffect()
        {
            // Arrange: Use the actual AudioClip
            var dropClip = AudioClip.Create("DropSound", 44100, 1, 44100, false);
            _mockSoundEffectsRepository.GetClip(SoundEffect.Drop).Returns(dropClip);

            // Call InitializeAsync and load the sound effect
            yield return _soundUseCase.InitializeAsync(CancellationToken.None).ToCoroutine();

            // Act: Play the drop sound
            _soundUseCase.PlaySoundEffect("Drop");

            // Proceed to the frame
            yield return null;

            // Assert: The correct clip is set in SE's AudioSource and confirm that it has been played
            Assert.AreEqual(dropClip, _audioSourceSe.clip);  // Is AudioClip set?
            Assert.IsTrue(_audioSourceSe.isPlaying);         // Is it regenerated?
        }

        [TearDown]
        public void TearDown()
        {
            _mockSoundVolumeRepository = null;
            _mockSoundEffectsRepository = null;
            // Discard GameObject after testing
            Object.DestroyImmediate(_gameObject);
        }
    }
}
