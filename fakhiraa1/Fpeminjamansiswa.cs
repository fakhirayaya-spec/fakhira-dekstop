using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fpeminjamansiswa : Form
    {
        // ID anggota/siswa yang sedang login
        private int idAnggota;

        // Constructor untuk Designer
        public Fpeminjamansiswa()
        {
            InitializeComponent();
        }

        // Constructor untuk membuka form berdasarkan ID siswa
        public Fpeminjamansiswa(int idAnggota)
        {
            InitializeComponent();

            this.idAnggota = idAnggota;
        }

        private void Fpeminjamansiswa_Load(object sender, EventArgs e)
        {
            // Kunci textbox agar hanya bisa dibaca
            kunciTextBoxDetail();

            // Tampilkan data peminjaman siswa
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
        // TAMPILKAN DATA PEMINJAMAN SISWA
        // =========================================================

        public void tampildata()
        {
            dataGridView1.Rows.Clear();

            string query = @"
                SELECT
                    p.id_pinjam,
                    p.kode_pinjam,
                    b.judul,
                    p.tanggal_pinjam AS tgl_pinjam,
                    pg.tanggal_kembali AS tgl_kembali,
                    p.status,
                    IFNULL(pg.denda, 0) AS denda
                FROM t_peminjaman p
                INNER JOIN t_buku b
                    ON p.id_buku = b.id_buku
                LEFT JOIN t_pengembalian pg
                    ON p.id_pinjam = pg.id_pinjam
                WHERE p.id_anggota = '" + idAnggota + "'";

            // =====================================================
            // PENCARIAN
            // =====================================================

            if (!string.IsNullOrWhiteSpace(txtcari.Text))
            {
                string cari = txtcari.Text.Trim();

                // Supaya tanda petik tidak membuat query error
                cari = cari.Replace("'", "''");

                query += @"
                    AND (
                        b.judul LIKE '%" + cari + @"%'
                        OR p.kode_pinjam LIKE '%" + cari + @"%'
                    )";
            }

            // Urutkan dari peminjaman terbaru
            query += " ORDER BY p.tanggal_pinjam DESC";

            // Jalankan query
            db.crud(query);

            // =====================================================
            // MASUKKAN DATA KE DATAGRIDVIEW
            // =====================================================

            if (db.ds != null &&
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;

                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string judul = baris["judul"].ToString();

                    // -----------------------------
                    // TANGGAL PINJAM
                    // -----------------------------

                    string tglPinjam = "-";

                    if (baris["tgl_pinjam"] != DBNull.Value &&
                        !string.IsNullOrWhiteSpace(baris["tgl_pinjam"].ToString()))
                    {
                        DateTime tanggalPinjam =
                            Convert.ToDateTime(baris["tgl_pinjam"]);

                        tglPinjam = tanggalPinjam.ToString("dd/MM/yyyy");
                    }

                    // -----------------------------
                    // TANGGAL KEMBALI
                    // -----------------------------

                    string tglKembali = "-";

                    if (baris["tgl_kembali"] != DBNull.Value &&
                        !string.IsNullOrWhiteSpace(baris["tgl_kembali"].ToString()))
                    {
                        DateTime tanggalKembali =
                            Convert.ToDateTime(baris["tgl_kembali"]);

                        tglKembali = tanggalKembali.ToString("dd/MM/yyyy");
                    }

                    // -----------------------------
                    // STATUS
                    // -----------------------------

                    string status = baris["status"].ToString();

                    // -----------------------------
                    // DENDA
                    // -----------------------------

                    decimal nominalDenda = 0;

                    if (baris["denda"] != DBNull.Value)
                    {
                        nominalDenda =
                            Convert.ToDecimal(baris["denda"]);
                    }

                    string dendaStr;

                    if (nominalDenda > 0)
                    {
                        dendaStr = "Rp " + nominalDenda.ToString("N0");
                    }
                    else
                    {
                        dendaStr = "Rp 0";
                    }

                    // -----------------------------
                    // TAMBAHKAN KE DATAGRID
                    // -----------------------------

                    int index = dataGridView1.Rows.Add(
                        no++,
                        judul,
                        tglPinjam,
                        tglKembali,
                        status,
                        dendaStr
                    );

                    // Simpan DataRow di Tag
                    dataGridView1.Rows[index].Tag = baris;
                }
            }
        }

        // =========================================================
        // ISI DETAIL PEMINJAMAN
        // =========================================================

        private void isiDetailPinjaman()
        {
            if (dataGridView1.CurrentRow == null)
                return;

            if (dataGridView1.CurrentRow.Tag == null)
                return;

            DataRow baris =
                (DataRow)dataGridView1.CurrentRow.Tag;

            // -----------------------------
            // JUDUL
            // -----------------------------

            txtjudul.Text =
                baris["judul"].ToString();

            // -----------------------------
            // TANGGAL PINJAM
            // -----------------------------

            if (baris["tgl_pinjam"] != DBNull.Value)
            {
                DateTime tanggalPinjam =
                    Convert.ToDateTime(baris["tgl_pinjam"]);

                txttglpinjam.Text =
                    tanggalPinjam.ToString("dd/MM/yyyy");
            }
            else
            {
                txttglpinjam.Text = "-";
            }

            // -----------------------------
            // TANGGAL KEMBALI
            // -----------------------------

            if (baris["tgl_kembali"] != DBNull.Value)
            {
                DateTime tanggalKembali =
                    Convert.ToDateTime(baris["tgl_kembali"]);

                txttglkembali.Text =
                    tanggalKembali.ToString("dd/MM/yyyy");
            }
            else
            {
                txttglkembali.Text = "-";
            }

            // -----------------------------
            // STATUS
            // -----------------------------

            txtstatus.Text =
                baris["status"].ToString();

            // -----------------------------
            // DENDA
            // -----------------------------

            decimal nominalDenda = 0;

            if (baris["denda"] != DBNull.Value)
            {
                nominalDenda =
                    Convert.ToDecimal(baris["denda"]);
            }

            if (nominalDenda > 0)
            {
                txtdenda.Text =
                    "Rp " + nominalDenda.ToString("N0");
            }
            else
            {
                txtdenda.Text = "Rp 0";
            }
        }

        // =========================================================
        // EVENT PENCARIAN
        // =========================================================

        private void txtcari_TextChanged(object sender, EventArgs e)
        {
            tampildata();
        }

        // =========================================================
        // SAAT BARIS DATAGRID DIKLIK
        // =========================================================

        private void dataGridView1_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            isiDetailPinjaman();
        }

        // =========================================================
        // SAAT BARIS BERUBAH
        // =========================================================

        private void dataGridView1_SelectionChanged(
            object sender,
            EventArgs e)
        {
            isiDetailPinjaman();
        }

        // =========================================================
        // LABEL
        // =========================================================

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}