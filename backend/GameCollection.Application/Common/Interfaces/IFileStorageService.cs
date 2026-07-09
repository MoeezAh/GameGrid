using System.IO;
using System.Threading.Tasks;

namespace GameCollection.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
    void DeleteFile(string filePath);
}
