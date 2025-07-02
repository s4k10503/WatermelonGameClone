using UseCase.DTO;

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UseCase.Interfaces
{
    public interface ILicenseUseCase
    {
        UniTask<IReadOnlyList<LicenseDto>> GetLicensesAsync(CancellationToken ct);
    }
}
