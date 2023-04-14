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

namespace sql
{
    /// <summary>
    /// ucLoading.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucLoading : UserControl
    {
        public delegate void InterruptClick(object sender);
        public event InterruptClick evInerruptClick;

        public ucLoading()
        {
            InitializeComponent();
        }

        private void btn_Interrupt_Click(object sender, RoutedEventArgs e)
        {
            if (evInerruptClick != null)
            {
                evInerruptClick(this);
            }
        }
    }
}
