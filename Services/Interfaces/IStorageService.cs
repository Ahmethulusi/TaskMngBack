namespace TaskMngBack.Services.Interfaces
{
    public interface IStorageService
    {
        Task<string> GeneratePresignedUploadUrlAsync(string storageKey, string contentType);
        Task<string> GeneratePresignedDownloadUrlAsync(string storageKey);
        Task DeleteObjectAsync(string storageKey);
    }
}
