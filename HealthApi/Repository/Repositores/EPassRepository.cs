using HealthApi.Models;
using HealthApi.Repository.Interfaces;

namespace HealthApi.Repository.Repositores
{
    public class EPassRepository : IEPassRepository
    {
        AppDbContext context;
        public EPassRepository(AppDbContext context) 
        {
            this.context = context;
        }

        public void UpdateEPass(EPass ePass)
        {
            var dbEPass = context.EPasses.FirstOrDefault(e => e.EPassID == ePass.EPassID);

            dbEPass.Status = ePass.Status;
            dbEPass.Description = ePass.Description;

            context.Update(dbEPass);
            context.SaveChanges();
        }
    }
}
