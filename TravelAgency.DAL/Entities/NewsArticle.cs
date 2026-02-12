namespace TravelAgency.DAL.Entities
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string AuthorId { get; set; } = null!;
        public virtual ApplicationUser Author { get; set; } = null!;
    }
}