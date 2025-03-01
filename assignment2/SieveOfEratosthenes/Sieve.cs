using System;

class Program
{
    static void Main()
    {
        const int limit=100;
        bool[] isPrime=new bool[limit+1];

        for (int i =2;i<=limit;i++)
        {
            isPrime[i]=true;

            for(int i=2;i*i<=limit;i++)
            {
                if(isPrime[i])
                {
                    for (int j=i*i;j<=limit;j+=i)
                        isPrime[j]=false;
                }
            }
        }
        Console.WriteLine("2~100以内的素数： ");
            for(int i=2;i<=limit;i++)
            {
                if(isPrime[i])
                    Console.Write(i+" ");
            }
    }
}