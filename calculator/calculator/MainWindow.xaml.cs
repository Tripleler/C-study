using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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

namespace calculator
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool p_IsDownShift = false;
        private string p_strExp = "";
        private string p_strNum = "0";

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            Button b = sender as Button;
            //지우기
            System.Diagnostics.Debug.WriteLine(b.Content.ToString());  /*지우기*/
            double n;
            bool check = false;  /*수식 마지막 기호체크시 리셋 확인*/
            string strBtn = b.Content.ToString();
            string strExp = lbl_exp.Content.ToString();
            if (double.TryParse(strBtn, out n))  /*숫자면*/
            {
                if (p_strNum == "0")
                {
                    p_strNum = strBtn;
                }
                else
                {
                    p_strNum += strBtn;
                }
                this.lbl_num.Content = p_strNum;
            }
            else if (strBtn == "+" || strBtn == "-" || strBtn == "X" || strBtn == "/")  /*기호면*/
            {
                System.Diagnostics.Debug.WriteLine(p_strExp + "||" + p_strNum);
                if (p_strExp != "") p_strExp += " ";
                if (p_strNum != "0") p_strExp += p_strNum;
                p_strExp += " " + strBtn;
                p_strExp.Trim();
                this.lbl_exp.Content = p_strExp;
                p_strNum = "0";
            }
            else if (strBtn == ".")  /*소수*/
            {
                //소수가 이미 적용된게 아닌 경우만
                if (!p_strNum.Contains("."))  
                    p_strNum += ".";
                    this.lbl_num.Content = p_strNum;
            }
            else if (strBtn == "+/-")  /*플마*/
            {
                if (p_strNum[0] == '0')
                {
                    
                }
                else if (p_strNum[0] == '-')
                {
                    p_strNum = p_strNum.Substring(1);
                }
                else
                {
                    p_strNum = "-" + p_strNum;
                }
                this.lbl_num.Content = p_strNum;
            }
            else if (strBtn == "back")
            {
                if (p_strNum.Length > 1)
                {
                    p_strNum = p_strNum.Substring(0, p_strNum.Length - 1);
                }
                else
                {
                    p_strNum = "0";
                }
                this.lbl_num.Content = p_strNum;
            }

                //if (strExp != "")
                //{
                //    string last = strExp.Substring(strExp.Length - 1, 1);
                //    if (last == "+" || last == "-" || last == "X" || last == "/")
                //    {
                //        //여기서 숫자 하나만 입력됨
                //        if (!check)
                //        {
                //            this.lbl_num.Content = b.Content.ToString();
                //            check = true;
                //        }
                //        else
                //        {
                //            this.lbl_num.Content += b.Content.ToString();
                //        }
                //    }
                //}
                //else
                //{
                //    if (this.lbl_num.Content.ToString() != "0")
                //    {
                //        this.lbl_num.Content += b.Content.ToString();
                //    }
                //    else this.lbl_num.Content = b.Content.ToString();
                //}
            //}
            //else  /*기호면*/
            //{
            //    if (this.lbl_exp.Content.ToString() != "")
            //    {
            //        this.lbl_exp.Content = this.lbl_exp.Content + " " + this.lbl_num.Content + " " + strBtn;
            //    }
            //    else
            //    {
            //        this.lbl_exp.Content = this.lbl_num.Content + " " + strBtn;
            //    }
            //    check = false;
            //}
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            p_strExp = "";
            p_strNum = "0";
            this.lbl_exp.Content = p_strExp;
            this.lbl_num.Content = p_strNum;
        }

        private void Calculate(object sender, RoutedEventArgs e)  /*지우기*/
        {
            if (p_strExp.Length > 0 || p_strExp.Substring(p_strExp.Length - 1) != "=") p_strExp += " " + p_strNum;
            this.lbl_exp.Content = p_strExp + " =";
            
            string strPostFix = Expression.ConvertToPostFix(p_strExp);
            System.Diagnostics.Debug.WriteLine(strPostFix);
            int intRst = Expression.Calculate(strPostFix.Split());
            System.Diagnostics.Debug.WriteLine(intRst);
            this.lbl_num.Content = intRst.ToString();
            p_strNum = "0";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lbl_exp.Content = "";
            this.lbl_num.Content = "0";
        }        

        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                p_IsDownShift = false;
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                p_IsDownShift = true;
            }
            else if(e.Key == Key.D8)
            {
                if (p_IsDownShift)
                    Debug.WriteLine("*");
                else
                    Debug.WriteLine("8");
            }
        }
    }
}
