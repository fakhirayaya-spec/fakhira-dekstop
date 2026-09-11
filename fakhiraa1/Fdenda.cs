using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fdenda : Form
    {
        private bool isUpdating = false;

        public Fdenda()
        {
            InitializeComponent();

          
            cmbkd.SelectedIndexChanged += cmbkd_SelectedIndexChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.DataError += DataGridView1_DataError;

            
            txtterlambat.ReadOnly = true;
            txtdenda.ReadOnly = true;

           
            cmbstatus.Items.Clear();
            cmbstatus.Items.Add("belum");
            cmbstatus.Items.Add("lunas");

           
            isiComboPinjam();
            tampildata();
            bersih();
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        
        public void bersih()
        {
            isUpdating = true; // Kunci event

            label2.Text = "";

            cmbkd.DataSource = null;
            isiComboPinjam();

            cmbstatus.SelectedIndex = -1;

            txtterlambat.Text = "";
            txtdenda.Text = "";

            cmbkd.Enabled = true;
            cmbstatus.Enabled = true;

            txtterlambat.ReadOnly = true;
            txtdenda.ReadOnly = true;

            isUpdating = false; 
        }

       
        public void isiComboPinjam()
        {
            string query = @"
                SELECT
                    p.id_pinjam,
                    p.kode_pinjam
                FROM t_peminjaman p
                INNER JOIN t_pengembalian k
                    ON p.id_pinjam = k.id_pinjam
                WHERE k.terlambat > 0
                AND k.denda > 0
                AND p.id_pinjam NOT IN
                (
                    SELECT id_pinjam
                    FROM t_denda
                )
                ORDER BY p.kode_pinjam ASC";

            db.crud(query);

            if (db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                cmbkd.DataSource = db.ds.Tables[0];
                cmbkd.DisplayMember = "kode_pinjam";
                cmbkd.ValueMember = "id_pinjam";
                cmbkd.SelectedIndex = -1;
            }
            else
            {
                cmbkd.DataSource = null;
            }
        }


        private void cmbkd_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Jangan jalankan jika sedang proses memilih data dari DataGridView untuk Update
            if (isUpdating) return;

            if (cmbkd.SelectedValue == null || cmbkd.SelectedIndex == -1) return;
            if (cmbkd.SelectedValue is DataRowView) return;

            string idpinjam = cmbkd.SelectedValue.ToString();

            db.crud(
                "SELECT terlambat, denda " +
                "FROM t_pengembalian " +
                "WHERE id_pinjam = '" + idpinjam + "'"
            );

            if (db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                DataRow data = db.ds.Tables[0].Rows[0];

                txtterlambat.Text = data["terlambat"].ToString();
                txtdenda.Text = data["denda"].ToString();
                if (string.IsNullOrEmpty(label2.Text))
                {
                    cmbstatus.SelectedItem = "belum";
                }
            }
            else
            {
                txtterlambat.Text = "";
                txtdenda.Text = "";
            }
        }

        public void tampildata()
        {
            dataGridView1.Rows.Clear();

            string query = @"
                SELECT
                    d.id_denda,
                    p.kode_pinjam,
                    d.terlambat,
                    d.jumlah_denda,
                    d.status_bayar
                FROM t_denda d
                INNER JOIN t_peminjaman p
                    ON d.id_pinjam = p.id_pinjam
                ORDER BY d.id_denda ASC";

            db.crud(query);

            if (db.ds.Tables.Count == 0 || db.ds.Tables[0].Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow data in db.ds.Tables[0].Rows)
            {
                string kodePinjam = data["kode_pinjam"].ToString();
                string terlambat = data["terlambat"].ToString();
                string jumlahDenda = data["jumlah_denda"].ToString();
                string statusBayar = data["status_bayar"].ToString();

                dataGridView1.Rows.Add(
                    kodePinjam,
                    terlambat,
                    jumlahDenda,
                    statusBayar
                );
            }
        }

      
        private void btnsimpan_Click(object sender, EventArgs e)
        {
            if (cmbkd.SelectedValue == null || cmbkd.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Kode Pinjam terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idpinjam = cmbkd.SelectedValue.ToString();

            db.crud("SELECT terlambat, denda FROM t_pengembalian WHERE id_pinjam = '" + idpinjam + "'");

            if (db.ds.Tables.Count == 0 || db.ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("Data pengembalian tidak ditemukan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow data = db.ds.Tables[0].Rows[0];

            string terlambat = data["terlambat"].ToString();
            string denda = data["denda"].ToString();

            string statusBayar = string.IsNullOrEmpty(cmbstatus.Text) ? "belum" : cmbstatus.Text.ToLower();

            db.crud(
                "INSERT INTO t_denda (id_pinjam, terlambat, jumlah_denda, status_bayar) VALUES (" +
                "'" + idpinjam + "', " +
                "'" + terlambat + "', " +
                "'" + denda + "', " +
                "'" + statusBayar + "')"
            );

            MessageBox.Show("Data denda berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            tampildata();
            bersih();
        }

        
        private void btnupdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(label2.Text))
            {
                MessageBox.Show("Pilih data denda yang mau diubah dari tabel terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cmbstatus.Text))
            {
                MessageBox.Show("Pilih status pembayaran (lunas / belum)!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idDenda = label2.Text;

            string statusBayar = cmbstatus.Text.Trim().ToLower();

        
            db.crud("UPDATE t_denda SET status_bayar = '" + statusBayar + "' WHERE id_denda = '" + idDenda + "'");

            MessageBox.Show("Data denda berhasil di-update menjadi: " + statusBayar, "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            tampildata();
            bersih();
        }

       
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            int baris = e.RowIndex;
            int kolom = e.ColumnIndex;

            if (dataGridView1.Rows[baris].Cells[0].Value == null) return;

            string kodePinjam = dataGridView1.Rows[baris].Cells[0].Value.ToString();

            db.crud(
                "SELECT d.id_denda, d.id_pinjam, d.terlambat, d.jumlah_denda, d.status_bayar " +
                "FROM t_denda d INNER JOIN t_peminjaman p ON d.id_pinjam = p.id_pinjam " +
                "WHERE p.kode_pinjam = '" + kodePinjam + "'"
            );

            if (db.ds.Tables.Count == 0 || db.ds.Tables[0].Rows.Count == 0) return;

            DataRow data = db.ds.Tables[0].Rows[0];

            string idDenda = data["id_denda"].ToString();
            string idPinjam = data["id_pinjam"].ToString();

            
            if (kolom == 4)
            {
                isUpdating = true; 

                label2.Text = idDenda;

              
                DataTable dt = new DataTable();
                dt.Columns.Add("id_pinjam");
                dt.Columns.Add("kode_pinjam");
                dt.Rows.Add(idPinjam, kodePinjam);

                cmbkd.DataSource = dt;
                cmbkd.DisplayMember = "kode_pinjam";
                cmbkd.ValueMember = "id_pinjam";
                cmbkd.SelectedValue = idPinjam;

            
                txtterlambat.Text = data["terlambat"].ToString();
                txtdenda.Text = data["jumlah_denda"].ToString();

              
                string status = data["status_bayar"].ToString().ToLower();
                cmbstatus.Text = status;

                
                cmbkd.Enabled = false;
                txtterlambat.ReadOnly = true;
                txtdenda.ReadOnly = true;
                cmbstatus.Enabled = true;

                isUpdating = false; 
                return;
            }

          
            if (kolom == 5)
            {
                DialogResult hasil = MessageBox.Show(
                    "Yakin ingin menghapus data denda?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (hasil != DialogResult.Yes) return;

                db.crud("DELETE FROM t_denda WHERE id_denda = '" + idDenda + "'");

                MessageBox.Show("Data denda berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                tampildata();
                bersih();
            }
        }

        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }
    }
}