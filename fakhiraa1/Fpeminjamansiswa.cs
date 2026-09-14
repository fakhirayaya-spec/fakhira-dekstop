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

        public Fpeminjamansiswa(int id)
        {
            InitializeComponent();
            Form1.idAnggotaLogin = id;
        }

        private void Fpeminjamansiswa_Load(object sender, EventArgs e)
        {
            kunciTextBoxDetail();
            tampildata();
        }

        // =========================================================
        // KUNCI TEXTBOX (READ-ONLY)
        // =========================================================
        private void kunciTextBoxDetail()
        {
            txtjudul.ReadOnly = true;
            txttglpinjam.ReadOnly = true;
            txttglkembali.ReadOnly = true;
            txtdenda.ReadOnly = true;
            txtstatus.ReadOnly = true;
        }

        // =========================================================
        // TAMPILKAN DATA DARI DATABASE KE DATAGRIDVIEW
        // =========================================================
        public void tampildata()
        {
            if (dataGridView1 == null) return;

            dataGridView1.Rows.Clear();

            // Query Utama
            string query = $@"
        SELECT 
            p.id_pinjam,
            p.kode_pinjam,
            IFNULL(b.judul, 'Buku Tanpa Judul') AS judul,
            p.tanggal_pinjam AS tgl_pinjam,
            p.tanggal_jatuh_tempo AS tenggat_kembali,
            pg.tanggal_kembali AS tgl_kembali,
            p.status,
            IFNULL(d.jumlah_bayar, IFNULL(pg.denda, 0)) AS denda
        FROM t_peminjaman p
        LEFT JOIN t_buku b ON p.id_buku = b.id_buku OR p.buku = b.id_buku
        LEFT JOIN t_pengembalian pg ON p.id_pinjam = pg.id_pinjam
        LEFT JOIN t_denda d ON p.id_pinjam = d.id_pinjam
        WHERE (p.id_anggota = '{Form1.idAnggotaLogin}' OR p.anggota = '{Form1.idAnggotaLogin}')";

            // Filter Pencarian (Hanya aktif jika txtcari diisi)
            if (txtcari != null && !string.IsNullOrWhiteSpace(txtcari.Text))
            {
                string cari = txtcari.Text.Trim().Replace("'", "''");
                query += $" AND (b.judul LIKE '%{cari}%' OR p.kode_pinjam LIKE '%{cari}%')";
            }

            query += " ORDER BY p.tanggal_pinjam DESC";

            try
            {
                db.crud(query);

                if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                {
                    int no = 1;

                    foreach (DataRow baris in db.ds.Tables[0].Rows)
                    {
                        string judul = baris["judul"].ToString();

                        string tglPinjam = "-";
                        if (baris["tgl_pinjam"] != DBNull.Value && !string.IsNullOrWhiteSpace(baris["tgl_pinjam"].ToString()))
                        {
                            tglPinjam = Convert.ToDateTime(baris["tgl_pinjam"]).ToString("dd/MM/yyyy");
                        }

                        string tenggatKembali = "-";
                        if (baris["tenggat_kembali"] != DBNull.Value && !string.IsNullOrWhiteSpace(baris["tenggat_kembali"].ToString()))
                        {
                            tenggatKembali = Convert.ToDateTime(baris["tenggat_kembali"]).ToString("dd/MM/yyyy");
                        }

                        string status = baris["status"].ToString();

                        decimal nominalDenda = 0;
                        if (baris["denda"] != DBNull.Value)
                        {
                            nominalDenda = Convert.ToDecimal(baris["denda"]);
                        }
                        string dendaStr = nominalDenda > 0 ? "Rp " + nominalDenda.ToString("N0") : "Rp 0";

                        int index = dataGridView1.Rows.Add(
                            no++,
                            judul,
                            tglPinjam,
                            tenggatKembali,
                            status,
                            dendaStr
                        );

                        dataGridView1.Rows[index].Tag = baris;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        // =========================================================
        // ISI DETAIL KE TEXTBOX SAAT BARIS TABEL DIKLIK
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

            // Tanggal Kembali (Menampilkan tanggal pengembalian jika ada, atau tenggat kembali)
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
            if (e.RowIndex >= 0)
            {
                isiDetailPinjaman();
            }
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