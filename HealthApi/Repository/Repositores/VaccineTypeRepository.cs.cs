using HealthApi.Models;
using HealthApi.Repository.Interfaces;

namespace HealthApi.Repository.Repositores
{
    public class VaccineTypeRepository : IVaccineTypeRepository
    {
        AppDbContext context;
        public VaccineTypeRepository(AppDbContext context) 
        {
            this.context = context;
        }

        public void addVaccineType(string name, string description)
        {
            if (name == null) throw new ArgumentNullException("name");

            var vaccineType = new VaccineType();

            vaccineType.VaccineName = name;
            vaccineType.Description = description;

            context.VaccineTypes.Add(vaccineType);
            context.SaveChanges();
        }

        public VaccineType GetVaccineTypeById(int id)
        {
            return context.VaccineTypes.FirstOrDefault(i => i.VaccineId == id);
        }

        public VaccineType GetVaccineTypeByName(string name)
        {
            return context.VaccineTypes.FirstOrDefault(i => i.VaccineName == name);
        }
    }
}
