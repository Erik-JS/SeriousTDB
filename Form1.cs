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
    public partial class Form1 : Form
    {

        public List<int> lstSearchResults = new List<int>();

        public int currentSearchResultIndex;

        public Form1()
        {
            InitializeComponent();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.tdb|*.tdb";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            if (!Tdb.LoadTdbFromFile(ofd.FileName))
                return;
            lblFile.Text = ofd.FileName;
            lblFile.ForeColor = Control.DefaultForeColor;
            lblLine.Text = "-";
            mnuSave.Enabled = true;
            mnuSaveAs.Enabled = true;
            listBox1.Items.Clear();
            foreach (var t in Tdb.lstTdb)
            {
                listBox1.Items.Add(t.Item1);
            }
            txtSearch.Enabled = true;
            txtSearch.Clear();
            btnNext.Enabled = false;
            lblSearchResults.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblLine.Text = listBox1.SelectedIndex.ToString();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            ShowEditTextDialog(listBox1.SelectedIndex);
        }

        private void ShowEditTextDialog(int idx)
        {
            using (var form2 = new Form2(idx, Tdb.lstTdb[idx].Item1))
            {
                if (form2.ShowDialog() == DialogResult.OK)
                {
                    var t = Tdb.lstTdb[idx];
                    if (t.Item1 == form2.NewText)
                        return;
                    Tdb.lstTdb[idx] = new Tuple<string, int, int>(form2.NewText, t.Item2, t.Item3);
                    listBox1.Items[idx] = form2.NewText;
                    lblFile.ForeColor = Color.Red;
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || e.KeyCode != Keys.Enter)
                return;
            ShowEditTextDialog(listBox1.SelectedIndex);
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (Tdb.SaveTdbToFile(lblFile.Text))
            {
                lblFile.ForeColor = Color.Blue;
            }
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.tdb|*.tdb";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            if(Tdb.SaveTdbToFile(sfd.FileName))
            {
                lblFile.Text = sfd.FileName;
                lblFile.ForeColor = Color.Blue;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if(txtSearch.Text.Length == 0)
            {
                btnNext.Enabled = false;
                lblSearchResults.Visible = false;
                return;
            }
            lstSearchResults.Clear();
            lblSearchResults.Visible = true;
            for (int i = 0; i < Tdb.lstTdb.Count; i++)
            {
                if (Tdb.lstTdb[i].Item1.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    lstSearchResults.Add(i);
                }
            }
            if (lstSearchResults.Count == 0)
            {
                btnNext.Enabled = false;
                lblSearchResults.Text = "0/0";
                return;
            }
            btnNext.Enabled = true;
            currentSearchResultIndex = 0;
            UpdateSearchResultsLabelAndListBox();
        }

        private void UpdateSearchResultsLabelAndListBox()
        {
            lblSearchResults.Text = String.Format("{0}/{1}", currentSearchResultIndex + 1, lstSearchResults.Count);
            listBox1.SelectedIndex = lstSearchResults[currentSearchResultIndex];
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentSearchResultIndex = (++currentSearchResultIndex % lstSearchResults.Count);
            UpdateSearchResultsLabelAndListBox();
        }
    }
}
