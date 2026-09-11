using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fkatalogsis : Form
    {
        public Fkatalogsis()
        {
            InitializeComponent();
        }

        private void Fkatalogsis_Load(object sender, EventArgs e)
        {
           
            kunciTextBoxDetail();

            tampilKategori();
            tampildata();
        }

       
        private void kunciTextBoxDetail()
        {
            txtjudul.Enabled = false;
            txtpenulis.Enabled = false;
            txtpenerbit.Enabled = false;
            txttahun.Enabled = false;
            txtnamakategori.Enabled = false;
            txtdeskripsi.Enabled = false;
        }

      
        public void tampilKategori()
        {
            cmbkategori.Items.Clear();
            cmbkategori.Items.Add("-- Semua Kategori --");

            string query = "SELECT DISTINCT nama_kategori FROM t_kategori ORDER BY nama_kategori ASC";
            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    cmbkategori.Items.Add(baris["nama_kategori"].ToString());
                }
            }

            if (cmbkategori.Items.Count > 0)
            {
                cmbkategori.SelectedIndex = 0;
            }
        }

        
        public void tampildata()
        {
            dataGridView1.Rows.Clear();

            string query = @"
                SELECT 
                    b.kode_buku,
                    b.judul,
                    b.penulis,
                    b.penerbit,
                    b.tahun_terbit,
                    IFNULL(k.nama_kategori, '-') AS nama_kategori,
                    b.stok,
                    b.status,
                    b.deskripsi
                FROM t_buku b
                LEFT JOIN t_kategori k ON b.id_kategori = k.id_kategori
                WHERE 1=1";

            
            if (cmbkategori.SelectedIndex > 0 && cmbkategori.SelectedItem != null)
            {
                string kat = cmbkategori.SelectedItem.ToString();
                query += $" AND k.nama_kategori = '{kat}'";
            }

           
            if (!string.IsNullOrWhiteSpace(txtcari.Text))
            {
                string cari = txtcari.Text.Trim();
                query += $" AND (b.judul LIKE '%{cari}%' OR b.penulis LIKE '%{cari}%' OR b.kode_buku LIKE '%{cari}%')";
            }

            query += " GROUP BY b.kode_buku, b.judul ORDER BY b.judul ASC";

            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;
                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string kode = baris["kode_buku"].ToString();
                    string judul = baris["judul"].ToString();
                    string penulis = baris["penulis"].ToString();
                    string kategori = baris["nama_kategori"].ToString();
                    string stok = baris["stok"].ToString();
                    string status = baris["status"].ToString();

                    int index = dataGridView1.Rows.Add(no++, kode, judul, penulis, kategori, stok, status);
                    dataGridView1.Rows[index].Tag = baris;
                }
            }
        }

       
        private void isiDetailBuku()
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Tag != null)
            {
                DataRow baris = (DataRow)dataGridView1.CurrentRow.Tag;

                txtjudul.Text = baris["judul"].ToString();
                txtpenulis.Text = baris["penulis"].ToString();
                txtpenerbit.Text = baris["penerbit"].ToString();
                txttahun.Text = baris["tahun_terbit"].ToString();
                txtnamakategori.Text = baris["nama_kategori"].ToString();
                txtdeskripsi.Text = baris["deskripsi"].ToString();
            }
        }

       
        private void txtcari_TextChanged(object sender, EventArgs e)
        {
            tampildata(); 
        }

        private void cmbkategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            tampildata();
        }

        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            isiDetailBuku();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            isiDetailBuku();
        }

        private void btntampil_Click_1(object sender, EventArgs e)
        {

        }
    }
}