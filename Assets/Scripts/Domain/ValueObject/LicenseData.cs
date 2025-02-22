using System;

namespace Domain.ValueObject
{
    [Serializable]
    public class LicenseContainer
    {
        public License[] licenses;
    }
    
    [Serializable]
    public class License
    {
        public string name;
        public string type;
        public string copyright;
        public string[] terms;
    }
}
