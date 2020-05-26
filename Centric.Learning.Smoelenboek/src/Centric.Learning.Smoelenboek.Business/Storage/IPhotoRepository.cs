using System.IO;
using System.Threading.Tasks;

namespace Centric.Learning.Smoelenboek.Business.Storage
{
    /// <summary>
    /// Photo Repository Interface
    /// </summary>
    public interface IPhotoRepository
    {
        /// <summary>
        /// Upload a photo
        /// </summary>
        /// <param name="name">name of the photo</param>
        /// <param name="fileStream">stream with photo content</param>
        /// <param name="contentType">content type</param>
        /// <returns></returns>
        Task UploadPhotoAsync(string name, Stream fileStream, string contentType);

        /// <summary>
        /// Delete a photo
        /// </summary>
        /// <param name="name">name of the photo</param>
        /// <returns></returns>
        Task DeletePhotoAsync(string name);

        /// <summary>
        /// Check whether a photo exists
        /// </summary>
        /// <param name="name">name of the photo</param>
        /// <returns>boolean</returns>
        Task<bool> PhotoExists(string name);

        /// <summary>
        /// Get the photo content
        /// </summary>
        /// <param name="name">name of the photo</param>
        /// <returns>content in byte[]</returns>
        Task<byte[]> GetPhotoAsync(string name);

    }
}