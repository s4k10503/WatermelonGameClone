using System;

namespace UseCase.DTO
{
    /// <summary>
    /// UseCase層用のライセンスデータDTO（値型）
    /// </summary>
    public readonly struct LicenseDto
    {
        public string Title { get; }
        public string Text { get; }
        public string Url { get; }

        public LicenseDto(string title, string text, string url)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}