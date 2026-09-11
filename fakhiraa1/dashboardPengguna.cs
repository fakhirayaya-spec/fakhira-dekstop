using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class dashboardPengguna : Form
    {
        public dashboardPengguna()
        {
            InitializeComponent();
        }

        public void bersih()
        {
            TXT_NAMA.Text = "";
            TXT_USER.Text = "";
            TXT_PASS.Text = "";
            CMBROLE.SelectedIndex = -1;
            label3.Text = "";
        }

        public void tampildata()
        {
            dataGridView2.Rows.Clear();
            db.crud($"SELECT * FROM `tuser`");
            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                string id = "" + baris["id"];
                string nm = "" + baris["nama"];
                string user = "" + baris["username"];
                string pass = new string('*', 8);
                string rl = "" + baris["id_role"];
                dataGridView2.Rows.Add(id, nm, user, pass, rl);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            string nm = TXT_NAMA.Text;
            string usr = TXT_USER.Text;
            string pass = TXT_PASS.Text;
            string rl = CMBROLE.Text;
            db.crud($"INSERT INTO tuser VALUES (null, '{nm}','{usr}','{pass}','{rl}')");
            bersih();
            tampildata();
        }

        private void CMBROLE_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void CMBROLE_DropDown(object sender, EventArgs e)
        {
            CMBROLE.Items.Clear();
            CMBROLE.Items.Add(1);
            CMBROLE.Items.Add(2);
        }

        private void TXT_USER_TextChanged(object sender, EventArgs e)
        {

        }



        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string id = label3.Text;
            string nm = TXT_NAMA.Text;
            string user = TXT_USER.Text;
            string pass = TXT_PASS.Text;
            string id_role = CMBROLE.Text;
            db.crud($"UPDATE tuser set nama = '{nm}' , username = '{user}', password = '{pass}' , id_role = '{id_role}' where id = '{id}'");
            bersih();
            tampildata();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            tampildata();
        }

        

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int baris = e.RowIndex;
            int kolom = e.ColumnIndex;
            string idnya = dataGridView2.Rows[baris].Cells[0].Value.ToString();
            if (kolom == 5)
            {
                db.crud($"SELECT * FROM `tuser` where id = '{idnya}'");
                foreach (DataRow bariss in db.ds.Tables[0].Rows)
                {
                    string id = "" + bariss["id"];
                    string nm = "" + bariss["nama"];
                    string user = "" + bariss["username"];
                    string pass = "" + bariss["password"];
                    string role = "" + bariss["id_role"];
                    label3.Text = id;
                    TXT_NAMA.Text = nm;
                    TXT_USER.Text = user;
                    TXT_PASS.Text = pass;
                    CMBROLE.Text = role;
                }
            }
            if (kolom == 6)
            {
                DialogResult setuju = MessageBox.Show("apakah kamu ingin hapus data?", "pemberitahuan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (setuju == DialogResult.Yes)
                {
                    db.crud($"DELETE FROM tuser WHERE id = '{idnya}'");
                    bersih();
                    tampildata();
                }
            }
    }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

