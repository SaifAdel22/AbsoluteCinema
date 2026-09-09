using Microsoft.VisualBasic.FileIO;


namespace AbsoluteCinema.Helper
{

    public enum FileType
    {
        Img = 1,
        Pdf = 2,
        Uploads = 3
    }

    public interface IFileUpload
    {
        string GenerateFileName(string fileName);
        string? GenerateFullPath(FileType fileType, string fileName);
        bool UploadFileLocally(string path, IFormFile file);
        bool DeleteFileLocally(string path);

        string? SaveFile(IFormFile? file, FileType fileType);
        string? UpdateFile(IFormFile? newFile, string? oldRelativePath, FileType fileType);
    }

}
