using BCrypt.Net;
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
using System.Xml.Linq;

namespace Librarian
{
    public partial class AdminPage : System.Web.UI.Page
    {
        string orderBy = "UserID ASC";
        string remove;
        string con_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adap;
        DataSet ds;
        SqlDataReader read;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Only load full table once
            {
                Showtable("UserID ASC", gvUsers);
            }


        }

        public void Showtable(string id)
        {
            try
            {
                conn = new SqlConnection(con_string);

                conn.Open();

                adap = new SqlDataAdapter();

                ds = new DataSet();

                string sql = @"SELECT * FROM tblUsers WHERE UserID=@id";

                comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@id", Convert.ToInt64(id));

                adap.SelectCommand = comm;
                adap.Fill(ds);

                GridView2.DataSource = ds;
                GridView2.DataBind();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Showtable(string orderBy, GridView gv)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();

                adap = new SqlDataAdapter();
                ds = new DataSet();

                string sql = $"SELECT * FROM tblUsers ORDER BY {orderBy}";

                comm = new SqlCommand(sql, conn);

                adap.SelectCommand = comm;
                adap.Fill(ds);

                gv.DataSource = ds;
                gv.DataBind();

                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }


        protected void btnAddLibAdm_Click(object sender, EventArgs e)
        {
            string role;
            if (radAdmin.Checked || radLib.Checked)
            {
                role = radLib.Checked ? radLib.Text : radAdmin.Text;


                decimal fees = 0.0m;
                try
                {
                    conn = new SqlConnection(con_string);

                    conn.Open();

                    //string sql = $"INSERT INTO tblUsers(UserName,UserSurname,UserMail,UserPasswd,UserRole,OutstandingFees)Values('{txtLibrarianFirstName.Text}','{txtLibrarianLastName.Text}','{txtLibrarianEmail.Text}','{txtLibrarianPassword.Text}','{role}','{fees}')";
                    string sql = "INSERT INTO tblUsers (UserName, UserSurname, UserMail, UserPasswd, UserRole, OutstandingFees) VALUES (@UserName, @UserSurname, @UserMail, @UserPasswd, @UserRole, @OutstandingFees)";
                    comm = new SqlCommand(sql, conn);

                    comm.Parameters.AddWithValue("@UserName", txtLibrarianFirstName.Text);
                    comm.Parameters.AddWithValue("@UserSurname", txtLibrarianLastName.Text);
                    comm.Parameters.AddWithValue("@UserMail", txtLibrarianEmail.Text);
                    comm.Parameters.AddWithValue("@UserPasswd", BCrypt.Net.BCrypt.HashPassword(txtLibrarianPassword.Text.Trim()));
                    comm.Parameters.AddWithValue("@UserRole", role);
                    comm.Parameters.AddWithValue("@OutstandingFees", fees);


                    int newUserId = (int)comm.ExecuteScalar();
                    if (newUserId > 0)
                    {
                        string emailError;
                        bool emailSent = SendRegistrationEmail(txtLibrarianEmail.Text.Trim(), newUserId, out emailError);

                        ShowMessage($"Registration successful! Your User ID is: {newUserId}" +
                                   (emailSent ? ". Confirmation email has been sent to your email address." :
                                   $". Note: Could not send email ({emailError}). Please note your User ID for login."), true);
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Registration failed.", false);
                    }
                    conn.Close();

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

                    Showtable(txtUserID.Text.Trim());

                    lblMessage.Text = "Successfully added....";


                    lblMessage.Visible = true;
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, false);
                }






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
            txtLibrarianFirstName.Text = "";
        txtLibrarianLastName.Text = "";
        txtLibrarianEmail.Text = "";
        txtLibrarianPassword.Text = "";
    }

    private void ShowMessage(string message, bool isSuccess)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        lblMessage.Visible = true;
    }
      
    

        protected void btnRemoveUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserIdDel.Text))
            {
                return;
            }
            try
            {
                conn = new SqlConnection(con_string);

                conn.Open();

                string sql = $"DELETE FROM tblUsers WHERE UserID = @delID";

                comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@delID", txtUserIdDel.Text.Trim());

                adap = new SqlDataAdapter();

                adap.DeleteCommand = comm;
                adap.DeleteCommand.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Showtable(txtUserIdDel.Text.Trim());
            lblMessage.Text = "User Seccesfully Deleted!!";
        }

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserID.Text) || string.IsNullOrEmpty(txtUpdate.Text))
            {
                return;
            }
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();

                string sql = "";

                // Check which radio button is selected
                if (radEmail.Checked)
                {
                    
                    sql = "UPDATE tblUsers SET UserMail = @NewValue WHERE UserID = @UserID";
                }
                else if (radLast.Checked)
                {
                    
                    sql = "UPDATE tblUsers SET UserSurname = @NewValue WHERE UserID = @UserID";
                }
                else if (radPass.Checked)
                {
                    
                    sql = "UPDATE tblUsers SET UserPasswd = @NewValue WHERE UserID = @UserID";
                }
                else
                {
                    lblMessage.Text = "Please select what you want to update.";
                    conn.Close();
                    return;
                }

                // SQL query using string interpolation for simplicity (not best practice but same as your style)
                

                comm = new SqlCommand(sql, conn);

                // Add values from textboxes
                comm.Parameters.AddWithValue("@UserID", txtUserID.Text.Trim());
                comm.Parameters.AddWithValue("@NewValue", radPass.Checked ? BCrypt.Net.BCrypt.HashPassword(txtUpdate.Text.Trim()) : txtUpdate.Text.Trim());

                int rows = comm.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblMessage.Text = "Successfully updated.";
                    txtUserID.Text = string.Empty;
                    txtUpdate.Text = string.Empty;
                }
                else
                {
                    lblMessage.Text = "No user found with that ID.";
                    txtUserID.Text = string.Empty;
                    txtUpdate.Text = string.Empty;
                }

                conn.Close();
                Showtable(txtUserID.Text.Trim()); // Refresh grid
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }


        protected void txtSearchUsers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();

                adap = new SqlDataAdapter();
                ds = new DataSet();

                string sql = "SELECT * FROM tblUsers WHERE UserName LIKE @search OR CAST(UserID AS NVARCHAR) LIKE @search";

                comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@search", "%" + txtSearchUsers.Text.Trim() + "%");

                adap.SelectCommand = comm;
                adap.Fill(ds);

                gvUsers.DataSource = ds;
                gvUsers.DataBind();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    lblMessage.Text = "No users found.";
                }
                else
                {
                    lblMessage.Text = "";
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }

        }

        protected void ddlSortUsers_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (ddlSortUsers.SelectedValue == "ID ASC")
            {
                orderBy = "UserID ASC";
            }
            else if (ddlSortUsers.SelectedValue == "ID DESC")
            {
                orderBy = "UserID DESC";
            }
            else if (ddlSortUsers.SelectedValue == "Name ASC")
            {
                orderBy = "UserName ASC";
            }
            else if (ddlSortUsers.SelectedValue == "Name DESC")
            {
                orderBy = "UserName DESC";
            }

            Showtable(orderBy, gvUsers);
        }

    }
}