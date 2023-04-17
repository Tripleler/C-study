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

namespace client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client = new TcpClient();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 9999);
                byte[] buf = Encoding.Default.GetBytes("클라이언트 : 접속합니다");

                client.GetStream().Write(buf, 0, buf.Length);

                // 파일 개수 받기
                //Thread.Sleep(100);
                //Array.Clear(buf, 0, buf.Length);
                //client.GetStream().Read(buf, 0, buf.Length);
                //int n = BitConverter.ToInt16(buf, 0);

                // 파일 명 받기
                Thread.Sleep(1000);
                buf = new byte[65536];
                client.GetStream().Read(buf, 0, buf.Length);
                string strData = Encoding.Default.GetString(buf);
                int len = strData.Length;
                strData = strData.Trim('\0');
                if (len == strData.Length)
                {
                    MessageBox.Show("파일리스트가 너무 많아 모두 불러오지 못했습니다.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                string[] strFilelist = strData.Split('\n');            
                Array.Clear(buf, 0, buf.Length);
                foreach (string fname in strFilelist)
                {
                    this.lbFilelist.Items.Add(fname);
                }
            }
            else Debug.Write("이미 연결됨");
        }        
    }
}
