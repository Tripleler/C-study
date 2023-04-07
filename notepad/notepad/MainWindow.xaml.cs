using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace notepad
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool p_bCheck = true;
        private string p_strOpenFileDir = null;
        private bool p_bFindNext = false;
        private bool p_bFindPrevious = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblOS.Text = "Windows (CRLF)";
            txtEditor.Focus();
            DoReset();
            lblCursorPosition.Text = "줄 1, 열 1";
            KeyGesture saveFile = new KeyGesture(Key.S, ModifierKeys.Control);
            KeyBinding saveKeyBinding = new KeyBinding(ApplicationCommands.Save, saveFile);
            InputBindings.Add(saveKeyBinding);
            lblFormat.Text = "UTF-8";
        }

        private void command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void txtEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int row = txtEditor.GetLineIndexFromCharacterIndex(txtEditor.CaretIndex);
            int col = txtEditor.CaretIndex - txtEditor.GetCharacterIndexFromLineIndex(row);
            lblCursorPosition.Text = "줄 " + (row + 1) + ", 열 " + (col + 1);
        }

        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            p_bCheck = false;
            if (!this.Title.StartsWith("*"))
            {
                this.Title = "*" + this.Title;
            }
        }

        private void Open_File(object sender, RoutedEventArgs e)
        {
            try
            {
                if (p_bCheck == false)
                {
                    MessageBoxResult msgResult = MessageBox.Show("현재 문서를 저장하시겠습니까?", "저장", MessageBoxButton.YesNoCancel);
                    if (msgResult == MessageBoxResult.Yes)
                    {
                        DoSave();
                    }
                    else if (msgResult == MessageBoxResult.Cancel)
                        return;
                }

                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
                }
                else
                {
                    return;
                }
                txtEditor.Select(txtEditor.Text.Length, 0);
                p_strOpenFileDir = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                this.Title = System.IO.Path.GetFileName(openFileDialog.FileName);
                p_bCheck = true;
                this.Title.TrimStart('*');
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OpenFile Error >>" + ex.Message);
            }
        }

        private void DoSave(bool isDiff = false)
        {
            try
            {
                // 이미 저장된 경로가 있는 경우 저장이면 바로저장
                if (p_strOpenFileDir != null && !isDiff)
                {
                    File.WriteAllText(p_strOpenFileDir + "\\" + this.Title.TrimStart('*'), txtEditor.Text);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "텍스트 파일(*.txt)|*.txt|모든 파일|*.*";
                    if (p_strOpenFileDir != null)
                    {
                        saveFileDialog.InitialDirectory = p_strOpenFileDir;
                    }
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, txtEditor.Text);
                    }
                    else return;
                }
                p_bCheck = true;
                this.Title = this.Title.TrimStart('*');
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DoSave Error >>" + ex.Message);
            }
        }

        private void DoReset()
        {
            txtEditor.FontSize = 10;
            lblTextSize.Text = "100%";
        }


        private void Save_File(object sender, RoutedEventArgs e)
        {
            DoSave();
        }

        private void Save_DiffFile(object sender, RoutedEventArgs e)
        {
            DoSave(true);
        }

        private void Print_File(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {

            }
        }

        private void Font_Big(object sender, RoutedEventArgs e)
        {
            if (txtEditor.FontSize < 50)
            {
                txtEditor.FontSize += 1;
                lblTextSize.Text = $"{txtEditor.FontSize}0%";
            }
        }

        private void Font_Small(object sender, RoutedEventArgs e)
        {
            if (txtEditor.FontSize > 1)
            {
                txtEditor.FontSize -= 1;
                lblTextSize.Text = $"{txtEditor.FontSize}0%";
            }
        }

        private void Font_Reset(object sender, RoutedEventArgs e)
        {
            DoReset();
        }



        private void Quit_All(object sender, RoutedEventArgs e)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                App.Current.Windows[intCounter].Close();
            }
        }

        private void Enter_Datetime(object sender, RoutedEventArgs e)
        {
            string strDt = DateTime.Now.ToString("tt hh:mm yyyy-MM-dd");
            int intCursor = strDt.Length + txtEditor.CaretIndex;
            this.txtEditor.Text = txtEditor.Text.Insert(txtEditor.CaretIndex, strDt);
            this.txtEditor.Select(intCursor, 0);
        }

        private void Show_Status(object sender, RoutedEventArgs e)
        {
            if (statusBar.Visibility == Visibility.Visible)
            {
                statusBar.Visibility = Visibility.Collapsed;
            }
            else statusBar.Visibility = Visibility.Visible;

        }

        private void New_Window(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
        }

        private void New_Process(object sender, RoutedEventArgs e)
        {
            Process.Start("C:\\Users\\Symation\\source\\repos\\notepad\\notepad\\bin\\Debug\\notepad.exe");
        }


        private void Wrap_Text(object sender, RoutedEventArgs e)
        {
            if (txtEditor.TextWrapping == TextWrapping.Wrap)
            {
                txtEditor.TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                txtEditor.TextWrapping = TextWrapping.Wrap;
            }
        }

        private void Select_All(object sender, RoutedEventArgs e)
        {
            txtEditor.SelectAll();
        }

        private void Find_String(object sender, RoutedEventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == "search")
                {
                    FindWindow fw = window as FindWindow;
                    if (fw != null)
                    {
                        window.Activate();
                        fw.txtBox.SelectAll();
                    }
                    return;
                }
            }
            if (this.pnlFind.Visibility == Visibility.Collapsed)
                this.pnlFind.Visibility = Visibility.Visible;

            //bool bIsOpenFind = false;
            //foreach (var item in this.pnltxtMain.Children)
            //{
            //    ucFind ucf = item as ucFind;
            //    if (ucf != null)
            //        bIsOpenFind = true;
            //}
            //if (bIsOpenFind == false)
            //{
            //    ucFind find = new ucFind();
            //    find.VerticalAlignment = VerticalAlignment.Top;
            //    find.Width = 400;
            //    find.Margin = new Thickness(0, 20, 0, 0);
            //    find.HorizontalAlignment = HorizontalAlignment.Center;
            //    this.pnltxtMain.Children.Add(find);
            //}
            //FindWindow findwindow = new FindWindow();
            //findwindow.Owner = this;
            //findwindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //findwindow.evFindClick += Findwindow_evFindClick;
            //findwindow.Show();
        }

        private void Findwindow_evFindClick(object sender, string FindText)
        {
            //string s_FindText = sender as string;
            //int idx = txtEditor.Text.IndexOf(s_FindText, txtEditor.CaretIndex);
            //if (idx == -1)
            //{
            //    MessageBox.Show("찾는 값이 없습니다.");
            //}
            //else
            //{
            //    Debug.WriteLine(idx);
            //    Debug.WriteLine(s_FindText.Length);
            //    txtEditor.Select(idx, s_FindText.Length);
            //    this.Focus();
            //    idx += s_FindText.Length;
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (p_bCheck == false)
                {
                    MessageBoxResult msgResult = MessageBox.Show("현재 문서를 저장하시겠습니까?", "저장", MessageBoxButton.YesNoCancel);
                    if (msgResult == MessageBoxResult.Yes)
                    {
                        DoSave();
                        Close();
                    }
                    else if (msgResult == MessageBoxResult.No)
                    {
                        Close();
                    }
                    else if (msgResult == MessageBoxResult.Cancel)
                        e.Cancel = true;
                }
                else Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Window_Closing Error >>" + ex.Message);
            }
        }
        private void Window_Close(object sender, RoutedEventArgs e)
        {
            Window_Closing(null, null);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.Write(e.Key.ToString());
            //if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    // 키 입력을 처리한다.
            //}
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift)
                && Keyboard.IsKeyDown(Key.N))
                New_Window(null, null);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.O))
                Open_File(null, null);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
                Save_File(null, null);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift)
                && Keyboard.IsKeyDown(Key.S))
                Save_DiffFile(null, null);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift)
                && Keyboard.IsKeyDown(Key.W))
                Window_Close(null, null);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.F))
                Find_String(null, null);
            if (Keyboard.IsKeyDown(Key.F3))
            {
                //foreach (Window window in App.Current.Windows)
                //{
                //    if (window.Name == "search")
                //    {
                //        FindWindow fw = window as FindWindow;
                //        if (fw != null)
                //        {
                //            window.Activate();
                //            fw.txtBox.SelectAll();
                //            Find_String(null, null);
                //            return;
                //        }                        
                //    }
                //}
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.F3))
            {

            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.H))
            {

            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && 
                (Keyboard.IsKeyDown(Key.OemPlus) || Keyboard.IsKeyDown(Key.Add)))            
                Font_Big(null, null);            
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && 
                (Keyboard.IsKeyDown(Key.OemMinus) || Keyboard.IsKeyDown(Key.Subtract)))            
                Font_Small(null, null);            
            if (Keyboard.IsKeyDown(Key.LeftCtrl) &&
                (Keyboard.IsKeyDown(Key.D0) || Keyboard.IsKeyDown(Key.NumPad0)))
                DoReset();
        }

        private void pnlFind_evClose(object sender)
        {
            if (this.pnlFind.Visibility == Visibility.Visible)
                this.pnlFind.Visibility = Visibility.Collapsed;
        }
    }
}