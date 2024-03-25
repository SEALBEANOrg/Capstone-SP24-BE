using System.Net;

namespace Services.Interfaces.Storage
{
    public interface IS3Services
    {
        Task<HttpStatusCode> UploadFileIntoS3Async(MemoryStream stream, string fileName);
        
        Task<byte[]> GetByteOfFileAsync(string key);
        Task<string> GetObjectUrlAsync(string key);
        Task<string> DownloadFileFromS3Async(string key);

        Task<HttpStatusCode> DeleteObjectAsync(string key);
        
    }
}
