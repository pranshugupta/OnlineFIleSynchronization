using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace file_transfer_client_
{
    public partial class User : Form
    {
        public string str1;
         byte[] a;
         string s;
         Socket clientSock;
        public User()
        {
            InitializeComponent();
        }
        public User(string st)
        {
            s = st;
            InitializeComponent();
        }
        
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Show the open file dialog to select our data.
            folderBrowserDialog1.ShowDialog();

            //Get the file name and write it into our text box.
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            if (textBox1.Text != null) button1.Enabled = true;
        }

        private void sendButton_Click_1(object sender, EventArgs e)
        {
            
            if (textBox2.Text == null)
                MessageBox.Show("Username cannot be null");
            else if (textBox3.Text == null)
                MessageBox.Show("Password Cannot be null");
            else if (textBox1.Text == null)
                MessageBox.Show("Browse a valid folder");
            else
            {
                str1 = textBox1.Text;
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                byte[] form_name = Encoding.UTF8.GetBytes(this.Text);
                byte[] text = Encoding.UTF8.GetBytes(textBox1.Text);
                byte[] username = Encoding.UTF8.GetBytes(textBox2.Text);
                byte[] password = Encoding.UTF8.GetBytes(textBox3.Text);
                byte[] form_name_len = BitConverter.GetBytes(form_name.Length);
                byte[] fileNameLen = BitConverter.GetBytes(text.Length); //lenght of text
                byte[] userNameLen = BitConverter.GetBytes(username.Length);
                byte[] passwordLen = BitConverter.GetBytes(password.Length);
                a = new byte[16 + form_name.Length + text.Length + username.Length + password.Length];
                form_name_len.CopyTo(a, 0);
                form_name.CopyTo(a, 4);
                userNameLen.CopyTo(a, 4 + form_name.Length);
                username.CopyTo(a, 8 + form_name.Length);
                passwordLen.CopyTo(a, 8 + form_name.Length + username.Length);
                password.CopyTo(a, 12 + form_name.Length + username.Length);
                fileNameLen.CopyTo(a, 12 + form_name.Length + username.Length + password.Length);
                text.CopyTo(a, 16 + form_name.Length + username.Length + password.Length);
                clientSock.Connect(s, 8888); //target machine's ip address and the port number
                clientSock.Send(a);
                byte[] b = new byte[1024];
                string str = Encoding.ASCII.GetString(b,0,clientSock.Receive(b));
                MessageBox.Show(str);
                clientSock.Close();
                transfer t = new transfer(s);
                t.Show();
                this.Close();
               
            }
        }

       
            }
       
    }

