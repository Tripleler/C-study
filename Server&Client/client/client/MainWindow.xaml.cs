using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using System.Net.WebSockets;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.ComponentModel;

namespace client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket clientSocket = null;
        private Socket cbSocket;
        private string strDir = null;
        private string strFileName = null;
        private string HOST = "192.168.1.24";
        private int PORT = 9999;
        private byte[] bufferReceive = new byte[1024];
        private int nLoop = 0;
        private bool isInfinitDownload = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Doconnect();
        }

        private void Doconnect()
        {
            if (this.clientSocket == null || !this.clientSocket.Connected)
            {
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.clientSocket.ReceiveTimeout = 5000;
                this.clientSocket.SendTimeout = 5000;
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
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Title = "Client";
                }));
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
                get_File_List();
                tempSocket.EndConnect(ar);
                this.cbSocket = tempSocket;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Title = $"Client {this.cbSocket.LocalEndPoint}";
                }));
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
                if (nReadSize > 0)
                {
                    if (bufferReceive != null)
                    {
                        // 파일리스트 크기 수신
                        if (bufferReceive[0] == 0)
                        {
                            int n = BitConverter.ToInt32(this.bufferReceive, 1);
                            this.bufferReceive = new byte[n + 1];
                        }

                        // 파일리스트 수신
                        else if (this.bufferReceive[0] == 1)
                        {
                            string strData = Encoding.Default.GetString(this.bufferReceive, 1, this.bufferReceive.Length - 1);
                            string[] strFilelist = strData.Split('\n');
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.lbFilelist.Items.Clear();
                            }));
                            foreach (string fname in strFilelist)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    this.lbFilelist.Items.Add(fname);
                                }));
                            }
                            Write_Status("파일리스트 갱신 완료", Brushes.Blue);
                            this.bufferReceive = new byte[1024];
                        }

                        // 파일
                        else if (this.bufferReceive[0] == 2)
                        {
                            // 파일 크기 수신
                            double filesize = BitConverter.ToInt64(this.bufferReceive, 1);
                            Write_Status($"파일 크기 정보 {filesize:#,###}byte 수신 완료", Brushes.Blue);                            
                            this.bufferReceive = new byte[1024];
                            long currentLength = 0;
                            string strfName = null;
                            if (this.isInfinitDownload)
                            {
                                nLoop++;
                                strfName = "C:\\Users\\Symation\\Desktop\\ttt\\" + Process.GetCurrentProcess().Id.ToString() + "_" + nLoop.ToString();
                            }
                            else
                            {
                                strfName = this.strDir + @"\\" + System.IO.Path.GetFileName(this.strFileName);
                                try
                                {
                                    FileStream stream = new FileStream(strfName, FileMode.Create);
                                }
                                catch (ArgumentException)
                                {
                                    this.cbSocket.BeginReceive(this.bufferReceive, 0, this.bufferReceive.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), this.cbSocket);
                                    return;
                                }
                            }
                            FileStream fileStream = new FileStream(strfName, FileMode.Create, FileAccess.Write, FileShare.Write);
                            BinaryWriter writer = new BinaryWriter(fileStream);
                            try
                            {
                                int iProgress = 0;
                                while (currentLength < filesize)
                                {
                                    // 파일 수신
                                    int receiveLength = this.clientSocket.Receive(this.bufferReceive);
                                    // 연결 끊김
                                    if (receiveLength == 0)
                                    {
                                        writer.Close();                                        
                                        this.clientSocket.Dispose();
                                        this.clientSocket.Close();
                                        this.Dispatcher.Invoke(new Action(() =>
                                        {
                                            this.btnDirectDown.Content = "Inf download";
                                            this.btn_Refresh.IsEnabled = true;
                                            this.lbFilelist.IsEnabled = true;
                                        }));
                                        Thread.Sleep(500);
                                        BeginConnect();
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
                                            this.pb_Download.Value = iProgress;
                                        }));
                                    }
                                }
                                writer.Close();
                                Write_Status("파일 다운로드 완료", Brushes.Blue);
                                if (this.isInfinitDownload)
                                {
                                    Thread.Sleep(100);
                                    byte[] bufferSend = Encoding.Default.GetBytes(this.strFileName);
                                    Array.Resize(ref bufferSend, bufferSend.Length + 1);
                                    Array.Copy(bufferSend, 0, bufferSend, 1, bufferSend.Length - 1);
                                    bufferSend[0] = 1;
                                    BeginSend(bufferSend, this.strFileName + "송신 요청 완료");
                                }
                            }
                            catch (SocketException)
                            {
                                BeginConnect();
                            }
                            catch (Exception ex)
                            {
                                Write_Status("txtWriter error >> " + ex.Message, Brushes.Red);
                                BeginConnect();
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }
                        // 파일 수신
                        else if (this.bufferReceive[0] == 3)
                        {
                            string strFileName = $"{this.strDir}\\{System.IO.Path.GetFileName(this.strFileName)}"; // === Version 1 ==================

                            // === Version 0 =======================
                            //string strFileName = $"{this.strDir}\\"+ Guid.NewGuid().ToString().Replace("-", "").Trim();                            
                            //string strfName = "C:\\Users\\Symation\\Desktop\\ttt\\" + Process.GetCurrentProcess().Id.ToString() + "_" + nLoop.ToString();
                            // === Version 0 End ======================

                            using (var stream = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                            {
                                stream.Write(this.bufferReceive, 1, this.bufferReceive.Length - 1);
                            }
                            Write_Status($"파일 : {strFileName} {this.bufferReceive.Length}byte 수신 완료", Brushes.Blue);
                            this.bufferReceive = new byte[1024];
                        }
                    }
                }
                this.cbSocket.BeginReceive(this.bufferReceive, 0, this.bufferReceive.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), this.cbSocket);  // === Version 0 =========
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.ConnectionReset)
                {
                    this.BeginConnect();
                }
            }
        }

        private void BeginSend(byte[] SendBuffer, string info)
        {
            if (SendBuffer == null || SendBuffer.Length <= 0)
                return;
            try
            {
                if (this.clientSocket.Connected)
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


        private void lbItem_DoubleClick(object sender, MouseEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item != null)
            {
                MessageBoxResult msgRst = MessageBox.Show($"{item.Content} 파일을 다운로드 하시겠습니까?", "DownLoad", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRst == MessageBoxResult.Yes)
                {
                    this.strFileName = item.Content.ToString();
                    DownloadFile();
                }
            }
        }

        private void DownloadFile()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult rst = dialog.ShowDialog();
            if (rst == System.Windows.Forms.DialogResult.OK)
            {
                // 파일 이름 송신
                this.strDir = dialog.SelectedPath;
                byte[] bufferSend = Encoding.Default.GetBytes(this.strFileName);
                Array.Resize(ref bufferSend, bufferSend.Length + 1);
                Array.Copy(bufferSend, 0, bufferSend, 1, bufferSend.Length - 1);
                bufferSend[0] = 1;
                BeginSend(bufferSend, "다운로드 할 파일 이름 송신 완료");
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

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            this.btn_Refresh.IsEnabled = false;
            get_File_List();
            Thread.Sleep(100);
            this.btn_Refresh.IsEnabled = true;
        }

        private void get_File_List()
        {
            // 파일리스트 전송 요청
            byte[] bufferSend = new byte[1] { 0 };
            BeginSend(bufferSend, "파일리스트 전송 요청 완료");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Write_Status("서버와 연결하려면 connect 버튼을 누르세요", Brushes.Black);  // === Version 0 ========
            Doconnect();  // === Version 1 ===========
        }

        private void btnDirectDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.clientSocket.Connected)
            {
                if (this.btnDirectDown.Content.ToString() == "Stop")
                {
                    this.btnDirectDown.Content = "Inf download";
                    this.btn_Refresh.IsEnabled = true;
                    this.lbFilelist.IsEnabled = true;
                    isInfinitDownload = false;
                }
                else
                {
                    //this.strDir = AppDomain.CurrentDomain.BaseDirectory;  // === Version 0 ============
                    this.strFileName = "C:\\test\\adda.txt";
                    byte[] bufferSend = Encoding.Default.GetBytes(this.strFileName);
                    Array.Resize(ref bufferSend, bufferSend.Length + 1);
                    Array.Copy(bufferSend, 0, bufferSend, 1, bufferSend.Length - 1);
                    bufferSend[0] = 1;
                    BeginSend(bufferSend, this.strFileName + "송신 요청 완료");
                    this.isInfinitDownload = true;
                    this.btnDirectDown.Content = "Stop";
                    this.btn_Refresh.IsEnabled = false;
                    this.lbFilelist.IsEnabled = false;
                }
            }            
        }
    }
}
