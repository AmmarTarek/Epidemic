using HealthApi.DTO;
using HealthApi.Models;
using HealthApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealthApi.Repository
{
    public class VaccineRecordsRepository : IVaccineRecordsRepository
    {
        AppDbContext context;

        public VaccineRecordsRepository(AppDbContext ctx)
        {
            context = ctx;
        }


        public List<VaccineRecordsDTO> GetAll()
        {
            return context.VaccineRecords
                          .Include(v => v.User)
                          .Include(v => v.VaccineType)
                          .Include(v => v.Lab)
                          .Select(v => new VaccineRecordsDTO
                          {
                              RecordId = v.RecordId,
                              UserId = v.UserId,
                              UserFullName = v.User.FirstName + " " + v.User.LastName,
                              UserSex = v.User.Sex,
                              UserAge = DateTime.Now.Year - v.User.DateOfBirth.Year -
                                        (DateTime.Now.Date < v.User.DateOfBirth.AddYears(DateTime.Now.Year - v.User.DateOfBirth.Year) ? 1 : 0),

                              VaccineTypeId = v.VaccineTypeId,
                              VaccineTypeName = v.VaccineType.VaccineName,

                              Status = v.Status,
                              DateOfAssignment = v.DateOfAssignment,
                              DateOfVaccined = v.DateOfVaccined,

                              LabId = v.LabId,
                              LabName = v.Lab.LabName
                          })
                          .ToList();
        }

        public VaccineRecordsDTO GetById(int id)
        {
            return context.VaccineRecords
                          .Include(v => v.User)
                          .Include(v => v.VaccineType)
                          .Include(v => v.Lab)
                          .Where(v => v.RecordId == id)
                          .Select(v => new VaccineRecordsDTO
                          {
                              RecordId = v.RecordId,
                              UserId = v.UserId,
                              UserFullName = v.User.FirstName + " " + v.User.LastName,
                              UserSex = v.User.Sex,
                              UserAge = DateTime.Now.Year - v.User.DateOfBirth.Year -
                                        (DateTime.Now.Date < v.User.DateOfBirth.AddYears(DateTime.Now.Year - v.User.DateOfBirth.Year) ? 1 : 0),

                              VaccineTypeId = v.VaccineTypeId,
                              VaccineTypeName = v.VaccineType.VaccineName,

                              Status = v.Status,
                              DateOfAssignment = v.DateOfAssignment,
                              DateOfVaccined = v.DateOfVaccined,

                              LabId = v.LabId,
                              LabName = v.Lab.LabName
                          })
                          .FirstOrDefault();
        }

        public VaccineRecordsDTO GetByUserId(int UserId)
        {
            return context.VaccineRecords
                          .Include(v => v.User)
                          .Include(v => v.VaccineType)
                          .Include(v => v.Lab)
                          .Where(v => v.UserId == UserId)
                          .Select(v => new VaccineRecordsDTO
                          {
                              RecordId = v.RecordId,
                              UserId = v.UserId,
                              UserFullName = v.User.FirstName + " " + v.User.LastName,
                              UserSex = v.User.Sex,
                              UserAge = DateTime.Now.Year - v.User.DateOfBirth.Year -
                                        (DateTime.Now.Date < v.User.DateOfBirth.AddYears(DateTime.Now.Year - v.User.DateOfBirth.Year) ? 1 : 0),

                              VaccineTypeId = v.VaccineTypeId,
                              VaccineTypeName = v.VaccineType.VaccineName,

                              Status = v.Status,
                              DateOfAssignment = v.DateOfAssignment,
                              DateOfVaccined = v.DateOfVaccined,

                              LabId = v.LabId,
                              LabName = v.Lab.LabName
                          })
                          .FirstOrDefault();
        }

        public VaccineRecordsDTO GetByTestType(int TestTypeId)
        {
            return context.VaccineRecords
                          .Include(v => v.User)
                          .Include(v => v.VaccineType)
                          .Include(v => v.Lab)
                          .Where(v => v.VaccineTypeId == TestTypeId)
                          .Select(v => new VaccineRecordsDTO
                          {
                              RecordId = v.RecordId,
                              UserId = v.UserId,
                              UserFullName = v.User.FirstName + " " + v.User.LastName,
                              UserSex = v.User.Sex,
                              UserAge = DateTime.Now.Year - v.User.DateOfBirth.Year -
                                        (DateTime.Now.Date < v.User.DateOfBirth.AddYears(DateTime.Now.Year - v.User.DateOfBirth.Year) ? 1 : 0),

                              VaccineTypeId = v.VaccineTypeId,
                              VaccineTypeName = v.VaccineType.VaccineName,

                              Status = v.Status,
                              DateOfAssignment = v.DateOfAssignment,
                              DateOfVaccined = v.DateOfVaccined,

                              LabId = v.LabId,
                              LabName = v.Lab.LabName
                          })
                          .FirstOrDefault();
        }

        public VaccineRecordsDTO GetByLabId(int LabId)
        {
            return context.VaccineRecords
                          .Include(v => v.User)
                          .Include(v => v.VaccineType)
                          .Include(v => v.Lab)
                          .Where(v => v.LabId == LabId)
                          .Select(v => new VaccineRecordsDTO
                          {
                              RecordId = v.RecordId,
                              UserId = v.UserId,
                              UserFullName = v.User.FirstName + " " + v.User.LastName,
                              UserSex = v.User.Sex,
                              UserAge = DateTime.Now.Year - v.User.DateOfBirth.Year -
                                        (DateTime.Now.Date < v.User.DateOfBirth.AddYears(DateTime.Now.Year - v.User.DateOfBirth.Year) ? 1 : 0),

                              VaccineTypeId = v.VaccineTypeId,
                              VaccineTypeName = v.VaccineType.VaccineName,

                              Status = v.Status,
                              DateOfAssignment = v.DateOfAssignment,
                              DateOfVaccined = v.DateOfVaccined,

                              LabId = v.LabId,
                              LabName = v.Lab.LabName
                          })
                          .FirstOrDefault();
        }
        public VaccineRecordsDTO GetBySex(string sex)
        {
            return context.VaccineRecords
                          .Include(v => v.User)
                          .Include(v => v.VaccineType)
                          .Include(v => v.Lab)
                          .Where(v => v.User.Sex == sex)
                          .Select(v => new VaccineRecordsDTO
                          {
                              RecordId = v.RecordId,
                              UserId = v.UserId,
                              UserFullName = v.User.FirstName + " " + v.User.LastName,
                              UserSex = v.User.Sex,
                              UserAge = DateTime.Now.Year - v.User.DateOfBirth.Year -
                                        (DateTime.Now.Date < v.User.DateOfBirth.AddYears(DateTime.Now.Year - v.User.DateOfBirth.Year) ? 1 : 0),

                              VaccineTypeId = v.VaccineTypeId,
                              VaccineTypeName = v.VaccineType.VaccineName,

                              Status = v.Status,
                              DateOfAssignment = v.DateOfAssignment,
                              DateOfVaccined = v.DateOfVaccined,

                              LabId = v.LabId,
                              LabName = v.Lab.LabName
                          })
                          .FirstOrDefault();
        }

        public void Add(VaccineRecordsDTO obj)
        {

            VaccineRecord newVAC = new VaccineRecord();
            VaccineRecordsDTO vaccineRecordsDTO = new VaccineRecordsDTO();

           newVAC.UserId = obj.UserId;
            newVAC.DateOfVaccined = obj.DateOfVaccined;
            newVAC.LabId = obj.LabId;
            newVAC.DateOfAssignment = obj.DateOfAssignment;
            newVAC.Status = obj.Status;
            newVAC.VaccineTypeId = obj.VaccineTypeId;


            context.VaccineRecords.Add(newVAC);
        }
        public void Save()
        {
            context.SaveChanges();
        }


        public void Update (VaccineRecordsDTO obj)
        {
            VaccineRecord record = context.VaccineRecords.FirstOrDefault(v=>v.RecordId == obj.RecordId);

            record.Status = obj.Status;
         

        }
    }
}
