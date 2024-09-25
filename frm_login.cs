using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace POS_Application
{
    public partial class frm_login : Form
    {
        public frm_login()
        {
            InitializeComponent();
        }

        // Necessary SQLite objects and data structures
        SQLiteConnection sql_conn;
        SQLiteCommand sql_cmd;
        SQLiteDataAdapter DB;
        DataSet DS = new DataSet();
        DataTable DT = new DataTable();

        
        // Set database connection
        private void SetConnection()
        {
            sql_conn = new SQLiteConnection("Data Source=pos_db.db;Version=3;New=False;Compress=True;");
        }

        // Execute query
        private void ExecuteQuery(string txtQuery)
        {
            try
            {
                SetConnection();
                sql_conn.Open();
                sql_cmd = sql_conn.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                sql_conn.Close();
            }
        }

        // Check credentials
        private bool CheckCredentials(string username, string password)
        {
            SetConnection();
            sql_conn.Open();

            string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
            sql_cmd = new SQLiteCommand(query, sql_conn);
            sql_cmd.Parameters.AddWithValue("@username", username);
            sql_cmd.Parameters.AddWithValue("@password", password);

            int count = Convert.ToInt32(sql_cmd.ExecuteScalar());

            sql_conn.Close();

            return count > 0;
        }


        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = txt_user.Text;
            string password = txt_password.Text;

            if (CheckCredentials(username, password))
            {
                // Navigate to frm_home if credentials are correct
                frm_home homeForm = new frm_home();
                homeForm.Show();
                this.Hide(); // Hide the login form
            }
            else
            {
                // Show error message if credentials are incorrect
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbl_forget_Click(object sender, EventArgs e)
        {
            // Navigate to frm_reset when lbl_forget is clicked
            frm_password_reset resetForm = new frm_password_reset();
            resetForm.Show();
            this.Hide(); // Optionally hide the login form
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            txt_password.Text = "";
            txt_user.Text = "";
        }
    }
}
