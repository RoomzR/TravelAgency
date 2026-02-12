using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class TourTypeRepository : BaseRepository<TourType>, ITourTypeRepository
    {
        public TourTypeRepository(ApplicationDbContext context) : base(context) { }

    }
}