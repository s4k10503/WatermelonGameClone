namespace Presentation.DTO
{
    public class LicenseDto
    {
        public string DisplayText { get; }

        public LicenseDto(string displayText)
        {
            DisplayText = displayText;
        }
    }
}
