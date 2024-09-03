using BestShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages
{
	public class ContactModel : PageModel
	{
		private readonly IGardenShopEmailSender _emailSender;

		public ContactModel(IGardenShopEmailSender emailSender)
		{
			_emailSender = emailSender;
		}

		public void OnGet()
		{
		}
		[BindProperty]
		[Required(ErrorMessage = "The First Name is required")]
		[Display(Name = "First Name*")]
		public string FirstName { get; set; } = "";


		[BindProperty, Required(ErrorMessage = "The Last Name is required")]
		[Display(Name = "Last Name*")]
		public string LastName { get; set; } = "";


		[BindProperty, Required(ErrorMessage = "The Email is required")]
		[EmailAddress]
		[Display(Name = "Email*")]
		public string Email { get; set; } = "";


		[BindProperty]
		public string? Phone { get; set; } = "";


		[BindProperty, Display(Name = "Subject*")]
		public string Subject { get; set; } = "";

		[BindProperty, Required(ErrorMessage = "The Message is required")]
		[MinLength(5, ErrorMessage = "The Message should be at least 5 characters")]
		[MaxLength(1024, ErrorMessage = "The Message should be less than 1024 characters")]
		[Display(Name = "Message*")]
		public string Message { get; set; } = "";

		public List<SelectListItem> SubjectList { get; } = new List<SelectListItem>
		{
			new SelectListItem { Value = "Order Status", Text = "Order Status"},
			new SelectListItem { Value = "Refund Request", Text = "Refund Request"},
			new SelectListItem { Value = "Job Application", Text = "Job Application" },
			new SelectListItem { Value = "Other", Text = "Other" },
		};

		public string SuccessMessage { get; set; } = "";

		public string ErrorMessage { get; set; } = "";

		public void OnPost()
		{
			// Check if any required field is empty
			if (!ModelState.IsValid)

			{
				//Error
				ErrorMessage = "Please fill all required fields";
				return;
			}

			if (Phone == null) Phone = "";
			//Add this message to the database

			try
			{
				string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BestShopDB;Trusted_Connection=true";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "INSERT INTO messages " +
						"(firstname, lastname, email, phone, subject, message) VALUES" +
						"(@firstname, @lastname, @email, @phone, @subject, @message);";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@firstname", FirstName);
						command.Parameters.AddWithValue("@lastname", LastName);
						command.Parameters.AddWithValue("@email", Email);
						command.Parameters.AddWithValue("@phone", Phone);
						command.Parameters.AddWithValue("@subject", Subject);
						command.Parameters.AddWithValue("@message", Message);

						command.ExecuteNonQuery();
					}
				}
			}

			catch (Exception ex)
			{
				//Error
				ErrorMessage = ex.Message;
				return;
			}




			//send Confirmation Email to the client
			string username = FirstName + " " + LastName;
			string emailSubject = "About your message";
			string emailMessage = "Dear " + username + ",\n" +
				"We received your message. athankyou you for contacting us.\n" +
				"Our team will contcat you very soon. \n" +
				"Best regards \n\n" +
				"Your Message:\n" + Message;
			_emailSender.SendEmail(Email, username, emailSubject, emailMessage).Wait();

			SuccessMessage = "Your message has been received correctly";

			FirstName = "";
			LastName = "";
			Email = "";
			Phone = "";
			Subject = "";
			Message = "";

			//clear properties
			ModelState.Clear();
		}
	}
}
