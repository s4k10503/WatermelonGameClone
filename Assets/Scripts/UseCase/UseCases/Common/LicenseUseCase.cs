using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public class LicenseUseCase : ILicenseUseCase
    {
        private readonly ILicenseRepository _licenseRepository;

        public LicenseUseCase(ILicenseRepository licenseRepository)
        {
            _licenseRepository = licenseRepository;
        }

        public async UniTask<IReadOnlyList<License>> GetLicensesAsync()
        {
            return await _licenseRepository.LoadLicensesAsync();
        }
    }
}
