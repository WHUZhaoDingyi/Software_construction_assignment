using System;

public class ToeplitzMatrixChecker 
{
    // 原有判断方法保持不变
    public bool IsToeplitzMatrix(int[][] matrix) 
    {
        if (matrix == null || matrix.Length == 0) return true;
        int cols = matrix[0].Length;
        
        for (int i = 0; i < matrix.Length; i++) 
        {
            if (matrix[i] == null || matrix[i].Length != cols) 
            {
                return false;
            }
        }

        for (int row = 1; row < matrix.Length; row++) 
        {
            for (int col = 1; col < cols; col++) 
            {
                if (matrix[row][col] != matrix[row - 1][col - 1]) 
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public class Program 
{
    public static void Main()
    {
        int[][] matrix = ReadMatrixFromUser();
        bool result = new ToeplitzMatrixChecker().IsToeplitzMatrix(matrix);
        
        Console.WriteLine("\n检测结果：");
        Console.WriteLine(result ? "是托普利茨矩阵" : "不是托普利茨矩阵");
        PrintMatrix(matrix);
    }

    /// <summary>
    /// 交互式矩阵输入方法
    /// </summary>
    private static int[][] ReadMatrixFromUser()
    {
        Console.WriteLine("矩阵输入指南：\n1. 每行元素用逗号或空格分隔\n2. 输入空行结束输入\n");
        
        var matrix = new System.Collections.Generic.List<int[]>();
        
        while (true)
        {
            Console.Write($"第 {matrix.Count + 1} 行 > ");
            string input = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(input)) break;
            
            try 
            {
                string[] parts = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int[] row = Array.ConvertAll(parts, int.Parse);
                matrix.Add(row);
            }
            catch 
            {
                Console.WriteLine("输入格式错误，请重新输入本行！");
            }
        }

        return matrix.ToArray();
    }

    /// <summary>
    /// 矩阵打印方法
    /// </summary>
    private static void PrintMatrix(int[][] matrix)
    {
        Console.WriteLine("\n输入矩阵结构：");
        foreach (var row in matrix)
        {
            Console.WriteLine($"[ {string.Join(", ", row)} ]");
        }
    }
}