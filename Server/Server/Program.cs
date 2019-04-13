using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using System.Data.SqlClient;

namespace Server
{
     public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;

        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder str1 = new StringBuilder();
    }

    class Program
    {
        static string folderA = "C:\\Users\\P k jain\\Desktop\\A";
        static SqlConnection x2;
        static SqlCommand y2;
        static SqlDataReader z2;
        static string[] str;
        private static IAsyncResult old;
                   static int i=0,j=0;
                   static Socket listener;
             static string s1,s;
            public static ManualResetEvent allDone = new ManualResetEvent(false);
            static Socket handler;
        public static void StartListening()
    {
        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 8888);
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
            Console.WriteLine(ex.ToString());
        }

    }
    public static void AcceptCallback(IAsyncResult ar)
    {

        allDone.Set();


        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);


    StateObject state = new StateObject();    
        state.workSocket = handler;
         old = handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        
    }
   
    public static void ReadCallback(IAsyncResult ar)
    {
        int fileNameLen = 1;
        j = 0;
        try
        {
            if (old == ar)
            {
                StateObject state = (StateObject)ar.AsyncState;
                handler = state.workSocket;
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
           Console.WriteLine( ex.ToString());
        }   
    
}
  public static void LabelWriter()
    {
        if(s1.CompareTo("File Transfer")==0)
        filetransfer();
        if (s1.CompareTo("New User") == 0)
            newuser();
        if (s1.CompareTo("Delete") == 0)
            delete();
        if (s1.CompareTo("New Password") == 0)
            newPassword();
        if (s1.CompareTo("Welcome") == 0)
            welcome();
       
    }

  public static void welcome()
  {
      Send(handler, s);
  }
        

public static void filetransfer()
{
    x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=G:\PRACHI\PROOOO\A\SERVER\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
    x2.Open();
    string cmdStr = "SELECT * FROM tr WHERE username='" + str[1] + "' and password='" + str[2] + "'";
    y2 = new SqlCommand(cmdStr, x2);
    z2 = y2.ExecuteReader();
    y2.Dispose();
    //Check the datareader here...if dr returns 1 record...user is in DB
    if (z2.Read() == true)
    {
        s = z2["location"].ToString();
        Send(handler,"Login Successful*"+s);
        Console.WriteLine("Login Successful");
        
       //User is in DB... do work here
        synchronize();
       // Send(handler, "Files are  synchronized");
    }

    else
    {
        Send(handler, "Invalid Credentials, Please Re-Enter");
        Console.WriteLine("Invalid Credentials, Please Re-Enter");
    }
    x2.Close();
    
}


public static void newuser()
{
    SqlCommand y3;
    x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=G:\PRACHI\PROOOO\A\SERVER\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
    x2.Open();
    string cmdStr = "SELECT username FROM tr WHERE username='" + str[1] +"'";
    y2 = new SqlCommand(cmdStr, x2);
    z2 = y2.ExecuteReader();

    //Check the datareader here...if dr returns 1 record...user is in DB
    if (z2.Read() == true)
    {
        Send(handler, "Enter a unique Username");
        // Begin sending the data to the remote device.
       // listener.BeginSend((byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), listener);
        Console.WriteLine("Enter a unique Username");
        y2.Dispose();
        z2.Close();
        x2.Close();
    }
    else
    {
        y2.Dispose();
        z2.Close();
        y3 = new SqlCommand("insert into tr values('" + str[1] + "','" + str[2] + "','" + str[3] + "')", x2);
        y3.ExecuteNonQuery();
        Send(handler,"record inserted");
        Console.WriteLine("record inserted");
        x2.Close();
        Directory.CreateDirectory(folderA+"\\"+str[1]);

    }
}
public static void delete()
{
    x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=G:\PRACHI\PROOOO\A\SERVER\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
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
        Send(handler, "record deleted");
        Console.WriteLine("record deleted");
        x2.Close();

    }
    else
    {
        Send(handler, "Invalid Credentials, Please Re-Enter");
        Console.WriteLine("Invalid Credentials, Please Re-Enter");
    }
    x2.Close();
}
public static void newPassword()
{
    try
    {
        x2 = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=G:\PRACHI\PROOOO\A\SERVER\SERVER\DATABASE1.MDF;Integrated Security=True;User Instance=True");
        x2.Open();
        y2 = new SqlCommand("UPDATE tr SET password = '" + str[2] + "' WHERE username = '" + str[1] + "'", x2);
        y2.ExecuteNonQuery();
        Send(handler, "record updated");
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
public static void synchronize()
{
    string s2 = str[1];
     string st = folderA+"\\"+s2;
     try
     {
         FileSyncProvider providerA = new FileSyncProvider(Guid.NewGuid(), st);
         FileSyncProvider providerB = new FileSyncProvider(Guid.NewGuid(), s);

         // Ask providers to detect changes
         providerA.DetectChanges();
         providerB.DetectChanges();

         // Sync changes
         SyncOrchestrator agent = new SyncOrchestrator();
         agent.LocalProvider = providerA;
         agent.RemoteProvider = providerB;
         agent.Direction = SyncDirectionOrder.UploadAndDownload;
         agent.Synchronize();
          Console.WriteLine("Files are  synchronized");
         
     }
     catch (Exception ex)
     {
         Console.WriteLine(ex.Message); 
     }
}
private static void Send(Socket handler, String data)
{
    // Convert the string data to byte data using ASCII encoding.
    byte[] byteData = Encoding.ASCII.GetBytes(data);

    // Begin sending the data to the remote device.
    handler.BeginSend(byteData, 0, byteData.Length, 0,new AsyncCallback(SendCallback), handler);
}

private static void SendCallback(IAsyncResult ar)
{
    try
    {
        // Retrieve the socket from the state object.
        Socket h = (Socket)ar.AsyncState;

        // Complete sending the data to the remote device.
        int bytesSent = h.EndSend(ar);
     

     

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}


 static void Main(string[] args)
        {  Thread t1;   
        t1 = new Thread(new ThreadStart(StartListening));
        t1.Start();
        }

        }
    }

