using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Application
{
    public partial class add_user : Form
    {
        public add_user()
        {
            InitializeComponent();
        }

        // Necessary SQLite objects
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

        // Save user to the database
        private void SaveUser(string username, string password, string name)
        {
            string query = "INSERT INTO users (username, password, name) VALUES (@username, @password, @name)";
            SetConnection();
            sql_conn.Open();

            sql_cmd = new SQLiteCommand(query, sql_conn);
            sql_cmd.Parameters.AddWithValue("@username", username);
            sql_cmd.Parameters.AddWithValue("@password", password);
            sql_cmd.Parameters.AddWithValue("@name", name);

            try
            {
                sql_cmd.ExecuteNonQuery();
                MessageBox.Show("User saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete user from the database
        private void DeleteUser(string username)
        {
            string query = "DELETE FROM users WHERE username = @username";
            SetConnection();
            sql_conn.Open();

            sql_cmd = new SQLiteCommand(query, sql_conn);
            sql_cmd.Parameters.AddWithValue("@username", username);

            try
            {
                int rowsAffected = sql_cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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


        //content into data grid view
        private void LoadData()
        {
            SetConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string CommandText = "select * from users";
            DB = new SQLiteDataAdapter(CommandText, sql_conn);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dgv_users.DataSource = DT;
            sql_conn.Close();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            string username = txt_username.Text;
            string password = txt_password.Text;
            string name = txt_name.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveUser(username, password, name);
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            string username = txt_userid.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DeleteUser(username);
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {

        }
    }
}
