
using HealthApi.DTO;
using HealthApi.Models;
namespace HealthApi.Repository

{
    public interface IVaccineRecordsRepository:IRepositoryTB<VaccineRecordsDTO>
    {
        VaccineRecordsDTO GetByTestType(int TestTypeId);
        VaccineRecordsDTO GetBySex(string sex);
        VaccineRecordsDTO GetByLabId(int LabId);
        void Update(VaccineRecordsDTO obj);

    }
}
