using Microsoft.Extensions.Hosting;
using Moq;
using System.IO;
using System.IO.Abstractions;
using Xunit;

namespace Centric.Learning.Smoelenboek.Storage.Photos.Filesystem.Tests
{
    public class FileSystemPhotoRepositoryTests
    {
        private const string PhotoDirectory = @"/Photos";
        protected FilesystemPhotoRepository CreateSystemUnderTest(Mock<IHostEnvironment> hostEnvironment = null, Mock<IFileSystem> fileSystem = null)
        {
            if (hostEnvironment == null)
                hostEnvironment = new Mock<IHostEnvironment>();

            if (fileSystem == null)
                fileSystem = new Mock<IFileSystem>();

            var sut = new FilesystemPhotoRepository(hostEnvironment.Object, fileSystem.Object);

            return sut;
        }

        public class ConstructorTests : FileSystemPhotoRepositoryTests
        {
            [Fact]
            public void WhenCreatingANewInstance_NoExceptionsOccur()
            {
                // arrange
                var hostEnvironment = new Mock<IHostEnvironment>();
                var fileSystem = new Mock<IFileSystem>();

                // act
                new FilesystemPhotoRepository(hostEnvironment.Object, fileSystem.Object);

                // assert
                Assert.True(true);
            }
        }

        public class PhotoExistsTests : FileSystemPhotoRepositoryTests
        {
            [Fact]
            public void WhenPhotoExists_ReturnsTrue()
            {
                // arrange
                var fileName = "SomeFileName";

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.File.Exists(It.Is<string>(s => s.Contains(fileName)))).Returns(true);

                var sut = CreateSystemUnderTest(fileSystem: fileSystem);

                // act
                var resultTask = sut.PhotoExists(fileName);

                // assert
                Assert.True(resultTask.Result, $"Photo with name {fileName} should have existed.");
            }

            [Fact]
            public void WhenPhotoDoesNotExist_ReturnsFalse()
            {
                // arrange
                var fileName = "SomeFileName";

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.File.Exists(It.Is<string>(s => s.Contains(fileName)))).Returns(false);

                var sut = CreateSystemUnderTest(fileSystem: fileSystem);

                // act
                var resultTask = sut.PhotoExists(fileName);

                // assert
                Assert.False(resultTask.Result, $"Photo with name {fileName} should not have existed.");
            }

            [Fact]
            public void WhenCalled_ChecksForPhotoInCorrectPath()
            {
                // arrange
                var fileName = "SomeFileName";
                var contentRoot = @"E:\SomeFolder";                
                var expectedFullPath = Path.GetFullPath(Path.Join($@"{contentRoot}{PhotoDirectory}", fileName));

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.File.Exists(It.IsAny<string>()));

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var resultTask = sut.PhotoExists(fileName);

                // assert
                fileSystem.Verify(f => f.File.Exists(It.Is<string>(p => Path.GetFullPath(p).Equals(expectedFullPath))), Times.Once, $"The file {fileName} was not searched for in the right path {expectedFullPath}");
            }
        }

        public class UploadPhotoAsyncTests : FileSystemPhotoRepositoryTests
        {
            [Fact]
            public void WhenPhotoDirectoryDoesNotExist_DirectoryIsCreated()
            {
                // arrange
                var sourceName = "SomeFileName";
                var sourceStream = new MemoryStream();
                var destinationStream = new MemoryStream();
                var contentType = string.Empty;
                var contentRoot = @"E:\SomeFolder";
                var expectedDirectoryPath = Path.GetFullPath($@"{contentRoot}{PhotoDirectory}");

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(false);
                fileSystem.Setup(f => f.File.Create(It.IsAny<string>())).Returns(destinationStream);

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var result = sut.UploadPhotoAsync(sourceName, sourceStream, contentType);

                // assert
                fileSystem.Verify(f => f.Directory.CreateDirectory(It.Is<string>(p => Path.GetFullPath(p).Equals(expectedDirectoryPath))), Times.Once);
            }

            [Fact]
            public void WhenPhotoDirectoryExists_DirectoryIsNotCreated()
            {
                // arrange
                var sourceName = "SomeFileName";
                var sourceStream = new MemoryStream();
                var destinationStream = new MemoryStream();
                var contentType = string.Empty;
                var contentRoot = @"E:\SomeFolder";
                var expectedDirectoryPath = Path.GetFullPath($@"{contentRoot}{PhotoDirectory}");

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
                fileSystem.Setup(f => f.File.Create(It.IsAny<string>())).Returns(destinationStream);

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var result = sut.UploadPhotoAsync(sourceName, sourceStream, contentType);

                // assert
                fileSystem.Verify(f => f.Directory.CreateDirectory(It.Is<string>(p => Path.GetFullPath(p).Equals(expectedDirectoryPath))), Times.Never);
            }

            [Fact]
            public void WhenCalled_CorrectFileIsCreated()
            {
                // arrange
                var sourceName = "SomeFileName";
                var sourceStream = new MemoryStream();
                var destinationStream = new MemoryStream();
                var contentType = string.Empty;
                var contentRoot = @"E:\SomeFolder";
                var expectedFilePath = Path.GetFullPath(Path.Join($@"{contentRoot}{PhotoDirectory}", sourceName));

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
                fileSystem.Setup(f => f.File.Create(It.Is<string>(f => Path.GetFullPath(f).Equals(expectedFilePath)))).Returns(destinationStream);

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var result = sut.UploadPhotoAsync(sourceName, sourceStream, contentType);

                // assert
                fileSystem.Verify(f => f.File.Create(It.Is<string>(f => Path.GetFullPath(f).Equals(expectedFilePath))), Times.Once);
            }

            [Fact]
            public void WhenCalled_StreamIsWrittenToFile()
            {
                // arrange
                var sourceName = "SomeFileName";
                var sourceContents = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                var sourceStream = new MemoryStream(sourceContents);
                var destinationStream = new MemoryStream();
                var contentType = string.Empty;
                var contentRoot = @"E:\SomeFolder";
                var expectedFilePath = Path.GetFullPath(Path.Join($@"{contentRoot}{PhotoDirectory}", sourceName));

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
                fileSystem.Setup(f => f.File.Create(It.IsAny<string>())).Returns(destinationStream);

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var resultTask = sut.UploadPhotoAsync(sourceName, sourceStream, contentType);

                // assert
                Assert.Equal(sourceContents, destinationStream.ToArray());
            }
        }

        public class DeletePhotoAsyncTests : FileSystemPhotoRepositoryTests
        {
            [Fact]
            public void WhenCalled_ThenCorrectFileIsDeleted()
            {
                // arrange
                var sourceName = "SomeFileName";
                var contentRoot = @"E:\SomeFolder";
                var expectedFilePath = Path.GetFullPath(Path.Join($@"{contentRoot}{PhotoDirectory}", sourceName));

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.File.Delete(It.IsAny<string>()));

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var resultTask = sut.DeletePhotoAsync(sourceName);

                // assert
                fileSystem.Verify(f => f.File.Delete(It.Is<string>(f => Path.GetFullPath(f).Equals(expectedFilePath))), Times.Once);
            }
        }

        public class GetPhotoAsyncTests : FileSystemPhotoRepositoryTests
        {
            [Fact]
            public void WhenCalled_ReadsFromCorrectFile()
            {
                // arrange
                var sourceName = "SomeFileName";
                var contentRoot = @"E:\SomeFolder";
                var expectedFilePath = Path.GetFullPath(Path.Join($@"{contentRoot}{PhotoDirectory}", sourceName));

                var fileSystem = new Mock<IFileSystem>();
                fileSystem.Setup(f => f.File.ReadAllBytesAsync(It.IsAny<string>(),default));

                var hostEnvironment = new Mock<IHostEnvironment>();
                hostEnvironment.SetupGet(h => h.ContentRootPath).Returns(contentRoot);

                var sut = CreateSystemUnderTest(hostEnvironment: hostEnvironment, fileSystem: fileSystem);

                // act
                var result = sut.GetPhotoAsync(sourceName);

                // assert
                fileSystem.Verify(f => f.File.ReadAllBytesAsync(It.Is<string>(f => Path.GetFullPath(f).Equals(expectedFilePath)),default), Times.Once);
            }
        }
    }
}
