using System;
using System.IO;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public sealed class JsonSoundVolumeRepository : ISoundVolumeRepository
    {
        private static readonly string s_soundSettingsFilePath = Path.Combine(Application.persistentDataPath, "sound_settings.json");

        public async UniTask SaveSoundSettingsAsync(float volumeBGM, float volumeSE, CancellationToken ct)
        {
            if (volumeBGM < 0 || volumeBGM > 1 || volumeSE < 0 || volumeSE > 1)
            {
                throw new InfrastructureException("Sound volumes must be between 0 and 1.");
            }

            try
            {
                VolumeSettings volumeSettings = new VolumeSettings
                {
                    VolumeBGM = volumeBGM,
                    VolumeSE = volumeSE
                };
                string json = JsonUtility.ToJson(volumeSettings);
                await File.WriteAllTextAsync(s_soundSettingsFilePath, json, ct);
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
                if (File.Exists(s_soundSettingsFilePath))
                {
                    string json = await File.ReadAllTextAsync(s_soundSettingsFilePath, ct);
                    VolumeSettings volumeSettings = JsonUtility.FromJson<VolumeSettings>(json);
                    return (volumeSettings.VolumeBGM, volumeSettings.VolumeSE);
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
