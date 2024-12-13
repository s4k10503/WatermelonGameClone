using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Domain
{
    public interface ILicenseRepository
    {
        UniTask<IReadOnlyList<License>> LoadLicensesAsync();
    }
}
