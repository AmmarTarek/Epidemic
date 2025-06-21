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
            if (userId < 0) 
            {
                return null;
            }

            return context.UserLocations.FirstOrDefault(i => i.UserId == userId);
        }

        public void SetUserLastLocation(int userId, double longitude, double latitude)
        {
            if (userId <= 0)
            {
                throw new ArgumentException();
            }

            var userLocation = new UserLocation();

            userLocation.UserId = userId;
            userLocation.Longitude = longitude;
            userLocation.Latitude = latitude;

            context.UserLocations.Add(userLocation);
            context.SaveChanges();
        }
    }
}
    