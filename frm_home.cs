using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Application
{
    public partial class frm_home : Form
    {
        public frm_home()
        {
            InitializeComponent();
        }

        private void ShowFormInPanel(Form form)
        {
            // Clear existing controls from panel1
            panel2.Controls.Clear();

            // Set form properties
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            // Add form to panel1
            panel2.Controls.Add(form);

            // Show form
            form.Show();
        }

        private void btn_adduser_Click(object sender, EventArgs e)
        {
            ShowFormInPanel(new add_user());
        }

        private void btn_neworder_Click(object sender, EventArgs e)
        {

            ShowFormInPanel(new frm_order());
        }

        private void btn_stock_Click(object sender, EventArgs e)
        {
            ShowFormInPanel(new frm_stock());
        }

        private void bnt_exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
