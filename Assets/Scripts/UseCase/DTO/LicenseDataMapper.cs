using Domain.ValueObject;
using UseCase.DTO;
using System.Collections.Generic;
using System.Linq;

namespace UseCase.DTO
{
    /// <summary>
    /// Domain層のLicenseとUseCase層のLicenseDtoを相互変換するマッパー
    /// </summary>
    public static class LicenseDataMapper
    {
        /// <summary>
        /// Domain層のLicenseをUseCase層のLicenseDtoに変換
        /// </summary>
        /// <param name="license">Domain層のLicense</param>
        /// <returns>UseCase層のLicenseDto</returns>
        public static LicenseDto? ToLicenseDto(License license)
        {
            if (license == null) return null;

            return new LicenseDto(
                license.title,
                license.text,
                license.url
            );
        }

        /// <summary>
        /// Domain層のLicenseコレクションをUseCase層のLicenseDtoコレクションに変換
        /// </summary>
        /// <param name="licenses">Domain層のLicenseコレクション</param>
        /// <returns>UseCase層のLicenseDtoコレクション</returns>
        public static IReadOnlyList<LicenseDto> ToLicenseDtoList(IReadOnlyList<License> licenses)
        {
            if (licenses == null) return new List<LicenseDto>();

            return licenses
                .Select(ToLicenseDto)
                .Where(dto => dto.HasValue)
                .Select(dto => dto.Value)
                .ToList();
        }

        /// <summary>
        /// UseCase層のLicenseDtoをDomain層のLicenseに変換
        /// （将来的に必要な場合のために用意）
        /// </summary>
        /// <param name="licenseDto">UseCase層のLicenseDto（nullable）</param>
        /// <returns>Domain層のLicense</returns>
        public static License ToLicense(LicenseDto? licenseDto)
        {
            if (!licenseDto.HasValue) return null;

            var dto = licenseDto.Value;
            return new License(
                dto.Title,
                dto.Text,  
                dto.Url
            );
        }
    }
}