using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace notepad
{
    /// <summary>
    /// FindWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FindWindow : Window
    {
        private bool bIsReFind = false;
        private int p_iCursorIdx = 0;
        public delegate void FindClick(object sender,string FindText);
        public event FindClick evFindClick;
        
        public FindWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // 이걸 쓰면 텍스트를 보냄
            //if(evFindClick != null)
            //{
            //    evFindClick(this, this.txtBox.Text);
            //}            
            if (!bIsReFind)
            {
                p_iCursorIdx = ((MainWindow)App.Current.MainWindow).txtEditor.CaretIndex;
            }
            int iFindIdx = ((MainWindow)App.Current.MainWindow).txtEditor.Text.IndexOf(txtBox.Text, p_iCursorIdx);
            if (iFindIdx == -1)
            {
                // 못찾았으니 처음부터 다시 찾기
                iFindIdx = ((MainWindow)App.Current.MainWindow).txtEditor.Text.IndexOf(txtBox.Text);
                if (iFindIdx == -1)
                {
                    // 그래도 없으면 아예 없음
                    MessageBox.Show("찾는 값이 없습니다.");
                }
                else
                {
                    ((MainWindow)App.Current.MainWindow).txtEditor.Select(iFindIdx, txtBox.Text.Length);
                    ((MainWindow)App.Current.MainWindow).Focus();
                    p_iCursorIdx = iFindIdx + txtBox.Text.Length;
                    bIsReFind = true;
                }
            }
            else
            {
                ((MainWindow)App.Current.MainWindow).txtEditor.Select(iFindIdx, txtBox.Text.Length);
                ((MainWindow)App.Current.MainWindow).Focus();
                p_iCursorIdx = iFindIdx + txtBox.Text.Length;
                bIsReFind = true;
            }
        }
        private void SearchUp_Click(object sender, RoutedEventArgs e)
        {
            if (!bIsReFind)
            {
                p_iCursorIdx = ((MainWindow)App.Current.MainWindow).txtEditor.CaretIndex;
            }
            int iFindIdx = ((MainWindow)App.Current.MainWindow).txtEditor.Text.LastIndexOf(txtBox.Text, p_iCursorIdx, 0);
            if (iFindIdx == -1)
            {
                // 못찾았으니 끝부터 다시 찾기
                iFindIdx = ((MainWindow)App.Current.MainWindow).txtEditor.Text.LastIndexOf(txtBox.Text);
                if (iFindIdx == -1)
                {
                    // 그래도 없으면 아예 없음
                    MessageBox.Show("찾는 값이 없습니다.");
                }
                else
                {
                    ((MainWindow)App.Current.MainWindow).txtEditor.Select(iFindIdx, txtBox.Text.Length);
                    ((MainWindow)App.Current.MainWindow).Focus();
                    p_iCursorIdx = iFindIdx;
                    bIsReFind = true;
                }
            }
            else
            {
                ((MainWindow)App.Current.MainWindow).txtEditor.Select(iFindIdx, txtBox.Text.Length);
                ((MainWindow)App.Current.MainWindow).Focus();
                p_iCursorIdx = iFindIdx;
                bIsReFind = true;
            }
        }
        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (txtBox.Text != "")
            {
                //p_iCursorIdx = ((MainWindow)App.Current.MainWindow).txtEditor.CaretIndex;
                //Regex regex = new Regex(txtBox.Text.Substring(p_iCursorIdx, ))
                //((MainWindow)App.Current.MainWindow).txtEditor.Text.Re
            }
        }

        private void ShowHide_Click(object sender, RoutedEventArgs e)
        {
            if (this.Height == 100)
            {
                this.MinHeight = 200;
                this.MaxHeight = 200;
                this.Height = 200;
                this.show.Source = new BitmapImage(new Uri("C:\\Users\\Symation\\source\\repos\\notepad\\notepad\\icon\\hide.PNG", UriKind.Absolute));
                this.Bottom_Layout.Height = new GridLength(100, GridUnitType.Star);
            }
            else
            {
                this.MinHeight = 100;
                this.MaxHeight = 100;
                this.Height = 100;
                this.show.Source = new BitmapImage(new Uri("C:\\Users\\Symation\\source\\repos\\notepad\\notepad\\icon\\show.PNG", UriKind.Absolute));
                this.Bottom_Layout.Height = new GridLength(0, GridUnitType.Star);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            bIsReFind = false;
        }        
        
        
        // 레이아웃 추가 히든 비지블   
    }
}
