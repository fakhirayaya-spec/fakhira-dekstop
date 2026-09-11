using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Flaporan : Form
    {
        public Flaporan()
        {
            InitializeComponent();
        }

        private void Flaporan_Load(object sender, EventArgs e)
        {
            // Set default periode ke bulan berjalan
            dtpdari.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpsampai.Value = DateTime.Now;

            tampildata();
        }

        public void tampildata()
        {
            string tglAwal = dtpdari.Value.ToString("yyyy-MM-dd");
            string tglAkhir = dtpsampai.Value.ToString("yyyy-MM-dd");

            tampilRingkasan(tglAwal, tglAkhir);
            tampilDetailAktivitas(tglAwal, tglAkhir);
            tampilDetailDenda(tglAwal, tglAkhir);
        }

        // 1. ISI DATAGRIDVIEW 1 (RINGKASAN AKTIVITAS)
        private void tampilRingkasan(string tglAwal, string tglAkhir)
        {
            dataGridView1.Rows.Clear();

            string query = $@"
                SELECT 
                    (SELECT COUNT(*) FROM t_peminjaman WHERE tanggal_pinjam BETWEEN '{tglAwal}' AND '{tglAkhir}') AS total_pinjam,
                    (SELECT COUNT(*) FROM t_pengembalian WHERE tanggal_kembali BETWEEN '{tglAwal}' AND '{tglAkhir}') AS total_kembali,
                    (SELECT COUNT(*) FROM t_peminjaman WHERE status = 'dipinjam') AS masih_pinjam,
                    (SELECT COUNT(*) FROM t_pengembalian WHERE terlambat > 0 AND tanggal_kembali BETWEEN '{tglAwal}' AND '{tglAkhir}') AS terlambat,
                    (SELECT IFNULL(SUM(jumlah_denda), 0) FROM t_denda WHERE id_pinjam IN 
                        (SELECT id_pinjam FROM t_pengembalian WHERE tanggal_kembali BETWEEN '{tglAwal}' AND '{tglAkhir}')
                    ) AS denda";

            db.crud(query);

            if (db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                DataRow baris = db.ds.Tables[0].Rows[0];

                string totalPinjam = baris["total_pinjam"].ToString() + " Transaksi";
                string totalKembali = baris["total_kembali"].ToString() + " Transaksi";
                string masihPinjam = baris["masih_pinjam"].ToString() + " Buku";
                string terlambat = baris["terlambat"].ToString() + " Buku";
                string denda = "Rp " + Convert.ToDecimal(baris["denda"]).ToString("N0");

                dataGridView1.Rows.Add(totalPinjam, totalKembali, masihPinjam, terlambat, denda);
            }
        }

        // 2. ISI DATAGRIDVIEW 2 (DETAIL AKTIVITAS PEMINJAMAN)
        private void tampilDetailAktivitas(string tglAwal, string tglAkhir)
        {
            dataGridView2.Rows.Clear();

            string query = $@"
                SELECT 
                    p.tanggal_pinjam,
                    a.nama_anggota,
                    b.judul,
                    k.nama_kategori,
                    p.status
                FROM t_peminjaman p
                INNER JOIN t_anggota a ON p.id_anggota = a.id_anggota
                INNER JOIN t_buku b ON p.id_buku = b.id_buku
                LEFT JOIN t_kategori k ON b.id_kategori = k.id_kategori
                WHERE p.tanggal_pinjam BETWEEN '{tglAwal}' AND '{tglAkhir}'
                ORDER BY p.tanggal_pinjam ASC";

            db.crud(query);

            if (db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;
                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string tgl = Convert.ToDateTime(baris["tanggal_pinjam"]).ToString("dd/MM/yyyy");
                    string anggota = baris["nama_anggota"].ToString();
                    string buku = baris["judul"].ToString();
                    string kategori = baris["nama_kategori"].ToString();
                    string status = baris["status"].ToString();

                    dataGridView2.Rows.Add(no++, tgl, anggota, buku, kategori, status);
                }
            }
        }

        // 3. ISI DATAGRIDVIEW 3 (DETAIL PENGEMBALIAN DAN DENDA)
        private void tampilDetailDenda(string tglAwal, string tglAkhir)
        {
            dataGridView3.Rows.Clear();

            string query = $@"
                SELECT 
                    a.nama_anggota,
                    b.judul,
                    k.tanggal_kembali,
                    k.terlambat,
                    IFNULL(d.jumlah_denda, 0) AS denda,
                    IFNULL(d.status_bayar, 'belum') AS status_bayar
                FROM t_pengembalian k
                INNER JOIN t_peminjaman p ON k.id_pinjam = p.id_pinjam
                INNER JOIN t_anggota a ON p.id_anggota = a.id_anggota
                INNER JOIN t_buku b ON p.id_buku = b.id_buku
                LEFT JOIN t_denda d ON k.id_pinjam = d.id_pinjam
                WHERE k.tanggal_kembali BETWEEN '{tglAwal}' AND '{tglAkhir}'
                ORDER BY k.tanggal_kembali ASC";

            db.crud(query);

            if (db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                int no = 1;
                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string anggota = baris["nama_anggota"].ToString();
                    string buku = baris["judul"].ToString();
                    string tglKembali = Convert.ToDateTime(baris["tanggal_kembali"]).ToString("dd/MM/yyyy");
                    string terlambat = baris["terlambat"].ToString() + " Hari";
                    string denda = "Rp " + Convert.ToDecimal(baris["denda"]).ToString("N0");
                    string statusBayar = baris["status_bayar"].ToString();

                    dataGridView3.Rows.Add(no++, anggota, buku, tglKembali, terlambat, denda, statusBayar);
                }
            }
        }

        // TOMBOL TAMPIL AKTIVITAS
        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }
    }
}