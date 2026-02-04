namespace TravelAgency.BLL.DTOs
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
    }

    public class NewsCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class NewsUpdateDTO : NewsCreateDTO
    {
        public int Id { get; set; }
    }
}