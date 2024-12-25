using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public interface ILicenseUseCase
    {
        UniTask<IReadOnlyList<License>> GetLicensesAsync(CancellationToken ct);
    }
}
