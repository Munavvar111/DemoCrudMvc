using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public int CartMaxQuantity { get; set; }

        public int CartItemCount { get; set; }

        [Required(ErrorMessage ="Please Enter The FirstName.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter The LastName.")]
        public string LastName { get; set; }=null!;

        [Required(ErrorMessage = "Please Enter The Email.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter The ZipCode.")]
        public int ZipCode { get; set; }

        [Required(ErrorMessage = "Please Enter The City.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter The Address.")]
        public string Address { get;set; } = null!; 

        public string CartFileName { get; set; } 
    }

    public class CartList
    {
        public List<CartItems> CartItems { get; set; }
    }
}
