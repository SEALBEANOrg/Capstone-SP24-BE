using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class S3Services : IS3Services
    {
        private readonly IOptions<AWSCredential> _options;
        
        public S3Services(IOptions<AWSCredential> options)
        {
            _options = options;
        }

        public async Task<HttpStatusCode> UploadFileIntoS3Async(MemoryStream stream, string fileName)
        {
            try
            {
                BasicAWSCredentials awsCredentials = new BasicAWSCredentials(_options.Value.AccessKey, _options.Value.SecretKey);
                var s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(_options.Value.Region));

                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = _options.Value.BucketName,
                    Key = fileName,
                    InputStream = stream
                };

                PutObjectResponse response = await s3Client.PutObjectAsync(request);
                return response.HttpStatusCode;
            }
            catch 
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<string> DownloadFileFromS3Async(string key)
        {
            try
            {
                BasicAWSCredentials awsCredentials = new BasicAWSCredentials(_options.Value.AccessKey, _options.Value.SecretKey);
                var s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(_options.Value.Region));

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _options.Value.BucketName,
                    Key = key
                };

                GetObjectResponse response = await s3Client.GetObjectAsync(request);

                return response.ResponseStream.ToString();
         
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetObjectUrlAsync(string key)
        {
            try
            {
                BasicAWSCredentials awsCredentials = new BasicAWSCredentials(_options.Value.AccessKey, _options.Value.SecretKey);
                var s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(_options.Value.Region));

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = _options.Value.BucketName,
                    Key = key,
                    Expires = DateTime.Now.AddMinutes(30)
                };

                string url = s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi S3Services - GetObjectUrl: " + ex.Message);  
            }
        }

        public async Task<HttpStatusCode> DeleteObjectAsync(string key)
        {
            try
            {
                BasicAWSCredentials awsCredentials = new BasicAWSCredentials(_options.Value.AccessKey, _options.Value.SecretKey);
                var s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(_options.Value.Region));

                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = _options.Value.BucketName,
                    Key = key
                };

                DeleteObjectResponse response = await s3Client.DeleteObjectAsync(request);
                return response.HttpStatusCode;
            }
            catch 
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<byte[]> GetByteOfFileAsync(string key)
        {
            try
            {
                BasicAWSCredentials awsCredentials = new BasicAWSCredentials(_options.Value.AccessKey, _options.Value.SecretKey);
                var s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(_options.Value.Region));

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _options.Value.BucketName,
                    Key = key
                };

                GetObjectResponse response = await s3Client.GetObjectAsync(request);

                byte[] buffer = new byte[16 * 1024];
                using (var ms = new MemoryStream())
                {
                    int read;
                    while ((read = response.ResponseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi S3Services - GetByteOfFile: " + ex.Message);
            }
        }
    }
}
