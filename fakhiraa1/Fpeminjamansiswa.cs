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

        // =========================================================
        // EVENT LOAD (OTOMATIS JALAN SAAT FORM DIBUKA)
        // =========================================================
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
            if (txtjudul != null) txtjudul.ReadOnly = true;
            if (txttglpinjam != null) txttglpinjam.ReadOnly = true;
            if (txttglkembali != null) txttglkembali.ReadOnly = true;
            if (txtdenda != null) txtdenda.ReadOnly = true;
            if (txtstatus != null) txtstatus.ReadOnly = true;
        }

        // =========================================================
        // TAMPILKAN DATA DARI DATABASE KE DATAGRIDVIEW
        // =========================================================
        public void tampildata()
        {
            if (dataGridView1 == null) return;

            dataGridView1.Rows.Clear();

            int idLogin = Form1.idAnggotaLogin;

            // Query diperbaiki: Menggunakan p.id_anggota saja
            string query = $@"
        SELECT 
            p.id_pinjam,
            p.kode_pinjam,
            IFNULL(b.judul, 'Buku Tanpa Judul') AS judul,
            p.tanggal_pinjam AS tgl_pinjam,
            p.tanggal_jatuh_tempo AS tenggat_kembali,
            pg.tanggal_kembali AS tgl_kembali,
            p.status,
            IFNULL(pg.denda, 0) AS denda
        FROM t_peminjaman p
        LEFT JOIN t_buku b ON p.id_buku = b.id_buku
        LEFT JOIN t_pengembalian pg ON p.id_pinjam = pg.id_pinjam
        WHERE p.id_anggota = '{idLogin}'";

            // Filter Pencarian jika txtcari diisi
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
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (txtjudul != null) txtjudul.Text = baris["judul"].ToString();

            if (txttglpinjam != null)
            {
                txttglpinjam.Text = (baris["tgl_pinjam"] != DBNull.Value)
                    ? Convert.ToDateTime(baris["tgl_pinjam"]).ToString("dd/MM/yyyy")
                    : "-";
            }

            if (txttglkembali != null)
            {
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
            }

            if (txtstatus != null) txtstatus.Text = baris["status"].ToString();

            if (txtdenda != null)
            {
                decimal nominalDenda = 0;
                if (baris["denda"] != DBNull.Value)
                {
                    nominalDenda = Convert.ToDecimal(baris["denda"]);
                }
                txtdenda.Text = nominalDenda > 0 ? "Rp " + nominalDenda.ToString("N0") : "Rp 0";
            }
        }

        // =========================================================
        // EVENT HANDLER DARI DESIGNER
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

        // Handler cadangan agar tidak error jika event terduplikasi di Designer
        private void Fpeminjamansiswa_Load_1(object sender, EventArgs e) { Fpeminjamansiswa_Load(sender, e); }
        private void txtcari_TextChanged_1(object sender, EventArgs e) { txtcari_TextChanged(sender, e); }
        private void label2_Click(object sender, EventArgs e) { }
    }
}