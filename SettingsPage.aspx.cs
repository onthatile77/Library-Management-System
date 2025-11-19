using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Librarian
{
    public partial class SettingsPage : System.Web.UI.Page
    {
        //CONNECTION STRING using DataDirectory 
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";

        //  PAGE LOAD: Load borrowed books for current user 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBorrowedBooks();
            }
        }

        //Load all borrowed books where UserID matches session
        private void LoadBorrowedBooks()
        {
           
            if (Session["UserID"] == null)
            {
                lblMessage.Text = "❌ You are not logged in.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                Response.Redirect("LoginPage.aspx");
                return;
            }

            string userID = Session["UserID"].ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                     // Get all borrowed books for this user
                    string query = @"
                        SELECT 
                            ISBN, 
                            BorrowDate, 
                            ReturnDate, 
                            Status, 
                            QRCode
                        FROM tblBorrowed 
                        WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Use parameters to prevent SQL injection (best practice)
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                lblMessage.Text = "📘 You haven't borrowed any books yet.";
                            }
                            else
                            {
                                lblMessage.Text = $"✅ You have borrowed {dt.Rows.Count} book(s).";
                            }

                            // Bind data to GridView
                            gvBorrowed.DataSource = dt;
                            gvBorrowed.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //
                lblMessage.Text = "⚠️ An error occurred: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        //  Redirect to change password page 
        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("ResetPasswordPage.aspx");
        }

        protected Boolean CheckDelete(string uID)
        {
            int borrowedCount = 0;
            // Check if user has borrowed books
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string checkQuery = "SELECT COUNT(*) FROM tblBorrowed WHERE UserID = @UserID AND Status = 'Borrowed'";
                    
                    using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", uID);
                        borrowedCount = (int)cmd.ExecuteScalar();
                    }
     
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "⚠️ Error checking borrowed books: " + ex.Message;
                return false;
            }

            if (borrowedCount > 0)
            {
                lblMessage.Text = "❌ You cannot delete your account while you have borrowed books. Please return them first.";

                return false;
            }
            else
            {


                // Check if user has outstandingfees > 0
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string feeQuery = "SELECT OutstandingFees FROM tblUsers WHERE UserID = @UserID";
                        using (SqlCommand cmd = new SqlCommand(feeQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@UserID", uID); //maybe make uID int
                            object result = cmd.ExecuteScalar();
                            if (result != null && decimal.TryParse(result.ToString(), out decimal fees))
                            {
                                if (fees > 0)
                                {
                                    lblMessage.Text = "❌ You cannot delete your account while you have outstanding fees. Please clear them first.";
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                lblMessage.Text = "❌ User not found";
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "⚠️ Error checking outstanding fees: " + ex.Message;
                    return false;
                }
            }

        }

        //  Delete account with confirmation 
        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                lblMessage.Text = "❌ Session expired. Please log in again.";
                return;
            }

            if (!CheckDelete(Session["UserID"].ToString()))
            {
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string deleteQuery = "DELETE FROM tblUsers WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Session.Clear();
                            Session.Abandon();
                            Response.Redirect("LoginPage.aspx?msg=AccountDeleted");
                        }
                        else
                        {
                            lblMessage.Text = "❌ No account found with this ID.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "⚠️ Error deleting account: " + ex.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                string sqlCheck = "SELECT BorrowID, BorrowDate FROM tblBorrowed WHERE QRCode = @QRCode AND Status = 'Pending'";
                SqlCommand cmd = new SqlCommand(sqlCheck, con);
                cmd.Parameters.AddWithValue("@QRCode", txtCodeCancel.Text.Trim());
                SqlDataReader reader = cmd.ExecuteReader();
                List<(int BorrowID, DateTime BorrowDate)> borrowList = new List<(int, DateTime)>();
                while (reader.Read())
                {
                    borrowList.Add((reader.GetInt32(0), reader.GetDateTime(1)));
                }
                reader.Close();

                if (borrowList.Count == 0)
                {
                    lblMessage.Text = "No pending bookings found for this QRCode.";
                    lblMessage.Visible = true;
                    con.Close();
                    return;
                }

                int cancelledCount = 0;
                foreach (var borrow in borrowList)
                {
                    // Optionally warn if cancelling future bookings
                    if (borrow.BorrowDate > DateTime.Now.Date)
                    {
                        lblMessage.Text = "Warning: Cancelling a booking scheduled for a future date.";
                        lblMessage.Visible = true;
                    }

                    string sqlDelete = "DELETE FROM tblBorrowed WHERE BorrowID = @BorrowID AND Status = 'Pending'";
                    cmd = new SqlCommand(sqlDelete, con);
                    cmd.Parameters.AddWithValue("@BorrowID", borrow.BorrowID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0) cancelledCount++;
                }

                lblMessage.Text = cancelledCount > 0 ? $"{cancelledCount} booking(s) cancelled successfully." : "No pending bookings were cancelled.";
                lblMessage.Visible = true;
                con.Close();
                LoadBorrowedBooks();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error cancelling booking: " + ex.Message;
                lblMessage.Visible = true;
               // con.Close();
            }
           
        }
    }
    
}
