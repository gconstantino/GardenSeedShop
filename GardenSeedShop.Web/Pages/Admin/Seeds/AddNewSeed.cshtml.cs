using GardenSeedShop.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Admin.Seeds
{
    [RequireAuth(RequiredRole = "admin")]
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
        [Required(ErrorMessage = "The Seed Depth is Required")]

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
        [Required(ErrorMessage = "The Image File is required")]
        public IFormFile ImageFile { get; set; }


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
            webHostEnvironment = env;
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

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ImageFile.FileName);

            string imageFolder = webHostEnvironment.WebRootPath + "/images/seeds/";

            string imageFullPath = Path.Combine(imageFolder, newFileName);
            Console.WriteLine("New image: " + imageFullPath);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ImageFile.CopyTo(stream);
            }

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    string sql = "INSERT INTO seeds " +
                    "(name, type, subtype, height, germination_days, seed_depth, plant_spacing, sun_requirement, season, price, image_filename) VALUES " +
                    "(@name, @type, @subtype, @height, @germination_days, @seed_depth, @plant_spacing, @sun_requirement, @season, @price, @image_filename);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", Name);
                        command.Parameters.AddWithValue("@type", Type);
                        command.Parameters.AddWithValue("@subtype", Subtype);
                        command.Parameters.AddWithValue("@height", Height);
                        command.Parameters.AddWithValue("@germination_days", GerminationDays);
                        command.Parameters.AddWithValue("@seed_depth", SeedDepth);
                        command.Parameters.AddWithValue("@plant_spacing", PlantSpacing);
                        command.Parameters.AddWithValue("@sun_requirement", SunRequirement);
                        command.Parameters.AddWithValue("@season", Season);
                        command.Parameters.AddWithValue("@image_filename", newFileName);
                        command.Parameters.AddWithValue("@price", Price);


                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Data saved correctly";
            Response.Redirect("/Admin/Seeds/Index");

        }
    }
}
