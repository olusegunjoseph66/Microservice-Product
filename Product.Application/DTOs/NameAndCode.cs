namespace Product.Application.DTOs
{
    public class NameAndCode
    {
        public NameAndCode(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}
