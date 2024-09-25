using BestShop.Models;
using GardenSeedShop.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GardenSeedShop.Web.Pages
{
    public class CartModel : PageModel


    {


        [Required(ErrorMessage = "The Address is required")]
        public string Address { get; set; } = "";

        [Required]
        public string PaymentMethod { get; set; } = "";

        public List<OrderItem> listOrderItems = new List<OrderItem>();
        public decimal subtotal = 0;
        public decimal shippingFee = 5;
        public decimal total = 0;
        private Dictionary<String, int> getSeedDictionary()
        {
            var seedDictionary = new Dictionary<string, int>();

           

                string cookieValue = Request.Cookies["shopping_cart"] ?? "";

                if (cookieValue.Length > 0)
                {
                    string[] seedIdArray = cookieValue.Split('-');

                    for (int i = 0; i < seedIdArray.Length; i++)
                    {
                        string bookId = seedIdArray[i];
                        if (seedDictionary.ContainsKey(bookId))
                        {
                            seedDictionary[bookId] += 1;
                        }
                        else
                        {
                            seedDictionary.Add(bookId, 1);
                        }
                    }
                }

                return seedDictionary;
            }
            public void OnGet()
            {
                var seedDictionary = getSeedDictionary();

            string? action = Request.Query["action"];
            string? id = Request.Query["id"];

            if (action != null && id != null && seedDictionary.ContainsKey(id))
            {
                if (action.Equals("add"))
                {
                    seedDictionary[id] += 1;
                }
                else if (action.Equals("sub"))
                {
                    if (seedDictionary[id] > 1) seedDictionary[id] -= 1;
                }
                else if (action.Equals("delete"))
                {
                    seedDictionary.Remove(id);
                }

                string newCookieValue = "";
                foreach (var keyValuePair in seedDictionary)
                {
                    for (int i = 0; i < keyValuePair.Value; i++)
                    {
                        newCookieValue += "-" + keyValuePair.Key;
                    }
                }

                if (newCookieValue.Length > 0)
                    newCookieValue = newCookieValue.Substring(1);

                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(365);
                cookieOptions.Path = "/";

                Response.Cookies.Append("shopping_cart", newCookieValue, cookieOptions);

                Response.Redirect(Request.Path.ToString());
                return;
            }

                try
                {
                    string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM seeds WHERE id=@id";
                        foreach (var keyValuePair in seedDictionary)
                        {
                            string seedID = keyValuePair.Key;
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@id", seedID);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        OrderItem item = new OrderItem();

                                        item.seedInfo.Id = reader.GetInt32(0);
                                        item.seedInfo.Name = reader.GetString(1);
                                        item.seedInfo.Type = reader.GetString(2);
                                        item.seedInfo.Subtype = reader.GetString(3);
                                        item.seedInfo.Height = reader.GetInt32(4);
                                        item.seedInfo.GerminationDays = reader.GetInt32(5);
                                        item.seedInfo.SeedDepth = reader.GetString(6);
                                        item.seedInfo.PlantSpacing = reader.GetInt32(7);
                                        item.seedInfo.SunRequirement = reader.GetString(8);
                                        item.seedInfo.Season = reader.GetString(9);
                                        item.seedInfo.Price = reader.GetDecimal(12);
                                        item.seedInfo.ImageFilename = reader.IsDBNull(reader.GetOrdinal("image_filename"))
                                                                ? null
                                                                : reader.GetString(reader.GetOrdinal("image_filename"));
                                        item.seedInfo.CreatedAt = reader.GetDateTime(11);

                                        item.numPackets = keyValuePair.Value;
                                        item.totalPrice = item.numPackets * item.seedInfo.Price;

                                        listOrderItems.Add(item);

                                        subtotal += item.totalPrice;
                                        total = subtotal + shippingFee;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Address = HttpContext.Session.GetString("address") ?? "";
            }

        public string errorMessage = "";
        public string successMessage = "";

        public void OnPost()
            {
            int client_id = HttpContext.Session.GetInt32("id") ?? 0;
            if (client_id < 1)
            {
                Response.Redirect("/Auth/Login");
                return;
            }

            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            var seedDictionary = getSeedDictionary();
            if (seedDictionary.Count < 1)
            {
                errorMessage = "Your cart is empty";
                return;
            }
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    
                    int newOrderId = 0;
                    string sqlOrder = "INSERT INTO orders (client_id, order_date, shipping_fee, " +
                        "delivery_address, payment_method, payment_status, order_status) " +
                        "OUTPUT INSERTED.id " +
                        "VALUES (@client_id, CURRENT_TIMESTAMP, @shipping_fee, " +
                        "@delivery_address, @payment_method, 'pending', 'created')";

                    using (SqlCommand command = new SqlCommand(sqlOrder, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", client_id);
                        command.Parameters.AddWithValue("@shipping_fee", shippingFee);
                        command.Parameters.AddWithValue("@delivery_address", Address);
                        command.Parameters.AddWithValue("@payment_method", PaymentMethod);

                        newOrderId = (int)command.ExecuteScalar();
                    }


                    string sqlItem = "INSERT INTO order_items (order_id, seed_id, quantity, unit_price) " +
                        "VALUES (@order_id, @seed_id, @quantity, @unit_price)";

                    foreach (var keyValuePair in seedDictionary)
                    {
                        string seedID = keyValuePair.Key;
                        int quantity = keyValuePair.Value;
                        decimal unitPrice = getSeedPrice(seedID);

                        using (SqlCommand command = new SqlCommand(sqlItem, connection))
                        {
                            command.Parameters.AddWithValue("@order_id", newOrderId);
                            command.Parameters.AddWithValue("@seed_id", seedID);
                            command.Parameters.AddWithValue("@quantity", quantity);
                            command.Parameters.AddWithValue("@unit_price", unitPrice);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Cookies.Delete("shopping_cart");

            successMessage = "Order created successfully";

        }
        private decimal getSeedPrice(string seedID)
        {
            decimal price = 0;

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT price FROM seeds WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", seedID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                price = reader.GetDecimal(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return price;
        }




        public class OrderItem
        {
            public Seed seedInfo = new Seed();
            public int numPackets = 0;
            public decimal totalPrice = 0;
        }
    }
}

