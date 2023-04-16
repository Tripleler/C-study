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
using System.Diagnostics;
using System.Collections;
using System.Data.Common;
using DevExpress.Xpo.DB.Helpers;
using ICSharpCode.AvalonEdit;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.AvalonEdit.Highlighting;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace sql
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection m_dbcon = null;
        public List<string> lstDBDirs = new List<string>();
        Thread p_th = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txt_Query.Options.HighlightCurrentLine = true;
            this.txt_Query.ShowLineNumbers = true;
            this.txt_Query.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("TSQL");            
        }

        private void menuAddDB_Click(object sender, RoutedEventArgs e)
        {
            DataBase_Add_Widget();
        }

        private void DataBase_Add_Widget()
        {
            frmAddDB addWidget = new frmAddDB() { Owner = this };
            if (addWidget.ShowDialog() == true)
            {
                string dbPath = addWidget.m_strFilePath;
                if (lstDBDirs.Contains(dbPath))
                {
                    MessageBox.Show($"{System.IO.Path.GetFileName(dbPath)} is alreay Connected");
                    return;
                }
                else try
                    {
                        lstDBDirs.Add(dbPath);
                        this.menuRemoveDB.IsEnabled = true;
                        this.contextRemoveDB.IsEnabled = true;
                        //string dbPath = @"C:\Users\Symation\Desktop\requestlog.db";                    
                        string dbConnection = string.Format("Data Source={0};", dbPath);
                        using (m_dbcon = new SQLiteConnection(dbConnection))
                        {
                            m_dbcon.Open();

                            // db파일명
                            //TreeViewItem tree_db_Name = new TreeViewItem() { Header = "requestlog" };
                            TreeViewItem tree_db_Name = new TreeViewItem() { Header = System.IO.Path.GetFileNameWithoutExtension(dbPath), Tag="DB", ToolTip=dbPath };
                            TreeViewItem tree_db_Table = new TreeViewItem() { Header = "Tables", Tag = "Tables", ToolTip = dbPath };
                            //TreeViewItem tree_db_Name = GetTreeView(System.IO.Path.GetFileNameWithoutExtension(dbPath), dbPath, "DB", @"icon\db.png");
                            //TreeViewItem tree_db_Tables = GetTreeView("Tables", dbPath, "Tables", @"icon\table.png");
                            //List<(string User_KEY, string RemotIP)> lstTable = new List<(string User_KEY, string RemotIP)>();

                            // db 테이블명
                            string str_Table_Query = "SELECT name FROM sqlite_master " +
                            "WHERE type = 'table'" +
                            "ORDER BY 1";
                            SQLiteCommand sqlCmd = new SQLiteCommand(str_Table_Query, m_dbcon);
                            SQLiteDataReader reader = sqlCmd.ExecuteReader();
                            int i = 0;  // 테이블 개수
                            while (reader.Read())
                            {
                                string s_Table_Name = reader.GetString(0);
                                TreeViewItem tree_db_Table_Name = new TreeViewItem()
                                {
                                    Header = s_Table_Name,
                                    Tag = "Table",
                                    ToolTip = dbPath
                                };
                                tree_db_Table.Items.Add(tree_db_Table_Name);
                                i++;
                            }
                            m_dbcon.Close();
                            tree_db_Table.Header += $"  ({i})";

                            for (int j = 0; j < i; j++)  // 테이블 마다
                            {
                                TreeViewItem tree_db_Columns = new TreeViewItem() { Header = "Columns", ToolTip = dbPath, Tag = "Columns" };
                                // 칼럼명
                                TreeViewItem tree_db_Table_Name = (TreeViewItem)tree_db_Table.Items[j];
                                string s_Column_Query = "SELECT * FROM " + ((TreeViewItem)tree_db_Table.Items[j]).Header;
                                StringBuilder sql = new StringBuilder();
                                sql.AppendLine("SELECT *");
                                sql.AppendLine("FROM @dbNAME");
                                m_dbcon = new SQLiteConnection(dbConnection);
                                m_dbcon.Open();
                                SQLiteDataReader reader_col = new SQLiteCommand(s_Column_Query, m_dbcon).ExecuteReader();
                                int k;  // 칼럼 개수
                                for (k = 0; k < reader_col.FieldCount; k++)
                                {
                                    string col_name = reader_col.GetName(k);
                                    tree_db_Columns.Items.Add(new TreeViewItem() { Header = col_name, Tag = "Column", ToolTip = dbPath });
                                }
                                tree_db_Columns.Header = $"Columns  ({k})";
                                tree_db_Table_Name.Items.Add(tree_db_Columns);
                            }

                            tree_db_Name.Items.Add(tree_db_Table);
                            TreeDB.Items.Add(tree_db_Name);
                            this.com_DB.Items.Add(new ComboBoxItem()
                            {
                                Content = System.IO.Path.GetFileNameWithoutExtension(dbPath),
                                ToolTip = dbPath
                            });
                            this.com_DB.SelectedIndex = this.com_DB.Items.Count - 1;
                            Write_Status($"{System.IO.Path.GetFileName(dbPath)} load sucessed!", Brushes.Blue);
                        }
                    }
                    catch (Exception ex)
                    {                        
                        Debug.WriteLine("DataBase_Add_Widget error >> " + ex.Message + "\n" + ex.StackTrace);
                    }
            }
        }
        private void OnItemMouseDoubleClick(object sender, EventArgs e)
        {
            var item = TreeDB.SelectedItem as TreeViewItem;
            if (item.Tag != null)
            {
                string strTag = item.Tag.ToString();
                if (strTag == "Table")
                {
                    item.IsExpanded = true;
                    string strHeader = item.Header.ToString();
                    string dbConnection = string.Format("Data Source={0};", item.ToolTip.ToString());

                    DataTable dtData = new DataTable();
                    DataTable dtStructure = new DataTable();
                    using (m_dbcon = new SQLiteConnection(dbConnection))
                    {
                        m_dbcon.Open();
                        string s_Column_Query = "SELECT * FROM " + strHeader;
                        SQLiteDataReader reader = new SQLiteCommand(s_Column_Query, m_dbcon).ExecuteReader();
                        dtData.Load(reader);

                        string str_StructureQuery = $"pragma table_info({strHeader})";
                        reader = new SQLiteCommand(str_StructureQuery, m_dbcon).ExecuteReader();
                        dtStructure.Load(reader);
                    }
                    this.dataGrid.ItemsSource = dtData.DefaultView;
                    this.dataGrid.Visibility = Visibility.Visible;

                    this.structureGrid.ItemsSource = dtStructure.DefaultView;
                    this.structureGrid.Visibility = Visibility.Visible;
                }
                else if (strTag == "Tables")
                {

                }
                else if (strTag == "DB")
                {

                }
                else
                {
                    Debug.WriteLine(item.Tag.ToString());
                }
            }
        }

        private void menuRemoveDB_Click(object sender, RoutedEventArgs e)
        {
            if (this.TreeDB.SelectedItem != null)
            {
                TreeViewItem item = this.TreeDB.SelectedItem as TreeViewItem;
                if (this.lstDBDirs.Contains(item.ToolTip.ToString()))
                {
                    for (int i = 0; i < this.TreeDB.Items.Count; i++)
                    {
                        if ((this.TreeDB.Items[i] as TreeViewItem).ToolTip.ToString() == item.ToolTip.ToString())
                        {
                            MessageBoxResult mbr = MessageBox.Show($"Are you sure remove {(this.TreeDB.Items[i] as TreeViewItem).Header} on the list?",
                                "Remove DB", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (mbr == MessageBoxResult.Yes)
                            {
                                Write_Status(System.IO.Path.GetFileName((this.TreeDB.Items[i] as TreeViewItem).ToolTip.ToString()) + " remove sucessed!", Brushes.Blue);
                                this.TreeDB.Items.Remove(this.TreeDB.Items[i]);
                                this.com_DB.Items.Remove(this.com_DB.Items[i]);
                                this.com_DB.SelectedIndex = 0;
                                this.lstDBDirs.Remove(item.ToolTip.ToString());
                                if (lstDBDirs.Count == 0)
                                {
                                    this.menuRemoveDB.IsEnabled = false;
                                    this.contextRemoveDB.IsEnabled = false;
                                }                                    
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void MenuRun_Click(object sender, RoutedEventArgs e)
        {
            Run_SQL();
        }

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            Run_SQL();
        }

        private void Run_SQL()
        {
            //backgroundWorker.RunWorkerAsync();
            this.pnlLoading.Visibility = Visibility.Visible;
            this.btnRun.IsEnabled = false;
            this.mnRUn.IsEnabled = false;
            string strQuery = this.txt_Query.Text;
            p_th = new Thread(() => {
                DateTime start = DateTime.Now;                
                if (strQuery == "" || strQuery == null || com_DB.Items.Count == 0)
                    return;
                string dbConnection = string.Format("Data Source={0};", _Global.g_strDBPath);
                using (m_dbcon = new SQLiteConnection(dbConnection))
                {
                    m_dbcon.Open();
                    SQLiteCommand sqlCmd = new SQLiteCommand(strQuery, m_dbcon);
                    try
                    {
                        SQLiteDataReader reader = sqlCmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(() => {
                            this.dataGrid.ItemsSource = dt.DefaultView;
                            this.dataGrid.Visibility = Visibility.Visible;
                            this.DataTab.SelectedIndex = 1;
                        }));
                        DateTime end = DateTime.Now;
                        TimeSpan ts = (end - start);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                            Write_Status($"Query sucessfully finished in {string.Format("{0:0.00}", ts.TotalSeconds)} second(s) ", Brushes.Blue);
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                            Write_Status(ex.Message.Replace("\r\n", " "), Brushes.Red);
                        }));
                    }
                    finally
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                            this.pnlLoading.Visibility = Visibility.Collapsed;
                            this.btnRun.IsEnabled = true;
                            this.mnRUn.IsEnabled = true;
                        }));
                    }
                }
            });
            p_th.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.F9) && this.btnRun.IsEnabled == true)
                Run_SQL();
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.O))
                DataBase_Add_Widget();
            else if (Keyboard.IsKeyDown(Key.Delete))
                menuRemoveDB_Click(null, null);
        }

        private void Write_Status(string msg, object color = null)
        {
            TextRange txt = new TextRange(this.txt_Status.Document.ContentEnd, this.txt_Status.Document.ContentEnd);
            txt.Text = $"[{DateTime.Now.ToString("HH:mm:ss")}] {msg}\n";
            txt.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            this.txt_Status.ScrollToEnd();
        }

        private void ShowStatus_Click(object sender, RoutedEventArgs e)
        {
            if (this.txt_Status.Visibility == Visibility.Visible)
            {
                this.txt_Status.Visibility = Visibility.Collapsed;
            }
            else
                this.txt_Status.Visibility = Visibility.Visible;
        }

        private void menuExpand_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.TreeDB.Items)
            {
                TreeViewItem tvi = item as TreeViewItem;
                if (tvi != null)
                    tvi.ExpandSubtree();
            }
        }

        private void Status_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0 && this.txt_Status.FontSize < 50)
                {
                    this.txt_Status.FontSize += 2;
                }
                else if (e.Delta < 0 && this.txt_Status.FontSize > 2)
                {
                    this.txt_Status.FontSize -= 2;
                }
            }                
        }
        private void SQL_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0 && this.txt_Query.FontSize < 50)
                {
                    this.txt_Query.FontSize += 2;
                }
                else if (e.Delta < 0 && this.txt_Query.FontSize > 2)
                {
                    this.txt_Query.FontSize -= 2;
                }
            }
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void com_DB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Global.g_strDBPath = (com_DB.SelectedItem as ComboBoxItem).ToolTip.ToString();
        }

        private void Interrupt_Thread()
        {
            if(p_th != null)
            {
                if (p_th.IsAlive)
                {
                    p_th.Abort();
                }
            }
        }

        private void pnlLoading_evInerruptClick(object sender)
        {
            Interrupt_Thread();
        }

        private TreeViewItem GetTreeView(string header, string tooltip, string tag, string imagePath)
        {
            TreeViewItem item = new TreeViewItem();
            item.ToolTip = tooltip;
            item.Tag = tag;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create Image
            Image image = new Image();
            image.Source = new BitmapImage
                (new Uri(System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Environment.CurrentDirectory, "../../../")) + imagePath));
            image.Width = 16;
            image.Height = 16;

            // Label
            Label lbl = new Label();
            lbl.Content = header;

            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            // assign stack to header
            item.Header = stack;
            return item;
        }
    }
}
