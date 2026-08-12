using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TaskMngBack.Configuration;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class R2StorageService : IStorageService, IDisposable
    {
        private readonly AmazonS3Client _s3Client;
        private readonly R2Settings _settings;

        public R2StorageService(IOptions<R2Settings> options)
        {
            _settings = options.Value;

            var credentials = new BasicAWSCredentials(_settings.AccessKeyId, _settings.SecretAccessKey);
            _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = $"https://{_settings.AccountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            });
        }

        public Task<string> GeneratePresignedUploadUrlAsync(string storageKey, string contentType)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = storageKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(5),
                ContentType = contentType
            };

            return Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        public Task<string> GeneratePresignedDownloadUrlAsync(string storageKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = storageKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            return Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        public Task DeleteObjectAsync(string storageKey)
        {
            return _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = storageKey
            });
        }

        public void Dispose()
        {
            _s3Client.Dispose();
        }
    }
}
