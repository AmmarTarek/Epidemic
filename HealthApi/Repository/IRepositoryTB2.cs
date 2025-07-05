namespace HealthApi.Repository
{
    public interface IRepositoryTB2<T>
    {
        List<T> GetAll();
        T GetById(int id);
        List<T> GetByRiskLevel(string level);

    }
}
