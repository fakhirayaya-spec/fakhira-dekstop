using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string pass = MD5Hash(TXTPASS.Text);

            // 1. Ambil data user dari tuser berdasarkan username & password
            db.crud($"SELECT * FROM tuser WHERE username = '{TXTUSER.Text}' AND password = '{pass}'");

            int cekjumlahbaris = db.ds.Tables[0].Rows.Count;

            if (cekjumlahbaris == 1)
            {
                int role = Convert.ToInt32(db.ds.Tables[0].Rows[0]["id_role"]);
                int idUser = Convert.ToInt32(db.ds.Tables[0].Rows[0]["id"]);

                if (role == 1)
                {
                    dashboard admin = new dashboard();
                    admin.Show();
                    this.Hide();
                }
                else if (role == 2)
                {
                    try
                    {
                        // 2. Cari id_anggota di tabel t_anggota berdasarkan id_user
                        db.crud($"SELECT id_anggota FROM t_anggota WHERE id_user = '{idUser}'");

                        if (db.ds != null && db.ds.Tables.Count > 0 && db.ds.Tables[0].Rows.Count > 0)
                        {
                            idAnggotaLogin = Convert.ToInt32(db.ds.Tables[0].Rows[0]["id_anggota"]);
                        }
                        else
                        {
                            idAnggotaLogin = idUser;
                        }
                    }
                    catch
                    {
                        // Fallback jika terjadi kendala struktur kolom
                        idAnggotaLogin = idUser;
                    }

                    dashboardsiswa user = new dashboardsiswa();
                    user.Show();
                    this.Hide();
                }
            }
            else
            {
                MessageBox.Show("Username / Password salah");
            }
        }
    }
}