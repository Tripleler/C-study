using System;
using System.Collections.Generic;
using System.IO;
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

using System.Data.SQLite;
using System.Data;

namespace sql
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection m_dbcon = null;

        public MainWindow()
        {
            InitializeComponent();
            //HierarchicalDataTemplate template = new HierarchicalDataTemplate(typeof(Folder));

            //template.ItemsSource = new Binding("ChildFolderList");

            //FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));

            //textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Name"));

            //template.VisualTree = textBlockFactory;

            //Folder folder = new Folder(new DirectoryInfo(System.IO.Path.GetPathRoot(Environment.SystemDirectory)));

            //TreeViewItem item = new TreeViewItem();

            //item.Header = folder.Name;
            //item.ItemsSource = folder.ChildFolderList;
            //item.ItemTemplate = template;

            //this.treeView.Items.Add(item);

            //item.IsExpanded = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {     
            string dbPath = @"D:\바탕 화면\requestlog.db";
            string dbConnection = string.Format("Data Source={0};", dbPath);
            m_dbcon = new SQLiteConnection(dbConnection);
            m_dbcon.Open();
            DataTable dt = null;

            // 쿼리 실행
            string qeury = "SELECT * FROM OCR_LOG";
            SQLiteCommand sqlCmd = new SQLiteCommand(qeury, m_dbcon);
            SQLiteDataReader reader = sqlCmd.ExecuteReader();
            dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            m_dbcon.Close();

            // DataGrid 에 바인딩
            dataGrid1.ItemsSource = dt.DefaultView;         
        }
    }
}
