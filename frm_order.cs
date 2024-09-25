using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Application
{
    public partial class frm_order : Form
    {
        public frm_order()
        {
            InitializeComponent();
        }

        private void ClearInputFields()
        {
            // Clear all the input fields
            txt_reference.Text = "";
            txt_name.Text = "";
            txt_warenty.Text = "";
            txt_brand.Text = "";
            txt_cost.Text = "";
            txt_discount.Text = "";
            txt_sell.Text = "";
            txt_quentity.Text = "";
            //txt_sub.Text = "";
        }

        private void UpdateStock(string itemReference, int quantitySold)
        {
            string connectionString = "Data Source=pos_db.db;Version=3;New=False;Compress=True;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Reduce item count in stock table
                        var updateStockCommand = new SQLiteCommand("UPDATE stock SET stock = stock - @quantity WHERE reference = @itemReference", connection);
                        updateStockCommand.Parameters.AddWithValue("@quantity", quantitySold);
                        updateStockCommand.Parameters.AddWithValue("@itemReference", itemReference);
                        updateStockCommand.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Stock updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error updating stock: " + ex.Message);
                    }
                }
                connection.Close();
            }
        }

        private void CalculateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgv_bill.Rows)
            {
                if (!row.IsNewRow)
                {
                    total += Convert.ToDecimal(row.Cells["subtotal"].Value);
                }
            }
            txt_sub.Text = total.ToString();
        }

        private void btn_addbill_Click(object sender, EventArgs e)
        {
            // Collect the details from the text boxes
            string reference = txt_reference.Text;
            string name = txt_name.Text;
            string warenty = txt_warenty.Text;
            string brand = txt_brand.Text;
            decimal sellPrice = decimal.Parse(txt_sell.Text);
            int quantity = int.Parse(txt_quentity.Text); // Assuming txt_quantity is a text box for quantity
            decimal subtotal = sellPrice * quantity;

            // Add the details to the DataGridView
            dgv_bill.Rows.Add(reference, name, warenty, brand, sellPrice, quantity, subtotal);

            // Update the stock table based on quantity
            UpdateStock(reference, quantity);

            // Calculate total
            CalculateTotal();

            // Clear the input fields after adding to the bill
            ClearInputFields();
        }

        private void txt_checkout_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=vinod.db;Version=3;New=False;Compress=True;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        decimal subtotal = Convert.ToDecimal(txt_sub.Text);
                        decimal payment = Convert.ToDecimal(txt_payment.Text);
                        decimal balance = Convert.ToDecimal(txt_balance.Text);

                        // Insert each item in the bill
                        foreach (DataGridViewRow row in dgv_bill.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string reference = row.Cells["reference"].Value.ToString();
                                string name = row.Cells["name"].Value.ToString();
                                string warenty = row.Cells["warenty"].Value.ToString();
                                string brand = row.Cells["brand"].Value.ToString();
                                decimal sellPrice = Convert.ToDecimal(row.Cells["sellPrice"].Value);
                                int quantity = Convert.ToInt32(row.Cells["quantity"].Value);
                                decimal itemSubtotal = Convert.ToDecimal(row.Cells["subtotal"].Value);

                                string query = "INSERT INTO sales (reference, name, warenty, brand, sell_price, quantity, subtotal) VALUES (@reference, @name, @warenty, @brand, @sellPrice, @quantity, @subtotal)";
                                using (var command = new SQLiteCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@reference", reference);
                                    command.Parameters.AddWithValue("@name", name);
                                    command.Parameters.AddWithValue("@warenty", warenty);
                                    command.Parameters.AddWithValue("@brand", brand);
                                    command.Parameters.AddWithValue("@sellPrice", sellPrice);
                                    command.Parameters.AddWithValue("@quantity", quantity);
                                    command.Parameters.AddWithValue("@subtotal", itemSubtotal);

                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        // Insert the transaction summary
                        string summaryQuery = "INSERT INTO transaction_summary (subtotal, payment, balance) VALUES (@subtotal, @payment, @balance)";
                        using (var summaryCommand = new SQLiteCommand(summaryQuery, connection))
                        {
                            summaryCommand.Parameters.AddWithValue("@subtotal", subtotal);
                            summaryCommand.Parameters.AddWithValue("@payment", payment);
                            summaryCommand.Parameters.AddWithValue("@balance", balance);

                            summaryCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Checkout completed and data saved successfully.");

                        // Clear DataGridView and input fields
                        dgv_bill.Rows.Clear();
                        ClearInputFields();
                        txt_sub.Text = "";
                        txt_payment.Text = "";
                        txt_balance.Text = "";

                        Print();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error during checkout: " + ex.Message);
                    }
                }

                connection.Close();
            }
        }

        private void txt_remove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgv_bill.SelectedRows)
            {
                dgv_bill.Rows.Remove(row);
            }

            // Recalculate total
            CalculateTotal();
        }

        private void btn_printbill_Click(object sender, EventArgs e)
        {
            Print();
        }

        public void Print()
        {
            var doc = new PrintDocument();
            doc.PrintPage += new PrintPageEventHandler(ProvideContent);
            doc.Print();
        }

        public void ProvideContent(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 10);

            float fontHeight = font.GetHeight();

            int startX = 0;
            int startY = 0;
            int Offset = 20;

            // Page width for 58mm thermal printer
            int pageWidth = 200; // Approximately 58mm in pixels at 203 DPI
            e.PageSettings.PaperSize = new PaperSize("Custom", pageWidth, 200);


            // Header
            string centerText = "විනු Trading";
            graphics.DrawString(centerText, new Font("Courier New", 12, FontStyle.Bold),
                                new SolidBrush(Color.Black), (pageWidth - graphics.MeasureString(centerText, new Font("Courier New", 12, FontStyle.Bold)).Width) / 2, startY + Offset);
            Offset += (int)fontHeight + 5;

            string headerLine = "------------------------------------------";
            graphics.DrawString(headerLine, new Font("Courier New", 8),
                                new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += (int)fontHeight;

            string shopName = "Telephone";
            string date = DateTime.Now.ToString("MM/dd/yyyy");
            string cashier = "Cashier:";
            string cashierName = "Vinod Dilshan";

            graphics.DrawString(shopName, font, new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString("077-5470364", font, new SolidBrush(Color.Black), pageWidth - graphics.MeasureString("077-5470364", font).Width, startY + Offset);
            Offset += (int)fontHeight;
            graphics.DrawString("Date:", font, new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString(date, font, new SolidBrush(Color.Black), pageWidth - graphics.MeasureString(date, font).Width, startY + Offset);
            Offset += (int)fontHeight;
            graphics.DrawString(cashier, font, new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString(cashierName, font, new SolidBrush(Color.Black), pageWidth - graphics.MeasureString(cashierName, font).Width, startY + Offset);
            Offset += (int)fontHeight;

            graphics.DrawString(headerLine, new Font("Courier New", 8),
                                new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += (int)fontHeight;

            // Column headers
            graphics.DrawString("Description", new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString("Price", new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), pageWidth - graphics.MeasureString("Price", new Font("Courier New", 10, FontStyle.Bold)).Width, startY + Offset);
            Offset += (int)fontHeight;

            graphics.DrawString(headerLine, new Font("Courier New", 8),
                                new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += (int)fontHeight;

            // Items
            foreach (DataGridViewRow row in dgv_bill.Rows)
            {
                if (row.IsNewRow) continue;

                string description = row.Cells["name"].Value.ToString();
                string quantity = row.Cells["quantity"].Value.ToString();
                string price = row.Cells["sellPrice"].Value.ToString();
                string subtotal = row.Cells["subtotal"].Value.ToString();

                graphics.DrawString(description, font, new SolidBrush(Color.Black), startX, startY + Offset);
                if (int.TryParse(quantity, out int qty) && qty > 1)
                {
                    Offset += (int)fontHeight;
                    graphics.DrawString("x " + quantity, font, new SolidBrush(Color.Black), startX + 20, startY + Offset);
                }
                graphics.DrawString(price, font, new SolidBrush(Color.Black), pageWidth - graphics.MeasureString(price, font).Width, startY + Offset);
                Offset += (int)fontHeight;
            }
            // Totals
            graphics.DrawString(headerLine, new Font("Courier New", 8),
                                new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += (int)fontHeight;

            decimal total = 0;
            foreach (DataGridViewRow row in dgv_bill.Rows)
            {
                if (!row.IsNewRow)
                {
                    total += Convert.ToDecimal(row.Cells["subtotal"].Value);
                }
            }

            // Retrieve received amount from txt_payment
            decimal receive = 0;
            if (!string.IsNullOrEmpty(txt_payment.Text))
            {
                receive = decimal.Parse(txt_payment.Text, CultureInfo.InvariantCulture); // Assuming input is in the invariant culture format
            }

            // Calculate balance based on received amount
            decimal balance = receive - total;

            // Display totals
            string totalLabel = "Total:";
            string receiveLabel = "Receive:";
            string balanceLabel = "Balance:";

            graphics.DrawString(totalLabel, new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString(total.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"), new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), pageWidth - graphics.MeasureString(total.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"), new Font("Courier New", 10, FontStyle.Bold)).Width, startY + Offset);
            Offset += (int)fontHeight;
            graphics.DrawString(receiveLabel, new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString(receive.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"), new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), pageWidth - graphics.MeasureString(receive.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"), new Font("Courier New", 10, FontStyle.Bold)).Width, startY + Offset);
            Offset += (int)fontHeight;
            graphics.DrawString(balanceLabel, new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);
            graphics.DrawString(balance.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"), new Font("Courier New", 10, FontStyle.Bold), new SolidBrush(Color.Black), pageWidth - graphics.MeasureString(balance.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"), new Font("Courier New", 10, FontStyle.Bold)).Width, startY + Offset);
            Offset += (int)fontHeight;

            graphics.DrawString(headerLine, new Font("Courier New", 8),
                                new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += (int)fontHeight;

            // Footer
            string thankYou = "THANK YOU!";
            graphics.DrawString(thankYou, new Font("Courier New", 12, FontStyle.Bold),
                                new SolidBrush(Color.Black),
                                (pageWidth - graphics.MeasureString(thankYou, new Font("Courier New", 12, FontStyle.Bold)).Width) / 2,
                                startY + Offset);
            Offset += (int)fontHeight;

            string footer = "See you again!\n";
            // Calculate the total height of the multiline footer text
            SizeF footerSize = graphics.MeasureString(footer, font, pageWidth);
            graphics.DrawString(footer, font,
                                new SolidBrush(Color.Black),
                                (pageWidth - footerSize.Width) / 2,
                                startY + Offset);
            Offset += (int)footerSize.Height;

            // Update TextBox controls
            txt_payment.Text = receive.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"); // Display receive amount with "Rs" symbol
            txt_balance.Text = balance.ToString("C", CultureInfo.CreateSpecificCulture("si-LK")).Replace("$", "Rs"); // Display balance amount with "Rs" symbol

        }

        private void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            string itemReference = txt_search.Text;

            // Fetch and populate the item details
            FetchItemDetails(itemReference);
        }

        private void FetchItemDetails(string itemReference)
        {
            string connectionString = "Data Source=vinod.db;Version=3;New=False;Compress=True;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT reference, name, warenty, brand, cost_price, discount_price, sell_price, stock FROM stock WHERE reference = @reference";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@reference", itemReference);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Assuming you have text boxes named txt_reference, txt_name, txt_warenty, txt_brand, txt_retail, txt_discount, and txt_sell
                            txt_reference.Text = reader["reference"].ToString();
                            txt_name.Text = reader["name"].ToString();
                            txt_warenty.Text = reader["warenty"].ToString();
                            txt_brand.Text = reader["brand"].ToString();
                            txt_cost.Text = reader["cost_price"].ToString();
                            txt_discount.Text = reader["discount_price"].ToString();
                            txt_sell.Text = reader["sell_price"].ToString();
                        }
                        else
                        {
                            // Clear the text boxes if no data is found
                            ClearInputFields();
                        }
                    }
                }
                connection.Close();
            }


        }

        private void txt_payment_KeyUp(object sender, KeyEventArgs e)
        {
            if (decimal.TryParse(txt_payment.Text, out decimal payment) && decimal.TryParse(txt_sub.Text, out decimal total))
            {
                decimal balance = payment - total;
                txt_balance.Text = balance.ToString();
            }
            else
            {
                txt_balance.Text = "";
            }
        }
    }
}
