using System;
namespace PrimeFactorsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入一个整数：");
            int number = int.Parse(Console.ReadLine());
            Console.WriteLine($"{number} 的素数因子是：");
            PrintPrimeFactors(number);
        }
        static void PrintPrimeFactors(int number)
        {
            while (number % 2 == 0)
            {
                Console.Write(2 + " ");
                number /= 2;
            }
            for (int i = 3; i * i <= number; i += 2)//除1以外的素数都是奇数，所以从3开始，步长为2
            {
                while (number % i == 0)
                {
                    Console.Write(i + " ");
                    number /= i;
                }
            }
            if (number > 2)
                Console.Write(number);
        }
    }
}
