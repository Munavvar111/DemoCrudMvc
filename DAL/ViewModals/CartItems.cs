using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
    public class CartItems
    {
        public int ProductId { get; set; }

        public string CartItemName { get; set; }    

        public int CartItemPrice { get; set; }

        public int CartItemQuantity { get; set; }   

        public int CartItemCount { get; set;}

        public string CartFileName { get; set; }    
    }
}
