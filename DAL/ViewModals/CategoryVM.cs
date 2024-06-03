using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
    public class CategoryVM
    {
        public int CategoriesID { get; set; }

        [Required(ErrorMessage = "Please Enter The categoryName.")]
        public string CategoriesName { get; set; } = null!;


        [Required(ErrorMessage = "Please Enter The CategoryDescription.")]
        public string CategoriesDescription { get; set; } = null!;

        public bool IsCategoryDeleted { get; set; } 
    }
}
