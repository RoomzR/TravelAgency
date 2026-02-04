using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class TourTypeRepository : BaseRepository<TourType>, ITourTypeRepository
    {
        public TourTypeRepository(ApplicationDbContext context) : base(context) { }

    }
}