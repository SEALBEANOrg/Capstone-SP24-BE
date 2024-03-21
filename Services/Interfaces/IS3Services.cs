using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IS3Services
    {
        Task<HttpStatusCode> DeleteObjectAsync(string key);
        Task<string> DownloadFileFromS3Async(string key);
        Task<byte[]> GetByteOfFileAsync(string key);
        Task<string> GetObjectUrlAsync(string key);
        Task<HttpStatusCode> UploadFileIntoS3Async(MemoryStream stream, string fileName);
    }
}
