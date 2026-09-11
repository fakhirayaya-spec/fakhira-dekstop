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
    public partial class masterrole : Form
    {
        public masterrole()
        {
            InitializeComponent();
        }


        public void bersih()
        {
            TXTNAMA_ROLE.Text = "";


        }

        public void tampildata()
        {
            dataGridView1.Rows.Clear();
            db.crud($"SELECT * FROM `trole`");
            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                string idr = "" + baris["id_role"];
                string nm = "" + baris["nama_role"];
                dataGridView1.Rows.Add(idr, nm);
            }
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string nm = TXTNAMA_ROLE.Text;
            db.crud($"INSERT INTO trole VALUES (null,'{nm}')");
            bersih();
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int baris = e.RowIndex;
            int kolom = e.ColumnIndex;

            string idnya = dataGridView1.Rows[baris].Cells[0].Value.ToString();

            if (kolom == 2)
            {
                db.crud($"SELECT * FROM trole WHERE id_role='{idnya}'");

                foreach (DataRow bariss in db.ds.Tables[0].Rows)
                {
                    label1.Text = bariss["id_role"].ToString();
                    TXTNAMA_ROLE.Text = bariss["nama_role"].ToString();
                }
            }

            if (kolom == 3)
            {
                DialogResult ya = MessageBox.Show("Apakah ingin menghapus data?", "Pemberitahuan", MessageBoxButtons.YesNo);

                if (ya == DialogResult.Yes)
                {
                    db.crud($"DELETE FROM trole WHERE id_role='{idnya}'");
                    tampildata();
                }
            }
        }

        private void masterrole_Load(object sender, EventArgs e)
        {
            tampildata();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string nm = TXTNAMA_ROLE.Text;
            db.crud($"UPDATE trole SET nama_role = '{TXTNAMA_ROLE.Text}' WHERE id_role = '{label1.Text}'");
            bersih();
            tampildata();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            tampildata();
        }
    }
}
