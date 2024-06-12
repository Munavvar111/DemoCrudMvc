using BAL.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repositry
{
    public class UploadFileRepo:IUploadFile
    {
        public  string UploadFile(IFormFile File)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            var UniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);
            var FilePath = Path.Combine(uploadsFolder, UniqueFileName);

            using (var Stream = new FileStream(FilePath, FileMode.Create))
            {
                 File.CopyToAsync(Stream);
            }
            return UniqueFileName;    
        }
    }
}
