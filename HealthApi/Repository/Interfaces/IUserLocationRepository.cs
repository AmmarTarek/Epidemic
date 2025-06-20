using HealthApi.Models;

namespace HealthApi.Repository.Interfaces
{
    public interface IUserLocationRepository : IRepository<UserLocation>
    {
        public UserLocation GetUserLastLocation(int userId);
        public UserLocation SetUserLastLocation(int userId, double longitude, double latitude);
    }
}
