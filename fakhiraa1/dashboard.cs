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
    public partial class dashboard : Form
    {
        public dashboard()
        {
            InitializeComponent();
        }



        private void dashboard_Load(object sender, EventArgs e)
        {
            panelDataMaster.Visible = false;
            panelTransaksi.Visible = false;
            PanelLaporan.Visible = false;
        }



        private void label17_Click(object sender, EventArgs e)
        {
            DialogResult setuju = MessageBox.Show("yakin mau keluar?", "pemberitahuan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (setuju == DialogResult.Yes)
            {
                Form1 YAYA = new Form1();
                YAYA.Visible = true;
                this.Hide();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            dashboardperpus fakhira = new dashboardperpus() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label3_Click_1(object sender, EventArgs e)
        {
            dashboardPengguna fakhira = new dashboardPengguna() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            masterrole fakhira = new masterrole() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void lblDataMaster_Click(object sender, EventArgs e)
        {
            panelDataMaster.Visible = !panelDataMaster.Visible;

        }


        private void label12_Click(object sender, EventArgs e)
        {
            panelTransaksi.Visible = !panelTransaksi.Visible;
        }

        private void label16_Click(object sender, EventArgs e)
        {
            PanelLaporan.Visible = !PanelLaporan.Visible;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            FBuku fakhira = new FBuku() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Fkategori fakhira = new Fkategori() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Fanggota fakhira = new Fanggota() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label15_Click(object sender, EventArgs e)
        {
            Fpeminjaman fakhira = new Fpeminjaman() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {
            Fpengembalian fakhira = new Fpengembalian() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Fdenda fakhira = new Fdenda() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
        }

        private void label19_Click(object sender, EventArgs e)
        {
            Flaporan fakhira = new Flaporan() { TopLevel = false, TopMost = true };
            KFPERPUS.untukform(fakhira, panel3);
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

        private void guna2PictureBox13_Click_1(object sender, EventArgs e)
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