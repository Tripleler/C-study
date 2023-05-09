using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Interop;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DevExpress.Internal;
using DevExpress.Pdf.Native;
using Microsoft.Win32;
using WebSocketSharp;
using WebSocketSharp.Server;
using DevExpress.XtraPrinting.BarCode.Native;

public enum ChatState
{
    Login,
    Login_Sucess,
    Login_Fail_None,
    Login_Fail_Duple,
    Register,
    Register_Sucess,
    Register_Fail,
    Info,
    Message,
    File_Info,
    File_Start,
    File,
}

namespace ChattingApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket clientSocket = null;
        private Socket cbSocket;
        private string HOST = "192.168.1.3";
        private int PORT = 9999;
        private string PORT_LOGIN = "9000";
        private byte[] bufferReceive = new byte[1024];

        private string id = string.Empty;
        private string pw = string.Empty;
        private bool IsLogin = false;

        private string fileName = string.Empty;
        private ucReceiveFile ucreceivefile = null;

        private bool p_bCtrlDown = false;

        private Client client;

        public MainWindow()
        {
            InitializeComponent();
        }

        // === Instant => 로그인이나 회원가입 체크용 ===========

        //private void InstantConnect()
        //{
        //    if (this.clientSocket == null || !this.clientSocket.Connected)
        //    {
        //        try
        //        {
        //            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //            this.clientSocket.BeginConnect(HOST, PORT, new AsyncCallback(InstantConnectCallBack), this.clientSocket);
        //        }
        //        catch (Exception ex)
        //        {
        //            Write_Status("InstantConnect Error >> " + ex.Message, Brushes.Red);
        //        }
        //    }
        //}

        //private void InstantConnectCallBack(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket tempSocket = ar.AsyncState as Socket;
        //        IPEndPoint ipep = tempSocket.RemoteEndPoint as IPEndPoint;
        //        Write_Status("서버 접속 성공! " + ipep.Address, Brushes.Black);
        //        tempSocket.EndConnect(ar);
        //        this.cbSocket = tempSocket;
        //        this.cbSocket.BeginReceive(this.bufferReceive, 0, this.bufferReceive.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), this.cbSocket);
        //        InstantSend();
        //    }
        //    catch (Exception ex)
        //    {
        //        Write_Status("InstantConnectCallBack Error >> " + ex.Message, Brushes.Red);
        //    }
        //}

        //private void InstantSend()
        //{
        //    byte[] buffer = Encoding.Default.GetBytes($"{this.id}\n{this.pw}");
        //    Array.Resize(ref buffer, buffer.Length + 1);
        //    Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
        //    string info = string.Empty;
        //    if (this.IsLogin)
        //    {
        //        buffer[0] = (int)LoginState.Login;
        //        info = $"{this.id} 로그인 시도";
        //    }
        //    else
        //    {
        //        buffer[0] = (int)LoginState.Register;
        //        info = $"{this.id} 회원가입 시도";
        //    }
        //    this.clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), info);
        //    this.id = string.Empty;
        //    this.pw = string.Empty;
        //}

        public void Connect()
        {
            string id = this.id;
            string pw = this.pw;
            byte[] buffer = Encoding.Default.GetBytes($"{this.id}\n{this.pw}");
            Array.Resize(ref buffer, buffer.Length + 1);
            Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
            buffer[0] = (int)ChatState.Login;
            this.client = new Client(this.HOST, this.PORT_LOGIN);
            this.client.evGetData += evGetData;
            this.client.SendData(buffer);
            this.id = string.Empty;
            this.pw = string.Empty;
        }

        public void evGetData(object sender)
        {
            if (sender != null)
            {
                byte[] buffer = sender as byte[];
                switch ((ChatState)buffer[0])
                {
                    case ChatState.Login_Sucess:  // 로그인 성공
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.lbl_ID.Content = this.txt_ID.Text.ToString();
                                this.txt_ID.Clear();
                                this.txt_PW.Clear();
                                this.lay_Login.Visibility = Visibility.Collapsed;
                                this.txt_send.Focus();
                            }));
                            Write_Status("로그인 성공", Brushes.Blue);
                            break;
                        }
                    case ChatState.Login_Fail_None:  // 로그인 실패 : 결과값 없음
                        {
                            try
                            {
                                this.client.ws.Close();
                                Write_Status("로그인 실패 : 결과값 없음", Brushes.Red);
                            }
                            catch (Exception ex)
                            {
                                Write_Status("LoginFail_None error >> " + ex.Message, Brushes.Red);
                            }
                            MessageBox.Show("가입되지 않은 ID 이거나 잘못된 비밀번호를 입력하셨습니다.", "LoginError", MessageBoxButton.OK, MessageBoxImage.Error);
                            this.client.ws.Close();
                            break;
                        }
                    case ChatState.Login_Fail_Duple:  // 로그인 실패 : 현재 접속중인 아이디
                        {
                            try
                            {
                                this.client.ws.Close();
                                Write_Status("로그인 실패 : 현재 접속중인 아이디", Brushes.Red);
                            }
                            catch (Exception ex)
                            {
                                Write_Status("LoginFail_duple error >> " + ex.Message, Brushes.Red);
                            }
                            MessageBox.Show("현재 접속중인 아이디입니다.", "LoginError", MessageBoxButton.OK, MessageBoxImage.Error);
                            this.client.ws.Close();
                            break;
                        }
                }
            }            
        }


        // === Instant End =========================


        private void Doconnect()
        {
            if (this.clientSocket == null || !this.clientSocket.Connected)
            {
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.BeginConnect();
            }
            else
            {
                Write_Status("이미 서버와 연결되어 있습니다.", Brushes.Red);
            }
        }

        private void BeginConnect()
        {
            Write_Status("서버 접속 대기 중", Brushes.Black);
            try
            {
                this.clientSocket.BeginConnect(HOST, PORT, new AsyncCallback(ConnectCallBack), this.clientSocket);
            }
            catch (SocketException e)
            {
                Write_Status("서버 접속에 실패하였습니다." + e.NativeErrorCode, Brushes.Red);
                Doconnect();
            }
            catch (InvalidOperationException)
            {
                Doconnect();
            }
            catch (Exception e)
            {
                Write_Status("BeginConnect error >> " + e.Message, Brushes.Red);
                Doconnect();
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = ar.AsyncState as Socket;
                IPEndPoint ipep = tempSocket.RemoteEndPoint as IPEndPoint;
                Write_Status("서버 접속 성공! " + ipep.Address, Brushes.Black);
                tempSocket.EndConnect(ar);
                this.cbSocket = tempSocket;
                this.cbSocket.BeginReceive(this.bufferReceive, 0, this.bufferReceive.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), this.cbSocket);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.NotConnected)
                {
                    BeginConnect();
                }
            }
        }

        private void OnReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = ar.AsyncState as Socket;
                int nReadSize = tempSocket.EndReceive(ar);
                if (nReadSize > 0 && this.bufferReceive != null)
                {
                    switch ((ChatState)this.bufferReceive[0])
                    {
                        //case State.Login_Sucess:  // 로그인 성공
                        //    {
                        //        this.Dispatcher.BeginInvoke(new Action(() =>
                        //        {
                        //            this.lbl_ID.Content = this.txt_ID.Text.ToString();
                        //            this.txt_ID.Clear();
                        //            this.txt_PW.Clear();
                        //            this.lay_Login.Visibility = Visibility.Collapsed;
                        //            this.txt_send.Focus();
                        //        }));
                        //        Write_Status("로그인 성공", Brushes.Blue);
                        //        break;
                        //    }
                        //case State.Login_Fail_None:  // 로그인 실패 : 결과값 없음
                        //    {
                        //        try
                        //        {
                        //            this.clientSocket.Close();
                        //            this.clientSocket.Dispose();
                        //            Write_Status("로그인 실패 : 결과값 없음", Brushes.Red);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Write_Status("LoginFail_None error >> " + ex.Message, Brushes.Red);
                        //        }
                        //        MessageBox.Show("가입되지 않은 ID 이거나 잘못된 비밀번호를 입력하셨습니다.", "LoginError", MessageBoxButton.OK, MessageBoxImage.Error);
                        //        break;
                        //    }
                        //case State.Login_Fail_duple:  // 로그인 실패 : 현재 접속중인 아이디
                        //    {
                        //        try
                        //        {
                        //            this.clientSocket.Close();
                        //            this.clientSocket.Dispose();
                        //            Write_Status("로그인 실패 : 현재 접속중인 아이디", Brushes.Red);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Write_Status("LoginFail_duple error >> " + ex.Message, Brushes.Red);
                        //        }
                        //        MessageBox.Show("현재 접속중인 아이디입니다.", "LoginError", MessageBoxButton.OK, MessageBoxImage.Error);
                        //        break;
                        //    }
                        //case State.Register_Sucess:  // 회원가입 성공
                        //    {
                        //        try
                        //        {
                        //            this.clientSocket.Close();
                        //            this.clientSocket.Dispose();
                        //            Write_Status("회원가입 성공", Brushes.Blue);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Write_Status("Register sucess error >> " + ex.Message, Brushes.Red);
                        //        }
                        //        MessageBox.Show("회원가입 성공! 로그인 화면으로 돌아갑니다.", "RegisterSucess", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        this.Dispatcher.BeginInvoke(new Action(() =>
                        //        {
                        //            this.lay_Register.Visibility = Visibility.Collapsed;
                        //        }));
                        //        break;
                        //    }
                        //case State.Register_Fail:  // 회원가입 실패
                        //    {
                        //        try
                        //        {
                        //            this.clientSocket.Close();
                        //            this.clientSocket.Dispose();
                        //            Write_Status("회원가입 실패", Brushes.Red);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Write_Status("Register fail error >> " + ex.Message, Brushes.Red);
                        //        }
                        //        MessageBox.Show("이미 사용중인 아이디입니다.", "RegisterError", MessageBoxButton.OK, MessageBoxImage.Error);
                        //        break;
                        //    }
                        case ChatState.Info:  // 입장 메시지
                            {
                                string strData = Encoding.Default.GetString(this.bufferReceive, 1, nReadSize - 1);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ucInfo ucinfo = new ucInfo();
                                    ucinfo.Enter_Text(strData);
                                    this.stpnl_chat.Children.Add(ucinfo);
                                }));
                                break;
                            }
                        case ChatState.Message:  // 메시지 수신
                            {
                                int iTxtSize = BitConverter.ToInt32(this.bufferReceive, 1);  // 메시지 크기
                                string user = Encoding.Default.GetString(this.bufferReceive, 5, nReadSize - 5);
                                byte[] bData = new byte[iTxtSize];
                                int currentLength = 0;
                                while (currentLength < iTxtSize)
                                {
                                    int receiveLength = this.clientSocket.Receive(this.bufferReceive);
                                    Array.Copy(this.bufferReceive, 0, bData, currentLength, receiveLength);
                                    currentLength += receiveLength;
                                }
                                string strData = Encoding.Default.GetString(bData);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ucReceive uc = new ucReceive();
                                    uc.EnterText(strData, user);
                                    if (this.stpnl_chat.Children.Count > 0)
                                    {
                                        var child_bubble = this.stpnl_chat.Children[this.stpnl_chat.Children.Count - 1];
                                        if (child_bubble.GetType() == typeof(ucReceive))
                                        {
                                            ucReceive ucLast = child_bubble as ucReceive;
                                            if (ucLast.GetTime() == DateTime.Now.ToString("tt h:mm") && ucLast.GetUser() == user)
                                            {
                                                ucLast.RemoveTime();
                                                uc.RemoveUser();
                                            }
                                        }
                                    }
                                    this.stpnl_chat.Children.Add(uc);
                                    this.scroll.ScrollToEnd();
                                }));
                                break;
                            }
                        case ChatState.file_info:  // 파일 이름과 송신자
                            {
                                string guid = Encoding.Default.GetString(this.bufferReceive, 1, 32);  //guid
                                int iSize = BitConverter.ToInt32(this.bufferReceive, 33);
                                string msg = Encoding.Default.GetString(this.bufferReceive, 37, iSize);
                                string user = Encoding.Default.GetString(this.bufferReceive, 37 + iSize, nReadSize - 37 - iSize);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ucReceiveFile uc = new ucReceiveFile();
                                    uc.Tag = guid;
                                    uc.EnterText(msg, user);
                                    uc.evDownloadClick += evDownLoadClick;
                                    if (this.stpnl_chat.Children.Count > 0)
                                    {
                                        var child_bubble = this.stpnl_chat.Children[this.stpnl_chat.Children.Count - 1];
                                        if (child_bubble.GetType() == typeof(ucReceiveFile))
                                        {
                                            ucReceiveFile ucLast = child_bubble as ucReceiveFile;
                                            if (ucLast.GetTime() == DateTime.Now.ToString("tt h:mm") && ucLast.GetUser() == user)
                                            {
                                                ucLast.RemoveTime();
                                                uc.RemoveUser();
                                            }
                                        }
                                    }
                                    this.stpnl_chat.Children.Add(uc);
                                    this.scroll.ScrollToEnd();
                                }));
                                break;
                            }
                        case ChatState.file:
                            {
                                string fname = this.fileName;
                                double filesize = BitConverter.ToInt32(this.bufferReceive, 1);
                                Write_Status($"파일 크기 정보 {filesize:#,###}byte 수신 완료", Brushes.Blue);
                                this.bufferReceive = new byte[1024];
                                long currentLength = 0;
                                FileStream fileStream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.Write);
                                BinaryWriter writer = new BinaryWriter(fileStream);
                                ucReceiveFile uc = this.ucreceivefile;
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    uc.ShowProgress();
                                }));
                                int iProgress = 0;
                                try
                                {
                                    while (currentLength < filesize)
                                    {
                                        // 파일 수신
                                        int receiveLength = this.clientSocket.Receive(this.bufferReceive);
                                        // 연결 끊김
                                        if (receiveLength == 0)
                                        {
                                            writer.Close();
                                            return;
                                        }
                                        writer.Write(this.bufferReceive, 0, receiveLength);
                                        currentLength += receiveLength;
                                        int k = (int)(currentLength / filesize * 100);
                                        if (k > iProgress)
                                        {
                                            iProgress = k;
                                            this.Dispatcher.BeginInvoke(new Action(() =>
                                            {
                                                uc.UpdateProgress(iProgress);
                                            }));
                                        }
                                    }
                                    writer.Close();
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        uc.RemoveProgress();
                                    }));
                                    Write_Status($"파일 다운로드 완료.\n저장위치 : {fname}", Brushes.Blue);
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
                    }
                }
                if (this.cbSocket.Connected)
                {
                    this.cbSocket.BeginReceive(this.bufferReceive, 0, this.bufferReceive.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), this.cbSocket);
                }
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.ConnectionReset)
                {
                    this.BeginConnect();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void BeginSend(byte[] SendBuffer, string info)
        {
            if (SendBuffer == null || SendBuffer.Length <= 0)
                return;
            try
            {
                if (this.clientSocket != null && this.clientSocket.Connected)
                {
                    this.clientSocket.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), info);
                }
            }
            catch (SocketException e)
            {
                Write_Status("BeginSend error >> " + e.Message, Brushes.Red);
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            string msg = ar.AsyncState.ToString();
            Write_Status(msg, Brushes.Blue);
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            this.id = this.txt_ID.Text;
            this.pw = this.txt_PW.Text;
            if (this.id == string.Empty)
            {
                MessageBox.Show("ID를 입력하세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (this.pw == string.Empty)
            {
                MessageBox.Show("비밀번호를 입력하세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                this.IsLogin = true;
                //InstantConnect();
                Connect();
            }
        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            string msg = this.txt_send.Text;
            if (msg != string.Empty)
            {
                byte[] bufferSend = Encoding.Default.GetBytes(msg);
                Array.Resize(ref bufferSend, bufferSend.Length + 1);
                Array.Copy(bufferSend, 0, bufferSend, 1, bufferSend.Length - 1);
                bufferSend[0] = (int)ChatState.Message;
                BeginSend(bufferSend, $" 다음 메시지를 서버에 전송 \"{msg}\"");
                if (this.stpnl_chat.Children.Count > 0)
                {
                    var child_bubble = this.stpnl_chat.Children[this.stpnl_chat.Children.Count - 1];
                    if (child_bubble.GetType() == typeof(ucSend))
                    {
                        ucSend ucLast = child_bubble as ucSend;
                        if (ucLast.GetTime() == DateTime.Now.ToString("tt h:mm"))
                        {
                            ucLast.RemoveTime();
                        }
                    }
                }
                ucSend uc = new ucSend();
                uc.EnterText(msg);
                this.stpnl_chat.Children.Add(uc);
                this.scroll.ScrollToEnd();
                this.txt_send.Clear();
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

        private void Show_Status()
        {
            if (this.lay_Status.Visibility == Visibility.Visible)
                this.lay_Status.Visibility = Visibility.Collapsed;
            else
                this.lay_Status.Visibility = Visibility.Visible;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.L))
                Show_Status();
            else if (e.Key == Key.Escape)
            {
                if (this.lay_Register.Visibility == Visibility.Visible)
                {
                    btn_Back_Click(null, null);
                    this.txt_ID.Focus();
                }
                else if (this.lay_Login.Visibility == Visibility.Collapsed)
                {
                    btn_Logout_Click(null, null);
                    this.txt_ID.Focus();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Doconnect();
            this.bd_chat.Drop += bd_chat_Drop;
            this.bd_chat.PreviewDragOver += bd_chat_PreviewDragOver;
            this.txt_ID.Focus();
            //test();
        }

        private void test()
        {
            try
            {
                ManualResetEvent evFin = new ManualResetEvent(false);
                var ws = new WebSocket("ws://" + HOST + ":" + 9000 + "/Server2");
                int nTimeout = 1000;

                ws.EmitOnPing = true;
                //ws.Compression = CompressionMethod.Deflate;
                //ws.SslConfiguration.ServerCertificateValidationCallback = (ss, certificate, chain, sslPolicyErrors) => { return true; };

                ws.OnClose += (ss, ee) =>
                {
                    //evFin.Set();
                };
                ws.OnError += (ss, ee) =>
                {
                    //strMessage = ee.Message;
                };
                ws.OnMessage += (ss, ee) =>
                {
                    if (ee.IsText == true)
                    {
                        //htRecv.SetStringData(ee.Data);
                        MessageBox.Show("Recv   " + ee.Data);
                        ws.Close();
                    }
                    //evFin.Set();
                };
                ws.OnOpen += (ss, ee) =>
                {
                    ws.Send("Send");
                    Debug.WriteLine("Open complete");
                    //ws.Send(htSend.GetStringData());
                    //ws.Send("Password=YTA4NWM3OTdhNDNhYzQyYjZmOGRkMzUwZmQ2ZGYxNTUwNWEyYmZlY2QwM2Q5Y2JiYTMxMGM3NGUxM2M3ZDI3MzhhNjA1ZTg1Yjk5MGFlYjBkY2FkZDQ1NDc0ZTJiMDI4YTI5MzcyNWQ3NmFkZTY0ZmQ1OWE1MDM3N2E0OTljMmI=MobileID = MDljYjRhZmEtODdiMy00OTRkLWFlZjYtZjEyOTdmMzE4NTA2Command = X19BQ0NPVU5UUENMSVNUUserID = bGpoODM5MEBzeW1hdGlvbi5jby5rcg ==PushToken = ");
                };
                ws.Connect();



                if (evFin.WaitOne(nTimeout) == false)
                {

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            this.id = this.txt_ID_Register.Text;

            if (this.id == string.Empty)
            {
                MessageBox.Show("아이디를 입력하세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this.id != Regex.Replace(id, @"[^a-zA-Z0-9가-힣]", ""))
            {
                MessageBox.Show("아이디는 숫자, 한글 또는 영문자만 가능합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.pw = this.txt_PW_Register.Password;

            if (this.pw == string.Empty)
            {
                MessageBox.Show("비밀번호를 입력하세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this.pw != Regex.Replace(pw, @"[^a-zA-Z0-9가-힣]", ""))
            {
                MessageBox.Show("비밀번호는 숫자, 한글 또는 영문자만 가능합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this.pw != this.txt_PW_Register_Check.Text)
            {
                MessageBox.Show("입력하신 비밀번호와 비밀번호 확인 문구가 서로 다릅니다", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] buffer = Encoding.Default.GetBytes($"{this.id}\n{this.pw}");
            Array.Resize(ref buffer, buffer.Length + 1);
            Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
            buffer[0] = (int)LoginState.Register;
            this.IsLogin = false;
            //InstantConnect();
        }

        private void btn_Register_Login_Click(object sender, RoutedEventArgs e)
        {
            this.lay_Register.Visibility = Visibility.Visible;
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.lay_Register.Visibility = Visibility.Collapsed;
        }

        private void btn_Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("로그아웃 하시겠습니까?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                this.lay_Login.Visibility = Visibility.Visible;
                this.stpnl_chat.Children.Clear();
                try
                {
                    this.clientSocket.Close();
                    this.clientSocket.Dispose();
                    Write_Status($"로그아웃", Brushes.Blue);
                }
                catch (Exception ex)
                {
                    Write_Status("btn_Logout error >> " + ex.Message, Brushes.Red);
                }
            }
        }

        private void txt_send_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    this.txt_send.AppendText("\n");
                    this.txt_send.Select(this.txt_send.Text.Length, 0);
                    this.txt_send.ScrollToEnd();
                }
                else
                {
                    btn_send_Click(null, null);
                }
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                this.p_bCtrlDown = true;
            }

            //if (e.Key == Key.V)
            //{
            //    if (this.p_bCtrlDown)
            //    {
            //        if (Clipboard.GetDataObject() != null)
            //        {
            //            try
            //            {
            //                Button btn = new Button();
            //                btn.Content = new Image { Source = Clipboard.GetImage() };
            //                this.stpnl_chat.Children.Add(btn);
            //            }
            //            catch (Exception)
            //            {
            //                return;
            //            }
            //        }
            //    }
            //}
        }

        private void txt_send_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                p_bCtrlDown = false;
            }
        }

        private void txt_PW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_Login_Click(null, null);
                this.txt_send.Focus();
            }
        }

        private void bd_chat_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void bd_chat_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var result = MessageBox.Show("다음 파일들을 전송합니다\n" + string.Join("\n", files), "파일 전송", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
                if (result == MessageBoxResult.OK)
                {
                    new Task(new Action(() =>
                    {
                        foreach (string fname in files)
                        {
                            FileInfo file = new FileInfo(fname);
                            if (file.Length > int.MaxValue) return;
                            byte[] buffer = new byte[1] { (int)ChatState.file };
                            buffer = buffer.Concat(BitConverter.GetBytes((int)file.Length)).ToArray();
                            buffer = buffer.Concat(Encoding.Default.GetBytes(System.IO.Path.GetFileName(fname))).ToArray();
                            this.clientSocket.Send(buffer);

                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ucSendFile uc = new ucSendFile();
                                uc.Enter_text(System.IO.Path.GetFileName(fname));
                                if (this.stpnl_chat.Children.Count > 0)
                                {
                                    var child_bubble = this.stpnl_chat.Children[this.stpnl_chat.Children.Count - 1];
                                    if (child_bubble.GetType() == typeof(ucSendFile))
                                    {
                                        ucSendFile ucLast = child_bubble as ucSendFile;
                                        if (ucLast.GetTime() == DateTime.Now.ToString("tt h:mm"))
                                        {
                                            ucLast.RemoveTime();
                                        }
                                    }
                                }
                                this.stpnl_chat.Children.Add(uc);
                            }));

                            long count = file.Length / 1024 + 1;
                            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                BinaryReader reader = new BinaryReader(stream);
                                int iProgress = 0;
                                buffer = new byte[1024];
                                for (double i = 0; i < count; i++)
                                {
                                    int k = (int)(i / count * 100);
                                    if (k > iProgress)
                                    {
                                        iProgress = k;
                                        this.Dispatcher.Invoke(new Action(() =>
                                        {
                                            ucSendFile uc = this.stpnl_chat.Children[this.stpnl_chat.Children.Count - 1] as ucSendFile;
                                            uc.pb_file.Value = iProgress;
                                        }));
                                    }
                                    buffer = reader.ReadBytes(1024);
                                    this.clientSocket.Send(buffer);
                                }
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ucSendFile uc = this.stpnl_chat.Children[this.stpnl_chat.Children.Count - 1] as ucSendFile;
                                    uc.Remove_Progress();
                                }));
                                reader.Close();
                            }

                            Thread.Sleep(100);
                        }
                    })).Start();
                }
            }
        }

        private void evDownLoadClick(object sender)
        {
            if (sender != null)
            {
                ucReceiveFile uc = sender as ucReceiveFile;
                if (uc != null)
                {
                    string fname = uc.GetText();
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = @"C:";
                    saveFileDialog.FileName = fname;
                    saveFileDialog.Title = "저장";
                    saveFileDialog.DefaultExt = System.IO.Path.GetExtension(fname);
                    saveFileDialog.Filter = "모든 파일|*.*";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        this.ucreceivefile = uc;
                        this.fileName = saveFileDialog.FileName;
                        byte[] buffer = new byte[1] { (int)ChatState.file_info };
                        buffer = buffer.Concat(Encoding.Default.GetBytes(uc.Tag.ToString())).ToArray();
                        this.clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), $"파일 : {fname} 송신 요청 완료");
                    }
                }
            }
        }
    }
    public class Client
    {
        public delegate void GetData(object sender);
        public event GetData evGetData;

        public WebSocket ws;

        public Client(string HOST, string PORT)
        {
            this.ws = new WebSocket("ws://" + HOST + ":" + PORT + "/Login");
            this.ws.OnMessage += onMessage;
            this.ws.Connect();
        }        

        public void SendData(byte[] data)
        {
            if (this.ws.IsAlive)
            {
                ws.SendAsync(data, null);
            }
            else
            {
                Debug.WriteLine("서버 연결 안됨");
            }
        }

        public void onMessage(object sender, MessageEventArgs e)
        {
            evGetData(e.RawData);
        }      
    }
}
