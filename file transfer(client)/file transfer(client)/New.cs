using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
namespace file_transfer_client_
{
    public partial class New : Form
    {
        public New()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            transfer f = new transfer("127.0.0.1");
            f.Show();
                      
        }

        private void button2_Click(object sender, EventArgs e)
        {
            transfer f = new transfer("192.168.0.1");
            f.Show();
           
        }
    }
}
