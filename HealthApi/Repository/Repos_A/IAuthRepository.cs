namespace HealthApi.Repository.Repos_A
{
    public interface IAuthRepository
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User> RegisterAsync(User user);
        Task<bool> UserExistsAsync(string email);
    }
}
