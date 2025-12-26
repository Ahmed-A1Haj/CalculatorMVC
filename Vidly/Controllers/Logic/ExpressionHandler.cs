using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidly.Interfaces;

namespace Arethmic_expressions_processor
{
    internal class ExpressionHandler : IExpressionHandler
    {
        // Operator prority for determining the order of operations
        static int Priority(char op)
        {
            return op switch
            {
                '+' or '-' => 1,
                '*' or '/' => 2,
                _ => 0
            };
        }

        // Convert normal expression to postfix using stacks
        public string InfixToPostfix(string infix)
        {
            Stack<char> ops = new Stack<char>();
            List<string> output = new List<string>();

            int bracketCount = 0;
            bool isNegative = false;
            for (int i = 0; i < infix.Length; i++)
            {
                char c = infix[i];
                if (char.IsWhiteSpace(c)) continue;
                if (c.Equals('.')) throw new InvalidOperationException("Must be a number before dot");
                

                if (infix[i] == '-' && i == 0 )
                {
                    output.Add("0");
                }

                // If it's a number, read the whole number (multi-digit support)
                if (char.IsDigit(c))
                {
                    string number = "";
                    bool isdot = false;
                    while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i].Equals('.')))
                    {
                        if (isdot && infix[i].Equals('.')) throw new InvalidOperationException("too many dots");
                        if (infix[i].Equals('.')) isdot = true;
                        // process negative numbers
                        if (isNegative)
                        {
                            isNegative = false;
                            number += '-';
                        }
                        number += infix[i];
                        i++;
                    }
                    i--;
                    if (number[number.Length - 1] == '.') throw new InvalidOperationException("Incorrect use of dot");
                    output.Add(number);
                }
                else if (c != '-' && c != '+' && c != '*' && c != '/' && c != '(' && c != ')') throw new InvalidOperationException("UnKnowen Operator user");
                else if (c == '(')
                {
                    ops.Push(c);
                    bracketCount++;
                    if (i != infix.Length - 1 && infix[i + 1] == '-')
                    {
                        isNegative = true;
                        i++;
                    }
                    if (i != infix.Length - 1 && !char.IsDigit(infix[i + 1])) throw new InvalidOperationException("First element in Bracket must be a number/Or a negative");
                }
                else if (c == ')' && bracketCount == 0) throw new InvalidOperationException("Unclosed bracket detected");
                else if (c == ')' && bracketCount > 0)
                {
                    bracketCount--;
                    while (ops.Peek() != '(')
                        output.Add(ops.Pop().ToString());

                    ops.Pop(); // remove '('
                }

                else // operator
                {
                    while (ops.Count > 0 && Priority(ops.Peek()) >= Priority(c))
                        output.Add(ops.Pop().ToString());

                    ops.Push(c);
                }
            }
            if (bracketCount > 0) throw new InvalidOperationException("Unclosed bracket detected");

            while (ops.Count > 0)
                output.Add(ops.Pop().ToString());

            return string.Join(" ", output);
        }

        // Evaluate postfix expression using a stack
        public decimal EvaluatePostfix(string postfix)
        {
            if (string.IsNullOrEmpty(postfix)) throw new InvalidOperationException("Expression is empty");

            Stack<decimal> answerStack = new Stack<decimal>();
            string[] tokens = postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                // Add Numbers
                if (decimal.TryParse(token, out decimal num))
                {
                    answerStack.Push(num);
                }

                else if (answerStack.Count < 2) throw new InvalidOperationException("not enough numbers before operator/if its a negative use Brackets");


                else // make operations
                {
                    decimal b = answerStack.Pop();
                    decimal a = answerStack.Pop();


                    switch (token)
                    {
                        case "+":
                            answerStack.Push(a + b);
                            break;
                        case "-":
                            answerStack.Push(a - b);
                            break;
                        case "*":
                            answerStack.Push(a * b);
                            break;
                        case "/":
                            //if (b == 0) throw new DivideByZeroException("Dividing by zero");
                            answerStack.Push(a / b);
                            break;
                        default:
                            throw new InvalidOperationException("Unknown Operator");

                    }
                }


            }
            if (answerStack.Count != 1) throw new InvalidOperationException("Expression has too many operands or too few operators.");
            return answerStack.Pop();
        }
    }
}
