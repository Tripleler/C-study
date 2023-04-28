using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using System.Xml;
using System.IO.Pipes;

namespace server
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private string rootdir = @"C:\test";
        private Socket m_ServerSocket;
        private List<Socket> m_ClientSocket;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Switch_Click(object sender, RoutedEventArgs e)
        {
            // 서버 시작
            if (this.btn_Switch.Content.ToString() == "Start")
            {
                this.m_ClientSocket = new List<Socket>();
                this.m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9998);
                this.m_ServerSocket.Bind(ipep);
                this.m_ServerSocket.Listen(1000);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
                m_ServerSocket.AcceptAsync(args);
                this.btn_Switch.Content = "Stop";
                Write_Status($"Server Start! using port : {ipep.Port}", Brushes.Black);
            }
            else
            {
                // 서버 종료
                //try
                //{                    
                //    foreach (Socket tempsocket in this.m_ClientSocket)
                //    {
                //        try
                //        {
                //            tempsocket.Close();
                //            tempsocket.Dispose();
                //        }
                //        catch (NullReferenceException)
                //        {

                //        }                        
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Write_Status("btn_Switch_Stop error >> " + ex.Message, Brushes.Red);
                //}
                //finally
                //{
                //    this.m_ServerSocket.Close();
                //    this.m_ServerSocket.Dispose();
                //    this.btn_Switch.Content = "Start";
                //}
                if (this.m_ServerSocket != null)
                {
                    this.m_ServerSocket.Close();
                    this.m_ServerSocket.Dispose();
                }
                Thread.Sleep(100);
                for (int i = 0; i < this.m_ClientSocket.Count; i++)
                {
                    this.m_ClientSocket[i].Close();
                    this.m_ClientSocket[i].Dispose();
                    Thread.Sleep(100);
                }
                this.m_ClientSocket.Clear();
                this.btn_Switch.Content = "Start";
            }
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;
            this.m_ClientSocket.Add(ClientSocket);
            if (this.m_ClientSocket != null && ClientSocket != null)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] buffer;
                buffer = new byte[1024];
                args.SetBuffer(buffer, 0, 1024);
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                ClientSocket.ReceiveAsync(args);
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
                        // ===================================
                        string check = Encoding.UTF8.GetString(buffer, 1, e.BytesTransferred - 1);
                        // ===================================
                        if (buffer[0] == 0)
                        {
                            string filelist = string.Join("\n", Directory.GetFiles(rootdir, "*", SearchOption.TopDirectoryOnly));
                            // 파일 리스트 크기 송신
                            int size = Encoding.Default.GetBytes(filelist).Length;
                            buffer = BitConverter.GetBytes(size);
                            Array.Resize(ref buffer, buffer.Length + 1);
                            Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
                            buffer[0] = 0;
                            ClientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), $"파일리스트 크기 정보 {buffer.Length}byte 송신 완료");

                            //Thread.Sleep(100);

                            // 파일 리스트 송신
                            buffer = Encoding.Default.GetBytes(filelist);
                            Array.Resize(ref buffer, buffer.Length + 1);
                            Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
                            buffer[0] = 1;
                            ClientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), $"파일리스트 {buffer.Length}byte 송신 완료");
                        }
                        else if (buffer[0] == 1)
                        {
                            // 파일명
                            string strFileName = Encoding.Default.GetString(buffer, 1, buffer.Length - 1);

                            // 파일 크기 송신
                            FileInfo file = new FileInfo(strFileName.Trim('\0'));
                            buffer = BitConverter.GetBytes(file.Length);
                            Array.Resize(ref buffer, buffer.Length + 1);
                            Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
                            buffer[0] = 2;
                            ClientSocket.Send(buffer, buffer.Length, SocketFlags.None);

                            Thread.Sleep(100);
                            
                            // 파일 송신
                            long count = file.Length / 1024 + 1;
                            using(var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                BinaryReader reader = new BinaryReader(stream);
                                for (int i = 0; i < count; i++)
                                {
                                    buffer = reader.ReadBytes(1024);
                                    ClientSocket.Send(buffer);
                                }
                                Write_Status($"클라이언트 : {ClientSocket.RemoteEndPoint}   파일: {file.FullName} {file.Length:#,###}byte 전송 완료", Brushes.Blue);
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
                    this.m_ClientSocket.Remove(ClientSocket);
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
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                Write_Status(msg, Brushes.Blue);
            }));
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Write_Status("서버시작 대기중", Brushes.Black);
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
    }
}
