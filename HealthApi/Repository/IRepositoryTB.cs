namespace HealthApi.Repository
{
    public interface IRepositoryTB<T>
    {
        List<T> GetAll();
        void Add(T obj);
        void Save();
        T GetById(int id);
        T GetByUserId(int userId);
        
    }
}
