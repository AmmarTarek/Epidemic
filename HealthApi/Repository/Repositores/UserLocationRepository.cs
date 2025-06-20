using HealthApi.Repository.Interfaces;
using HealthApi.Models;
using Microsoft.EntityFrameworkCore;


namespace HealthApi.Repository.Repositores
{
    public class UserLocationRepository : IUserLocationRepository
    {
        AppDbContext context;

        public UserLocationRepository (AppDbContext context) 
        {
            this.context = context;
        }

        public UserLocation GetUserLastLocation(int userId)
        {
            throw new NotImplementedException();
        }

        public UserLocation SetUserLastLocation(int userId, double longitude, double latitude)
        {
            throw new NotImplementedException();
        }
    }
}
    