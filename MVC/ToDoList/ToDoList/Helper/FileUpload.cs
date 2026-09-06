namespace Lecture_21.Helper;

public class FileUpload
{
    public string GenerateFileName(IFormFile file)
    {
        return Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
    }

    public string GeneratePath(string fileName)
    {
        return Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "Files",
            fileName);
    }
}