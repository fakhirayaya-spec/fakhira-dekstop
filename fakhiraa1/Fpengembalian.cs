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

            
            cmbkd.SelectedIndexChanged += cmbkd_SelectedIndexChanged;
            dtpkembali.ValueChanged += dtpkembali_ValueChanged;

            
            txtterlambat.ReadOnly = true;
            txtdenda.ReadOnly = true;

            isiComboPinjam();
            tampildata();
            bersih();
        }


   
        public void bersih()
        {
            label2.Text = "";

            cmbkd.SelectedIndex = -1;

            dtppinjam.Value = DateTime.Now;
            dtpkembali.Value = DateTime.Now;

            txtterlambat.Text = "";
            txtdenda.Text = "";
        }


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

                
                cmbkd.DisplayMember =
                    "kode_pinjam";

                
                cmbkd.ValueMember =
                    "id_pinjam";

                cmbkd.SelectedIndex = -1;
            }
            else
            {
                cmbkd.DataSource = null;
            }
        }


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


               
                dtpkembali.Value =
                    DateTime.Today;


                hitungTerlambatDanDenda();
            }
        }


        private void dtpkembali_ValueChanged(
            object sender,
            EventArgs e)
        {
            hitungTerlambatDanDenda();
        }


      
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


         
            DateTime tanggalJatuhTempo =
                Convert.ToDateTime(
                    baris["tanggal_jatuh_tempo"]
                ).Date;


      
            DateTime tanggalKembali =
                dtpkembali.Value.Date;


            int jumlahHariTerlambat =
                (
                    tanggalKembali -
                    tanggalJatuhTempo
                ).Days;


      
            if (jumlahHariTerlambat <= 0)
            {
                txtterlambat.Text = "0";
                txtdenda.Text = "0";
            }
            else
            {
            
                txtterlambat.Text =
                    jumlahHariTerlambat.ToString();


                int denda =
                    jumlahHariTerlambat * 2000;


                txtdenda.Text =
                    denda.ToString();
            }
        }


        
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


            
            if (
                jumlahTerlambat == 0 &&
                jumlahDenda == 0
            )
            {
                return "dikembalikan";
            }


            
            return "terlambat";
        }


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


        private void btnsimpan_Click(
            object sender,
            EventArgs e)
        {
           
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


           
           
            string idpinjam =
                cmbkd.SelectedValue.ToString();


         
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


         
            string status =
                tentukanStatus();


            string tglkembali =
                dtpkembali.Value.ToString(
                    "yyyy-MM-dd"
                );


         
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


          
            db.crud(
                $"INSERT INTO t_pengembalian " +
                $"(id_pinjam, tanggal_kembali, terlambat, denda, status) " +
                $"VALUES " +
                $"('{idpinjam}', '{tglkembali}', " +
                $"'{terlambat}', '{denda}', '{status}')"
            );


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


           
            isiComboPinjam();
            tampildata();
            bersih();
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


         
            string status =
                tentukanStatus();


            string tglkembali =
                dtpkembali.Value.ToString(
                    "yyyy-MM-dd"
                );


           
            db.crud(
                $"UPDATE t_pengembalian SET " +
                $"id_pinjam = '{idpinjam}', " +
                $"tanggal_kembali = '{tglkembali}', " +
                $"terlambat = '{terlambat}', " +
                $"denda = '{denda}', " +
                $"status = '{status}' " +
                $"WHERE id_kembali = '{idkembali}'"
            );


            
        
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


            isiComboPinjam();
            tampildata();
            bersih();


            MessageBox.Show(
                "Data pengembalian berhasil diupdate!"
            );
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


            string tglkembali =
                dataGridView1.Rows[baris]
                .Cells[1]
                .Value
                .ToString();


      
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


                
                label2.Text =
                    data["id_kembali"].ToString();


                string idPinjamEdit =
                    data["id_pinjam"].ToString();


                isiComboPinjam(
                    idPinjamEdit
                );


                cmbkd.SelectedValue =
                    idPinjamEdit;


             
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


         
            hitungTerlambatDanDenda();


          
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


                   
                    db.crud(
                        $"DELETE FROM t_pengembalian " +
                        $"WHERE id_kembali = '{idkembali}'"
                    );


                
                    if (idpinjam != "")
                    {
                        db.crud(
                            $"UPDATE t_peminjaman " +
                            $"SET status = 'dipinjam' " +
                            $"WHERE id_pinjam = '{idpinjam}'"
                        );
                    }


                  
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


                  
                    isiComboPinjam();

                    tampildata();

                    bersih();
                }
            }
        }


       
        private void btntampil_Click(
            object sender,
            EventArgs e)
        {
            tampildata();
        }
    }
}
