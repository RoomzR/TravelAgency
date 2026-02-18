using System.Collections.Generic;
using TravelAgency.BLL.DTOs;
using TravelAgency.DAL.Entities;

namespace TravelAgency.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Tour> FeaturedTours { get; set; } = new();
        public List<Tour> PopularTours { get; set; } = new();
        public List<Country> Countries { get; set; } = new();
        public List<NewsArticle> NewsArticles { get; set; } = new();
        public int TourCount { get; set; }
        public int ClientCount { get; set; }
        public int BookingCount { get; set; }

        public IEnumerable<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}