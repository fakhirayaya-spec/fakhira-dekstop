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

        // =========================================================
        // KUNCI TEXTBOX DETAIL
        // =========================================================

        private void kunciTextBoxDetail()
        {
            txtjudul.ReadOnly = true;
            txttglpinjam.ReadOnly = true;
            txttglkembali.ReadOnly = true;
            txtstatus.ReadOnly = true;
            txtdenda.ReadOnly = true;
        }

        // =========================================================
        // TAMPILKAN DATA RIWAYAT PEMINJAMAN SISWA
        // =========================================================

        public void tampildata()
        {
            dataGridView1.Rows.Clear();

            // Query disesuaikan persis dengan relasi tabel di database kamu
            string query = $@"
                SELECT
                    p.id_pinjam,
                    p.kode_pinjam,
                    b.judul,
                    p.tanggal_pinjam AS tgl_pinjam,
                    p.tanggal_jatuh_tempo AS tenggat_kembali,
                    pg.tanggal_kembali AS tgl_kembali,
                    p.status,
                    IFNULL(d.jumlah_bayar, IFNULL(pg.denda, 0)) AS denda
                FROM t_peminjaman p
                INNER JOIN t_buku b ON p.id_buku = b.id_buku
                LEFT JOIN t_pengembalian pg ON p.id_pinjam = pg.id_pinjam
                LEFT JOIN t_denda d ON p.id_pinjam = d.id_pinjam
                WHERE p.id_anggota = '{Form1.idAnggotaLogin}'";

            // Pencarian berdasarkan Judul atau Kode Pinjam
            if (!string.IsNullOrWhiteSpace(txtcari.Text))
            {
                string cari = txtcari.Text.Trim().Replace("'", "''");
                query += $@" AND (b.judul LIKE '%{cari}%' OR p.kode_pinjam LIKE '%{cari}%')";
            }

            query += " ORDER BY p.tanggal_pinjam DESC";

            // Jalankan query ke database
            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;

                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string judul = baris["judul"].ToString();

                    // Tanggal Pinjam
                    string tglPinjam = "-";
                    if (baris["tgl_pinjam"] != DBNull.Value && !string.IsNullOrWhiteSpace(baris["tgl_pinjam"].ToString()))
                    {
                        tglPinjam = Convert.ToDateTime(baris["tgl_pinjam"]).ToString("dd/MM/yyyy");
                    }

                    // Tenggat Kembali / Tanggal Jatuh Tempo
                    string tenggatKembali = "-";
                    if (baris["tenggat_kembali"] != DBNull.Value && !string.IsNullOrWhiteSpace(baris["tenggat_kembali"].ToString()))
                    {
                        tenggatKembali = Convert.ToDateTime(baris["tenggat_kembali"]).ToString("dd/MM/yyyy");
                    }

                    // Status Peminjaman
                    string status = baris["status"].ToString();

                    // Nominal Denda
                    decimal nominalDenda = 0;
                    if (baris["denda"] != DBNull.Value)
                    {
                        nominalDenda = Convert.ToDecimal(baris["denda"]);
                    }
                    string dendaStr = nominalDenda > 0 ? "Rp " + nominalDenda.ToString("N0") : "Rp 0";

                    // Tambahkan baris ke DataGridView (6 Kolom)
                    int index = dataGridView1.Rows.Add(
                        no++,
                        judul,
                        tglPinjam,
                        tenggatKembali,
                        status,
                        dendaStr
                    );

                    // Simpan objek DataRow ke dalam Tag agar mudah diambil saat diklik
                    dataGridView1.Rows[index].Tag = baris;
                }
            }
        }

        // =========================================================
        // ISI DETAIL PEMINJAMAN KE TEXTBOX
        // =========================================================

        private void isiDetailPinjaman()
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Tag == null)
                return;

            DataRow baris = (DataRow)dataGridView1.CurrentRow.Tag;

            // Judul Buku
            txtjudul.Text = baris["judul"].ToString();

            // Tanggal Pinjam
            if (baris["tgl_pinjam"] != DBNull.Value)
            {
                txttglpinjam.Text = Convert.ToDateTime(baris["tgl_pinjam"]).ToString("dd/MM/yyyy");
            }
            else
            {
                txttglpinjam.Text = "-";
            }

            // Tanggal Kembali (Menampilkan tanggal pengembalian jika sudah dikembalikan, atau tenggat kembali)
            if (baris["tgl_kembali"] != DBNull.Value && !string.IsNullOrWhiteSpace(baris["tgl_kembali"].ToString()))
            {
                txttglkembali.Text = Convert.ToDateTime(baris["tgl_kembali"]).ToString("dd/MM/yyyy");
            }
            else if (baris["tenggat_kembali"] != DBNull.Value)
            {
                txttglkembali.Text = Convert.ToDateTime(baris["tenggat_kembali"]).ToString("dd/MM/yyyy");
            }
            else
            {
                txttglkembali.Text = "-";
            }

            // Status
            txtstatus.Text = baris["status"].ToString();

            // Denda
            decimal nominalDenda = 0;
            if (baris["denda"] != DBNull.Value)
            {
                nominalDenda = Convert.ToDecimal(baris["denda"]);
            }
            txtdenda.Text = nominalDenda > 0 ? "Rp " + nominalDenda.ToString("N0") : "Rp 0";
        }

        // =========================================================
        // EVENT HANDLER
        // =========================================================

        private void txtcari_TextChanged(object sender, EventArgs e)
        {
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            isiDetailPinjaman();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            isiDetailPinjaman();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}