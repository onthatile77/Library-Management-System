using BCrypt.Net;
using System;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Librarian
{
    public partial class ForgotPassPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible = false;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = txtEmail.Text.Trim();

                // Validate inputs
                if (string.IsNullOrEmpty(email))
                {
                    ShowMessage("Email field cannot be empty.", false);
                    return;
                }
                else if (!IsValidEmail(email))
                {
                    ShowMessage("Please provide a valid email address.", false);
                    return;
                }

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";
                string newPassword = GenerateRandomPassword();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM tblUsers WHERE UserMail = @UserMail";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserMail", email);
                            int count = (int)cmd.ExecuteScalar();
                            if (count == 0)
                            {
                                ShowMessage("No user found with the provided email.", false);
                                return;
                            }
                        }

                        // Update password (hashed)
                        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        string updateQuery = "UPDATE tblUsers SET UserPasswd = @UserPasswd WHERE UserMail = @UserMail";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserPasswd", newPasswordHash);
                            cmd.Parameters.AddWithValue("@UserMail", email);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                ShowMessage("Failed to update password in database. Please try again.", false);
                                return;
                            }
                            System.Diagnostics.Debug.WriteLine($"Password updated for {email}. New Password: {newPassword}");
                        }
                    }

                    // Send email with temporary password
                    string emailError;
                    bool emailSent = SendPasswordEmail(email, newPassword, out emailError);
                    if (emailSent)
                    {
                        ShowMessage("A new password has been sent to your email.", true);
                    }
                    else
                    {
                        ShowMessage($"Failed to send email: {emailError}. Please try again later.", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("An error occurred: " + ex.Message, false);
                    System.Diagnostics.Debug.WriteLine($"Error in btnSubmit_Click: {ex.Message}");
                }
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

        private string GenerateRandomPassword(int length = 8)
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";
            Random random = new Random();
            char[] password = new char[length];

            // Ensure at least one of each required character type
            password[0] = uppercase[random.Next(uppercase.Length)];
            password[1] = lowercase[random.Next(lowercase.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = special[random.Next(special.Length)];

            // Fill remaining characters
            const string allChars = uppercase + lowercase + digits + special;
            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the password
            for (int i = length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                char temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }

            string result = new string(password);
            System.Diagnostics.Debug.WriteLine($"Generated Password: {result}");
            return result;
        }

        private bool SendPasswordEmail(string toEmail, string newPassword, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("bokamosoconnectlms@gmail.com", "Bokamoso Library System");
                    mail.To.Add(toEmail);
                    mail.Subject = "Your New Password";
                    mail.Body = $"Your new password is: '{newPassword}' (copy exactly as shown).\n\nPlease log in and change your password immediately.\n\nBest regards,\nBokamoso Library System";
                    mail.IsBodyHtml = false;

                    using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential("bokamosoconnectlms@gmail.com", "xbawfjqgxhvcvgad");
                        client.Timeout = 15000;

                        client.Send(mail);
                    }

                    System.Diagnostics.Debug.WriteLine($"Email sent successfully to: {toEmail}");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = $"SMTP Error: {smtpEx.Message}";
                System.Diagnostics.Debug.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"General Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                return false;
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Visible = true;
        }
    }
}
