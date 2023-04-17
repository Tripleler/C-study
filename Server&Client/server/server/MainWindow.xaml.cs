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

namespace server
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread p_th = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Server()
        {
            this.txt_Status.AppendText("서버콘솔창. \n\n\n");
            string rootdir = @"C:\test";
            string files = string.Join("\n", Directory.GetFiles(rootdir, "*", SearchOption.TopDirectoryOnly));
            p_th = new Thread(() =>
            {
                TcpListener server = new TcpListener(IPAddress.Any, 9999);
                server.Start();


                // 클라이언트 객체를 만들어 9999에 연결한 client를 받아온다
                TcpClient client = server.AcceptTcpClient();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    this.txt_Status.AppendText("클라이언트가 접속하였습니다.\n파일 리스트를 전송합니다.\n");
                }));

                // 파일리스트를 전송
                byte[] buf = Encoding.Default.GetBytes(files);
                client.GetStream().Write(buf, 0, buf.Length);

                //while (true)
                //{
                //    // Socket은 byte[] 형식으로 데이터를 주고받으므로 byte[]형 변수를 선언합니다.
                //    byte[] byteData = new byte[1024];
                //    // client가 write한 정보를 읽어옵니다.
                //    // 아래의 작업 이후에 byteData에는 읽어온 데이터가 들어갑니다.
                //    try  // exception 잡아야함
                //    {
                //        client.GetStream().Read(byteData, 0, byteData.Length);
                //    }
                //    catch (SocketException)
                //    {
                //        break;
                //    }

                //    // 출력을 위해 string형으로 바꿔줍니다.
                //    string strData = Encoding.Default.GetString(byteData);

                //    // byteData의 크기는 1024인데 스트림에서 읽어온 데이터가 1024보다 작은경우
                //    // 공백이 출력되니 비어있는 문자열을 제거합니다.
                //    int endPoint = strData.IndexOf('\0');
                //    string parsedMessage = strData.Substring(0, endPoint + 1);

                //    // 파싱된 데이터를 출력해주고 while루프를 돕니다.
                //    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                //    {
                //        this.txt_Status.AppendText(parsedMessage);
                //    }));  
                //}
            });
            p_th.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Server();
        }
    }
}
