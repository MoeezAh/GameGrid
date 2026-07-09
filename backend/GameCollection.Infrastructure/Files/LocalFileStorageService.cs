using System;
using System.IO;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;

namespace GameCollection.Infrastructure.Files;

public class LocalFileStorageService : IFileStorageService
{
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
    {
        var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsPath = Path.Combine(wwwRootPath, "uploads", folderName);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        // Clean filename of spaces
        var cleanFileName = fileName.Replace(" ", "_");
        var uniqueFileName = $"{Guid.NewGuid()}_{cleanFileName}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // Return relative web URL (standard forward slashes)
        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    public void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;

        // Convert relative URL back to local path
        var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(wwwRootPath, relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
