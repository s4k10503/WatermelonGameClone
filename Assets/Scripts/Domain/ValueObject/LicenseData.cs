namespace WatermelonGameClone.Domain
{
    public class License
    {
        public string Name { get; }
        public string Type { get; }
        public string Copyright { get; }
        public string[] Terms { get; }

        public License(
            string name,
            string type,
            string copyright,
            string[] terms)
        {
            Name = name;
            Type = type;
            Copyright = copyright;
            Terms = terms;
        }
    }
}
