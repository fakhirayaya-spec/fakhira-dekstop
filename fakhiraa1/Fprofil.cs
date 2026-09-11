using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fprofil : Form
    {
      
        private string nisnSiswa = "12345";

        public Fprofil()
        {
            InitializeComponent();
        }

        private void Fprofil_Load(object sender, EventArgs e)
        {
            kunciDataDiri();
            tampilProfilSiswa();
        }

        
        private void kunciDataDiri()
        {
            txtnisn.Enabled = false;
            txtnama.Enabled = false;
            txtkelas.Enabled = false;
            txtjk.Enabled = false;
            txtnohp.Enabled = false;
            txtalamat.Enabled = false;
        }

       
        public void tampilProfilSiswa()
        {
            string query = $"SELECT * FROM t_anggota WHERE nisn = '{nisnSiswa}' OR id_anggota = '{nisnSiswa}'";
            db.crud(query);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
                DataRow baris = db.ds.Tables[0].Rows[0];

                txtnisn.Text = baris["nisn"].ToString();
                txtnama.Text = baris["nama"].ToString();

               
                if (baris.Table.Columns.Contains("kelas"))
                    txtkelas.Text = baris["kelas"].ToString();

                if (baris.Table.Columns.Contains("jk"))
                    txtjk.Text = baris["jk"].ToString();

                if (baris.Table.Columns.Contains("no_hp"))
                    txtnohp.Text = baris["no_hp"].ToString();

                if (baris.Table.Columns.Contains("alamat"))
                    txtalamat.Text = baris["alamat"].ToString();
            }
        }

        private void btnsimpan_Click(object sender, EventArgs e)
        {
            string passLama = txtpasslama.Text.Trim();
            string passBaru = txtpassbaru.Text.Trim();

            if (string.IsNullOrEmpty(passLama) || string.IsNullOrEmpty(passBaru))
            {
                MessageBox.Show("Password lama dan password baru wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
            string queryCek = $"SELECT * FROM t_anggota WHERE (nisn = '{nisnSiswa}' OR id_anggota = '{nisnSiswa}') AND password = '{passLama}'";
            db.crud(queryCek);

            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
            {
               
                string queryUpdate = $"UPDATE t_anggota SET password = '{passBaru}' WHERE nisn = '{nisnSiswa}' OR id_anggota = '{nisnSiswa}'";
                db.crud(queryUpdate);

                MessageBox.Show("Password berhasil diperbarui!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtpasslama.Clear();
                txtpassbaru.Clear();
            }
            else
            {
                MessageBox.Show("Password lama salah!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}