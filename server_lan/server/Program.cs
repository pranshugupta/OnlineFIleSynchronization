using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;

namespace server
{
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;

        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
     }

    class Program
    {
        SqlConnection x2;
        SqlCommand y2;
        SqlDataReader z2;
        string[] str;
        int flag = 0;
        private IAsyncResult old;
        int i = 0, j = 0;
        string s1;
        string receivedPath = "";
        string folderA = "E:\\A_lan";
        public ManualResetEvent allDone = new ManualResetEvent(false);
        StateObject s = new StateObject();      
       
    public void StartListening()
    {
        byte[] bytes = new Byte[1024];
        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 8888);
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener.Bind(ipEnd);
            listener.Listen(100);
            while (true)
            {
                allDone.Reset();
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                allDone.WaitOne();

            }
        }
        catch (Exception ex)
        {
           Console.WriteLine(ex.Message);
        }

    }
    public  void AcceptCallback(IAsyncResult ar)
    {


        allDone.Set();


        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        StateObject state = new StateObject(); 
        state.workSocket = handler;
        s.workSocket=handler;
        old=handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        
    }

    public  void ReadCallback(IAsyncResult ar)
    {          
         int fileNameLen = 1;
        j = 0;
        try
        {
            if (old == ar)
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                int bytesRead = handler.EndReceive(ar);
                str = new string[1024];
                if (bytesRead > 0)
                {
                    
                    i = 0;
                    while (i < bytesRead)
                    {
                        fileNameLen = BitConverter.ToInt32(state.buffer, i);
                        string fileName = Encoding.UTF8.GetString(state.buffer, 4 + i, fileNameLen);
                        str[j] = fileName;
                         j++;
                        i = i + 4 + fileNameLen;
                    }
                    s1 = str[0];
                    LabelWriter();
                   }
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                }
            
                           
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }   
    
}
public  void LabelWriter()
{
     if(s1.CompareTo("File Transfer")==0)
        filetransfer();
        if (s1.CompareTo("New User") == 0)
            newuser();
        if (s1.CompareTo("Delete") == 0)
            delete();
        if (s1.CompareTo("New Password") == 0)
            newPassword();
       
}
public  void handle()
{
    s.workSocket.BeginReceive(s.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RCallback), s);
    //handler.BeginReceive(s.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RCallback), s);
    flag = 0;
}

public  void RCallback(IAsyncResult ar)
{
    try
    {
        int fileNameLen = 1;
        String content = String.Empty;
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;
        int bytesRead = handler.EndReceive(ar);
        if (bytesRead > 0)
        {
            

            if (flag == 0)
            {
                fileNameLen = BitConverter.ToInt32(state.buffer, 0);
                string fileName = Encoding.UTF8.GetString(state.buffer, 4, fileNameLen);
                receivedPath = "E:\\A_lan\\" + str[1] + "\\" + fileName;
                flag++;
            }
            if (flag >= 1)
            {
               
                BinaryWriter writer = new BinaryWriter(File.Open(receivedPath, FileMode.Append));
                if (flag == 1)
                {
                    writer.Write(state.buffer, 4 + fileNameLen, bytesRead - (4 + fileNameLen));
                    flag++;
                }
                else
                    writer.Write(state.buffer, 0, bytesRead);
                writer.Close();
                
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RCallback), state);
                
                
            }
            
            
        }
        else
        {
            LWriter();
            
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

}

public  void LWriter()
{
    //Console.WriteLine(flag.ToString());
    Console.WriteLine("data received");
}




       


        public  void filetransfer()
{
    x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\SERVER_LAN\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
    x2.Open();
    string cmdStr = "SELECT * FROM tr WHERE username='" + str[1] + "' and password='" + str[2] + "'";
    y2 = new SqlCommand(cmdStr, x2);
    z2 = y2.ExecuteReader();
    y2.Dispose();
    //Check the datareader here...if dr returns 1 record...user is in DB
    if (z2.Read() == true)
    {
        Send(s.workSocket, "Login Successful");
        Console.WriteLine("Login Successful");
        handle();
        
    }

    else
    {
        Send(s.workSocket, "Invalid Credentials, Please Re-Enter");
        Console.WriteLine("Invalid Credentials, Please Re-Enter");
    }
    x2.Close();
    
}

        


public  void newuser()
{
    SqlCommand y3;
    x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\SERVER_LAN\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
    x2.Open();
    string cmdStr = "SELECT username FROM tr WHERE username='" + str[1] +"'";
    y2 = new SqlCommand(cmdStr, x2);
    z2 = y2.ExecuteReader();

    //Check the datareader here...if dr returns 1 record...user is in DB
    if (z2.Read() == true)
    {
        Send(s.workSocket, "Enter a unique Username");
        // Begin sending the data to the remote device.
       Console.WriteLine("Enter a unique Username");
        y2.Dispose();
        z2.Close();
        x2.Close();
    }
    else
    {
        y2.Dispose();
        z2.Close();
        y3 = new SqlCommand("insert into tr values('" + str[1] + "','" + str[2] + "')", x2);
        y3.ExecuteNonQuery();
        Send(s.workSocket,"record inserted");
        Console.WriteLine("record inserted");
        x2.Close();
        Directory.CreateDirectory(folderA+"\\"+str[1]);

    }
}
public  void delete()
{
    x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\SERVER_LAN\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
    x2.Open();
    string cmdStr = "SELECT * FROM tr WHERE username='" + str[1] + "' and password='" + str[2] + "'";
    y2 = new SqlCommand(cmdStr, x2);
    z2 = y2.ExecuteReader();

    //Check the datareader here...if dr returns 1 record...user is in DB
    if (z2.Read() == true)
    {
        z2.Dispose();
        string st = " delete from tr where username='" + str[1] + "' and password='" + str[2] + "'";
        y2 = new SqlCommand(st, x2);
        y2.ExecuteNonQuery();
        Directory.Delete(folderA+"\\"+str[1],true);
        y2.Dispose();
        Send(s.workSocket, "record deleted");
        Console.WriteLine("record deleted");
        x2.Close();

    }
    else
    {
        Send(s.workSocket, "Invalid Credentials, Please Re-Enter");
        Console.WriteLine("Invalid Credentials, Please Re-Enter");
    }
    x2.Close();
}
public  void newPassword()
{
    try
    {
        x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\SERVER_LAN\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
        x2.Open();
        y2 = new SqlCommand("UPDATE tr SET password = '" + str[2] + "' WHERE username = '" + str[1] + "'", x2);
        y2.ExecuteNonQuery();
        Send(s.workSocket, "record updated");
        Console.WriteLine("record updated");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        x2.Close();
    }

}

        private void Send(Socket handler, String data)
{
    // Convert the string data to byte data using ASCII encoding.
    byte[] byteData = Encoding.ASCII.GetBytes(data);

    // Begin sending the data to the remote device.
    handler.BeginSend(byteData, 0, byteData.Length, 0,new AsyncCallback(SendCallback), handler);
}

private void SendCallback(IAsyncResult ar)
{
    try
    {
        // Retrieve the socket from the state object.
        Socket h = (Socket)ar.AsyncState;

        // Complete sending the data to the remote device.
        int bytesSent = h.EndSend(ar);
     

     

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
        static void Main(string[] args)
        {
            Thread t1;
            Program p = new Program();
            t1 = new Thread(new ThreadStart(p.StartListening));
        t1.Start();
        }






        }
    }
