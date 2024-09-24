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
            catch (Exception e)
            {
                Debug.LogError($"Failed to save sound settings to JSON: {e.Message}");
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
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load sound settings from JSON: {e.Message}");
            }

            return (1.0f, 1.0f);
        }
    }
}
