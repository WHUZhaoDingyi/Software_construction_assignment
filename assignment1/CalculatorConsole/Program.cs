using System;

class Program
{
    static void Main(string[] args)
    {
        // 提示用户输入第一个数字
        Console.WriteLine("Enter the first number:");
        double num1 = Convert.ToDouble(Console.ReadLine());

        // 提示用户输入第二个数字
        Console.WriteLine("Enter the second number:");
        double num2 = Convert.ToDouble(Console.ReadLine());

        // 提示用户输入运算符
        Console.WriteLine("Enter an operator (+, -, *, /):");
        string operatorInput = Console.ReadLine();

        // 初始化计算结果变量
        double result = 0;

        // 根据运算符进行运算
        switch (operatorInput)
        {
            case "+":
                result = num1 + num2;
                break;
            case "-":
                result = num1 - num2;
                break;
            case "*":
                result = num1 * num2;
                break;
            case "/":
                if (num2 != 0)
                {
                    result = num1 / num2;
                }
                else
                {
                    Console.WriteLine("Error: Cannot divide by zero.");
                    return; // 退出程序
                }
                break;
            default:
                Console.WriteLine("Invalid operator. Please enter one of the following: +, -, *, /.");
                return; // 退出程序
        }

        // 输出计算结果
        Console.WriteLine($"The result of {num1} {operatorInput} {num2} = {result}");
    }
}

