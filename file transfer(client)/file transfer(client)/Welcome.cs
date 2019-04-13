using System;
using System.IO;
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
    public partial class Welcome : Form
    {
        Socket clientSock;
        string st,str;
        public Welcome()
        {
            InitializeComponent();
        }
        public Welcome(string s)
        {
            InitializeComponent();
            st = s;
            label1.Text = "Welcome " + s + " !!";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            transfer t = new transfer("127.0.0.1");
            t.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           

            ListDirectory(treeView1, str);
            treeView1.ExpandAll();
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Nodes.Add(new TreeNode(file.Name));
            return directoryNode;
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSock.Connect("127.0.0.1", 8888);
            byte[] form_name = Encoding.UTF8.GetBytes(this.Text);
            byte[] form_name_len = BitConverter.GetBytes(form_name.Length);
            byte[] a = new byte[4 + form_name.Length];
            form_name_len.CopyTo(a, 0);
            form_name.CopyTo(a, 4);
            clientSock.Send(a);
            byte[] b = new byte[1024];
            str = Encoding.ASCII.GetString(b, 0, clientSock.Receive(b));
        }

    }
}