using Centric.Learning.Smoelenboek.Business.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Centric.Learning.Smoelenboek.Storage
{
    /// <summary>
    /// Blob Storage implementation of IPhotoRepository
    /// </summary>
    public class BlobStoragePhotoRepository : IPhotoRepository
    {
        protected readonly CloudBlobContainer _cloudBlobContainer;

        protected virtual string ContainerName => "public-photos";

        public BlobStoragePhotoRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("StorageConnection");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create a blob client for interacting with the blob service.
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container for organizing blobs within the storage account.
            _cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName);

            _cloudBlobContainer.CreateIfNotExistsAsync().Wait();
            _cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }).Wait();
        }

        public async Task<bool> PhotoExists(string fileName)
        {
            var blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
            return await blockBlob.ExistsAsync();
        }

        public async Task<byte[]> GetPhotoAsync(string fileName)
        {
            var blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
            MemoryStream ms = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(ms);
            return ms.ToArray();
        }


        public async Task UploadPhotoAsync(string fileName, Stream fileStream, string contentType)
        {
            var blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);

            // Set the blob's content type so that the browser knows to treat it as an image.
            blockBlob.Properties.ContentType = contentType;

            await blockBlob.UploadFromStreamAsync(fileStream);
        }

        public async Task DeletePhotoAsync(string fileName)
        {
            var blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
            await blockBlob.DeleteIfExistsAsync();
        }
    }
}
