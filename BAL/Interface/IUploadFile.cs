using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IUploadFile
    {
        public  string UploadFile(IFormFile File);
    }
}
