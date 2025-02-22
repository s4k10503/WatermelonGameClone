using Domain.Interfaces;
using Domain.ValueObject;
using Infrastructure.Services;

using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Repositories
{
    public sealed class JsonSoundVolumeRepository : ISoundVolumeRepository
    {
        private static readonly string SSoundSettingsFilePath = Path.Combine(Application.persistentDataPath, "sound_settings.json");

        public async UniTask SaveSoundSettingsAsync(float volumeBGM, float volumeSe, CancellationToken ct)
        {
            if (volumeBGM < 0 || volumeBGM > 1 || volumeSe < 0 || volumeSe > 1)
            {
                throw new InfrastructureException("Sound volumes must be between 0 and 1.");
            }

            try
            {
                VolumeSettings volumeSettings = new VolumeSettings
                {
                    volumeBGM = volumeBGM,
                    volumeSe = volumeSe
                };
                string json = JsonUtility.ToJson(volumeSettings);
                await File.WriteAllTextAsync(SSoundSettingsFilePath, json, ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to save sound settings to JSON file.", ex);
            }
        }

        public async UniTask<(float VolumeBGM, float VolumeSE)> LoadSoundSettingsAsync(CancellationToken ct)
        {
            try
            {
                if (File.Exists(SSoundSettingsFilePath))
                {
                    string json = await File.ReadAllTextAsync(SSoundSettingsFilePath, ct);
                    VolumeSettings volumeSettings = JsonUtility.FromJson<VolumeSettings>(json);
                    return (volumeSettings.volumeBGM, volumeSettings.volumeSe);
                }
                else
                {
                    // Sound settings file not found. Returning default values.
                    return (1.0f, 1.0f);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to load sound settings from JSON file.", ex);
            }
        }
    }
}
