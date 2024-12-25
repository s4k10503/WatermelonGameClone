using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Domain
{
    public interface ILicenseRepository
    {
        UniTask<IReadOnlyList<License>> LoadLicensesAsync(CancellationToken ct);
    }
}
