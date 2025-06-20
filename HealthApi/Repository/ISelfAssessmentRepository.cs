using HealthApi.Models;
using HealthApi.DTO;
namespace HealthApi.Repository
{
    public interface ISelfAssessmentRepository:IRepositoryTB<SelfAssessmentDTO>
    {
        SelfAssessmentDTO GetByIsFlagged(bool isFlagged);
    }
}
