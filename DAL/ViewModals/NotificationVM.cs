using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
    public class NotificationVM
    {
        public string CustomerName { get; set; } = null!;

        public string OrderId { get; set; } = null!;    

        public TimeOnly OrderTime { get; set; }

    }
}
