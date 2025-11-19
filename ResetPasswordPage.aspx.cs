using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCrypt.Net;
using System.Net.Mail;
using System.Diagnostics;

namespace Librarian
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in 
            if (Session["UserID"] == null)
            {
                ShowMessage("Ensure we gave you a userID to reset your password.", false);
            }
            else
            {
                // Store UserID in ViewState to survive postbacks
                ViewState["UserID"] = Session["UserID"];
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // Input validation
            if (string.IsNullOrEmpty(txtCurrent.Text) || string.IsNullOrEmpty(txtNew.Text) || string.IsNullOrEmpty(txtConfirm.Text))
            {
                ShowMessage("All fields are required.", false);
                return;
            }

            if (txtNew.Text != txtConfirm.Text)
            {
                ShowMessage("New password and confirmation do not match.", false);
                return;
            }

            string newPassword = txtNew.Text;

            // Validate password strength
            if (!IsPasswordValid(newPassword))
            {
                ShowMessage("Password must be at least 8 characters with uppercase, lowercase, number, and special character.", false);
                return;
            }

            // Get UserID from ViewState (which survives postbacks)
            if (ViewState["UserID"] == null)
            {
                ShowMessage("Session expired. Please log in again.", false);
                return;
            }

            int userId = (int)ViewState["UserID"];
            string userEmail = "";
            string currentStoredHash = "";

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Verify current password and get user email
                    string query = "SELECT UserPasswd, UserMail FROM tblUsers WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentStoredHash = reader["UserPasswd"].ToString();
                                userEmail = reader["UserMail"].ToString();

                                // Verify current password matches
                                bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(txtCurrent.Text, currentStoredHash);

                                if (!isCurrentPasswordValid)
                                {
                                    ShowMessage("Invalid current password.", false);
                                    return;
                                }
                            }
                            else
                            {
                                ShowMessage("User not found.", false);
                                return;
                            }
                        }
                    }

                    // 2. Generate new password hash
                    string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

                    // 3. Update password in database
                    string updateQuery = "UPDATE tblUsers SET UserPasswd = @NewPassword WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewPassword", newPasswordHash);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            // 4. Verify the update worked
                            string verifyQuery = "SELECT UserPasswd FROM tblUsers WHERE UserID = @UserID";
                            using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn))
                            {
                                verifyCmd.Parameters.AddWithValue("@UserID", userId);
                                string updatedHash = (string)verifyCmd.ExecuteScalar();

                                // Test if new password verifies against the stored hash
                                bool canLoginWithNewPassword = BCrypt.Net.BCrypt.Verify(newPassword, updatedHash);

                                if (canLoginWithNewPassword)
                                {
                                    // 5. Send confirmation email
                                    string emailError;
                                    bool emailSent = SendPasswordResetConfirmation(userEmail, out emailError);

                                    ShowMessage("Password reset successfully! You can now login with your new password. " +
                                               (emailSent ? "Confirmation email sent." : "Note: Could not send email."), true);

                                    // Clear fields
                                    txtCurrent.Text = "";
                                    txtNew.Text = "";
                                    txtConfirm.Text = "";

                                    // Clear session and redirect
                                    Session.Remove("UserID");
                                    ViewState.Remove("UserID");

                                    // Redirect to login after 3 seconds
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect",
                                        "setTimeout(function(){ window.location.href = 'LoginPage.aspx'; }, 3000);", true);
                                }
                                else
                                {
                                    ShowMessage("Password update failed verification. Please contact support.", false);
                                }
                            }
                        }
                        else
                        {
                            ShowMessage("Failed to update password in database.", false);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("String or binary data would be truncated"))
                {
                    ShowMessage("Password storage error. The password hash is too long. Please contact support.", false);
                }
                else
                {
                    ShowMessage("Database error: " + sqlEx.Message, false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("An error occurred: " + ex.Message, false);
            }
        }

        private bool SendPasswordResetConfirmation(string email, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("bokamosoconnectlms@gmail.com", "Bokamoso Library System");
                    mail.To.Add(email);
                    mail.Subject = "Password Reset Successful";
                    mail.Body = $@"Dear Library User,

Your password has been successfully reset!

✅ Password changed on: {DateTime.Now.ToString("f")}

You can now login with your new password.

If you did not request this change, please contact the library immediately.

Best regards,
Bokamoso Library Team
";
                    mail.IsBodyHtml = false;

                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Send(mail);
                    }

                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = $"SMTP Error: {smtpEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private bool IsPasswordValid(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Visible = true;
        }

        protected void btnBackToLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }
    }
}
