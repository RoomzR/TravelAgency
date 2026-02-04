namespace TravelAgency.BLL.DTOs
{
    public class CountryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int ToursCount { get; set; }
        public int CitiesCount { get; set; }
    }

    public class CountryCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CountryUpdateDTO : CountryCreateDTO
    {
        public int Id { get; set; }
    }

    public class CityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public int HotelsCount { get; set; }
        public int ToursCount { get; set; }
    }
}