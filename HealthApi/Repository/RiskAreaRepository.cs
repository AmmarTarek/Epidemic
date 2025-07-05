using HealthApi.Models;

namespace HealthApi.Repository
{
    public class RiskAreaRepository : IRiskAreaRepository
    {
        AppDbContext context;

        public RiskAreaRepository(AppDbContext ctx)
        {
            context = ctx;
        }


        public List<RiskArea> GetAll()
        {

            return context.RiskAreas.ToList();
        }

        public RiskArea GetById(int id)
        {
            return context.RiskAreas.Where(r => r.AreaId == id)
                .Select(r => new RiskArea { AreaId = r.AreaId, RiskLevel = r.RiskLevel }).FirstOrDefault();

        }

        public List<RiskArea> GetByRiskLevel(string level)
        {
            return context.RiskAreas.Where(r => r.RiskLevel == level)
                .Select(r => new RiskArea { AreaId = r.AreaId, RiskLevel = r.RiskLevel }).ToList();

        }
    }
}
