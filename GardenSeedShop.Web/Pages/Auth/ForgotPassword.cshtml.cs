using GardenSeedShop.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GardenSeedShop.Web.Pages.Auth
{
    [RequireNoAuth]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IGardenShopEmailSender _emailSender;

        public ForgotPasswordModel(IGardenShopEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [BindProperty, Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet() { 
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // 1) create token, 2) save token in the database, 3) send token by email to the user
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM users WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstname = reader.GetString(1);
                                string lastname = reader.GetString(2);

                                string token = Guid.NewGuid().ToString();

                                
                                SaveToken(Email, token);

                                
                                string resetUrl = Url.PageLink("/Auth/ResetPassword") + "?token=" + token;
                                string username = firstname + " " + lastname;
                                string subject = "Password Reset";
                                string message = "Dear " + username + ",\n\n" +
                                    "You can reset your password using the following link:\n\n" +
                                    resetUrl + "\n\n" +
                                    "Best Regards";

                                
                                _emailSender.SendEmail(Email, username, subject, message).Wait();
                            }
                            else
                            {
                                errorMessage = "We have no user with this email address";
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Please check your email and click on the reset password link";
        }

        private void SaveToken(string email, string token)
        {
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                   
                    string sql = "DELETE FROM password_resets WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }

                    
                    sql = "INSERT INTO password_resets (email, token) VALUES (@email, @token)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@token", token);

                        command.ExecuteNonQuery();
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

