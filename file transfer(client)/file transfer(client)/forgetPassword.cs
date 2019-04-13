using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Net.Sockets;

namespace file_transfer_client_
{
    public partial class forgetPassword : Form
    {
        byte[] a;
        string s;
        Socket clientSock;
        public forgetPassword()
        {
            InitializeComponent();
        }
        public forgetPassword(string st)
        {
            s = st;
            InitializeComponent();
        }
        public string GetRandomPasswordUsingGUID(int length)
        {
            // Get the GUID
            string guidResult = System.Guid.NewGuid().ToString();

            // Remove the hyphens
            guidResult = guidResult.Replace("-", string.Empty);

            // Make sure length is valid
            if (length <= 0 || length > guidResult.Length)
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length);

            // Return the first length bytes
            return guidResult.Substring(0, length);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string guidResult = GetRandomPasswordUsingGUID(5).ToString();
            textBox2.Text = guidResult;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            byte[] form_name = Encoding.UTF8.GetBytes(this.Text);
            byte[] username = Encoding.UTF8.GetBytes(textBox1.Text);
            byte[] password = Encoding.UTF8.GetBytes(textBox2.Text);
            byte[] form_name_len = BitConverter.GetBytes(form_name.Length);
            byte[] userNameLen = BitConverter.GetBytes(username.Length);
            byte[] passwordLen = BitConverter.GetBytes(password.Length);
            a = new byte[12 + form_name.Length + username.Length + password.Length];
            form_name_len.CopyTo(a, 0);
            form_name.CopyTo(a, 4);
            userNameLen.CopyTo(a, 4 + form_name.Length);
            username.CopyTo(a, 8 + form_name.Length);
            passwordLen.CopyTo(a, 8 + form_name.Length + username.Length);
            password.CopyTo(a, 12 + form_name.Length + username.Length);
            clientSock.Connect(s, 8888); //target machine's ip address and the port number
            clientSock.Send(a);
            byte[] b = new byte[1024];
            string str = Encoding.ASCII.GetString(b, 0, clientSock.Receive(b));
            MessageBox.Show(str);
            clientSock.Close();
            transfer t = new transfer(s);
            t.Show();
            this.Close();
            
            
        }

        }
}
