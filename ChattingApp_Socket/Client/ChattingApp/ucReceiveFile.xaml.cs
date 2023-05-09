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

namespace ChattingApp
{
    /// <summary>
    /// ucReceiveFile.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucReceiveFile : UserControl
    {
        public delegate void DownloadClick(object sender);
        public event DownloadClick evDownloadClick;

        public ucReceiveFile()
        {
            InitializeComponent();
        }

        public void EnterText(string text, string user)
        {
            this.txt_msg.Text = text;
            this.txt_user.Text = user;
            this.txt_time.Text = DateTime.Now.ToString("tt h:mm");
        }

        public void RemoveTime()
        {
            this.border.Margin = new Thickness(10, 10, 0, 0);
            this.txt_time.Visibility = Visibility.Hidden;
        }

        public string GetTime()
        {
            return this.txt_time.Text;
        }

        public string GetUser()
        {
            return this.txt_user.Text;
        }

        public string GetText()
        {
            return this.txt_msg.Text;
        }

        public void RemoveUser()
        {
            this.txt_user.Visibility = Visibility.Collapsed;
        }

        private void btn_download_Click(object sender, RoutedEventArgs e)
        {
            if (this.evDownloadClick != null)
            {
                evDownloadClick(this);
            }
        }

        public void UpdateProgress(int value)
        {
            this.pb_file.Value = value;
        }

        public void ShowProgress()
        {
            this.pb_file.Visibility = Visibility.Visible;
        }

        public void RemoveProgress()
        {
            this.pb_file.Visibility = Visibility.Collapsed;
        }
    }    
}
