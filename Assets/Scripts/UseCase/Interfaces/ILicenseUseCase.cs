using Domain.ValueObject;

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UseCase.Interfaces
{
    public interface ILicenseUseCase
    {
        UniTask<IReadOnlyList<License>> GetLicensesAsync(CancellationToken ct);
    }
}
