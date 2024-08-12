using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BestShop.Pages.Admin.Seeds
{
    public class AddNewModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The Name is Required")]
        [MaxLength(100, ErrorMessage = "The Name cannot exceed 100 characters")]
        public string Name { get; set; } = "";

        [BindProperty, Required]
        public string Type { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The SubType is Required")]
        [MaxLength(100, ErrorMessage = "The Subtype cannot exceed 100 characters")]
        public string Subtype { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The Height is Required")]
        [Range(1, 10000, ErrorMessage = "The Height must be in the range from 1 to 10000")]
        public int Height { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The Germination Days is Required")]
        [Range(0, 10000, ErrorMessage = "The Germination Days must be in the range from 0 to 10000")]
        public int GerminationDays { get; set; }

        [BindProperty]
        [Required (ErrorMessage = "The Seed Depth is Required")]
        
        public string SeedDepth { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The Plant Spacing is Required")]
        [Range(0, 10000, ErrorMessage = "The Plant Spacing must be in the range from 0 to 10000")]
        public int PlantSpacing { get; set; }

        [BindProperty, Required]
        
        public string SunRequirement { get; set; } = "";

        [BindProperty, Required]
        public string Season { get; set; } = "";

        [BindProperty]

        public string? ImageFilename { get; set; } = "";

        [BindProperty]
      
        public DateTime CreatedAt { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The Price is Required")]
        public decimal Price { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;
        public AddNewModel(IWebHostEnvironment env)
        {
        }

        public void OnGet()
        {
        }

        public void OnPost() 
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }
            if (ImageFilename == null) ImageFilename = "";

            successMessage = "Data saved correctly";
                
            
            //successful data validation


        }
    }
}
