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

namespace sql
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HierarchicalDataTemplate template = new HierarchicalDataTemplate(typeof(Folder));

            template.ItemsSource = new Binding("ChildFolderList");

            FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));

            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Name"));

            template.VisualTree = textBlockFactory;

            Folder folder = new Folder(new DirectoryInfo(System.IO.Path.GetPathRoot(Environment.SystemDirectory)));

            TreeViewItem item = new TreeViewItem();

            item.Header = folder.Name;
            item.ItemsSource = folder.ChildFolderList;
            item.ItemTemplate = template;

            this.treeView.Items.Add(item);

            item.IsExpanded = true;
        }
    }
}
