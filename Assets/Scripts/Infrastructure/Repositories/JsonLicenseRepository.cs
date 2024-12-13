using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;
using UnityEngine;

namespace WatermelonGameClon.Infrastructure
{
    public class LicenseRepository : ILicenseRepository
    {
        private const string FilePath = "licenses.json";

        public async UniTask<IReadOnlyList<License>> LoadLicensesAsync()
        {
            string path = Path.Combine(Application.streamingAssetsPath, FilePath);
            if (!File.Exists(path))
                throw new FileNotFoundException($"License file not found at {path}");

            string json = await File.ReadAllTextAsync(path);
            var container = JsonUtility.FromJson<LicenseContainerDto>(json);

            var licenses = new List<License>();
            foreach (var dto in container.Licenses)
            {
                licenses.Add(new License(dto.Name, dto.LicenseType, dto.Copyright, dto.Terms));
            }

            return licenses;
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
            public string LicenseType;
            public string Copyright;
            public string[] Terms;
        }
    }
}
