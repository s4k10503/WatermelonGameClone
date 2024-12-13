namespace WatermelonGameClone.Domain
{
    public class License
    {
        public string Name { get; }
        public string LicenseType { get; }
        public string Copyright { get; }
        public string[] Terms { get; }

        public License(string name, string licenseType, string copyright, string[] terms)
        {
            Name = name;
            LicenseType = licenseType;
            Copyright = copyright;
            Terms = terms;
        }
    }
}
