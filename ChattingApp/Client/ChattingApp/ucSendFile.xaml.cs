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
    /// ucSendFile.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucSendFile : UserControl
    {
        public ucSendFile()
        {
            InitializeComponent();
        }

        public void Enter_text(string fname)
        {
            this.txt_file.Text = fname;            
        }

        public void RemoveTime()
        {
            this.border.Margin = new Thickness(0, 10, 10, 0);
            this.txt_time.Visibility = Visibility.Hidden;
        }

        public void Remove_Progress()
        {
            this.pb_file.Visibility = Visibility.Collapsed;
            this.txt_time.Visibility = Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.txt_time.Text = DateTime.Now.ToString("tt h:mm");
        }

        public string GetTime()
        {
            return this.txt_time.Text;
        }
    }
}
