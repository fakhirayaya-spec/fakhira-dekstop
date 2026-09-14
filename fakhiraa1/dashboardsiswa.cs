using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class dashboardsiswa : Form
    {
        private int idAnggotaLogin;

        public dashboardsiswa()
        {
            InitializeComponent();
        }

        // Constructor penerima ID siswa dari Form1
        public dashboardsiswa(int idAnggota)
        {
            InitializeComponent();
            this.idAnggotaLogin = idAnggota;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Fkatalogsis katalog = new Fkatalogsis();
            katalog.TopLevel = false;
            katalog.FormBorderStyle = FormBorderStyle.None;
            katalog.Dock = DockStyle.Fill;
            panel3.Controls.Clear();
            panel3.Controls.Add(katalog);
            katalog.Show();
            katalog.tampilKategori();
            katalog.tampildata();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Mengirimkan idAnggotaLogin ke Form Peminjaman
            Fpeminjamansiswa fakhira = new Fpeminjamansiswa() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Fprofil formProfil = new Fprofil();
            formProfil.TopLevel = false;
            formProfil.FormBorderStyle = FormBorderStyle.None;
            formProfil.Dock = DockStyle.Fill;

            panel3.Controls.Clear(); // Ganti panelKonten dengan nama Panel tempat Form dipasang
            panel3.Controls.Add(formProfil);

            formProfil.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox13_Click(object sender, EventArgs e)
        {
            // Tampilkan konfirmasi sebelum logout
            DialogResult result = MessageBox.Show(
                "Apakah Anda yakin ingin keluar?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // 1. Sembunyikan form saat ini (Form Main / Admin)
                this.Hide();

                // 2. Buka kembali Form Login
                Form1 formLogin = new Form1();
                formLogin.ShowDialog();

                // 3. Tutup form utama secara permanen setelah Form Login ditutup
                this.Close();
            }
        }
    }
}