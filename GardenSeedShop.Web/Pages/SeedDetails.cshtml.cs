using BestShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GardenSeedShop.Web.Pages
{
    public class SeedDetailsModel : PageModel
    {
        public Seed seedInfo = new Seed();

        public void OnGet(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/");
                return;
            }

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM seeds WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                seedInfo.Id = reader.GetInt32(0);
                                seedInfo.Name = reader.GetString(1);
                                seedInfo.Type = reader.GetString(2);
                                seedInfo.Subtype = reader.GetString(3);
                                seedInfo.Height = reader.GetInt32(4);
                                seedInfo.GerminationDays = reader.GetInt32(5);
                                seedInfo.SeedDepth = reader.GetString(6);
                                seedInfo.PlantSpacing = reader.GetInt32(7);
                                seedInfo.SunRequirement = reader.GetString(8);
                                seedInfo.Season = reader.GetString(9);
                                seedInfo.Price = reader.GetDecimal(12);
                                seedInfo.ImageFilename = reader.IsDBNull(reader.GetOrdinal("image_filename"))
                                                        ? null
                                                        : reader.GetString(reader.GetOrdinal("image_filename"));
                                seedInfo.CreatedAt = reader.GetDateTime(11);
                            }
                            else
                            {
                                Response.Redirect("/");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/");
                return;
            }
        }
    }
}
