using Centric.Learning.Smoelenboek.Business.Storage;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Centric.Learning.Smoelenboek.Storage
{
    /// <summary>
    /// Filesystem implementation of IPhotoRepository
    /// </summary>
    public class FilesystemPhotoRepository : IPhotoRepository
    {
        private readonly string _rootPath = null;
        private readonly IFileSystem _fileSystem;

        public FilesystemPhotoRepository(IHostEnvironment environment, IFileSystem fileSystem)
        {
            _rootPath = environment.ContentRootPath;
            _fileSystem = fileSystem;
        }

        public async Task<bool> PhotoExists(string name)
        {
            var path = $"/Photos/{name}";
            var localPath = MapPath($"~{path}");

            await Task.CompletedTask;
            return _fileSystem.File.Exists(localPath);
        }

        public async Task UploadPhotoAsync(string name, Stream fileStream, string contentType)
        {
            var directoryName = MapPath("~/Photos");
            if (!_fileSystem.Directory.Exists(directoryName))
            {
                _fileSystem.Directory.CreateDirectory(directoryName);
            }
            using (var destinationStream = _fileSystem.File.Create($"{directoryName}/{name}"))
            {
                await fileStream.CopyToAsync(destinationStream);
            }
        }

        public async Task DeletePhotoAsync(string name)
        {
            _fileSystem.File.Delete(MapPath("~/Photos/" + name));
            await Task.CompletedTask;
        }

        public async Task<byte[]> GetPhotoAsync(string name)
        {
            var directoryName = MapPath("~/Photos");
            return await _fileSystem.File.ReadAllBytesAsync($"{directoryName}/{name}");
        }

        private string MapPath(string path)
        {
            return path.Replace("~", _rootPath);
        }

    }
}
