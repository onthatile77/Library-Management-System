using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Librarian
{
    public partial class StudentPage : System.Web.UI.Page
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookDB.mdf;Integrated Security=True";
        private SqlCommand cmd;
        private SqlDataAdapter ad;
        private SqlConnection conn;

        private string userId;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UserID"] != null)
            {
                userId = Session["UserID"].ToString();
            }
            else
            {
                return;
            }


        }



        public DataTable SearchBooks(string query)
        {
            try
            {
                string sql = "SELECT ISBN, BookTitle, BookAuthor, BookEdition, Year FROM tblBooks WHERE ISBN LIKE @id OR BookTitle LIKE @title OR BookAuthor LIKE @author";
                conn = new SqlConnection(connectionString);

                conn.Open();
                cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", "%" + query + "%");
                cmd.Parameters.AddWithValue("@title", "%" + query + "%");
                cmd.Parameters.AddWithValue("@author", "%" + query + "%");
                ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
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

            DataTable results = SearchBooks(query); // Your method to fetch filtered data

            // Bind to gridview
            gvBooks.DataSource = results;
            gvBooks.DataBind();
        }

        protected void btnAddToSelection_Click(object sender, EventArgs e)
        {
            // Clear existing items in ListBox to avoid duplicates (optional, based on requirements)
            lstSelectedBooks.Items.Clear();

            // Loop through GridView rows to find selected checkboxes
            foreach (GridViewRow row in gvBooks.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect.Checked)
                {
                    // Get ISBN from the first column (index 0 of data cells, since checkbox is a TemplateField)
                    string isbn = row.Cells[1].Text;
                    string bName = row.Cells[2].Text;
                    lstSelectedBooks.Items.Add(new ListItem(isbn + " - " + bName));
                }
            }


        }

        protected void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            // Remove selected items from ListBox (iterate backwards to avoid index issues)
            for (int i = lstSelectedBooks.Items.Count - 1; i >= 0; i--)
            {
                if (lstSelectedBooks.Items[i].Selected)
                {
                    lstSelectedBooks.Items.RemoveAt(i);
                }
            }
        }
        protected void btnConfirmSelections_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            if (lstSelectedBooks.Items.Count == 0)
            {
                lblError.Text = "No books selected. Please add books to your selection before confirming.";
                return;
            }
            Session["SelectedBooks"] = lstSelectedBooks.Items.Cast<ListItem>().Select(item => item.Text).ToList();

            if (calCollectDate.SelectedDate == DateTime.MinValue || calCollectDate.SelectedDate < DateTime.Today)
            {
                lblError.Text = "Please select a valid collection date (today or later).";
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Visible = true;
                return;
            }

            DateTime collectDT = calCollectDate.SelectedDate;
            DateTime returnDT = collectDT.AddDays(8);
            string[] arrBookIDs = new string[4];
            int i = 0;
            foreach (ListItem item in lstSelectedBooks.Items)
            {
                // Assuming the format is "123 - Book Title"
                string[] parts = item.Text.Split('-');
                if (parts.Length > 0)
                {
                    arrBookIDs[i] = parts[0].Trim(); // Get the ID part and trim whitespace
                    i++;
                }
            }

            string transactionCode = "BL" + collectDT.ToShortDateString() + userId;

            conn = new SqlConnection(connectionString);

            for (int j = 0; j < i; j++)
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO tblBorrowed (UserID, ISBN, BorrowDate, ReturnDate, Status, QRCode) VALUES(@id, @is, @b, @r, @st, @qr)";
                    string status = "Pending";
                    cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(userId.Trim()));
                    cmd.Parameters.AddWithValue("@is", Convert.ToInt64(arrBookIDs[j].Trim()));
                    cmd.Parameters.AddWithValue("@b", collectDT);
                    cmd.Parameters.AddWithValue("@r", returnDT);
                    cmd.Parameters.AddWithValue("@st", status);
                    cmd.Parameters.AddWithValue("@qr", transactionCode);

                    cmd.ExecuteNonQuery();
                    conn.Close();


                }
                catch (SqlException ex)
                {
                    lblError.Text = " Couldnt place booking for ISBN: " + arrBookIDs[j] + "\n" + ex.Message;
                    return;
                }

            }
            
            Response.Redirect("ConfirmSelectionPage.aspx");
            lblError.Text = "Collection QR CODE: " + transactionCode;


        }
    }
}
