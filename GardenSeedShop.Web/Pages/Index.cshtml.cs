using BestShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;



namespace BestShop.Pages
{
    public class IndexModel : PageModel
    {
        public List<Models.Seed> listNewestSeeds = new List<Models.Seed>();
        public List<Models.Seed> listTopSales = new List<Models.Seed>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT TOP 4 * FROM seeds ORDER BY created_at DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Seed seedInfo = new Seed();
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

                                listNewestSeeds.Add(seedInfo);
                            }
                        }
                    }

                    sql = "SELECT TOP 4 seeds.*, (" +
                       "SELECT SUM(order_items.quantity) FROM order_items WHERE seeds.id = order_items.seed_id" +
                       ") AS total_sales " +
                       "FROM seeds " +
                       "ORDER BY total_sales DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Seed seedInfo = new Seed();
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

                                listTopSales.Add(seedInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}


