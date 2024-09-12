using GardenSeedShop.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GardenSeedShop.Web.Pages.Auth
{
    [RequireNoAuth]
    public class ResetPasswordModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        public string Password { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            string token = Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/");
                return;
            }
        }
        public void OnPost()
        {
            string token = Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/");
                return;
            }

            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }


            
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                   
                    string email = "";
                    string sql = "SELECT * FROM password_resets WHERE token=@token";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@token", token);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                email = reader.GetString(0);
                            }
                            else
                            {
                                errorMessage = "Wrong or Expired Token";
                                return;
                            }
                        }
                    }

                    
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                    sql = "UPDATE users SET password=@password WHERE email=@email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }

                  
                    sql = "DELETE FROM password_resets WHERE email=@email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Password reset successfully";
        }
    }
}
