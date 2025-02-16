using WatermelonGameClone.Domain;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Infrastructure
{
    public class JsonLicenseRepository : ILicenseRepository
    {
        private const string LicenseAddressableKey = "License";

        public async UniTask<IReadOnlyList<License>> LoadLicensesAsync(CancellationToken ct)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(LicenseAddressableKey);

            try
            {
                await handle.ToUniTask(cancellationToken: ct);
                TextAsset textAsset = handle.Result;

                if (textAsset == null)
                {
                    throw new InfrastructureException($"License asset with key '{LicenseAddressableKey}' not found.");
                }

                // Acquired the JSON character string from TextAsset.Text and Perth
                string json = textAsset.text;
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
                throw new InfrastructureException("Failed to load license file from Addressables.", ex);
            }
            finally
            {
                Addressables.Release(handle);
            }
        }

        [Serializable]
        private class LicenseContainerDto
        {
            public LicenseDto[] Licenses;
        }

        [Serializable]
        private class LicenseDto
        {
            public string Name;
            public string Type;
            public string Copyright;
            public string[] Terms;
        }
    }
}
