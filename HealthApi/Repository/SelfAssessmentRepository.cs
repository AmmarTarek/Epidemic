using HealthApi.Models;
using HealthApi.DTO;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HealthApi.Repository
{
    public class SelfAssessmentRepository:ISelfAssessmentRepository
    {
        AppDbContext context;

        public SelfAssessmentRepository(AppDbContext ctx) { 
        context = ctx;
        }

        public List<SelfAssessmentDTO> GetAll() {
            return context.SelfAssessments.Include(s => s.User)
                    .Select(s => new SelfAssessmentDTO
                    {
                        AssessmentId = s.AssessmentId,
                        Symptoms = s.Symptoms,
                        IsFlagged = s.IsFlagged,
                        UserId = s.UserId,
                        Date = s.Date,
                        UserFullName = s.User.FirstName + " " + s.User.LastName
                    })
                    .ToList();
        }


        public void Add(SelfAssessmentDTO obj) { 

            SelfAssessment newSA = new SelfAssessment
            {
                Symptoms = obj.Symptoms,
                IsFlagged = obj.IsFlagged,
                Date = obj.Date,
                UserId = obj.UserId
            };
            //SelfAssessmentDTO SADTO = new SelfAssessmentDTO();
            //newSA.AssessmentId = SADTO.AssessmentId;
            //newSA.Symptoms = SADTO.Symptoms;
            //newSA.IsFlagged = SADTO.IsFlagged;
            //newSA.Date = SADTO.Date;
            //newSA.UserId = SADTO.UserId;    
              context.SelfAssessments.Add(newSA);
        }

        public void Save() { 
        context.SaveChanges();
        }

        public SelfAssessmentDTO GetById(int id) {
            SelfAssessmentDTO SA = context.SelfAssessments.Include(s => s.User)
                    .Where(s => s.AssessmentId==id)
                    .Select(s => new SelfAssessmentDTO
                    {
                        AssessmentId = s.AssessmentId,
                        Symptoms = s.Symptoms,
                        IsFlagged = s.IsFlagged,
                        UserId= s.UserId,
                        Date = s.Date,
                        UserFullName = s.User.FirstName + " " + s.User.LastName
                    })
                    .FirstOrDefault();

            return SA;
        }

        public SelfAssessmentDTO GetByUserId(int userId) {
            SelfAssessmentDTO SA = context.SelfAssessments.Include(s => s.User)
                    .Where(s => s.UserId == userId)
                    .Select(s => new SelfAssessmentDTO
                    {
                        AssessmentId = s.AssessmentId,
                        Symptoms = s.Symptoms,
                        IsFlagged = s.IsFlagged,
                        Date = s.Date,
                        UserId = s.UserId,
                        UserFullName = s.User.FirstName + " " + s.User.LastName
                    })
                    .FirstOrDefault();
        
            return SA; 
        }
        public SelfAssessmentDTO GetByIsFlagged(bool IsFlagged)
        {
            SelfAssessmentDTO SA = context.SelfAssessments.Include(s => s.User)
                    .Where(s => s.IsFlagged==IsFlagged)
                    .Select(s => new SelfAssessmentDTO
                    {
                        AssessmentId = s.AssessmentId,
                        Symptoms = s.Symptoms,
                        IsFlagged = s.IsFlagged,
                        UserId = s.UserId,
                        Date = s.Date,
                        UserFullName = s.User.FirstName + " " + s.User.LastName
                    })
                    .FirstOrDefault();
            return SA;
        }


    }
}
