using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IEmailService
    {
        public bool IsSendEmail(string toEmail, string subject, string body);
    }
}
