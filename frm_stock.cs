using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Application
{
    public partial class frm_stock : Form
    {
        public frm_stock()
        {
            InitializeComponent();
            pb_item_image.Click += new EventHandler(pb_item_image_Click); // Attach the click event handler
            txt_discount.TextChanged += new EventHandler(txt_discount_TextChanged); // Attach the TextChanged event handler
            btn_save.Text = "Save"; // Add label to Save button
            btn_update.Text = "Update"; // Add label to Update button
        }

        // necessary SQLite objects and data structures
        SQLiteConnection sql_conn;
        SQLiteCommand sql_cmd;
        SQLiteDataAdapter DB;
        DataSet DS = new DataSet();
        DataTable DT = new DataTable();

        // Set database connection
        private void SetConnection()
        {
            sql_conn = new SQLiteConnection("Data Source=vinod.db;Version=3;New=False;Compress=True;");
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

        // Load data into DataGridView
        private void LoadData()
        {
            try
            {
                SetConnection();
                sql_conn.Open();
                sql_cmd = sql_conn.CreateCommand();
                string CommandText = "select * from stock";
                DB = new SQLiteDataAdapter(CommandText, sql_conn);
                DS.Reset();
                DB.Fill(DS);
                DT = DS.Tables[0];
                dgv_stock.DataSource = DT;
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

        private void pb_item_image_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pb_item_image.Image = Image.FromFile(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        // This method should be called to setup the event handler for DataBindingComplete
        private void InitializeDataGridView()
        {
            dgv_stock.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dgv_stock_DataBindingComplete);
        }

        // This is the event handler where you change the row color based on the condition
        private void dgv_stock_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgv_stock.Rows)
            {
                if (row.Cells["stock"].Value != null && int.TryParse(row.Cells["stock"].Value.ToString(), out int stockQuantity))
                {
                    if (stockQuantity < 10)
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }
            }
        }

        private byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                // Save as PNG to ensure compatibility
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void txt_discount_TextChanged(object sender, EventArgs e)
        {
            CalculateSellPrice();
        }

        private void CalculateSellPrice()
        {
            try
            {
                if (decimal.TryParse(txt_label.Text, out decimal cost) && decimal.TryParse(txt_discount.Text, out decimal discount))
                {
                    decimal sellPrice = cost - ((cost * discount) / 100);
                    txt_sell.Text = sellPrice.ToString("0.00");
                }
                else
                {
                    txt_sell.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                string stockAddDate = dtp_stock_add_date.Value.ToString("yyyy-MM-dd");
                byte[] imgBytes = ImageToByteArray(pb_item_image.Image);

                string txtQuery = "insert into stock (reference, name, warenty, brand, stock, stock_add_date, item_image, cost_price, discount_price, label_price, sell_price) values (@reference, @name, @item_warenty, @item_brand, @item_stock, @item_stock_add_date, @item_image, @cost_price, @discount_price, @label_price, @sell_price)";

                SetConnection();
                sql_conn.Open();
                sql_cmd = sql_conn.CreateCommand();
                sql_cmd.CommandText = txtQuery;

                sql_cmd.Parameters.AddWithValue("@reference", txt_reference.Text);
                sql_cmd.Parameters.AddWithValue("@name", txt_name.Text);
                sql_cmd.Parameters.AddWithValue("@item_warenty", txt_warenty.Text);
                sql_cmd.Parameters.AddWithValue("@item_brand", txt_brand.Text);
                sql_cmd.Parameters.AddWithValue("@item_stock", txt_stock.Text);
                sql_cmd.Parameters.AddWithValue("@item_stock_add_date", stockAddDate);
                sql_cmd.Parameters.AddWithValue("@item_image", imgBytes);
                sql_cmd.Parameters.AddWithValue("@cost_price", txt_cost.Text);
                sql_cmd.Parameters.AddWithValue("@discount_price", txt_discount.Text); // Correct parameter name
                sql_cmd.Parameters.AddWithValue("@label_price", txt_label.Text); // Correct parameter name
                sql_cmd.Parameters.AddWithValue("@sell_price", txt_sell.Text);

                sql_cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            finally
            {
                sql_conn.Close();
                LoadData();
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            txt_reference.Text = "";
            txt_name.Text = "";
            txt_warenty.Text = "";
            txt_brand.Text = "";
            txt_stock.Text = "";
            txt_label.Text = "";
            txt_sell.Text = "";
            txt_cost.Text = "";
            txt_discount.Text = "";
            dtp_stock_add_date.Value = DateTime.Now;
            pb_item_image.Image = null;
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            try
            {
                string txtQuery = "delete from stock where reference=@reference";
                SetConnection();
                sql_conn.Open();
                sql_cmd = sql_conn.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.Parameters.AddWithValue("@reference", txt_reference.Text);
                sql_cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            finally
            {
                sql_conn.Close();
                LoadData();
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            try
            {
                string stockAddDate = dtp_stock_add_date.Value.ToString("yyyy-MM-dd");
                byte[] imgBytes = null;

                if (pb_item_image.Image != null)
                {
                    imgBytes = ImageToByteArray(pb_item_image.Image);
                }

                string txtQuery = imgBytes == null ?
                    "update stock set name=@name, warenty=@item_warenty, brand=@item_brand, stock=@item_stock, stock_add_date=@item_stock_add_date, cost_price=@cost_price, discount_price=@discount_price, label_price=@label_price, sell_price=@sell_price where reference=@reference" :
                    "update stock set name=@name, warenty=@item_warenty, brand=@item_brand, stock=@item_stock, stock_add_date=@item_stock_add_date, item_image=@item_image, cost_price=@cost_price, discount_price=@discount_price, label_price=@label_price, sell_price=@sell_price where reference=@reference";

                SetConnection();
                sql_conn.Open();
                sql_cmd = sql_conn.CreateCommand();
                sql_cmd.CommandText = txtQuery;

                sql_cmd.Parameters.AddWithValue("@reference", txt_reference.Text);
                sql_cmd.Parameters.AddWithValue("@name", txt_name.Text);
                sql_cmd.Parameters.AddWithValue("@item_warenty", txt_warenty.Text);
                sql_cmd.Parameters.AddWithValue("@item_brand", txt_brand.Text);
                sql_cmd.Parameters.AddWithValue("@item_stock", txt_stock.Text);
                sql_cmd.Parameters.AddWithValue("@item_stock_add_date", stockAddDate);
                sql_cmd.Parameters.AddWithValue("@cost_price", txt_cost.Text);
                sql_cmd.Parameters.AddWithValue("@discount_price", txt_discount.Text);
                sql_cmd.Parameters.AddWithValue("@label_price", txt_label.Text);
                sql_cmd.Parameters.AddWithValue("@sell_price", txt_sell.Text);

                if (imgBytes != null)
                {
                    sql_cmd.Parameters.AddWithValue("@item_image", imgBytes);
                }

                sql_cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            finally
            {
                sql_conn.Close();
                LoadData();
            }
        }

        private void dgv_stock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                if (dgv_stock.SelectedRows.Count > 0)
                {
                    txt_reference.Text = dgv_stock.SelectedRows[0].Cells["reference"].Value.ToString();
                    txt_name.Text = dgv_stock.SelectedRows[0].Cells["name"].Value.ToString();
                    txt_warenty.Text = dgv_stock.SelectedRows[0].Cells["warenty"].Value.ToString();
                    txt_brand.Text = dgv_stock.SelectedRows[0].Cells["brand"].Value.ToString();
                    txt_cost.Text = dgv_stock.SelectedRows[0].Cells["cost_price"].Value.ToString();
                    txt_label.Text = dgv_stock.SelectedRows[0].Cells["label_price"].Value.ToString();
                    txt_discount.Text = dgv_stock.SelectedRows[0].Cells["discount_price"].Value.ToString();
                    txt_sell.Text = dgv_stock.SelectedRows[0].Cells["sell_price"].Value.ToString();
                    txt_stock.Text = dgv_stock.SelectedRows[0].Cells["stock"].Value.ToString();
                    dtp_stock_add_date.Value = DateTime.Parse(dgv_stock.SelectedRows[0].Cells["stock_add_date"].Value.ToString());

                    // Handle DBNull for item_image
                    var itemImageValue = dgv_stock.SelectedRows[0].Cells["item_image"].Value;
                    if (itemImageValue != DBNull.Value)
                    {
                        pb_item_image.Image = ByteArrayToImage((byte[])itemImageValue);
                    }
                    else
                    {
                        pb_item_image.Image = null; // or set a default image
                    }
                }
            }
        }
    }
}
