using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;
using UnityEngine;
using System.Threading;
using System;

namespace WatermelonGameClone.Infrastructure
{
    public class JsonLicenseRepository : ILicenseRepository
    {
        private const string FilePath = "License.json";

        public async UniTask<IReadOnlyList<License>> LoadLicensesAsync(CancellationToken ct)
        {
            string path = Path.Combine(Application.streamingAssetsPath, FilePath);

            if (string.IsNullOrEmpty(path))
            {
                throw new InfrastructureException("StreamingAssets path is invalid.");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"License file not found at {path}");
            }

            try
            {
                string json = await File.ReadAllTextAsync(path, ct);
                var container = JsonUtility.FromJson<LicenseContainerDto>(json);

                var licenses = new List<License>();
                foreach (var dto in container.Licenses)
                {
                    licenses.Add(new License(dto.Name, dto.Type, dto.Copyright, dto.Terms));
                }

                return licenses;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to load license file.", ex);
            }
        }

        [System.Serializable]
        private class LicenseContainerDto
        {
            public LicenseDto[] Licenses;
        }

        [System.Serializable]
        private class LicenseDto
        {
            public string Name;
            public string Type;
            public string Copyright;
            public string[] Terms;
        }
    }
}
