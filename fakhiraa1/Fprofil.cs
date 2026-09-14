using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fprofil : Form
    {
        private int idUserLogin = 0;

        public Fprofil()
        {
            InitializeComponent();
        }

        private void Fprofil_Load(object sender, EventArgs e)
        {
            AturPropertiTextBox();
            TampilDataProfil();
        }

        private void AturPropertiTextBox()
        {
            // Data Diri: Tetap bisa dibaca (Enabled true), tapi nggak bisa diketik (ReadOnly true)
            if (txtnisn != null) { txtnisn.Enabled = true; txtnisn.ReadOnly = true; }
            if (txtnama != null) { txtnama.Enabled = true; txtnama.ReadOnly = true; }
            if (txtjk != null) { txtjk.Enabled = true; txtjk.ReadOnly = true; }
            if (txtnohp != null) { txtnohp.Enabled = true; txtnohp.ReadOnly = true; }
            if (txtalamat != null) { txtalamat.Enabled = true; txtalamat.ReadOnly = true; }
            if (txtkelas != null) { txtkelas.Enabled = true; txtkelas.ReadOnly = true; }

            // Ubah Password: BISA DIKETIK
            if (txtpasslama != null) { txtpasslama.Enabled = true; txtpasslama.ReadOnly = false; txtpasslama.UseSystemPasswordChar = true; }
            if (txtpassbaru != null) { txtpassbaru.Enabled = true; txtpassbaru.ReadOnly = false; txtpassbaru.UseSystemPasswordChar = true; }
            if (txtkonfir != null) { txtkonfir.Enabled = true; txtkonfir.ReadOnly = false; txtkonfir.UseSystemPasswordChar = true; }
        }

        private void TampilDataProfil()
        {
            // Mengambil ID Login dari Form1, kalau 0 otomatis ambil data pertama di database untuk tes
            int idLogin = Form1.idAnggotaLogin;

            string query = "";

            if (idLogin > 0)
            {
                query = $@"SELECT a.nis, a.nama_anggota, a.jenis_kelamin, a.no_hp, a.alamat, a.kelas, a.id_user 
                           FROM t_anggota a 
                           WHERE a.id_anggota = '{idLogin}' OR a.id_user = '{idLogin}'";
            }
            else
            {
                // Jika ID login belum tersimpan, ambil baris pertama dari t_anggota supaya tetap tampil
                query = @"SELECT a.nis, a.nama_anggota, a.jenis_kelamin, a.no_hp, a.alamat, a.kelas, a.id_user 
                           FROM t_anggota a LIMIT 1";
            }

            try
            {
                db.crud(query);

                if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                {
                    DataRow baris = db.ds.Tables[0].Rows[0];

                    if (txtnisn != null) txtnisn.Text = baris["nis"].ToString();
                    if (txtnama != null) txtnama.Text = baris["nama_anggota"].ToString();
                    if (txtjk != null) txtjk.Text = baris["jenis_kelamin"].ToString();
                    if (txtnohp != null) txtnohp.Text = baris["no_hp"].ToString();
                    if (txtalamat != null) txtalamat.Text = baris["alamat"].ToString();
                    if (txtkelas != null) txtkelas.Text = baris["kelas"].ToString();

                    if (baris["id_user"] != DBNull.Value)
                    {
                        idUserLogin = Convert.ToInt32(baris["id_user"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data profil: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessSimpanPassword()
        {
            string passLama = txtpasslama.Text.Trim();
            string passBaru = txtpassbaru.Text.Trim();
            string konfirPass = txtkonfir.Text.Trim();

            if (string.IsNullOrEmpty(passLama) || string.IsNullOrEmpty(passBaru) || string.IsNullOrEmpty(konfirPass))
            {
                MessageBox.Show("Semua kolom password wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (passBaru != konfirPass)
            {
                MessageBox.Show("Password baru dan konfirmasi password tidak cocok!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtkonfir.Focus();
                return;
            }

            try
            {
                string queryCek = $"SELECT password FROM tuser WHERE id = '{idUserLogin}'";
                db.crud(queryCek);

                if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                {
                    string passDiDb = db.ds.Tables[0].Rows[0]["password"].ToString();

                    if (passLama != passDiDb)
                    {
                        MessageBox.Show("Password lama Anda salah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtpasslama.Focus();
                        return;
                    }

                    string queryUpdate = $"UPDATE tuser SET password = '{passBaru}' WHERE id = '{idUserLogin}'";
                    db.crud(queryUpdate);

                    MessageBox.Show("Password berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtpasslama.Clear();
                    txtpassbaru.Clear();
                    txtkonfir.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memperbarui password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e) { ProcessSimpanPassword(); }
        private void button1_Click(object sender, EventArgs e) { ProcessSimpanPassword(); }
        private void btnSimpan_Click_1(object sender, EventArgs e) { ProcessSimpanPassword(); }
        private void btnsimpan_Click_2(object sender, EventArgs e) { ProcessSimpanPassword(); }
    }
}