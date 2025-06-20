namespace HealthApi.Repository
{
    public interface IRepositoryTB2<T>
    {
        List<T> GetAll();
        void Add(T obj);
        void Save();
        T GetById(int id);
        T GetByUserId(int userId);
        T GetByTestType(int TestTypeId);
        T GetBySex(string sex);
        T GetByLabId(int LabId);

    }
}
