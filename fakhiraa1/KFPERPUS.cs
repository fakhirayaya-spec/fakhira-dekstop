using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fakhiraa1
{
    class KFPERPUS
    {
        public static void untukform(Form formperpus, Panel pnlperpus)
        {
            pnlperpus.Controls.Clear();
            pnlperpus.Controls.Add(formperpus);
            formperpus.FormBorderStyle = FormBorderStyle.None;
            formperpus.Dock = DockStyle.Fill;
            formperpus.Show();
        }
    }
}
