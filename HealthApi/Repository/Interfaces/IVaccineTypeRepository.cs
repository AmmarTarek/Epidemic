using HealthApi.Models;

namespace HealthApi.Repository.Interfaces
{
    public interface IVaccineTypeRepository
    {
        public VaccineType GetVaccineTypeById(int id);
        public VaccineType GetVaccineTypeByName(string name);
        public void addVaccineType(string name, string description);

    }
}
