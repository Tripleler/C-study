using System;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string strItem = "배열 슬라이싱 연습";
            byte[] buffer = Encoding.Default.GetBytes(strItem);
            foreach (var item in buffer)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("==============================");
            Array.Resize(ref buffer, buffer.Length + 1);
            Array.Copy(buffer, 0, buffer, 1, buffer.Length - 1);
            foreach (var item in buffer)
            {
                Console.WriteLine(item);
            }
        }        
    }
}