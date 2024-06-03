using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
	public class ProductVM
	{
		public int ProductId { get; set; }	

		[Required(ErrorMessage = "Please Enter The ProductName.")]
		public string ProductName { get; set; } = null!;

		[Required(ErrorMessage ="Please Enter The Description.")]
		public string ProductDescription { get; set; } = null!;

		[Required(ErrorMessage ="Please Select The Category.")]
		public string CategoryName { get; set; } = null!;

		[Required(ErrorMessage ="Please Enter The Price Of Category.")]
		public int Price { get; set; }

		public string? UniqueNo { get;set; }
        
		[FromForm]
        public List<IFormFile> Files { get; set; }=new List<IFormFile> { };	

        public string? filename { get; set; }
        public List<string> FileNames { get; set; } = new List<string>();

		public bool IsDeleted { get; set; }

		public int Quantity { get; set; }	

		public DateTime DatePicker { get;set; }

		public string? featurePhoto { get;set; }
    }
}
