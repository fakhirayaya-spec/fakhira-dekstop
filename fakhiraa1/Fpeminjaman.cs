using System;
using System.Data;
using System.Windows.Forms;

namespace fakhiraa1
{
    public partial class Fpeminjaman : Form
    {
        public Fpeminjaman()
        {
            InitializeComponent();

            txtkodpin.ReadOnly = true;

            dtppinjam.MinDate = DateTime.Today;
            dtppinjam.MaxDate = DateTime.Today;
            dtppinjam.Value = DateTime.Today;
        }


        public string generateKodePinjam()
        {
            int nomorBerikutnya = 1;

            db.crud(
                "SELECT kode_pinjam " +
                "FROM t_peminjaman " +
                "ORDER BY id_pinjam DESC " +
                "LIMIT 1"
            );

            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                string kodeTerakhir =
                    db.ds.Tables[0]
                    .Rows[0]["kode_pinjam"]
                    .ToString();

                if (kodeTerakhir.StartsWith("P"))
                {
                    string angka =
                        kodeTerakhir.Substring(1);

                    int nomorLama;

                    if (
                        int.TryParse(
                            angka,
                            out nomorLama
                        )
                    )
                    {
                        nomorBerikutnya =
                            nomorLama + 1;
                    }
                }
            }

            return "P" + nomorBerikutnya.ToString("D3");
        }


       
        public void bersih()
        {
            
            txtkodpin.Text =
                generateKodePinjam();

           
            dtppinjam.MinDate =
                DateTime.Today;

            dtppinjam.MaxDate =
                DateTime.Today;

            dtppinjam.Value =
                DateTime.Today;

           
            dtptempo.Value =
                DateTime.Today.AddDays(7);

            cmbbuku.SelectedIndex = -1;
            cmbanggota.SelectedIndex = -1;

            label2.Text = "";
        }


       
        public void tampildata()
        {
            dataGridView1.Rows.Clear();

            db.crud(
                "SELECT * FROM t_peminjaman"
            );

            if (
                db.ds.Tables.Count == 0
            )
            {
                return;
            }

            foreach (DataRow baris in db.ds.Tables[0].Rows)
            {
                string kd =
                    baris["kode_pinjam"].ToString();

                string idang =
                    baris["id_anggota"].ToString();

                string idbuk =
                    baris["id_buku"].ToString();

                string tglpen = "";

                if (
                    baris["tanggal_pinjam"] != DBNull.Value
                )
                {
                    tglpen =
                        Convert.ToDateTime(
                            baris["tanggal_pinjam"]
                        ).ToString("dd/MM/yyyy");
                }

                string tgltem = "";

                if (
                    baris["tanggal_jatuh_tempo"] != DBNull.Value
                )
                {
                    tgltem =
                        Convert.ToDateTime(
                            baris["tanggal_jatuh_tempo"]
                        ).ToString("dd/MM/yyyy");
                }

                string ss =
                    baris["status"].ToString();

                dataGridView1.Rows.Add(
                    kd,
                    idang,
                    idbuk,
                    tglpen,
                    tgltem,
                    ss
                );
            }
        }


        
        private void btnsimpan_Click(
            object sender,
            EventArgs e)
        {
           
             
            if (
                cmbanggota.SelectedValue == null ||
                cmbanggota.SelectedIndex == -1
            )
            {
                MessageBox.Show(
                    "Pilih anggota terlebih dahulu!"
                );
                return;
            }


           
            if (
                cmbbuku.SelectedValue == null ||
                cmbbuku.SelectedIndex == -1
            )
            {
                MessageBox.Show(
                    "Pilih buku terlebih dahulu!"
                );
                return;
            }



            string kd =
                generateKodePinjam();


            string idang =
                cmbanggota.SelectedValue.ToString();


            string idbuk =
                cmbbuku.SelectedValue.ToString();


           
            string tglpin =
                DateTime.Today.ToString(
                    "yyyy-MM-dd"
                );


            string tgltempo =
                dtptempo.Value.ToString(
                    "yyyy-MM-dd"
                );


            string ss =
                "dipinjam";


      
            db.crud(
                $"INSERT INTO t_peminjaman " +
                $"(kode_pinjam, id_anggota, id_buku, " +
                $"tanggal_pinjam, tanggal_jatuh_tempo, status) " +
                $"VALUES " +
                $"('{kd}', '{idang}', '{idbuk}', " +
                $"'{tglpin}', '{tgltempo}', '{ss}')"
            );


           
            db.crud(
                $"UPDATE t_buku " +
                $"SET stok = stok - 1 " +
                $"WHERE id_buku = '{idbuk}'"
            );


            MessageBox.Show(
                "Data peminjaman berhasil disimpan!"
            );


            tampildata();
            bersih();
        }


        
        private void btntampil_Click(
            object sender,
            EventArgs e)
        {
            tampildata();
        }


      
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
                cmbanggota.SelectedValue == null ||
                cmbanggota.SelectedIndex == -1
            )
            {
                MessageBox.Show(
                    "Pilih anggota terlebih dahulu!"
                );
                return;
            }


           
            if (
                cmbbuku.SelectedValue == null ||
                cmbbuku.SelectedIndex == -1
            )
            {
                MessageBox.Show(
                    "Pilih buku terlebih dahulu!"
                );
                return;
            }


            string id =
                label2.Text;


            string kd =
                txtkodpin.Text;


            string idang =
                cmbanggota.SelectedValue.ToString();


            string idbuk =
                cmbbuku.SelectedValue.ToString();


            string tglpin =
                dtppinjam.Value.ToString(
                    "yyyy-MM-dd"
                );


            string tgltempo =
                dtptempo.Value.ToString(
                    "yyyy-MM-dd"
                );


            string statusLama = "";

            db.crud(
                $"SELECT status " +
                $"FROM t_peminjaman " +
                $"WHERE id_pinjam = '{id}'"
            );


            if (
                db.ds.Tables.Count > 0 &&
                db.ds.Tables[0].Rows.Count > 0
            )
            {
                statusLama =
                    db.ds.Tables[0]
                    .Rows[0]["status"]
                    .ToString();
            }


            string ss =
                statusLama;


            db.crud(
                $"UPDATE t_peminjaman SET " +
                $"kode_pinjam = '{kd}', " +
                $"id_anggota = '{idang}', " +
                $"id_buku = '{idbuk}', " +
                $"tanggal_pinjam = '{tglpin}', " +
                $"tanggal_jatuh_tempo = '{tgltempo}', " +
                $"status = '{ss}' " +
                $"WHERE id_pinjam = '{id}'"
            );


            MessageBox.Show(
                "Data peminjaman berhasil diupdate!"
            );


            bersih();
            tampildata();
        }


      
        private void Fpeminjaman_Load(
            object sender,
            EventArgs e)
        {
            
            
           
            txtkodpin.ReadOnly = true;

            txtkodpin.Text =
                generateKodePinjam();



            dtppinjam.MinDate =
                DateTime.Today;

            dtppinjam.MaxDate =
                DateTime.Today;

            dtppinjam.Value =
                DateTime.Today;


           
            db.crud(
                "SELECT * FROM t_buku"
            );

            cmbbuku.DataSource =
                db.ds.Tables[0];

            cmbbuku.DisplayMember =
                "judul";

            cmbbuku.ValueMember =
                "id_buku";

            cmbbuku.SelectedIndex = -1;


        
            db.crud(
                "SELECT * FROM t_anggota"
            );

            cmbanggota.DataSource =
                db.ds.Tables[0];

            cmbanggota.DisplayMember =
                "nama_anggota";

            cmbanggota.ValueMember =
                "id_anggota";

            cmbanggota.SelectedIndex = -1;


            
            tampildata();
        }


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


           
            string kodepinjam =
                dataGridView1.Rows[baris]
                .Cells[0]
                .Value
                .ToString();

            if (kolom == 6)
            {
                db.crud(
                    $"SELECT * " +
                    $"FROM t_peminjaman " +
                    $"WHERE kode_pinjam = '{kodepinjam}'"
                );


                if (
                    db.ds.Tables.Count == 0 ||
                    db.ds.Tables[0].Rows.Count == 0
                )
                {
                    MessageBox.Show(
                        "Data tidak ditemukan!"
                    );
                    return;
                }


                foreach (DataRow b in db.ds.Tables[0].Rows)
                {
                   
                    label2.Text =
                        b["id_pinjam"].ToString();


                   
                    txtkodpin.Text =
                        b["kode_pinjam"].ToString();


                   
                    cmbanggota.SelectedValue =
                        b["id_anggota"].ToString();


                   
                    cmbbuku.SelectedValue =
                        b["id_buku"].ToString();


                   
                    if (
                        b["tanggal_pinjam"] != DBNull.Value
                    )
                    {
                        DateTime tanggalPinjam =
                            Convert.ToDateTime(
                                b["tanggal_pinjam"]
                            );

                        dtppinjam.MinDate =
                            tanggalPinjam;

                        dtppinjam.MaxDate =
                            tanggalPinjam;

                        dtppinjam.Value =
                            tanggalPinjam;
                    }


                    
                    if (
                        b["tanggal_jatuh_tempo"] != DBNull.Value
                    )
                    {
                        dtptempo.Value =
                            Convert.ToDateTime(
                                b["tanggal_jatuh_tempo"]
                            );
                    }
                }
            }

            if (kolom == 7)
            {
                DialogResult setuju =
                    MessageBox.Show(
                        "Apakah kamu ingin hapus data?",
                        "Pemberitahuan",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );


                if (
                    setuju ==
                    DialogResult.Yes
                )
                {
                    string idbuk = "";


                    db.crud(
                        $"SELECT id_buku " +
                        $"FROM t_peminjaman " +
                        $"WHERE kode_pinjam = '{kodepinjam}'"
                    );


                    if (
                        db.ds.Tables.Count > 0 &&
                        db.ds.Tables[0].Rows.Count > 0
                    )
                    {
                        idbuk =
                            db.ds.Tables[0]
                            .Rows[0]["id_buku"]
                            .ToString();
                    }


                  
                    db.crud(
                        $"DELETE FROM t_peminjaman " +
                        $"WHERE kode_pinjam = '{kodepinjam}'"
                    );


           
                    if (
                        !string.IsNullOrEmpty(idbuk)
                    )
                    {
                        db.crud(
                            $"UPDATE t_buku " +
                            $"SET stok = stok + 1 " +
                            $"WHERE id_buku = '{idbuk}'"
                        );
                    }


                    MessageBox.Show(
                        "Data berhasil dihapus!"
                    );


                    bersih();
                    tampildata();
                }
            }
        }
    }
}
