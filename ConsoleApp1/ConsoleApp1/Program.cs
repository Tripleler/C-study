// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;

namespace CS
{
    class CodingTest
    {
        public int Calculate(string[] tokens)
        {
            Stack<int> stack = new Stack<int>();
            try
            {
                foreach (string token in tokens)
                {
                    switch (token)
                    {
                        case "+": stack.Push(stack.Pop() + stack.Pop()); break;
                        case "-": stack.Push(-(stack.Pop() - stack.Pop())); break;
                        case "*": stack.Push(stack.Pop() * stack.Pop()); break;
                        case "/":
                            int rv = stack.Pop();
                            stack.Push(stack.Pop() / rv); break;
                        default: stack.Push(int.Parse(token)); break;
                    }
                }
                return stack.Pop();
            }
            catch
            {
                return 0;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CodingTest ct = new CodingTest();
            int value = ct.Calculate(new string[] { "2", "3", "5", "*", "+", "7", "-" });
            Console.WriteLine(value);
        }
    }
}