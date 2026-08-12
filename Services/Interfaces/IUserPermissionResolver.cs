namespace TaskMngBack.Services.Interfaces
{
    public interface IUserPermissionResolver
    {
        Task<List<string>> GetPermissionKeysAsync(int userId);
    }
}
