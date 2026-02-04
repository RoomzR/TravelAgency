namespace TravelAgency.BLL.DTOs
{
    public class FAQDTO
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
    }

    public class FAQCreateDTO
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class FAQUpdateDTO : FAQCreateDTO
    {
        public int Id { get; set; }
    }
}