using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator
{
    public static class Expression
    {
        public static string ConvertToPostFix(string expStr)
        {
            // 1. 수식을 각 토큰별로 구분하여 읽어들인다
            string[] tokens = expStr.Split(' ');

            string[] ops = new string[] { "+", "-", "X", "/", "(", ")" };
            Dictionary<string, int> precs = new Dictionary<string, int>
            {
                ["X"] = 2,
                ["/"] = 2,
                ["+"] = 1,
                ["-"] = 1,
                ["("] = 0,
            };

            Stack<string> opStack = new Stack<string>(); // 스택
            List<string> output = new List<string>(); // 출력 리스트

            foreach (string item in tokens)
            {
                if (ops.Contains(item) == false)
                {
                    // 2. 토큰이 피 연산자이면 출력 리스트에 넣는다.
                    output.Add(item);
                }
                else if (item == "(")
                {
                    // 3. 토큰이 왼쪽 괄호이면 스택에 푸시한다.
                    opStack.Push(item);
                }
                else if (item == ")")
                {
                    // 5. 토큰이 오른쪽 괄호이면 왼쪽 괄호가 나올 때까지 스택에서 팝하여 순서대로 리스트에 넣는다.
                    while (opStack.Peek() != "(")
                    {
                        output.Add(opStack.Pop());
                    }

                    // 왼쪽 괄호 자체는 버린다.
                    opStack.Pop();
                }
                else
                {
                    // 4. 토큰이 연산자이면, 
                    while (opStack.Count != 0)
                    {
                        if (precs[opStack.Peek()] >= precs[item])
                        {
                            // 스택에 있는 연산자의 우선 순위가 자신보다 높거나 같다면 출력 리스트에 이어 붙여준다.
                            output.Add(opStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    opStack.Push(item);
                }
            }

            // 6. 더 이상 읽을 토큰이 없다면, 스택에서 연산자를 팝하여 붙인다.
            while (opStack.Count != 0)
            {
                output.Add(opStack.Pop());
            }

            return string.Join(" ", output);
        }
        public static int Calculate(string[] tokens)
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
                        case "X": stack.Push(stack.Pop() * stack.Pop()); break;
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
}
