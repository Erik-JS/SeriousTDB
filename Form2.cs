using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeriousTDB
{
    public partial class Form2 : Form
    {

        public string NewText;

        public Form2(int entrynumber, string text)
        {
            InitializeComponent();
            label1.Text = "#" + entrynumber.ToString();
            textBox1.Text = text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            NewText = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
