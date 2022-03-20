using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetaTools
{
    public partial class ReleaseBiliDown : Form
    {
        public ReleaseBiliDown()
        {
            InitializeComponent();
        }

        private void ReleaseBiliDown_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
            Main form = new Main();
            form.Show();
        }
    }
}
