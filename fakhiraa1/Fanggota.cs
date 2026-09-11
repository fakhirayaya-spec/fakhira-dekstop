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
    public partial class Fanggota : Form
    {
        public Fanggota()
        {
            InitializeComponent();
        }

        // Method untuk mengisi ComboBox Nama khusus user yang id_role = 2
        public void loadNamaAnggota()
        {
            db.crud("SELECT nama FROM tuser WHERE id_role = '2'");

            cmbnama.Items.Clear();
            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                cmbnama.Items.Add(baris["nama"].ToString());
            }
        }

        public void bersih()
        {
            txtid.Text = "2"; // Tetap dikunci ke angka 2
            cmbnama.SelectedIndex = -1;
            txtnis.Text = "";
            txtkelas.Text = "";
            txtalamat.Text = "";
            txtno.Text = "";
            txtemail.Text = "";
            cmbjk.SelectedIndex = -1;
            label10.Text = "";
        }

        public void tampildata()
        {
            dataGridView1.Rows.Clear();
            db.crud("SELECT * FROM `t_anggota`");
            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                string id = "" + baris["id_anggota"];
                string ids = "" + baris["id_role"];
                string nis = "" + baris["nis"];
                string nm = "" + baris["nama_anggota"];
                string jk = "" + baris["jenis_kelamin"];
                string kel = "" + baris["kelas"];
                string al = "" + baris["alamat"];
                string no = "" + baris["no_hp"];
                string em = "" + baris["email"];
                dataGridView1.Rows.Add(id, ids, nis, nm, jk, kel, al, no, em);
            }
        }

        // Jalankan otomatis saat Form pertama kali dibuka
        private void Fanggota_Load(object sender, EventArgs e)
        {
            txtid.Text = "2";
            txtid.ReadOnly = true; // Mengunci TextBox agar tidak bisa diedit

            loadNamaAnggota();
            tampildata();
        }

        // Event saat ComboBox Nama diklik / dibuka
        private void cmbnama_DropDown(object sender, EventArgs e)
        {
            loadNamaAnggota();
        }

        // Event saat ComboBox Jenis Kelamin diklik / dibuka
        private void cmbjk_DropDown(object sender, EventArgs e)
        {
            cmbjk.Items.Clear();
            cmbjk.Items.Add("pria");
            cmbjk.Items.Add("wanita");
        }

        private void btntampil_Click(object sender, EventArgs e)
        {
            tampildata();
        }

        private void btnsimpan_Click(object sender, EventArgs e)
        {
            string ids = "2"; // Selalu menyimpan nilai 2 ke database
            string nis = txtnis.Text;
            string nm = cmbnama.Text; // Mengambil nama yang dipilih di ComboBox
            string jk = cmbjk.Text;
            string kel = txtkelas.Text;
            string al = txtalamat.Text;
            string no = txtno.Text;
            string em = txtemail.Text;

            db.crud($"INSERT INTO t_anggota VALUES (null, '{ids}','{nis}','{nm}','{jk}','{kel}','{al}','{no}','{em}')");
            bersih();
            tampildata();
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            string id = label10.Text;
            string ids = "2"; // Selalu bernilai 2
            string nis = txtnis.Text;
            string nm = cmbnama.Text;
            string jk = cmbjk.Text;
            string kel = txtkelas.Text;
            string al = txtalamat.Text;
            string no = txtno.Text;
            string em = txtemail.Text;

            db.crud($"UPDATE t_anggota set id_role = '{ids}' , nis = '{nis}', nama_anggota = '{nm}' , jenis_kelamin = '{jk}' , kelas = '{kel}' , alamat = '{al}' , no_hp = '{no}' , email = '{em}' where id_anggota = '{id}'");
            bersih();
            tampildata();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int baris = e.RowIndex;
            int kolom = e.ColumnIndex;
            string idnya = dataGridView1.Rows[baris].Cells[0].Value.ToString();

            if (kolom == 9) // Edit
            {
                db.crud($"SELECT * FROM `t_anggota` where id_anggota = '{idnya}'");
                foreach (DataRow bariss in db.ds.Tables[0].Rows)
                {
                    string id = "" + bariss["id_anggota"];
                    string ids = "" + bariss["id_role"];
                    string nis = "" + bariss["nis"];
                    string nm = "" + bariss["nama_anggota"];
                    string jk = "" + bariss["jenis_kelamin"];
                    string kel = "" + bariss["kelas"];
                    string al = "" + bariss["alamat"];
                    string no = "" + bariss["no_hp"];
                    string em = "" + bariss["email"];

                    label10.Text = id;
                    txtid.Text = "2";
                    txtnis.Text = nis;
                    cmbnama.Text = nm;
                    cmbjk.Text = jk;
                    txtkelas.Text = kel;
                    txtalamat.Text = al;
                    txtno.Text = no;
                    txtemail.Text = em;
                }
            }
            if (kolom == 10) // Hapus
            {
                DialogResult setuju = MessageBox.Show("Apakah kamu ingin hapus data?", "Pemberitahuan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (setuju == DialogResult.Yes)
                {
                    db.crud($"DELETE FROM t_anggota WHERE id_anggota = '{idnya}'");
                    bersih();
                    tampildata();
                }
            }
        }

        private void Fanggota_Load_1(object sender, EventArgs e)
        {
            txtid.Text = "2";         
            txtid.ReadOnly = true;   

            loadNamaAnggota();       
            tampildata();
        }
    }
}