using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Shared.Interface;
namespace Project;
public class FileService(IWebHostEnvironment environment) : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            throw new ArgumentException("Invalid file.");
        }

        var contentPath = environment.ContentRootPath;
        var uploadPath = Path.Combine(contentPath, "wwwroot");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        // ფაილის სახელი უნიკალური რომ იყოს, ვიყენებთ `Guid.NewGuid()`
        var fileExtension = Path.GetExtension(imageFile.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        Console.WriteLine($"File saved: {filePath}");

        return fileName;
    }


    public void ModifyFile(string fileNameWithExtension)
    {
        throw new NotImplementedException();
    }


    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }

        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "wwwroot", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {fileNameWithExtension}");
        }

        try
        {
            File.Delete(path);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Error deleting file: {fileNameWithExtension}", ex);
        }
    }


    public async Task<byte[]> GetFileAsync(string imageName)
    {
        if (string.IsNullOrEmpty(imageName) || imageName.Contains(".."))
        {
            throw new ArgumentException("Invalid file name.");
        }

        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "wwwroot", imageName);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {imageName}");
        }

        return await File.ReadAllBytesAsync(path);
    }

}