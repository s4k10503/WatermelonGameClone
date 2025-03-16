using Domain.Interfaces;
using Domain.ValueObject;
using UseCase.Interfaces;

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UseCase.UseCases.Common
{
    public sealed class LicenseUseCase : ILicenseUseCase
    {
        private readonly ILicenseRepository _licenseRepository;

        public LicenseUseCase(ILicenseRepository licenseRepository)
        {
            _licenseRepository = licenseRepository;
        }

        public async UniTask<IReadOnlyList<License>> GetLicensesAsync(CancellationToken ct)
        {
            return await _licenseRepository.LoadLicensesAsync(ct);
        }
    }
}
