using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Librarian
{
    public partial class librarianPage : System.Web.UI.Page
    {
        string con_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adap;
        DataSet ds;
        SqlDataReader read;

        protected void Page_Load(object sender, EventArgs e)
        {
            ChargeOverdueFees();
            DeleteExpiredBookings();
            Showtable("BorrowDate ASC", "tblBorrowed", gvCollect, $" WHERE BorrowDate='{DateTime.Today:yyyy-MM-dd}'");
            Showtable("ReturnDate ASC", "tblBorrowed", gvReturn, $" WHERE ReturnDate='{DateTime.Today:yyyy-MM-dd}'");
        }

        protected void ChargeOverdueFees()
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();

                // Get distinct UserIDs with overdue books
                string sqlUsers = "SELECT DISTINCT UserID FROM tblBorrowed WHERE Status = 'Overdue'";
                comm = new SqlCommand(sqlUsers, conn);
                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    int userID = reader.GetInt32(0);

                    // Count the number of overdue books for this user
                    string sqlCount = "SELECT COUNT(*) FROM tblBorrowed WHERE UserID = @UserID AND Status = 'Overdue'";
                    SqlCommand commCount = new SqlCommand(sqlCount, conn);
                    commCount.Parameters.AddWithValue("@UserID", userID);
                    int overdueCount = (int)commCount.ExecuteScalar();

                    // Calculate daily charge: R20 per overdue book
                    decimal dailyCharge = 20 * overdueCount;

                    // Add the daily charge to OutstandingFees
                    string sqlUpdate = "UPDATE tblUsers SET OutstandingFees = OutstandingFees + @DailyCharge WHERE UserID = @UserID";
                    SqlCommand commUpdate = new SqlCommand(sqlUpdate, conn);
                    commUpdate.Parameters.AddWithValue("@DailyCharge", dailyCharge);
                    commUpdate.Parameters.AddWithValue("@UserID", userID);
                    commUpdate.ExecuteNonQuery();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle error, e.g., show message
                lblMessage.Text = "Error charging overdue fees: " + ex.Message;
                lblMessage.Visible = true;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void DeleteExpiredBookings()
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();

               // string sqlDelete = "DELETE FROM tblBorrowed WHERE BorrowDate < @Today AND Status = 'Pending' OR ";
                string sqlDelete = "DELETE FROM tblBorrowed WHERE (BorrowDate < @Today AND Status = 'Pending') OR (Status = 'Returned')";
                comm = new SqlCommand(sqlDelete, conn);
                comm.Parameters.AddWithValue("@Today", DateTime.Today);
                int rows = comm.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblMessage.Text = $"{rows} expired booking(s) cancelled.";
                    lblMessage.Visible = true;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error deleting expired bookings: " + ex.Message;
                lblMessage.Visible = true;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        public DataTable SearchBooks(string query)
        {
            try
            {
                string sql = "SELECT ISBN, BookTitle, BookAuthor, BookEdition, Year FROM tblBooks WHERE ISBN LIKE @id OR BookTitle LIKE @title OR BookAuthor LIKE @author";
                conn = new SqlConnection(con_string);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@id", "%" + query + "%");
                comm.Parameters.AddWithValue("@title", "%" + query + "%");
                comm.Parameters.AddWithValue("@author", "%" + query + "%");
                adap = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                adap.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (SqlException)
            {
                return null;
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                gvBooks.DataSource = null;
                gvBooks.DataBind();
                return;
            }
            DataTable results = SearchBooks(query);
            gvBooks.DataSource = results;
            gvBooks.DataBind();
        }

        protected void btnAddBook_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();
                string sql = "INSERT INTO tblBooks (ISBN, BookTitle, BookAuthor, BookGenre, BookEdition, Year, Available) VALUES (@ISBN, @BookTitle, @BookAuthor, @BookGenre, @BookEdition, @Year, @Available)";
                string genre = dropdownGenre.SelectedItem.ToString();
                comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@ISBN", txtISBN.Text);
                comm.Parameters.AddWithValue("@BookTitle", txtTitle.Text);
                comm.Parameters.AddWithValue("@BookAuthor", txtAuthor.Text);
                comm.Parameters.AddWithValue("@BookGenre", genre);
                comm.Parameters.AddWithValue("@BookEdition", txtEdition.Text);
                comm.Parameters.AddWithValue("@Year", txtYear.Text);
                comm.Parameters.AddWithValue("@Available", txtAvailable.Text);
                comm.ExecuteNonQuery();
                lblMessage.Text = "Books added successfully!";
                lblMessage.Visible = true;
                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnRemoveBook_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();
                string sql = "DELETE FROM tblBooks WHERE ISBN = @delISBN";
                comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@delISBN", txtEnterISBN.Text.Trim());
                int rows = comm.ExecuteNonQuery();
                lblMessage.Text = rows > 0 ? "Book successfully deleted!" : "No book found with that ISBN.";
                lblMessage.Visible = true;
                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnConfirmCollection_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();
                string sqlCheckBorrowed = "SELECT BorrowID, ISBN FROM tblBorrowed WHERE QRCode = @QRCode AND Status = 'Pending'";
                comm = new SqlCommand(sqlCheckBorrowed, conn);
                comm.Parameters.AddWithValue("@QRCode", txtCollectionCode.Text.Trim());
                SqlDataReader reader = comm.ExecuteReader();
                List<(int BorrowID, int ISBN)> borrowList = new List<(int, int)>();
                while (reader.Read())
                {
                    borrowList.Add((reader.GetInt32(0), reader.GetInt32(1)));
                }
                reader.Close();

                if (borrowList.Count == 0)
                {
                    lblMessage.Text = "No pending books found for this QRCode.";
                    lblMessage.Visible = true;
                    conn.Close();
                    return;
                }

                int booksProcessed = 0;
                foreach (var borrow in borrowList)
                {
                    string sqlCheck = "SELECT Available FROM tblBooks WHERE ISBN = @ISBN";
                    comm = new SqlCommand(sqlCheck, conn);
                    comm.Parameters.AddWithValue("@ISBN", borrow.ISBN);
                    object result = comm.ExecuteScalar();

                    if (result != null)
                    {
                        int available = Convert.ToInt32(result);
                        if (available > 0)
                        {
                            string sqlUpdate = "UPDATE tblBooks SET Available = @Available WHERE ISBN = @ISBN";
                            comm = new SqlCommand(sqlUpdate, conn);
                            comm.Parameters.AddWithValue("@Available", available - 1);
                            comm.Parameters.AddWithValue("@ISBN", borrow.ISBN);
                            comm.ExecuteNonQuery();

                            string sqlUpdateBorrow = "UPDATE tblBorrowed SET Status = 'Collected' WHERE BorrowID = @BorrowID";
                            comm = new SqlCommand(sqlUpdateBorrow, conn);
                            comm.Parameters.AddWithValue("@BorrowID", borrow.BorrowID);
                            comm.ExecuteNonQuery();

                            booksProcessed++;
                        }
                        else
                        {
                            lblMessage.Text = $"No available copies left for ISBN {borrow.ISBN}.";
                            lblMessage.Visible = true;
                            conn.Close();
                            return;
                        }
                    }
                    else
                    {
                        lblMessage.Text = $"Book with ISBN {borrow.ISBN} not found.";
                        lblMessage.Visible = true;
                        conn.Close();
                        return;
                    }
                }

                lblMessage.Text = $"{booksProcessed} book(s) successfully collected.";
                lblMessage.Visible = true;
                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        protected void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();
                string sqlCheckBorrowed = "SELECT BorrowID, ISBN, ReturnDate FROM tblBorrowed WHERE QRCode = @QRCode AND Status = 'Collected'";
                comm = new SqlCommand(sqlCheckBorrowed, conn);
                comm.Parameters.AddWithValue("@QRCode", txtCollectionCode.Text.Trim());
                SqlDataReader reader = comm.ExecuteReader();
                List<(int BorrowID, int ISBN, DateTime ReturnDate)> borrowList = new List<(int, int, DateTime)>();
                while (reader.Read())
                {
                    borrowList.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetDateTime(2)));
                }
                reader.Close();

                if (borrowList.Count == 0)
                {
                    lblMessage.Text = "No collected books found for this QRCode.";
                    lblMessage.Visible = true;
                    conn.Close();
                    return;
                }

                int booksProcessed = 0;
                foreach (var borrow in borrowList)
                {
                    string sqlCheck = "SELECT Available FROM tblBooks WHERE ISBN = @ISBN";
                    comm = new SqlCommand(sqlCheck, conn);
                    comm.Parameters.AddWithValue("@ISBN", borrow.ISBN);
                    object result = comm.ExecuteScalar();

                    if (result != null)
                    {
                        int available = Convert.ToInt32(result);
                        string sqlUpdateBook = "UPDATE tblBooks SET Available = @Available WHERE ISBN = @ISBN";
                        comm = new SqlCommand(sqlUpdateBook, conn);
                        comm.Parameters.AddWithValue("@Available", available + 1);
                        comm.Parameters.AddWithValue("@ISBN", borrow.ISBN);
                        comm.ExecuteNonQuery();

                        string newStatus = (borrow.ReturnDate < DateTime.Now.Date) ? "Overdue" : "Collected";
                        if (newStatus == "Overdue")
                        {
                            string sqlUpdateOverdue = "UPDATE tblBorrowed SET Status = 'Overdue' WHERE BorrowID = @BorrowID";
                            comm = new SqlCommand(sqlUpdateOverdue, conn);
                            comm.Parameters.AddWithValue("@BorrowID", borrow.BorrowID);
                            comm.ExecuteNonQuery();
                        }

                        string sqlUpdateBorrow = "UPDATE tblBorrowed SET Status = 'Returned', ReturnDate = @ReturnDate WHERE BorrowID = @BorrowID";
                        comm = new SqlCommand(sqlUpdateBorrow, conn);
                        comm.Parameters.AddWithValue("@ReturnDate", DateTime.Now.Date);
                        comm.Parameters.AddWithValue("@BorrowID", borrow.BorrowID);
                        comm.ExecuteNonQuery();

                        booksProcessed++;
                    }
                    else
                    {
                        lblMessage.Text = $"Book with ISBN {borrow.ISBN} not found.";
                        lblMessage.Visible = true;
                        conn.Close();
                        return;
                    }
                }

                lblMessage.Text = $"{booksProcessed} book(s) successfully returned.";
                lblMessage.Visible = true;
                conn.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        

        public void Showtable(string orderBy, string table, GridView gv, string condition)
        {
            try
            {
                conn = new SqlConnection(con_string);
                conn.Open();
                adap = new SqlDataAdapter();
                ds = new DataSet();
                string sql = $"SELECT * FROM {table}{condition} ORDER BY {orderBy}";
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
                lblMessage.Visible = true;
            }
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string orderBy = ddlSort.SelectedIndex == -1 ? "BookTitle ASC" : ddlSort.SelectedItem.Value.ToString();
            Showtable(orderBy, "tblBooks", gvBooks, "");
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
        
            try
            {
                int userID = int.Parse(txtUserID.Text);
                decimal amount = decimal.Parse(txtPaymentAmount.Text);

                conn = new SqlConnection(con_string);
                conn.Open();

                // Retrieve current outstanding fee for the user
                string sqlRetrieve = "SELECT OutstandingFees FROM tblUsers WHERE UserID = @UserID";
                comm = new SqlCommand(sqlRetrieve, conn);
                comm.Parameters.AddWithValue("@UserID", userID);
                object objFee = comm.ExecuteScalar();

                if (objFee == null || objFee == DBNull.Value)
                {
                    lblMessage.Text = "User not found or no outstanding fees.";
                    lblMessage.Visible = true;
                    return;
                }

                decimal currentFee = Convert.ToDecimal(objFee);
                decimal result = currentFee - amount;
                if (result < 0) result = 0; // Prevent negative fees

                // Update the outstanding fees
                string sqlUpdateFees = "UPDATE tblUsers SET OutstandingFees = @Result WHERE UserID = @UserID";
                comm = new SqlCommand(sqlUpdateFees, conn);
                comm.Parameters.AddWithValue("@Result", result);
                comm.Parameters.AddWithValue("@UserID", userID);
                comm.ExecuteNonQuery();

                // If result is zero, update status in tblBorrowed to 'Returned' for pending borrows
                if (result == 0)
                {
                    string sqlUpdateStatus = "UPDATE tblBorrowed SET Status = 'Returned' WHERE UserID = @UserID AND Status = 'Overdue'";
                    comm = new SqlCommand(sqlUpdateStatus, conn);
                    comm.Parameters.AddWithValue("@UserID", userID);
                    comm.ExecuteNonQuery();
                }

                lblMessage.Text = "Payment processed successfully.";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
    
}