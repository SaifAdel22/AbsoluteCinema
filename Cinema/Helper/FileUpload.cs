using Microsoft.VisualBasic.FileIO;
using AbsoluteCinema;
namespace AbsoluteCinema.Helper
{
    
        public class FileUpload : IFileUpload
        {
            public string GenerateFileName(string fileName)
            {
                return $"{Guid.NewGuid()}-{DateTime.Now:dd-MM-yyyy}{Path.GetExtension(fileName)}";
            }

            public string? GenerateFullPath(FileType fileType, string fileName)
            {
                string folderName = fileType switch
                {
                    FileType.Img => "imgs",
                    FileType.Pdf => "pdfs",
                    _ => "uploads"
                };

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName, fileName);

                string? directoryPath = Path.GetDirectoryName(filePath);
                if (directoryPath != null && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return filePath;
            }

            public bool UploadFileLocally(string path, IFormFile file)
            {
                if (file == null || file.Length == 0) return false;

                using (var stream = System.IO.File.Create(path))
                {
                    file.CopyTo(stream);
                    return true;
                }
            }

            public bool DeleteFileLocally(string path)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }

                return false;
            }


            public string? SaveFile(IFormFile? file, FileType fileType)
            {
                if (file == null || file.Length == 0) return null;

                string generatedName = GenerateFileName(file.FileName);
                string? fullPath = GenerateFullPath(fileType, generatedName);

                if (fullPath == null) return null;

                bool isUploaded = UploadFileLocally(fullPath, file);
                if (!isUploaded) return null;

                string folderName = fileType == FileType.Img ? "imgs" : "pdfs";
                return $"/{folderName}/{generatedName}";
            }

            public string? UpdateFile(IFormFile? newFile, string? oldRelativePath, FileType fileType)
            {
                if (newFile == null || newFile.Length == 0)
                {
                    return oldRelativePath;
                }

                if (!string.IsNullOrEmpty(oldRelativePath))
                {
                    string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldRelativePath.TrimStart('/'));
                    DeleteFileLocally(oldFullPath);
                }

                return SaveFile(newFile, fileType);
            }
        }
    }

