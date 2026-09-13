using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Flaporan : Form
    {
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreview = new PrintPreviewDialog();

        // Variable kontrol cetak halaman
        private int currentTable = 0; // 0: Ringkasan, 1: Peminjaman, 2: Pengembalian
        private int currentRow = 0;   // Baris yang sedang dicetak
        private int nomorHalaman = 1;

        public Flaporan()
        {
            InitializeComponent();

            btntampil.Click -= btntampil_Click;
            btntampil.Click += btntampil_Click;

            btncetak.Click -= btncetak_Click;
            btncetak.Click += btncetak_Click;

            printDocument.PrintPage += printDocument_PrintPage;
            printDocument.DefaultPageSettings.Landscape = true;
            printDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
        }

        private void Flaporan_Load(object sender, EventArgs e)
        {
            dtpdari.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpsampai.Value = DateTime.Now;
            tampildata();
        }

        public void tampildata()
        {
            if (dtpdari.Value.Date > dtpsampai.Value.Date)
            {
                MessageBox.Show("Tanggal dari tidak boleh lebih besar dari tanggal sampai.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tanggalDari = dtpdari.Value.ToString("yyyy-MM-dd");
            string tanggalSampai = dtpsampai.Value.ToString("yyyy-MM-dd");

            tampilRingkasan(tanggalDari, tanggalSampai);
            tampilPeminjaman(tanggalDari, tanggalSampai);
            tampilPengembalian(tanggalDari, tanggalSampai);
        }

        private void tampilRingkasan(string tanggalDari, string tanggalSampai)
        {
            dataGridView1.Rows.Clear();
            string query = $@"
                SELECT
                (SELECT COUNT(*) FROM t_peminjaman WHERE tanggal_pinjam >= '{tanggalDari}' AND tanggal_pinjam <= '{tanggalSampai}') AS total_pinjam,
                (SELECT COUNT(*) FROM t_pengembalian WHERE tanggal_kembali >= '{tanggalDari}' AND tanggal_kembali <= '{tanggalSampai}') AS total_kembali,
                (SELECT COUNT(*) FROM t_peminjaman WHERE status = 'dipinjam') AS masih_pinjam,
                (SELECT COUNT(*) FROM t_pengembalian WHERE terlambat > 0 AND tanggal_kembali >= '{tanggalDari}' AND tanggal_kembali <= '{tanggalSampai}') AS terlambat,
                (SELECT IFNULL(SUM(denda), 0) FROM t_pengembalian WHERE tanggal_kembali >= '{tanggalDari}' AND tanggal_kembali <= '{tanggalSampai}') AS total_denda
            ";

            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                DataRow data = db.ds.Tables[0].Rows[0];
                string totalPinjam = data["total_pinjam"].ToString() + " Transaksi";
                string totalKembali = data["total_kembali"].ToString() + " Transaksi";
                string masihPinjam = data["masih_pinjam"].ToString() + " Buku";
                string terlambat = data["terlambat"].ToString() + " Buku";

                decimal totalDenda = 0;
                if (data["total_denda"] != DBNull.Value)
                    totalDenda = Convert.ToDecimal(data["total_denda"]);

                string denda = "Rp " + totalDenda.ToString("N0");

                dataGridView1.Rows.Add(totalPinjam, totalKembali, masihPinjam, terlambat, denda);
            }
        }

        private void tampilPeminjaman(string tanggalDari, string tanggalSampai)
        {
            dataGridView2.Rows.Clear();
            string query = $@"
                SELECT p.id_pinjam, p.kode_pinjam, p.tanggal_pinjam, p.tanggal_jatuh_tempo, 
                       a.nama_anggota, b.judul, k.nama_kategori, p.status
                FROM t_peminjaman p
                INNER JOIN t_anggota a ON p.id_anggota = a.id_anggota
                INNER JOIN t_buku b ON p.id_buku = b.id_buku
                LEFT JOIN t_kategori k ON b.id_kategori = k.id_kategori
                WHERE p.tanggal_pinjam >= '{tanggalDari}' AND p.tanggal_pinjam <= '{tanggalSampai}'
                ORDER BY p.tanggal_pinjam ASC
            ";

            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;
                foreach (DataRow data in db.ds.Tables[0].Rows)
                {
                    string tanggal = Convert.ToDateTime(data["tanggal_pinjam"]).ToString("dd/MM/yyyy");
                    string anggota = data["nama_anggota"].ToString();
                    string buku = data["judul"].ToString();
                    string kategori = data["nama_kategori"] != DBNull.Value ? data["nama_kategori"].ToString() : "-";
                    string status = data["status"].ToString();

                    dataGridView2.Rows.Add(no, tanggal, anggota, buku, kategori, status);
                    no++;
                }
            }
        }

        private void tampilPengembalian(string tanggalDari, string tanggalSampai)
        {
            dataGridView3.Rows.Clear();
            string query = $@"
                SELECT p.id_pinjam, a.nama_anggota, b.judul, pg.tanggal_kembali, pg.terlambat, pg.denda,
                       CASE WHEN d.status_bayar IS NOT NULL THEN d.status_bayar ELSE 'belum' END AS status_bayar
                FROM t_pengembalian pg
                INNER JOIN t_peminjaman p ON pg.id_pinjam = p.id_pinjam
                INNER JOIN t_anggota a ON p.id_anggota = a.id_anggota
                INNER JOIN t_buku b ON p.id_buku = b.id_buku
                LEFT JOIN t_denda d ON pg.id_pinjam = d.id_pinjam
                WHERE pg.tanggal_kembali >= '{tanggalDari}' AND pg.tanggal_kembali <= '{tanggalSampai}'
                ORDER BY pg.tanggal_kembali ASC
            ";

            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;
                foreach (DataRow data in db.ds.Tables[0].Rows)
                {
                    string anggota = data["nama_anggota"].ToString();
                    string buku = data["judul"].ToString();
                    string tanggalKembali = data["tanggal_kembali"] != DBNull.Value ? Convert.ToDateTime(data["tanggal_kembali"]).ToString("dd/MM/yyyy") : "-";
                    string terlambat = data["terlambat"].ToString() + " Hari";

                    decimal denda = 0;
                    if (data["denda"] != DBNull.Value)
                        denda = Convert.ToDecimal(data["denda"]);

                    string dendaText = "Rp " + denda.ToString("N0");
                    string statusBayar = data["status_bayar"].ToString();

                    dataGridView3.Rows.Add(no, anggota, buku, tanggalKembali, terlambat, dendaText, statusBayar);
                    no++;
                }
            }
        }

        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }

        private void btncetak_Click(object sender, EventArgs e)
        {
            if (dtpdari.Value.Date > dtpsampai.Value.Date)
            {
                MessageBox.Show("Tanggal dari tidak boleh lebih besar dari tanggal sampai.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tampildata();

            currentTable = 0;
            currentRow = 0;
            nomorHalaman = 1;

            printDocument.DefaultPageSettings.Landscape = true;
            printPreview.Document = printDocument;
            printPreview.WindowState = FormWindowState.Maximized;
            printPreview.ShowDialog();
        }

        // =====================================================
        // PROSES CETAK GABUNGAN 3 TABEL
        // =====================================================
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float kiri = e.MarginBounds.Left;
            float atas = e.MarginBounds.Top;
            float bawah = e.MarginBounds.Bottom;
            float lebar = e.MarginBounds.Width;

            Font fontJudul = new Font("Arial", 16, FontStyle.Bold);
            Font fontBagian = new Font("Arial", 11, FontStyle.Bold);
            Font fontHeader = new Font("Arial", 9, FontStyle.Bold);
            Font fontNormal = new Font("Arial", 8.5f);
            Font fontKecil = new Font("Arial", 8);
            Pen garis = new Pen(Color.Black);

            // Cetak Judul Utama dan Periode (Hanya di Halaman Pertama)
            if (nomorHalaman == 1 && currentTable == 0 && currentRow == 0)
            {
                string judul = "LAPORAN AKTIVITAS PERPUSTAKAAN";
                SizeF ukuranJudul = g.MeasureString(judul, fontJudul);
                g.DrawString(judul, fontJudul, Brushes.Black, kiri + (lebar - ukuranJudul.Width) / 2, atas);
                atas += 25;

                string periode = "Periode : " + dtpdari.Value.ToString("dd/MM/yyyy") + " s/d " + dtpsampai.Value.ToString("dd/MM/yyyy");
                SizeF ukuranPeriode = g.MeasureString(periode, fontNormal);
                g.DrawString(periode, fontNormal, Brushes.Black, kiri + (lebar - ukuranPeriode.Width) / 2, atas);
                atas += 30;
            }

            // Loop untuk menggambar semua tabel secara kontinu
            while (currentTable < 3)
            {
                // -------------------------------------------------
                // TABEL 1: RINGKASAN AKTIVITAS
                // -------------------------------------------------
                if (currentTable == 0)
                {
                    if (atas + 80 > bawah) { KeluarKeHalamanBerikutnya(e, fontKecil); return; }

                    g.DrawString("1. RINGKASAN AKTIVITAS", fontBagian, Brushes.Black, kiri, atas);
                    atas += 20;

                    float[] lebar1 = { 180, 180, 160, 160, 200 };
                    string[] header1 = { "Total Peminjaman", "Total Pengembalian", "Masih Dipinjam", "Terlambat", "Total Denda" };

                    gambarHeader(g, kiri, ref atas, lebar1, header1, fontHeader, garis);

                    if (dataGridView1.Rows.Count > 0 && !dataGridView1.Rows[0].IsNewRow)
                    {
                        DataGridViewRow row = dataGridView1.Rows[0];
                        string[] data1 = {
                            row.Cells[0].Value?.ToString() ?? "",
                            row.Cells[1].Value?.ToString() ?? "",
                            row.Cells[2].Value?.ToString() ?? "",
                            row.Cells[3].Value?.ToString() ?? "",
                            row.Cells[4].Value?.ToString() ?? ""
                        };
                        gambarBaris(g, kiri, ref atas, lebar1, data1, fontNormal, garis);
                    }

                    atas += 25; // Jarak ke tabel berikutnya
                    currentTable = 1;
                    currentRow = 0;
                }

                // -------------------------------------------------
                // TABEL 2: DETAIL PEMINJAMAN
                // -------------------------------------------------
                if (currentTable == 1)
                {
                    float[] lebar2 = { 40, 90, 180, 280, 160, 130 };
                    string[] header2 = { "No", "Tanggal", "Anggota", "Buku", "Kategori", "Status" };

                    // Gambar Judul Sub-Tabel dan Header jika baru mulai tabel ini
                    if (currentRow == 0)
                    {
                        if (atas + 60 > bawah) { KeluarKeHalamanBerikutnya(e, fontKecil); return; }

                        g.DrawString("2. DETAIL AKTIVITAS PEMINJAMAN", fontBagian, Brushes.Black, kiri, atas);
                        atas += 20;
                        gambarHeader(g, kiri, ref atas, lebar2, header2, fontHeader, garis);
                    }

                    // Loop baris DataGridView2
                    while (currentRow < dataGridView2.Rows.Count)
                    {
                        if (dataGridView2.Rows[currentRow].IsNewRow) { currentRow++; continue; }
                        if (atas + 22 > bawah) { KeluarKeHalamanBerikutnya(e, fontKecil); return; }

                        DataGridViewRow row = dataGridView2.Rows[currentRow];
                        string[] data2 = {
                            row.Cells[0].Value?.ToString() ?? "",
                            row.Cells[1].Value?.ToString() ?? "",
                            row.Cells[2].Value?.ToString() ?? "",
                            row.Cells[3].Value?.ToString() ?? "",
                            row.Cells[4].Value?.ToString() ?? "",
                            row.Cells[5].Value?.ToString() ?? ""
                        };
                        gambarBaris(g, kiri, ref atas, lebar2, data2, fontNormal, garis);
                        currentRow++;
                    }

                    atas += 25; // Jarak ke tabel berikutnya
                    currentTable = 2;
                    currentRow = 0;
                }

                // -------------------------------------------------
                // TABEL 3: DETAIL PENGEMBALIAN & DENDA
                // -------------------------------------------------
                if (currentTable == 2)
                {
                    float[] lebar3 = { 40, 180, 260, 110, 100, 150, 140 };
                    string[] header3 = { "No", "Anggota", "Buku", "Tgl Kembali", "Terlambat", "Denda", "Status Bayar" };

                    if (currentRow == 0)
                    {
                        if (atas + 60 > bawah) { KeluarKeHalamanBerikutnya(e, fontKecil); return; }

                        g.DrawString("3. DETAIL PENGEMBALIAN DAN DENDA", fontBagian, Brushes.Black, kiri, atas);
                        atas += 20;
                        gambarHeader(g, kiri, ref atas, lebar3, header3, fontHeader, garis);
                    }

                    while (currentRow < dataGridView3.Rows.Count)
                    {
                        if (dataGridView3.Rows[currentRow].IsNewRow) { currentRow++; continue; }
                        if (atas + 22 > bawah) { KeluarKeHalamanBerikutnya(e, fontKecil); return; }

                        DataGridViewRow row = dataGridView3.Rows[currentRow];
                        string[] data3 = {
                            row.Cells[0].Value?.ToString() ?? "",
                            row.Cells[1].Value?.ToString() ?? "",
                            row.Cells[2].Value?.ToString() ?? "",
                            row.Cells[3].Value?.ToString() ?? "",
                            row.Cells[4].Value?.ToString() ?? "",
                            row.Cells[5].Value?.ToString() ?? "",
                            row.Cells[6].Value?.ToString() ?? ""
                        };
                        gambarBaris(g, kiri, ref atas, lebar3, data3, fontNormal, garis);
                        currentRow++;
                    }

                    currentTable = 3; // Selesai semua tabel
                }
            }

            // Footer halaman terakhir
            footer(g, e, fontKecil);
            e.HasMorePages = false;
        }

        // Helper untuk menangani perpindahan ke halaman baru secara dinamis
        private void KeluarKeHalamanBerikutnya(PrintPageEventArgs e, Font fontKecil)
        {
            footer(e.Graphics, e, fontKecil);
            nomorHalaman++;
            e.HasMorePages = true;
        }

        // =====================================================
        // HELPER DRAWING TABEL
        // =====================================================
        private void gambarHeader(Graphics g, float kiri, ref float atas, float[] lebarKolom, string[] header, Font font, Pen garis)
        {
            float posX = kiri;
            float tinggiHeader = 22;

            g.DrawRectangle(garis, kiri, atas, SumArray(lebarKolom), tinggiHeader);

            for (int i = 0; i < header.Length; i++)
            {
                g.DrawString(header[i], font, Brushes.Black, posX + 4, atas + 3);
                posX += lebarKolom[i];
                if (i < header.Length - 1)
                {
                    g.DrawLine(garis, posX, atas, posX, atas + tinggiHeader);
                }
            }
            atas += tinggiHeader;
        }

        private void gambarBaris(Graphics g, float kiri, ref float atas, float[] lebarKolom, string[] data, Font font, Pen garis)
        {
            float posX = kiri;
            float tinggiBaris = 20;

            g.DrawRectangle(garis, kiri, atas, SumArray(lebarKolom), tinggiBaris);

            for (int i = 0; i < data.Length; i++)
            {
                g.DrawString(data[i], font, Brushes.Black, posX + 4, atas + 3);
                posX += lebarKolom[i];
                if (i < data.Length - 1)
                {
                    g.DrawLine(garis, posX, atas, posX, atas + tinggiBaris);
                }
            }
            atas += tinggiBaris;
        }

        private float SumArray(float[] arr)
        {
            float total = 0;
            foreach (float val in arr) total += val;
            return total;
        }

        private void footer(Graphics g, PrintPageEventArgs e, Font font)
        {
            string strFooter = "Halaman " + nomorHalaman;
            SizeF sz = g.MeasureString(strFooter, font);
            g.DrawString(strFooter, font, Brushes.Black, e.MarginBounds.Right - sz.Width, e.MarginBounds.Bottom + 10);
        }
    }
}