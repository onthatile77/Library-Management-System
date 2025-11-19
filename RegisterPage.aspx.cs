using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Librarian
{
    public partial class RegisterPage : System.Web.UI.Page
    {
        public string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible = false;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string surname = txtSurname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate inputs
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowMessage("All fields are required.", false);
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowMessage("Please enter a valid email address.", false);
                return;
            }

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

                    // Check if email already exists
                    string checkQuery = "SELECT COUNT(*) FROM tblUsers WHERE UserMail = @UserMail";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    {
                        checkCmd.Parameters.Add("@UserMail", SqlDbType.NVarChar, 30).Value = email;
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ShowMessage("Email already registered.", false);
                            conn.Close();
                            return;
                        }
                    }

                    // Insert new user (let UserID be auto-generated)
                    string insertQuery = @"INSERT INTO tblUsers (UserName, UserSurname, UserMail, UserPasswd, UserRole, OutstandingFees) 
                                          OUTPUT INSERTED.UserID
                                          VALUES (@UserName, @UserSurname, @UserMail, @UserPasswd, 'Student', 0)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserName", name);
                        insertCmd.Parameters.AddWithValue("@UserSurname", (surname));
                        insertCmd.Parameters.AddWithValue("@UserMail", (email));
                        insertCmd.Parameters.AddWithValue("@UserPasswd", (BCrypt.Net.BCrypt.HashPassword(password)));

                        int newUserId = (int)insertCmd.ExecuteScalar();
                        if (newUserId > 0)
                        {
                            string emailError;
                            bool emailSent = SendRegistrationEmail(email, newUserId, out emailError);

                            ShowMessage($"Registration successful! Your User ID is: {newUserId}" +
                                       (emailSent ? ". Confirmation email has been sent to your email address." :
                                       $". Note: Could not send email ({emailError}). Please note your User ID for login."), true);
                            ClearForm();
                        }
                        else
                        {
                            ShowMessage("Registration failed.", false);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint violation
                {
                    ShowMessage("Email already registered.", false);
                }
                else if (ex.Message.Contains("String or binary data would be truncated"))
                {
                    ShowMessage("Database error: Password storage failed. Please contact support.", false);
                }
                else
                {
                    ShowMessage("Database error: " + ex.Message, false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, false);
            }
        }

        private bool SendRegistrationEmail(string email, int userId, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("bokamosoconnectlms@gmail.com", "Bokamoso Library System");
                    mail.To.Add(email);
                    mail.Subject = "Registration Successful - Your Library Account";
                    mail.Body = $@"Dear Library User,

Your registration with Bokamoso Library System was successful!

📚 Your User ID: {userId}
📧 Your Email: {email}

Please use your User ID and password to log in to your account.

Best regards,
Bokamoso Library Team
";
                    mail.IsBodyHtml = false;

                    // Direct SMTP configuration (more reliable than Web.config)
                    using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential(
                            "bokamosoconnectlms@gmail.com",
                            "xbawfjqgxhvcvgad"  // APP PASSWORD WITHOUT SPACES
                        );
                        client.Timeout = 15000;

                        client.Send(mail);
                    }

                    System.Diagnostics.Debug.WriteLine($"Email sent successfully to: {email}");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = $"Email Error: {smtpEx.Message}";
                System.Diagnostics.Debug.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsPasswordValid(string password)
        {
            // At least 8 characters, with uppercase, lowercase, number, and special character
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtSurname.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Visible = true;
        }
      
    
    }
}
