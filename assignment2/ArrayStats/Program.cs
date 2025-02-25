using System;

namespace ArrayCalculator
{
    public class Calculator
    {
        public static void Main()
        {
            int[] numbers = ReadNumberArray();
            var results = CalculateResults(numbers);
            PrintResults(numbers, results);
        }

        private static int[] ReadNumberArray()
        {
            Console.WriteLine("请输入整数组（用空格或逗号分隔）：");
            string input = Console.ReadLine().Trim();
            string[] parts = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return Array.ConvertAll(parts, int.Parse);
        }

        private static (int max, int min, int sum, double avg) CalculateResults(int[] numbers)
        {
            int max = numbers[0], min = numbers[0], sum = 0;
            
            foreach (int num in numbers)
            {
                max = Math.Max(max, num);
                min = Math.Min(min, num);
                sum += num;
            }

            double avg = (double)sum / numbers.Length;
            return (max, min, sum, avg);
        }

        private static void PrintResults(int[] numbers, (int max, int min, int sum, double avg) results)
        {
            Console.WriteLine("\n计算结果：");
            Console.WriteLine($"数组内容：[{string.Join(", ", numbers)}]");
            Console.WriteLine($"最大值：{results.max}");
            Console.WriteLine($"最小值：{results.min}");
            Console.WriteLine($"总和：{results.sum}");
            Console.WriteLine($"平均值：{results.avg:F2}");
        }
    }
}