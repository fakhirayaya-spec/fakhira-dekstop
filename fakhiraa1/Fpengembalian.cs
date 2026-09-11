using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fpengembalian : Form
    {
        public Fpengembalian()
        {
            InitializeComponent();

            // =====================================================
            // EVENT
            // =====================================================
            cmbkd.SelectedIndexChanged += cmbkd_SelectedIndexChanged;
            dtpkembali.ValueChanged += dtpkembali_ValueChanged;

            // =====================================================
            // TERLAMBAT DAN DENDA TIDAK BISA DIINPUT MANUAL
            // =====================================================
            txtterlambat.ReadOnly = true;
            txtdenda.ReadOnly = true;

            isiComboPinjam();
            tampildata();
            bersih();
        }


        // =========================================================
        // BERSIH
        // =========================================================
        public void bersih()
        {
            label2.Text = "";

            cmbkd.SelectedIndex = -1;

            dtppinjam.Value = DateTime.Now;
            dtpkembali.Value = DateTime.Now;

            txtterlambat.Text = "";
            txtdenda.Text = "";
        }


        // =========================================================
        // ISI COMBO KODE PINJAM
        // HANYA PEMINJAMAN YANG MASIH DIPINJAM
        //
        // Yang ditampilkan : kode_pinjam
        // Yang disimpan     : id_pinjam
        // =========================================================
        public void isiComboPinjam(string idPinjamEdit = "")
        {
            string query = "";

            if (idPinjamEdit != "")
            {
                query =
                    "SELECT id_pinjam, kode_pinjam " +
                    "FROM t_peminjaman " +
                    "WHERE status = 'dipinjam' " +
                    $"OR id_pinjam = '{idPinjamEdit}' " +
                    "ORDER BY kode_pinjam ASC";
            }
            else
            {
                query =
                    "SELECT id_pinjam, kode_pinjam " +
                    "FROM t_peminjaman " +
                    "WHERE status = 'dipinjam' " +
                    "ORDER BY kode_pinjam ASC";
            }

            db.crud(query);

            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                cmbkd.DataSource = null;

                cmbkd.DataSource =
                    db.ds.Tables[0];

                // Yang terlihat di ComboBox
                cmbkd.DisplayMember =
                    "kode_pinjam";

                // Yang dipakai sebagai value
                cmbkd.ValueMember =
                    "id_pinjam";

                cmbkd.SelectedIndex = -1;
            }
            else
            {
                cmbkd.DataSource = null;
            }
        }


        // =========================================================
        // KETIKA KODE PINJAM DIPILIH
        // =========================================================
        private void cmbkd_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (cmbkd.SelectedValue == null)
                return;

            if (cmbkd.SelectedIndex == -1)
                return;

            if (cmbkd.SelectedValue is DataRowView)
                return;

            string idpinjam =
                cmbkd.SelectedValue.ToString();


            // =====================================================
            // AMBIL TANGGAL PINJAM DAN JATUH TEMPO
            // =====================================================
            db.crud(
                $"SELECT tanggal_pinjam, " +
                $"tanggal_jatuh_tempo " +
                $"FROM t_peminjaman " +
                $"WHERE id_pinjam = '{idpinjam}'"
            );


            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                DataRow baris =
                    db.ds.Tables[0].Rows[0];


                // =================================================
                // TANGGAL PINJAM
                // =================================================
                if (
                    baris["tanggal_pinjam"] !=
                    DBNull.Value
                )
                {
                    dtppinjam.Value =
                        Convert.ToDateTime(
                            baris["tanggal_pinjam"]
                        );
                }


                // =================================================
                // TANGGAL KEMBALI DEFAULT HARI INI
                // =================================================
                dtpkembali.Value =
                    DateTime.Today;


                // =================================================
                // HITUNG OTOMATIS
                // =================================================
                hitungTerlambatDanDenda();
            }
        }


        // =========================================================
        // KETIKA TANGGAL KEMBALI DIUBAH
        // =========================================================
        private void dtpkembali_ValueChanged(
            object sender,
            EventArgs e)
        {
            hitungTerlambatDanDenda();
        }


        // =========================================================
        // HITUNG TERLAMBAT DAN DENDA OTOMATIS
        // =========================================================
        private void hitungTerlambatDanDenda()
        {
            if (
                cmbkd.SelectedValue == null ||
                cmbkd.SelectedIndex == -1
            )
            {
                txtterlambat.Text = "";
                txtdenda.Text = "";
                return;
            }


            if (cmbkd.SelectedValue is DataRowView)
            {
                txtterlambat.Text = "";
                txtdenda.Text = "";
                return;
            }


            string idpinjam =
                cmbkd.SelectedValue.ToString();


            // =====================================================
            // AMBIL TANGGAL JATUH TEMPO
            // =====================================================
            db.crud(
                $"SELECT tanggal_jatuh_tempo " +
                $"FROM t_peminjaman " +
                $"WHERE id_pinjam = '{idpinjam}'"
            );


            if (
                db.ds.Tables.Count == 0 ||
                db.ds.Tables[0].Rows.Count == 0
            )
            {
                txtterlambat.Text = "0";
                txtdenda.Text = "0";
                return;
            }


            DataRow baris =
                db.ds.Tables[0].Rows[0];


            if (
                baris["tanggal_jatuh_tempo"] ==
                DBNull.Value
            )
            {
                txtterlambat.Text = "0";
                txtdenda.Text = "0";
                return;
            }


            // =====================================================
            // TANGGAL JATUH TEMPO
            // =====================================================
            DateTime tanggalJatuhTempo =
                Convert.ToDateTime(
                    baris["tanggal_jatuh_tempo"]
                ).Date;


            // =====================================================
            // TANGGAL KEMBALI
            // =====================================================
            DateTime tanggalKembali =
                dtpkembali.Value.Date;


            // =====================================================
            // HITUNG JUMLAH HARI TERLAMBAT
            // =====================================================
            int jumlahHariTerlambat =
                (
                    tanggalKembali -
                    tanggalJatuhTempo
                ).Days;


            // =====================================================
            // TIDAK TERLAMBAT
            // =====================================================
            if (jumlahHariTerlambat <= 0)
            {
                txtterlambat.Text = "0";
                txtdenda.Text = "0";
            }
            else
            {
                // =================================================
                // TERLAMBAT
                // =================================================
                txtterlambat.Text =
                    jumlahHariTerlambat.ToString();


                // =================================================
                // DENDA Rp2.000 / HARI
                // =================================================
                int denda =
                    jumlahHariTerlambat * 2000;


                txtdenda.Text =
                    denda.ToString();
            }
        }


        // =========================================================
        // TENTUKAN STATUS OTOMATIS
        // =========================================================
        private string tentukanStatus()
        {
            int jumlahTerlambat = 0;
            int jumlahDenda = 0;


            int.TryParse(
                txtterlambat.Text,
                out jumlahTerlambat
            );


            int.TryParse(
                txtdenda.Text,
                out jumlahDenda
            );


            // =====================================================
            // TIDAK ADA DENDA
            // =====================================================
            if (
                jumlahTerlambat == 0 &&
                jumlahDenda == 0
            )
            {
                return "dikembalikan";
            }


            // =====================================================
            // ADA DENDA
            // =====================================================
            return "terlambat";
        }


        // =========================================================
        // TAMPIL DATA
        // =========================================================
        public void tampildata()
        {
            dataGridView1.Rows.Clear();


            string query = @"
                SELECT 
                    p.id_kembali,
                    pm.kode_pinjam,
                    p.tanggal_kembali,
                    p.terlambat,
                    p.denda,
                    p.status
                FROM t_pengembalian p
                JOIN t_peminjaman pm
                ON p.id_pinjam = pm.id_pinjam
                ORDER BY p.id_kembali DESC";


            db.crud(query);


            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                foreach (DataRow baris in db.ds.Tables[0].Rows)
                {
                    string kodepinjam =
                        baris["kode_pinjam"].ToString();


                    string tglkembali = "";


                    if (
                        baris["tanggal_kembali"] !=
                        DBNull.Value
                    )
                    {
                        tglkembali =
                            Convert.ToDateTime(
                                baris["tanggal_kembali"]
                            ).ToString(
                                "dd/MM/yyyy"
                            );
                    }


                    string terlambat =
                        baris["terlambat"].ToString();


                    string denda =
                        baris["denda"].ToString();


                    string status =
                        baris["status"].ToString();


                    // =================================================
                    // SESUAIKAN DENGAN KOLOM GRID
                    // =================================================
                    dataGridView1.Rows.Add(
                        kodepinjam,
                        tglkembali,
                        terlambat,
                        denda,
                        status
                    );
                }
            }
        }


        // =========================================================
        // SIMPAN
        // =========================================================
        private void btnsimpan_Click(
            object sender,
            EventArgs e)
        {
            // =====================================================
            // CEK KODE PINJAM
            // =====================================================
            if (
                cmbkd.SelectedValue == null ||
                cmbkd.SelectedIndex == -1
            )
            {
                MessageBox.Show(
                    "Pilih Kode Pinjam terlebih dahulu!"
                );
                return;
            }


            // =====================================================
            // ID PINJAM DIAMBIL OTOMATIS DARI KODE PINJAM
            // =====================================================
            string idpinjam =
                cmbkd.SelectedValue.ToString();


            // =====================================================
            // HITUNG ULANG OTOMATIS
            // =====================================================
            hitungTerlambatDanDenda();


            int jumlahTerlambat = 0;
            int jumlahDenda = 0;


            int.TryParse(
                txtterlambat.Text,
                out jumlahTerlambat
            );


            int.TryParse(
                txtdenda.Text,
                out jumlahDenda
            );


            string terlambat =
                jumlahTerlambat.ToString();


            string denda =
                jumlahDenda.ToString();


            // =====================================================
            // STATUS OTOMATIS
            // =====================================================
            string status =
                tentukanStatus();


            string tglkembali =
                dtpkembali.Value.ToString(
                    "yyyy-MM-dd"
                );


            // =====================================================
            // CEK SUDAH PERNAH DIKEMBALIKAN ATAU BELUM
            // =====================================================
            db.crud(
                $"SELECT id_kembali " +
                $"FROM t_pengembalian " +
                $"WHERE id_pinjam = '{idpinjam}'"
            );


            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                MessageBox.Show(
                    "Kode pinjam tersebut sudah dikembalikan!"
                );
                return;
            }


            // =====================================================
            // SIMPAN DATA PENGEMBALIAN
            // =====================================================
            db.crud(
                $"INSERT INTO t_pengembalian " +
                $"(id_pinjam, tanggal_kembali, terlambat, denda, status) " +
                $"VALUES " +
                $"('{idpinjam}', '{tglkembali}', " +
                $"'{terlambat}', '{denda}', '{status}')"
            );


            // =====================================================
            // AMBIL ID BUKU
            // =====================================================
            string idbuku = "";


            db.crud(
                $"SELECT id_buku " +
                $"FROM t_peminjaman " +
                $"WHERE id_pinjam = '{idpinjam}'"
            );


            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                idbuku =
                    db.ds.Tables[0]
                    .Rows[0]["id_buku"]
                    .ToString();
            }


            // =====================================================
            // UPDATE STATUS PEMINJAMAN
            // =====================================================
            if (status == "dikembalikan")
            {
                db.crud(
                    $"UPDATE t_peminjaman " +
                    $"SET status = 'selesai' " +
                    $"WHERE id_pinjam = '{idpinjam}'"
                );
            }
            else
            {
                db.crud(
                    $"UPDATE t_peminjaman " +
                    $"SET status = 'terlambat' " +
                    $"WHERE id_pinjam = '{idpinjam}'"
                );
            }


            // =====================================================
            // STOK BUKU +1
            // =====================================================
            if (idbuku != "")
            {
                db.crud(
                    $"UPDATE t_buku " +
                    $"SET stok = stok + 1 " +
                    $"WHERE id_buku = '{idbuku}'"
                );
            }


            MessageBox.Show(
                "Data pengembalian berhasil disimpan!"
            );


            // =====================================================
            // REFRESH
            // =====================================================
            isiComboPinjam();
            tampildata();
            bersih();
        }


        // =========================================================
        // UPDATE
        // =========================================================
        private void btnupdate_Click(
            object sender,
            EventArgs e)
        {
            if (label2.Text == "")
            {
                MessageBox.Show(
                    "Pilih data yang ingin diupdate terlebih dahulu!"
                );
                return;
            }


            if (
                cmbkd.SelectedValue == null ||
                cmbkd.SelectedIndex == -1
            )
            {
                MessageBox.Show(
                    "Pilih Kode Pinjam terlebih dahulu!"
                );
                return;
            }


            string idkembali =
                label2.Text;


            string idpinjam =
                cmbkd.SelectedValue.ToString();


            // =====================================================
            // HITUNG ULANG OTOMATIS
            // =====================================================
            hitungTerlambatDanDenda();


            int jumlahTerlambat = 0;
            int jumlahDenda = 0;


            int.TryParse(
                txtterlambat.Text,
                out jumlahTerlambat
            );


            int.TryParse(
                txtdenda.Text,
                out jumlahDenda
            );


            string terlambat =
                jumlahTerlambat.ToString();


            string denda =
                jumlahDenda.ToString();


            // =====================================================
            // STATUS OTOMATIS
            // =====================================================
            string status =
                tentukanStatus();


            string tglkembali =
                dtpkembali.Value.ToString(
                    "yyyy-MM-dd"
                );


            // =====================================================
            // UPDATE PENGEMBALIAN
            // =====================================================
            db.crud(
                $"UPDATE t_pengembalian SET " +
                $"id_pinjam = '{idpinjam}', " +
                $"tanggal_kembali = '{tglkembali}', " +
                $"terlambat = '{terlambat}', " +
                $"denda = '{denda}', " +
                $"status = '{status}' " +
                $"WHERE id_kembali = '{idkembali}'"
            );


            // =====================================================
            // UPDATE STATUS PEMINJAMAN
            // =====================================================
            if (status == "dikembalikan")
            {
                db.crud(
                    $"UPDATE t_peminjaman " +
                    $"SET status = 'selesai' " +
                    $"WHERE id_pinjam = '{idpinjam}'"
                );
            }
            else
            {
                db.crud(
                    $"UPDATE t_peminjaman " +
                    $"SET status = 'terlambat' " +
                    $"WHERE id_pinjam = '{idpinjam}'"
                );
            }


            // =====================================================
            // REFRESH
            // =====================================================
            isiComboPinjam();
            tampildata();
            bersih();


            MessageBox.Show(
                "Data pengembalian berhasil diupdate!"
            );
        }


        // =========================================================
        // CLICK DATA GRID VIEW
        // =========================================================
        private void dataGridView1_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            int baris =
                e.RowIndex;


            int kolom =
                e.ColumnIndex;


            if (
                dataGridView1.Rows[baris]
                .Cells[0]
                .Value == null
            )
            {
                return;
            }


            // =====================================================
            // AMBIL KODE PINJAM DARI GRID
            // =====================================================
            string kodepinjam =
                dataGridView1.Rows[baris]
                .Cells[0]
                .Value
                .ToString();


            string tglkembali =
                dataGridView1.Rows[baris]
                .Cells[1]
                .Value
                .ToString();


            // =====================================================
            // AMBIL DATA BERDASARKAN KODE PINJAM
            // =====================================================
            db.crud(
                $"SELECT p.id_kembali, " +
                $"p.id_pinjam, " +
                $"pm.kode_pinjam, " +
                $"pm.tanggal_pinjam, " +
                $"pm.tanggal_jatuh_tempo " +
                $"FROM t_pengembalian p " +
                $"JOIN t_peminjaman pm " +
                $"ON p.id_pinjam = pm.id_pinjam " +
                $"WHERE pm.kode_pinjam = '{kodepinjam}'"
            );


            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                DataRow data =
                    db.ds.Tables[0].Rows[0];


                // =================================================
                // ID KEMBALI
                // Tetap disimpan di label untuk proses update
                // =================================================
                label2.Text =
                    data["id_kembali"].ToString();


                string idPinjamEdit =
                    data["id_pinjam"].ToString();


                // =================================================
                // MASUKKAN KODE PINJAM KE COMBO
                // =================================================
                isiComboPinjam(
                    idPinjamEdit
                );


                cmbkd.SelectedValue =
                    idPinjamEdit;


                // =================================================
                // TANGGAL PINJAM
                // =================================================
                if (
                    data["tanggal_pinjam"] !=
                    DBNull.Value
                )
                {
                    dtppinjam.Value =
                        Convert.ToDateTime(
                            data["tanggal_pinjam"]
                        );
                }
            }


            // =====================================================
            // TANGGAL KEMBALI
            // =====================================================
            DateTime tanggal;


            if (
                DateTime.TryParse(
                    tglkembali,
                    out tanggal
                )
            )
            {
                dtpkembali.Value =
                    tanggal;
            }


            // =====================================================
            // HITUNG TERLAMBAT DAN DENDA LAGI
            // =====================================================
            hitungTerlambatDanDenda();


            // =====================================================
            // HAPUS
            // KOLOM 6
            // =====================================================
            if (kolom == 6)
            {
                if (label2.Text == "")
                {
                    MessageBox.Show(
                        "Data tidak ditemukan!"
                    );
                    return;
                }


                DialogResult hasil =
                    MessageBox.Show(
                        "Yakin ingin menghapus data?",
                        "Konfirmasi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );


                if (
                    hasil ==
                    DialogResult.Yes
                )
                {
                    string idkembali =
                        label2.Text;


                    string idpinjam =
                        "";


                    // =================================================
                    // AMBIL ID PINJAM
                    // =================================================
                    db.crud(
                        $"SELECT id_pinjam " +
                        $"FROM t_pengembalian " +
                        $"WHERE id_kembali = '{idkembali}'"
                    );


                    if (
                        db.ds.Tables.Count > 0 &&
                        db.ds.Tables[0].Rows.Count > 0
                    )
                    {
                        idpinjam =
                            db.ds.Tables[0]
                            .Rows[0]["id_pinjam"]
                            .ToString();
                    }


                    // =================================================
                    // AMBIL ID BUKU
                    // =================================================
                    string idbuku =
                        "";


                    if (idpinjam != "")
                    {
                        db.crud(
                            $"SELECT id_buku " +
                            $"FROM t_peminjaman " +
                            $"WHERE id_pinjam = '{idpinjam}'"
                        );


                        if (
                            db.ds.Tables.Count > 0 &&
                            db.ds.Tables[0].Rows.Count > 0
                        )
                        {
                            idbuku =
                                db.ds.Tables[0]
                                .Rows[0]["id_buku"]
                                .ToString();
                        }
                    }


                    // =================================================
                    // HAPUS PENGEMBALIAN
                    // =================================================
                    db.crud(
                        $"DELETE FROM t_pengembalian " +
                        $"WHERE id_kembali = '{idkembali}'"
                    );


                    // =================================================
                    // STATUS PEMINJAMAN KEMBALI DIPINJAM
                    // =================================================
                    if (idpinjam != "")
                    {
                        db.crud(
                            $"UPDATE t_peminjaman " +
                            $"SET status = 'dipinjam' " +
                            $"WHERE id_pinjam = '{idpinjam}'"
                        );
                    }


                    // =================================================
                    // STOK DIKURANGI LAGI
                    // =================================================
                    if (idbuku != "")
                    {
                        db.crud(
                            $"UPDATE t_buku " +
                            $"SET stok = stok - 1 " +
                            $"WHERE id_buku = '{idbuku}'"
                        );
                    }


                    MessageBox.Show(
                        "Data berhasil dihapus!"
                    );


                    // =================================================
                    // REFRESH
                    // =================================================
                    isiComboPinjam();

                    tampildata();

                    bersih();
                }
            }
        }


        // =========================================================
        // BUTTON TAMPIL
        // =========================================================
        private void btntampil_Click(
            object sender,
            EventArgs e)
        {
            tampildata();
        }
    }
}
