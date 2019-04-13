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
//using file_transfer_client_
namespace file_transfer_client_
{
    
    public partial class transfer : Form
    {
        string s;
        byte[] a;
        Socket clientSock;
        public transfer()
        {
            InitializeComponent();
            
        }
        public transfer(string st)
        {
            s = st;
            InitializeComponent();

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            forgetPassword f = new forgetPassword(s);
            f.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            if (s.CompareTo("127.0.0.1") == 0)
            {
                User n = new User(s);
                n.Show();
            }
            else
            {
                New_User n = new New_User(s);
                n.Show();
            }

            
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            delete d = new delete(s);
            d.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSock.Connect(s,8888);
            byte[] form_name = Encoding.UTF8.GetBytes(this.Text);
            byte[] username = Encoding.UTF8.GetBytes(textBox1.Text);
            byte[] password = Encoding.UTF8.GetBytes(textBox2.Text);
            byte[] form_name_len = BitConverter.GetBytes(form_name.Length);
            byte[] userNameLen = BitConverter.GetBytes(username.Length);
            byte[] passwordLen = BitConverter.GetBytes(password.Length);
            a = new byte[12+form_name.Length+username.Length + password.Length];
            form_name_len.CopyTo(a,0);
            form_name.CopyTo(a,4);
            userNameLen.CopyTo(a,4+form_name.Length);
            username.CopyTo(a, 8+form_name.Length);
            passwordLen.CopyTo(a, 8+form_name.Length + username.Length);
            password.CopyTo(a, 12 +form_name.Length+ username.Length);
             clientSock.Send(a);
             byte[] b = new byte[1024];
             string str = Encoding.ASCII.GetString(b, 0, clientSock.Receive(b));
            
             string[] s1 = str.Split('*');
           // s1[0]=str.Split("*", 2);
             MessageBox.Show(s1[0]);
              if (s1[0].CompareTo("Login Successful") == 0)
             {
                
                 if (s.CompareTo("127.0.0.1") == 0)
                 {
                     frmNotifier fr = new frmNotifier(s, s1[1]);
                     fr.Show();
                     Welcome w = new Welcome(textBox1.Text);
                     w.Show();
                 }
                 else
                 {
                     Form1 f = new Form1(textBox1.Text, clientSock);
                     f.Show();
                 } 
                 this.Hide();
                 
             }
             else if (str.CompareTo("Invalid Credentials, Please Re-Enter") == 0)
             {
                 textBox1.Clear();
                 textBox2.Clear();
                 clientSock.Close();
             }
        }

   
    }
}
