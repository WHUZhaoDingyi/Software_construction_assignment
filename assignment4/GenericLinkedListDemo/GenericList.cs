using System;

namespace GenericLinkedListDemo
{
    public class GenericLinkedList<T> where T : IComparable<T>
    {
        private class Node
        {
            public T Data;
            public Node Next;

            public Node(T data)
            {
                Data = data;
                Next = null;
            }
        }

        private Node head;

        public void Add(T data)
        {
            Node newNode = new Node(data);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                Node current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
        }

        public void ForEach(Action<T> action)
        {
            Node current = head;
            while (current != null)
            {
                action(current.Data);
                current = current.Next;
            }
        }

        public bool IsEmpty()
        {
            return head == null;
        }

        public T Aggregate(Func<T, T, T> func)
        {
            if (IsEmpty()) throw new InvalidOperationException("链表为空！");
            Node current = head;
            T result = current.Data;
            current = current.Next;
            while (current != null)
            {
                result = func(result, current.Data);
                current = current.Next;
            }
            return result;
        }

        public int Sum(Func<T, int> selector)
        {
            int sum = 0;
            ForEach(item => sum += selector(item));
            return sum;
        }
    }

    public class Program
    {
        public static void Main()
        {
            GenericLinkedList<int> list = new GenericLinkedList<int>();

            Console.WriteLine("请输入链表元素（输入非数字结束）：");
            string input = Console.ReadLine();

            string[] parts = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int value))
                {
                    list.Add(value);
                }
                else
                {
                    Console.WriteLine($"非数字元素 \"{part}\" 被忽略");
                }
            }

            if (list.IsEmpty())
            {
                Console.WriteLine("链表为空！");
                return;
            }

            Console.Write("链表元素：");
            list.ForEach(item => Console.Write(item + " "));
            Console.WriteLine();

            // 求最大值、最小值和总和
            int max = list.Aggregate((a, b) => a > b ? a : b);
            int min = list.Aggregate((a, b) => a < b ? a : b);
            int sum = list.Sum(item => item);

            Console.WriteLine($"最大值：{max}");
            Console.WriteLine($"最小值：{min}");
            Console.WriteLine($"总和：{sum}");
        }
    }
}
