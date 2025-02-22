using Domain.Interfaces;
using Domain.ValueObject;
using Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Infrastructure.Repositories
{
    public sealed class JsonLicenseRepository : ILicenseRepository
    {
        private const string LicenseAddressableKey = "License";

        public async UniTask<IReadOnlyList<License>> LoadLicensesAsync(CancellationToken ct)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(LicenseAddressableKey);

            try
            {
                await handle.ToUniTask(cancellationToken: ct);
                var textAsset = handle.Result;

                if (!textAsset)
                {
                    throw new InfrastructureException($"License asset with key '{LicenseAddressableKey}' not found.");
                }

                var json = textAsset.text;
                var container = JsonUtility.FromJson<LicenseContainer>(json);
                if (container?.licenses == null)
                {
                    throw new InfrastructureException("License file content is invalid.");
                }

                return container.licenses;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to load license file from Addressable.", ex);
            }
            finally
            {
                Addressables.Release(handle);
            }
        }
    }
}
