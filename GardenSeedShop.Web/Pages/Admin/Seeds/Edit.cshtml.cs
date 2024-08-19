using BestShop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GardenSeedShop.Web.Pages.Admin.Seeds
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }

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
        public string ImageFileName { get; set; } = "";

        [BindProperty]
        public IFormFile? ImageFile { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "The Price is Required")]
        public decimal Price { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public EditModel(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }


        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM seeds WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Id = reader.GetInt32(0);
                                Name = reader.GetString(1);
                                Type = reader.GetString(2);
                                Subtype = reader.GetString(3);
                                Height = reader.GetInt32(4);
                                GerminationDays = reader.GetInt32(5);
                                SeedDepth = reader.GetString(6);
                                PlantSpacing = reader.GetInt32(7);
                                SunRequirement = reader.GetString(8);
                                Season = reader.GetString(9);
                                Price = reader.GetDecimal(12);
                                ImageFileName = reader.IsDBNull(reader.GetOrdinal("image_filename"))
                                                        ? null
                                                        : reader.GetString(reader.GetOrdinal("image_filename"));
                            }
                            else
                            {
                                Response.Redirect("/Admin/Seeds/Index");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Seeds/Index");
            }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }
            // if we have a new ImageFile => upload the new image and delete the old image
            string newFileName = ImageFileName;
            if (ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(ImageFile.FileName);

                string imageFolder = webHostEnvironment.WebRootPath + "/images/seeds/";
                string imageFullPath = Path.Combine(imageFolder, newFileName);
                Console.WriteLine("New image (Edit): " + imageFullPath);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    ImageFile.CopyTo(stream);
                }

                // delete old image
                string oldImageFullPath = Path.Combine(imageFolder, ImageFileName);
                System.IO.File.Delete(oldImageFullPath);
                Console.WriteLine("Delete Image " + oldImageFullPath);
            }

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE seeds SET name=@name, type=@type, subtype=@subtype, " +
                        "height=@height, germination_days=@germination_days, seed_depth=@seed_depth, " +
                        "plant_spacing=@plant_spacing,sun_requirement=@sun_requirement, season=@season, price=@price, image_filename=@image_filename WHERE id=@id;";

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
                        command.Parameters.AddWithValue("@id", Id);

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
