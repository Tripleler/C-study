using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

public enum ChatState
{
    Login,
    Login_Sucess,
    Login_Fail_None,
    Login_Fail_duple,
    Register,
    Register_Sucess,
    Register_Fail,
    Info,
    Message,
    file_info,
    file_start,
    file,
    file_end,
}

namespace ChattingApp_Server
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket m_ServerSocket;
        //private List<Socket> m_ClientSocket;
        private Dictionary<Socket, string> m_ClientSocket;
        //private SQLiteConnection m_dbcon = null;

        public static string SERVER_DIR = "C:\\Users\\Symation\\Desktop\\Server\\";
        public static string USER_DB = @"C:\Users\Symation\Desktop\USER.db";
        public static int PORT = 9999;

        private WebSocketServer ws;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Switch_Click(object sender, RoutedEventArgs e)
        {
            // 서버 시작
            if (this.btn_Switch.Content.ToString() == "Start")
            {
                // 채팅 서버
                //this.m_ClientSocket = new Dictionary<Socket, string>();
                //this.m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //IPEndPoint ipep = new IPEndPoint(IPAddress.Any, PORT_CHAT);
                //this.m_ServerSocket.Bind(ipep);
                //this.m_ServerSocket.Listen(1000);
                //SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                //args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
                //this.m_ServerSocket.AcceptAsync(args);
                

                // 로그인 서버
                this.ws = new WebSocketServer(PORT);
                this.ws.AddWebSocketService<LoginServer>("/Login");
                this.ws.AddWebSocketService<ChatServer>("/Chat");
                this.ws.Start();
                this.btn_Switch.Content = "Stop";
                Write_Status($"서버 시작! 사용중인 포트 : {PORT}", Brushes.Black);
            }
            else
            {
                // 서버 종료
                //if (this.m_ServerSocket != null)
                //{
                //    this.m_ServerSocket.Close();
                //    this.m_ServerSocket.Dispose();
                //}
                //Thread.Sleep(100);
                //List<Socket> ClientList = this.m_ClientSocket.Keys.ToList();
                //for (int i = 0; i < ClientList.Count; i++)
                //{
                //    ClientList[i].Close();
                //    ClientList[i].Dispose();
                //    Thread.Sleep(100);
                //}
                //this.m_ClientSocket.Clear();
                this.ws.Stop();
                _Global.m_ClientIDs.Clear();
                this.btn_Switch.Content = "Start";
            }
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;
            //this.m_ClientSocket.Add(ClientSocket);
            if (ClientSocket != null)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] buffer;
                buffer = new byte[1024];
                args.SetBuffer(buffer, 0, 1024);
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                ClientSocket.ReceiveAsync(args);
                //Write_Status($"현재 접속중인 클라이언트 수 : {this.m_ClientSocket.Count}", Brushes.Blue);
            }
            e.AcceptSocket = null;
            try
            {
                this.m_ServerSocket.AcceptAsync(e);
            }
            catch (Exception ex)
            {
                Write_Status("Accept_Complted error >> " + ex.Message, Brushes.Red);
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = sender as Socket;
            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                byte[] buffer = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, buffer, e.BytesTransferred);
                if (buffer != null)
                {
                    try
                    {
                        // === Check ================================
                        string check = Encoding.UTF8.GetString(buffer, 1, e.BytesTransferred - 1);
                        // === Check End ============================
                        switch ((ChatState)buffer[0])
                        {
                            case ChatState.Message:
                                {
                                    if (this.m_ClientSocket.Count > 1)  // 접속중인 클라이언트가 둘 이상인 경우만
                                    {
                                        string msg = Encoding.Default.GetString(buffer, 1, buffer.Length - 1);
                                        byte[] bufferInfo = new byte[1] { (int)ChatState.Message };
                                        bufferInfo = bufferInfo.Concat(BitConverter.GetBytes(buffer.Length)).ToArray();
                                        bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(this.m_ClientSocket[ClientSocket])).ToArray();
                                        try
                                        {
                                            foreach (Socket tempSocket in this.m_ClientSocket.Keys)
                                            {
                                                if (tempSocket != ClientSocket)
                                                {
                                                    tempSocket.Send(bufferInfo, bufferInfo.Length, SocketFlags.None);
                                                    tempSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), $"받은 메시지\"{msg}\"를 {this.m_ClientSocket[tempSocket]}에게 전송 완료");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Write_Status("Receive_Completed Error => " + ex.Message, Brushes.Red);
                                        }
                                    }
                                    break;
                                }
                            case ChatState.file:
                                {
                                    int filesize = BitConverter.ToInt32(buffer, 1);
                                    byte[] bufferReceive = new byte[1024];
                                    long currentLength = 0;
                                    string fname = Encoding.Default.GetString(buffer, 5, buffer.Length - 5);
                                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                                    string dir = SERVER_DIR + guid;
                                    FileStream fileStream = new FileStream(dir, FileMode.Create, FileAccess.Write, FileShare.Write);
                                    BinaryWriter writer = new BinaryWriter(fileStream);
                                    try
                                    {
                                        while (currentLength < filesize)
                                        {
                                            // 파일 수신
                                            int receiveLength = ClientSocket.Receive(bufferReceive);
                                            // 연결 끊김
                                            if (receiveLength == 0)
                                            {
                                                writer.Close();
                                                return;
                                            }
                                            writer.Write(bufferReceive, 0, receiveLength);
                                            currentLength += receiveLength;
                                        }

                                        Thread.Sleep(10);

                                        // 파일정보 송신
                                        byte[] bufferInfo = new byte[1] { (int)ChatState.file_info };
                                        bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(guid)).ToArray();  // Guid
                                        bufferInfo = bufferInfo.Concat(BitConverter.GetBytes(Encoding.Default.GetBytes(fname).Length)).ToArray();  // 파일이름 길이
                                        bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(fname)).ToArray();  // 파일이름
                                        bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(this.m_ClientSocket[ClientSocket])).ToArray(); // 유저이름
                                        foreach (Socket tempSocket in this.m_ClientSocket.Keys)
                                        {
                                            if (tempSocket != ClientSocket)
                                            {
                                                tempSocket.Send(bufferInfo, bufferInfo.Length, SocketFlags.None);
                                                Write_Status($"받은 파일 정보\"{fname}\"를 {this.m_ClientSocket[tempSocket]}에게 전송 완료", Brushes.Red);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Write_Status("file receive error >> " + ex.Message, Brushes.Red);
                                    }
                                    finally
                                    {
                                        writer.Close();
                                    }
                                    break;
                                }
                            case ChatState.file_info:
                                {
                                    // 파일명
                                    string strFileName = Encoding.Default.GetString(buffer, 1, 32);

                                    // 파일 크기 송신
                                    FileInfo file = new FileInfo(SERVER_DIR + strFileName);
                                    byte[] bufferSend = new byte[1] { (int)ChatState.file };
                                    bufferSend = bufferSend.Concat(BitConverter.GetBytes((int)file.Length)).ToArray();
                                    ClientSocket.Send(bufferSend, bufferSend.Length, SocketFlags.None);

                                    Thread.Sleep(100);

                                    // 파일 송신
                                    long count = file.Length / 1024 + 1;
                                    using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        BinaryReader reader = new BinaryReader(stream);
                                        for (int i = 0; i < count; i++)
                                        {
                                            bufferSend = reader.ReadBytes(1024);
                                            ClientSocket.Send(bufferSend);
                                        }
                                        Write_Status($"사용자 : {this.m_ClientSocket[ClientSocket]}  파일: {file.FullName} {file.Length:#,###}byte 전송 완료", Brushes.Blue);
                                    }
                                    break;
                                }

                        }
                    }
                    catch (SocketException)
                    {
                        ClientSocket.Close();
                        this.m_ClientSocket.Remove(ClientSocket);
                        Write_Status($"클라이언트의 연결이 끊어졌습니다. 현재 접속중인 클라이언트 수 : {this.m_ClientSocket.Count}", Brushes.Red);
                    }
                    catch (Exception ex)
                    {
                        Write_Status("Receive_Completed Error => " + ex.Message, Brushes.Red);
                    }
                }
                Array.Clear(buffer, 0, buffer.Length);
                try
                {
                    if (ClientSocket.Connected)
                    {
                        ClientSocket.ReceiveAsync(e);
                    }
                }
                catch (Exception ex)
                {
                    Write_Status("Receive Completed error >> " + ex.Message, Brushes.Red);
                }
            }
            else
            {
                try
                {
                    ClientSocket.Disconnect(false);
                    byte[] buffer = new byte[1] { (int)ChatState.Info };
                    buffer = buffer.Concat(Encoding.Default.GetBytes($"{this.m_ClientSocket[ClientSocket]}님이 채팅방에서 나갔습니다.")).ToArray();
                    this.m_ClientSocket.Remove(ClientSocket);
                    List<Socket> sockets = this.m_ClientSocket.Keys.ToList();
                    for (int i = 0; i < sockets.Count; i++)
                    {
                        Socket tempSocket = sockets[i];
                        tempSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), string.Empty);
                    }
                    Write_Status(ClientSocket.RemoteEndPoint.ToString() + "의 연결이 끊어졌습니다. 현재 접속된 클라이언트 수 : " + this.m_ClientSocket.Count, Brushes.Red);
                }
                catch (Exception ex)
                {
                    Write_Status("Receive_Completed error >> " + ex.Message, Brushes.Red);
                }
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            string msg = ar.AsyncState.ToString();
            if (msg != string.Empty)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Write_Status(msg, Brushes.Blue);
                }));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Write_Status("서버시작 대기중", Brushes.Black);
            btn_Switch_Click(null, null);
            //test();
        }

        private void test()
        {
            try
            {
                ws = new WebSocketServer(9000);
                ws.AddWebSocketService<LoginServer>("/Login");
                //ws.AddWebSocketService<ChatServer>("/Chat");
                ws.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Write_Status(string msg, object color = null)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                TextRange txt = new TextRange(this.txt_Status.Document.ContentEnd, this.txt_Status.Document.ContentEnd);
                txt.Text = $"[{DateTime.Now.ToString("HH:mm:ss")}] {msg}\n";
                txt.ApplyPropertyValue(TextElement.ForegroundProperty, color);
                this.txt_Status.ScrollToEnd();
            }));
        }

        private void btn_Check_Click(object sender, RoutedEventArgs e)
        {
            Write_Status(this.ws.IsListening.ToString(), Brushes.Green);
        }
    }

    public class LoginServer : WebSocketBehavior
    {
        private SQLiteConnection m_dbcon = null;
        public static string SERVER_DIR = "C:\\Users\\Symation\\Desktop\\Server\\";
        public static string USER_DB = @"C:\Users\Symation\Desktop\USER.db";

        private void SendCallBack(string msg)
        {
            if (msg != string.Empty)
            {
                //MainWindow.Write_Status(msg, Brushes.Blue);
                Debug.WriteLine(msg);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            byte[] buffer = e.RawData;
            try
            {
                switch ((ChatState)buffer[0])
                {
                    case ChatState.Login:
                        {
                            string query = "SELECT * FROM CHATMATE WHERE USER_ID = @id AND USER_PW = @pw";
                            string dbConnection = string.Format("Data Source={0};", @"C:\Users\Symation\Desktop\USER.db");
                            using (m_dbcon = new SQLiteConnection(dbConnection))
                            {
                                string[] user = Encoding.Default.GetString(buffer, 1, buffer.Length - 1).Split('\n');
                                byte[] result = new byte[1];
                                string info = "";
                                m_dbcon.Open();
                                SQLiteCommand sqlCmd = new SQLiteCommand(query, m_dbcon);
                                sqlCmd.Parameters.AddWithValue("@id", user[0]);
                                sqlCmd.Parameters.AddWithValue("@pw", user[1]);
                                var reader = sqlCmd.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    // 중복로그인 체크
                                    if (_Global.m_ClientIDs.Values.ToList().Contains(user[0]))
                                    {
                                        result[0] = (int)ChatState.Login_Fail_duple;
                                        info = $"ID : {user[0]} 로그인 실패 : 현재 접속중인 아이디";
                                    }                                    
                                    else
                                    {
                                        //로그인 성공
                                        result[0] = (int)ChatState.Login_Sucess;
                                        info = $"ID : {user[0]} 로그인 성공! 현재 접속중인 유저 수 : {_Global.m_ClientIDs.Count + 1}";
                                        byte[] bufferInfo = Encoding.Default.GetBytes($"{user[0]}님이\n채팅방에 입장하셨습니다.");
                                        Array.Resize(ref bufferInfo, bufferInfo.Length + 1);
                                        Array.Copy(bufferInfo, 0, bufferInfo, 1, bufferInfo.Length - 1);
                                        bufferInfo[0] = (int)ChatState.Info;
                                        var ids = Sessions.IDs.Where(x => x != ID).ToList();
                                        foreach (var id in ids)
                                        {
                                            Sessions.SendToAsync(bufferInfo, id, new Action<bool>((completed) =>
                                            {
                                                if (completed)
                                                {
                                                    SendCallBack(info);
                                                }
                                            }));
                                        }
                                        _Global.m_ClientIDs.Add(ID, user[0]);
                                    }
                                }
                                else
                                {
                                    result[0] = (int)ChatState.Login_Fail_None;
                                    info = $"ID : {user[0]} 로그인 실패 : 결과값 없음";
                                }
                                SendAsync(result, new Action<bool>((completed) =>
                                {
                                    if (completed)
                                    {
                                        SendCallBack(info);
                                    }
                                }));
                            }
                            break;
                        }
                    case ChatState.Register:
                        {
                            string query = "INSERT INTO CHATMATE(USER_ID, USER_PW) VALUES (@id, @pw)";
                            string dbConnection = string.Format("Data Source={0};", USER_DB);
                            using (m_dbcon = new SQLiteConnection(dbConnection))
                            {
                                string[] user = Encoding.Default.GetString(buffer, 1, buffer.Length - 1).Split('\n');
                                byte[] result = new byte[1];
                                string info = "";
                                m_dbcon.Open();
                                SQLiteCommand sqlCmd = new SQLiteCommand(query, m_dbcon);
                                sqlCmd.Parameters.AddWithValue("@id", user[0]);
                                sqlCmd.Parameters.AddWithValue("@pw", user[1]);
                                try
                                {
                                    var reader = sqlCmd.ExecuteReader();
                                    result[0] = (int)ChatState.Register_Sucess;
                                    info = $"ID : {user[0]} 회원가입 성공";
                                }
                                catch (Exception ex)
                                {
                                    info = $"ID : {user[0]} 회원가입 실패 >> " + ex.Message;
                                    result[0] = (int)ChatState.Register_Fail;
                                }
                                SendAsync(result, new Action<bool>((completed) =>
                                {
                                    if (completed)
                                    {
                                        SendCallBack(info);
                                    }
                                }));
                            }
                            break;
                        }

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _Global.m_ClientIDs.Remove(ID);
        }
    }

    public class ChatServer : WebSocketBehavior
    {
        public static string SERVER_DIR = "C:\\Users\\Symation\\Desktop\\Server\\";

        int filesize;
        BinaryWriter writer;

        private void SendCallBack(string msg)
        {
            if (msg != string.Empty)
            {
                //MainWindow.Write_Status(msg, Brushes.Blue);
                Debug.WriteLine(msg);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            byte[] buffer = e.RawData.Skip(1).ToArray();            
            try
            {
                switch ((ChatState)e.RawData[0])
                {
                    case ChatState.Message:
                        {
                            if (_Global.m_ClientIDs.Count > 1)  // 접속중인 클라이언트가 둘 이상인 경우만
                            {
                                string msg = Encoding.Default.GetString(buffer);
                                byte[] bufferInfo = new byte[1] { (int)ChatState.Message };
                                bufferInfo = bufferInfo.Concat(BitConverter.GetBytes(buffer.Length)).ToArray();
                                bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(_Global.m_ClientIDs[ID])).ToArray();
                                try
                                {
                                    foreach (string id in _Global.m_ClientIDs.Keys.Where(x => x != ID))
                                    {
                                        this.Sessions.SendTo(bufferInfo, id);
                                        this.Sessions.SendToAsync(buffer, id, new Action<bool>((completed) =>
                                        {
                                            if (completed)
                                            {
                                                SendCallBack($"받은 메시지\"{msg}\"를 {_Global.m_ClientIDs[ID]}에게 전송 완료");
                                            }
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Write_Status("Receive_Completed Error => " + ex.Message, Brushes.Red);
                                    Debug.WriteLine("Receive_Completed Error => " + ex.Message);
                                }
                            }
                            break;
                        }
                    case ChatState.file_info:  // 파일정보받고 파일 스트림 준비
                        {
                            this.filesize = BitConverter.ToInt32(buffer, 0);
                            string fname = Encoding.Default.GetString(buffer, 4, buffer.Length - 4);
                            string guid = Guid.NewGuid().ToString().Replace("-", "");
                            string dir = SERVER_DIR + guid;
                            FileStream fileStream = new FileStream(dir, FileMode.Create, FileAccess.Write, FileShare.Write);
                            this.writer = new BinaryWriter(fileStream);
                            break;
                        }
                    case ChatState.file:  // 파일 수신
                        {                            
                            long currentLength = 0;                         
                            try
                            {
                                while (currentLength < this.filesize)
                                {
                                    // 파일 수신
                                    int receiveLength = buffer.Length;
                                    // 연결 끊김
                                    if (receiveLength == 0)
                                    {
                                        this.writer.Close();
                                        return;
                                    }
                                    this.writer.Write(buffer, 0, receiveLength);
                                    currentLength += receiveLength;
                                }

                                Thread.Sleep(10);

                                // 파일정보 송신
                                byte[] bufferInfo = new byte[1] { (int)ChatState.file_info };
                                bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(guid)).ToArray();  // Guid
                                bufferInfo = bufferInfo.Concat(BitConverter.GetBytes(Encoding.Default.GetBytes(fname).Length)).ToArray();  // 파일이름 길이
                                bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(fname)).ToArray();  // 파일이름
                                bufferInfo = bufferInfo.Concat(Encoding.Default.GetBytes(this.m_ClientSocket[ClientSocket])).ToArray(); // 유저이름
                                foreach (Socket tempSocket in this.m_ClientSocket.Keys)
                                {
                                    if (tempSocket != ClientSocket)
                                    {
                                        tempSocket.Send(bufferInfo, bufferInfo.Length, SocketFlags.None);
                                        Write_Status($"받은 파일 정보\"{fname}\"를 {this.m_ClientSocket[tempSocket]}에게 전송 완료", Brushes.Red);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Write_Status("file receive error >> " + ex.Message, Brushes.Red);
                            }
                            finally
                            {
                                writer.Close();
                            }
                            break;
                        }
                    case State.file_info:
                        {
                            // 파일명
                            string strFileName = Encoding.Default.GetString(buffer, 1, 32);

                            // 파일 크기 송신
                            FileInfo file = new FileInfo(SERVER_DIR + strFileName);
                            byte[] bufferSend = new byte[1] { (int)State.file };
                            bufferSend = bufferSend.Concat(BitConverter.GetBytes((int)file.Length)).ToArray();
                            ClientSocket.Send(bufferSend, bufferSend.Length, SocketFlags.None);

                            Thread.Sleep(100);

                            // 파일 송신
                            long count = file.Length / 1024 + 1;
                            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                BinaryReader reader = new BinaryReader(stream);
                                for (int i = 0; i < count; i++)
                                {
                                    bufferSend = reader.ReadBytes(1024);
                                    ClientSocket.Send(bufferSend);
                                }
                                Write_Status($"사용자 : {this.m_ClientSocket[ClientSocket]}  파일: {file.FullName} {file.Length:#,###}byte 전송 완료", Brushes.Blue);
                            }
                            break;
                        }

                }
            }
            catch (SocketException)
            {
                ClientSocket.Close();
                this.m_ClientSocket.Remove(ClientSocket);
                Write_Status($"클라이언트의 연결이 끊어졌습니다. 현재 접속중인 클라이언트 수 : {this.m_ClientSocket.Count}", Brushes.Red);
            }
            catch (Exception ex)
            {
                Write_Status("Receive_Completed Error => " + ex.Message, Brushes.Red);
            }
        }
    }
}

