using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fpeminjamansiswa : Form
    {
        public Fpeminjamansiswa()
        {
            InitializeComponent();
        }

        private void Fpeminjamansiswa_Load(object sender, EventArgs e)
        {
            kunciTextBoxDetail();
            tampildata();
        }

        // KUNCI SEMUA TEXTBOX DETAIL AGAR HANYA BISA DIBACA
        private void kunciTextBoxDetail()
        {
            txtjudul.Enabled = false;
            txttglpinjam.Enabled = false;
            txttglkembali.Enabled = false;
            txtstatus.Enabled = false;
            txtdenda.Enabled = false;
        }

        // TAMPILKAN DATA PEMINJAMAN
        public void tampildata()
        {
            dataGridView1.Rows.Clear();

            string query = @"
                SELECT 
                    p.id_pinjam,
                    b.judul,
                    p.tgl_peminjaman AS tgl_pinjam,
                    p.tgl_pengembalian AS tgl_kembali,
                    p.status,
                    IFNULL(p.denda, 0) AS denda
                FROM t_peminjaman p
                JOIN t_buku b ON p.id_buku = b.id_buku
                WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(txtcari.Text))
            {
                string cari = txtcari.Text.Trim();
                query += $" AND (b.judul LIKE '%{cari}%' OR p.id_pinjam LIKE '%{cari}%')";
            }

            query += " ORDER BY p.tgl_peminjaman DESC";

            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;
                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string judul = baris["judul"].ToString();
                    string tglPinjam = Convert.ToDateTime(baris["tgl_pinjam"]).ToString("dd/MM/yyyy");
                    string tglKembali = Convert.ToDateTime(baris["tgl_kembali"]).ToString("dd/MM/yyyy");
                    string status = baris["status"].ToString();

                    decimal nominalDenda = Convert.ToDecimal(baris["denda"]);
                    string dendaStr = nominalDenda > 0 ? "Rp " + nominalDenda.ToString("N0") : "Rp 0";

                    int index = dataGridView1.Rows.Add(no++, judul, tglPinjam, tglKembali, status, dendaStr);
                    dataGridView1.Rows[index].Tag = baris;
                }
            }
        }

        // ISI DETAIL SAAT BARIS DIKLIK
        private void isiDetailPinjaman()
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Tag != null)
            {
                DataRow baris = (DataRow)dataGridView1.CurrentRow.Tag;

                txtjudul.Text = baris["judul"].ToString();
                txttglpinjam.Text = Convert.ToDateTime(baris["tgl_pinjam"]).ToString("dd/MM/yyyy");
                txttglkembali.Text = Convert.ToDateTime(baris["tgl_kembali"]).ToString("dd/MM/yyyy");
                txtstatus.Text = baris["status"].ToString();

                decimal nominalDenda = Convert.ToDecimal(baris["denda"]);
                txtdenda.Text = nominalDenda > 0 ? "Rp " + nominalDenda.ToString("N0") : "Rp 0";
            }
        }

        // EVENT HANDLERS
        private void txtcari_TextChanged(object sender, EventArgs e)
        {
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            isiDetailPinjaman();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            isiDetailPinjaman();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            //cobaingithub
        }
    }
}