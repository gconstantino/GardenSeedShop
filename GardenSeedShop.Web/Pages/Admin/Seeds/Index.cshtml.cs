using BestShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BestShop.Pages.Admin.Seeds
{
    public class IndexModel : PageModel
    {
        public List<Seed> Seeds = new List<Seed>();
        public string search = "";
        public int page = 1; //the current html page
        public int totalPages = 0;
        private readonly int pageSize = 10;

        public string column = "id";
        public string order = "desc";
        public void OnGet()
        {
            page = 1;
            search = Request.Query["search"];
            if (search == null)
            {
                search = "";
            }


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

            string[] validColumns = { "id", "name", "type", "subtype", "height", "germination_days", "seed_depth", "plant_spacing", "sun_requirement", "season", "price" };
            column = Request.Query["column"];
            if (column == null || !validColumns.Contains(column))
            {
                column = "id";
            }
            order = Request.Query["order"];
            if (order == null || !order.Equals("desc"))
            {
                order = "asc";
            }
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlWhere = "";
                    if (search.Length > 0)
                    {
                        sqlWhere += " WHERE name LIKE @Search OR subtype LIKE @Search";
                    }

                    string sqlCount = $"SELECT COUNT (*) FROM seeds {sqlWhere}";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {

                        command.Parameters.AddWithValue("@Search", "%" + search + "%");
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = $"SELECT * FROM seeds {sqlWhere}";
                    sql += " ORDER BY " + column + " " + order; //" ORDER BY id ASC";
                    sql += " OFFSET @skip ROWS Fetch NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Search", "%" + search + "%");
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Seed seed = new Seed();
                                seed.Id = reader.GetInt32(0);
                                seed.Name = reader.GetString(1);
                                seed.Type = reader.GetString(2);
                                seed.Subtype = reader.GetString(3);
                                seed.Height = reader.GetInt32(4);
                                seed.GerminationDays = reader.GetInt32(5);
                                seed.SeedDepth = reader.GetString(6);
                                seed.PlantSpacing = reader.GetInt32(7);
                                seed.SunRequirement = reader.GetString(8);
                                seed.Season = reader.GetString(9);
                                seed.Price = reader.GetDecimal(12);
                                seed.ImageFilename = reader.IsDBNull(reader.GetOrdinal("image_filename"))
                                                        ? null
                                                        : reader.GetString(reader.GetOrdinal("image_filename"));
                                seed.CreatedAt = reader.GetDateTime(11);

                                Seeds.Add(seed);
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
