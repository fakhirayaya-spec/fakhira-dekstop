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
    public partial class Fkategori : Form
    {
        public Fkategori()
        {
            InitializeComponent();
        }

        public void bersih()
        {
            txtkate.Text = "";
            txtdes.Text = "";
            label5.Text = "";
        }

        public void tampildata()
        {
            dataGridView1.Rows.Clear();
            db.crud($"SELECT * FROM `t_kategori`");
            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                string id = "" + baris["id_kategori"];
                string nmkat = "" + baris["nama_kategori"];
                string des = "" + baris["deskripsi"];
                dataGridView1.Rows.Add(id, nmkat, des);
            }
        }

        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }

        private void btnsimpan_Click(object sender, EventArgs e)
        {
            string nmkat = txtkate.Text;
            string des = txtdes.Text;
            db.crud($"INSERT INTO t_kategori VALUES (null, '{nmkat}','{des}')");
            bersih();
            tampildata();
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            string id = label5.Text;
            string nmkat  = txtkate.Text;
            string des = txtdes.Text;
            db.crud($"UPDATE t_kategori set nama_kategori = '{nmkat}' , deskripsi = '{des}' where id_kategori = '{id}'");
            bersih();
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int baris = e.RowIndex;
            int kolom = e.ColumnIndex;
            string idnya = dataGridView1.Rows[baris].Cells[0].Value.ToString();
            if (kolom == 3)
            {
                db.crud($"SELECT * FROM `t_kategori` where id_kategori = '{idnya}'");
                foreach (DataRow bariss in db.ds.Tables[0].Rows)
                {
                    string id = "" + bariss["id_kategori"];
                    string nmkat = "" + bariss["nama_kategori"];
                    string des = "" + bariss["deskripsi"];
                    label5.Text = id;
                    txtkate.Text = nmkat;
                    txtdes.Text = des;
                }
            }
            if (kolom == 4)
            {
                DialogResult setuju = MessageBox.Show("apakah kamu ingin hapus data?", "pemberitahuan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (setuju == DialogResult.Yes)
                {
                    db.crud($"DELETE FROM t_kategori WHERE id_kategori = '{idnya}'");
                    bersih();
                    tampildata();
                }
            }
        }
    }
}
