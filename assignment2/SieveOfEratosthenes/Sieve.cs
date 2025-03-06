using System;

class Program
{
    static void Main()
    {
        const int limit = 100;
        bool[] isPrime = new bool[limit + 1];

        for (int i = 2; i <= limit; i++)
        {
            isPrime[i] = true;
        }

        for (int p = 2; p * p <= limit; p++) // 这里用 p 代替 i
        {
            if (isPrime[p])
            {
                for (int j = p * p; j <= limit; j += p)
                    isPrime[j] = false;
            }
        }

        Console.WriteLine("2~100 以内的素数：");
        for (int i = 2; i <= limit; i++)
        {
            if (isPrime[i])
                Console.Write(i + " ");
        }
    }
}
