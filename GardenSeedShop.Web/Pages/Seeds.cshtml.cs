using BestShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GardenSeedShop.Web.Pages
{
    [BindProperties(SupportsGet = true)]
    public class SeedsModel : PageModel
    {
        public string? Search { get; set; }
        public string PriceRange { get; set; } = "any";
        public string Type { get; set; } = "any";
        public string SunRequirement { get; set; } = "any";
        public string Season { get; set; } = "any";

        public List<Seed> listSeeds = new List<Seed>();

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // seeds per page

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlCount = "SELECT COUNT(*) FROM seeds";
                    sqlCount += " WHERE (name LIKE @search)";

                    if (PriceRange.Equals("0_2"))
                    {
                        sqlCount += " AND price <= 2";
                    }
                    else if (PriceRange.Equals("3_5"))
                    {
                        sqlCount += " AND price >= 3 AND price <= 5";
                    }
                    else if (PriceRange.Equals("above5"))
                    {
                        sqlCount += " AND price >= 5";
                    }


                    if (!Type.Equals("any"))
                    {
                        sqlCount += " AND type=@type";
                    }

                    if (!SunRequirement.Equals("any"))
                    {
                        sqlCount += " AND Sun_Requirement=@SunRequirement";
                    }

                    if (!Season.Equals("any"))
                    {
                        sqlCount += " AND Season=@Season";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        command.Parameters.AddWithValue("@type", Type);
                        command.Parameters.AddWithValue("@SunRequirement", SunRequirement);
                        command.Parameters.AddWithValue("@Season", Season);


                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM seeds";
                    sql += " WHERE (name LIKE @search)";

                    if (PriceRange.Equals("0_2"))
                    {
                        sql += " AND price <= 2";
                    }
                    else if (PriceRange.Equals("3_5"))
                    {
                        sql += " AND price >= 3 AND price <= 5";
                    }
                    else if (PriceRange.Equals("above5"))
                    {
                        sql += " AND price >= 5";
                    }


                    if (!Type.Equals("any"))
                    {
                        sql += " AND type=@type";
                    }

                    if (!SunRequirement.Equals("any"))
                    {
                        sql += " AND Sun_Requirement=@SunRequirement";
                    }

                    if (!Season.Equals("any"))
                    {
                        sql += " AND Season=@Season";
                    }

                    sql += " ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        command.Parameters.AddWithValue("@type", Type);
                        command.Parameters.AddWithValue("@SunRequirement", SunRequirement);
                        command.Parameters.AddWithValue("@Season", Season);
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

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

                                listSeeds.Add(seedInfo);

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

