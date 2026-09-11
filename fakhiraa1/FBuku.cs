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
    public partial class FBuku : Form
    {
        public FBuku()
        {
            InitializeComponent();
        }

        public void bersih()
        {
            txtkodebuku.Text = "";
            txtjudul.Text = "";
            txtpenulis.Text = "";
            txtpenerbit.Text = "";
            txttahun.Text = "";
            txtstok.Text = "";
            txtdes.Text = "";
            cmbkat.SelectedIndex = -1;
            cmbstatus.SelectedIndex = -1;
            label5.Text = "";
        }

        public void tampildata()
        {
            dataGridView1.Rows.Clear();
            db.crud($"SELECT * FROM `t_buku`");
            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                string id = "" + baris["id_buku"];
                string kd = "" + baris["kode_buku"];
                string judul = "" + baris["judul"];
                string pen = "" + baris["penulis"];
                string pener = "" + baris["penerbit"];
                string tt = "" + baris["tahun_terbit"];
                string idkat = "" + baris["id_kategori"];
                string stok = "" + baris["stok"];
                string des = "" + baris["deskripsi"];
                string ss = "" + baris["status"];
               dataGridView1.Rows.Add(id, kd, judul, pen, pener, tt, idkat, stok, des, ss);
            }
        }

        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }

        private void cmbstatus_DropDown(object sender, EventArgs e)
        {
            cmbstatus.Items.Clear();
            cmbstatus.Items.Add("tersedia");
            cmbstatus.Items.Add("dipinjam");
        }

        private void btnsimpan_Click(object sender, EventArgs e)
        {
            string kd = txtkodebuku.Text;
            string judul = txtjudul.Text;
            string pen = txtpenulis.Text;
            string pener = txtpenerbit.Text;
            string tt = txttahun.Text;
            string idkat = cmbkat.SelectedValue.ToString();
            string stok = txtstok.Text;
            string des = txtdes.Text;
            string ss = cmbstatus.Text;
            db.crud($"INSERT INTO t_buku VALUES (null, '{kd}','{judul}','{pen}','{pener}','{tt}','{idkat}','{stok}','{des}','{ss}')");
            bersih();
            tampildata();
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            string id = label5.Text;
            string kd = txtkodebuku.Text;
            string judul = txtjudul.Text;
            string pen = txtpenulis.Text;
            string pener = txtpenerbit.Text;
            string tt = txttahun.Text;
            string idkat = cmbkat.SelectedValue.ToString();
            string stok = txtstok.Text;
            string des = txtdes.Text;
            string ss = cmbstatus.Text;
            db.crud($"UPDATE t_buku set kode_buku = '{kd}' , judul = '{judul}', penulis = '{pen}' , penerbit = '{pener}' , tahun_terbit = '{tt}' , id_kategori = '{idkat}' , stok = '{stok}' , deskripsi = '{des}' , status = '{ss}'  where id_buku = '{id}'");
            bersih();
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int baris = e.RowIndex;
            int kolom = e.ColumnIndex;
            string idnya = dataGridView1.Rows[baris].Cells[0].Value.ToString();
            if (kolom == 10)
            {
                db.crud($"SELECT * FROM `t_buku` where id_buku = '{idnya}'");
                foreach (DataRow bariss in db.ds.Tables[0].Rows)
                {
                    string id = "" + bariss["id_buku"];
                    string kd = "" + bariss["kode_buku"];
                    string judul = "" + bariss["judul"];
                    string pen = "" + bariss["penulis"];
                    string pener = "" + bariss["penerbit"];
                    string tt = "" + bariss["tahun_terbit"];
                    string idkat = "" + bariss["id_kategori"].ToString();
                    string stok = "" + bariss["stok"];
                    string des = "" + bariss["deskripsi"];
                    string ss = "" + bariss["status"];
                    label5.Text = id;
                    txtkodebuku.Text = kd;
                    txtjudul.Text = judul;
                    txtpenulis.Text = pen;
                    txtpenerbit.Text = pener;
                    txttahun.Text = tt;
                    cmbkat.SelectedValue = idkat;
                    txtstok.Text = stok;
                    txtdes.Text = des;
                    cmbstatus.Text = ss;
                }
            }
            if (kolom == 11)
            {
                DialogResult setuju = MessageBox.Show("apakah kamu ingin hapus data?", "pemberitahuan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (setuju == DialogResult.Yes)
                {
                    db.crud($"DELETE FROM t_buku WHERE id_buku = '{idnya}'");
                    bersih();
                    tampildata();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FBuku_Load(object sender, EventArgs e)
        {
            db.crud("SELECT * FROM t_kategori");

            cmbkat.DataSource = db.ds.Tables[0];
            cmbkat.DisplayMember = "nama_kategori";
            cmbkat.ValueMember = "id_kategori";
        }
    }
}
