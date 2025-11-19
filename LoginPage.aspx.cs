using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Librarian
{
    public partial class LoginPage : System.Web.UI.Page
    {
        public string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any existing session
                Session.Clear();
                lblMessage.Visible = false;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string userIdStr = txtLibraryNumber.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate inputs
            if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Please enter both User ID and password.", false);
                return;
            }

            // Validate UserID is numeric
            if (!int.TryParse(userIdStr, out int userId))
            {
                ShowMessage("User ID must be a valid number.", false);
                return;
            }

            // Enforce password complexity
            if (!IsPasswordValid(password))
            {
                ShowMessage("Password must be at least 8 characters with uppercase, lowercase, number, and special character.", false);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Check if UserID exists
                    string checkQuery = "SELECT COUNT(*) FROM tblUsers WHERE UserID = @UserID";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserID", userId);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            ShowMessage("User ID not found.", false);
                            return;
                        }
                    }

                    // Verify password
                    string query = "SELECT UserPasswd, UserRole, UserName, UserSurname FROM tblUsers WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["UserPasswd"].ToString();
                                string userRole = reader["UserRole"].ToString();
                                string firstName = reader["UserName"].ToString();
                                string lastName = reader["UserSurname"].ToString();

                                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    // Login successful
                                    Session["UserID"] = userId;
                                    Session["UserType"] = userRole;
                                    Session["UserName"] = $"{firstName} {lastName}";

                                    // Redirect based on user role
                                    switch (userRole)
                                    {
                                        case "Student":
                                            Response.Redirect("StudentPage.aspx");
                                            break;
                                        case "Librarian":
                                            Response.Redirect("librarianPage.aspx");
                                            break;
                                        case "Admin":
                                            Response.Redirect("AdminPage.aspx");
                                            break;
                                        default:
                                            Response.Redirect("LandingPage.aspx");
                                            break;
                                    }
                                }
                                else
                                {
                                    ShowMessage("Invalid password.", false);
                                }
                            }
                            else
                            {
                                ShowMessage("User ID not found.", false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, false);
            }
        }

        private bool IsPasswordValid(string password)
        {
            // At least 8 characters, with uppercase, lowercase, number, and special character
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Visible = true;
        }
   

    }

}
