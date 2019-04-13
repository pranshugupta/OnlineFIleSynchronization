using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace file_transfer_client_
{
    public partial class Form1 : Form
    {
        Socket clientSock;
        string m_splitter = "'\\'";
        string m_fName;
        string[] m_split = null;
        byte[] m_clientData;
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(string st, Socket s)
        {
            InitializeComponent();
            label1.Text = "Welcome " + st + " !!";
            clientSock = s;
        }
        
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Text =null;
            openFileDialog1.FileName = null;
            openFileDialog1.ShowDialog();
            
        }

        private void sendButton_Click_1(object sender, EventArgs e)
        {
           // clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if(m_fName == null||textBox1.Text==null)
            {
                MessageBox.Show("Select a file");
            }
            else
            {
            byte[] fileName = Encoding.UTF8.GetBytes(m_fName); //file name
            byte[] fileData = File.ReadAllBytes(textBox1.Text); //file
            byte[] fileNameLen = BitConverter.GetBytes(fileName.Length); //lenght of file name
            m_clientData = new byte[4 + fileName.Length + fileData.Length];

            fileNameLen.CopyTo(m_clientData, 0);
            fileName.CopyTo(m_clientData, 4);
            fileData.CopyTo(m_clientData, 4 + fileName.Length);


           //clientSock.Connect("192.168.0.1", 8888); //target machine's ip address and the port number
            clientSock.Send(m_clientData);
          
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            clientSock.Close();
            transfer t = new transfer("192.168.0.1");
            t.Show();
            this.Close();
           
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

            char[] delimiter = m_splitter.ToCharArray();
            textBox1.Text = openFileDialog1.FileName;

            m_split = textBox1.Text.Split(delimiter);
            int limit = m_split.Length;

            m_fName = m_split[limit - 1].ToString();
        }

            }
       
    }

