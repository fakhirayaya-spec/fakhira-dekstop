using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Form1 : Form
    {
        // Variabel static untuk menyimpan ID Anggota yang sedang login
        public static int idAnggotaLogin = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public static string MD5Hash(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e) { }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TXTUSER.Text) || string.IsNullOrWhiteSpace(TXTPASS.Text))
            {
                MessageBox.Show("Username dan Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pass = MD5Hash(TXTPASS.Text);

            try
            {
                // 1. Ambil data user dari tuser berdasarkan username & password
                db.crud($"SELECT * FROM tuser WHERE username = '{TXTUSER.Text.Trim()}' AND password = '{pass}'");

                if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                {
                    // Simpan data user ke variabel lokal dulu agar tidak hilang tertimpa
                    DataRow userRow = db.ds.Tables[0].Rows[0];
                    int role = Convert.ToInt32(userRow["id_role"]);
                    int idUser = Convert.ToInt32(userRow["id"]);
                    string namaUser = userRow["nama"].ToString();

                    if (role == 1) // ADMIN
                    {
                        dashboard admin = new dashboard();
                        admin.Show();
                        this.Hide();
                    }
                    else if (role == 2) // SISWA
                    {
                        // 2. Cari id_anggota di t_anggota
                        try
                        {
                            // Cek berdasarkan id_user (jika kolom id_user sudah dibuat di DB)
                            db.crud($"SELECT id_anggota FROM t_anggota WHERE id_user = '{idUser}'");

                            if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                            {
                                idAnggotaLogin = Convert.ToInt32(db.ds.Tables[0].Rows[0]["id_anggota"]);
                            }
                            else
                            {
                                // Alternative: Mencari berdasarkan kecocokan nama jika id_user belum di-update
                                db.crud($"SELECT id_anggota FROM t_anggota WHERE nama_anggota LIKE '%{namaUser}%'");

                                if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                                {
                                    idAnggotaLogin = Convert.ToInt32(db.ds.Tables[0].Rows[0]["id_anggota"]);
                                }
                                else
                                {
                                    idAnggotaLogin = idUser;
                                }
                            }
                        }
                        catch
                        {
                            idAnggotaLogin = idUser;
                        }

                        dashboardsiswa user = new dashboardsiswa();
                        user.Show();
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Username atau Password salah!", "Gagal Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi Kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}