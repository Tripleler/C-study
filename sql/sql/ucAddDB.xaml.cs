using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using DevExpress.Utils.Drawing;

namespace sql
{
    /// <summary>
    /// ucAddDB.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frmAddDB : Window
    {
        public string m_strFilePath {  get; set; }
        private bool bDuplicate = false;
        
        public frmAddDB()
        {
            InitializeComponent();
        }

        private void btnOpenDir(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "DataBase file";
            ofd.Filter = "db files (*.db)|*.db";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txt_dir.Text = ofd.FileName;  // File 텍스트박스                
                this.txt_DBname.Text = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);  // Name 텍스트박스
                this.m_strFilePath = this.txt_dir.Text;
                Check_DB_Duplicate();
            }            
        }

        private void Check_DB_Duplicate()
        {
            if (((MainWindow)App.Current.MainWindow).lstDBDirs.Contains(this.m_strFilePath))
            {                
                this.img_OK.Visibility = Visibility.Hidden;
                this.img_NO.Visibility = Visibility.Visible;
                bDuplicate = true;
                System.Windows.Forms.MessageBox.Show($"{System.IO.Path.GetFileName(this.m_strFilePath)} is alreay Connected");
            }
            else
            {
                this.img_OK.Visibility = Visibility.Visible;
                this.img_NO.Visibility = Visibility.Hidden;
                bDuplicate = false;
            }
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (bDuplicate)
            {
                System.Windows.Forms.MessageBox.Show($"{System.IO.Path.GetFileName(this.m_strFilePath)} is alreay Connected");
            }
            else if (this.m_strFilePath == "" || this.m_strFilePath == null)
            {
                System.Windows.Forms.MessageBox.Show("File not selected");
            }
            else if (System.IO.Path.GetExtension(this.m_strFilePath) != ".db")
            {
                System.Windows.Forms.MessageBox.Show($"Cannot open file {this.m_strFilePath}");
            }
            else
            {
                DialogResult = true;
                this.Close();
            }            
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
