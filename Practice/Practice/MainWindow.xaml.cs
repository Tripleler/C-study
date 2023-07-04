using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using System.Threading;

namespace Practice
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void Clear_Click(object sender, RoutedEventArgs e)
        //{
        //    var window = new Window2();
        //    window.Show();
        //}

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //string str = Encoding.UTF8.GetString(Convert.FromBase64String("Q2FsZW5kYXJMb2dSdW5uZXI7U1lNQVRJT05fRVg7U1RBVFVTPTE7TEFTVEFDVElWRVRJTUU9MjAyMzA2MjMxNTM0MjM7"));
            //MessageBox.Show(str);
            TimeSpan LogSpan = new TimeSpan(1, 12, 23, 62);
            MessageBox.Show("LogSpan:" + $"{LogSpan.ToString("ss\\.fff")}s");


        }

        private object obj = new object();

        private void test()
        {
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    CustomData data = obj as CustomData;
                    if (data == null) return;

                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Thread.Sleep(i * 1000);
                },
                new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks });
            }
            Task.WaitAll(taskArray);
            foreach (var task in taskArray)
            {
                var data = task.AsyncState as CustomData;
                if (data != null)
                    Debug.WriteLine("Task #{0} created at {1}, ran on thread #{2}.",
                                      data.Name, data.CreationTime, data.ThreadNum);
            }

            //string str = Encoding.UTF8.GetString(Convert.FromBase64String("Q01TZXJ2ZXJCYWNrdXBTZXJ2aWNlO1NUQVRVUz0xO0xBU1RBQ1RJVkVUSU1FPTIwMjMwNjIzMTA0ODMzO0Vycm9yTWVzc2FnZT07"));
            //MessageBox.Show(str);

        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            test();
        }
    }

    enum State
    {
        class1,
        class2,
        class3,
        class4,
        class5,
    }

    class CustomData
    {
        public long CreationTime;
        public int Name;
        public int ThreadNum;
    }
}
