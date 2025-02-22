using Domain.ValueObject;

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILicenseRepository
    {
        UniTask<IReadOnlyList<License>> LoadLicensesAsync(CancellationToken ct);
    }
}
