using System;

public interface IShape
{
    bool IsValid();  // 判断形状是否合法
    double CalculateArea();  // 计算面积
    string GetShapeType();  // 获取形状类型
}

public abstract class Shape : IShape
{
    public abstract bool IsValid();
    public abstract double CalculateArea();
    public abstract string GetShapeType();
}

public class Triangle : Shape
{
    private double side1, side2, side3;

    public Triangle(double side1, double side2, double side3)
    {
        this.side1 = side1;
        this.side2 = side2;
        this.side3 = side3;
    }

    public override bool IsValid()
    {
        // 判断三角形是否合法：任意两边之和大于第三边
        return side1 + side2 > side3 && side1 + side3 > side2 && side2 + side3 > side1;
    }

    public override double CalculateArea()
    {
        if (!IsValid())
            throw new InvalidOperationException("无效的三角形");

        // 使用海伦公式计算三角形面积
        double semiPerimeter = (side1 + side2 + side3) / 2;
        return Math.Sqrt(semiPerimeter * (semiPerimeter - side1) * (semiPerimeter - side2) * (semiPerimeter - side3));
    }

    public override string GetShapeType()
    {
        return "三角形";
    }
}

public class Rectangle : Shape
{
    protected double length, width; // Protected to allow access in derived classes

    public Rectangle(double length, double width)
    {
        this.length = length;
        this.width = width;
    }

    public override bool IsValid()
    {
        // 判断矩形是否合法：两条相对边相等且长宽不等
        return length > 0 && width > 0 ;
    }

    public override double CalculateArea()
    {
        if (!IsValid())
            throw new InvalidOperationException("无效的矩形");

        return length * width;
    }

    public override string GetShapeType()
    {
        return "长方形";
    }
}

public class Square : Rectangle
{
    public Square(double side) : base(side, side)
    {
    }

    public override bool IsValid()
    {
        // 正方形要求四条边相等
        return base.IsValid() && length == width;
    }

    public override double CalculateArea()
    {
        if (!IsValid())
            throw new InvalidOperationException("无效的正方形");

        return base.CalculateArea();
    }

    public override string GetShapeType()
    {
        return "正方形";
    }
}

public class Program
{
    public static void Main()
    {
        do
        {
            Console.WriteLine("请输入形状的边长度（用空格分隔）：");

            string input = Console.ReadLine();

            // 处理输入为空的情况
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("输入无效");
                continue;
            }

            string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                Console.WriteLine("输入无效");
                continue;
            }

            double[] sides;
            try
            {
                sides = Array.ConvertAll(parts, double.Parse);
            }
            catch (FormatException)
            {
                Console.WriteLine("输入无效，请输入正确的数字。");
                continue;
            }

            IShape shape = null;

            if (sides.Length == 3)
            {
                shape = new Triangle(sides[0], sides[1], sides[2]);
            }
            else if (sides.Length == 4)
            {
                Array.Sort(sides);  // 排序便于比较
                bool isSquare = sides[0] == sides[1] && sides[1] == sides[2] && sides[2] == sides[3];
                bool isRectangle = sides[0] == sides[1] && sides[2] == sides[3];

                if (isSquare)
                {
                    shape = new Square(sides[0]);
                }
                else if (isRectangle)
                {
                    shape = new Rectangle(sides[0], sides[2]);
                }
            }

            if (shape != null)
            {
                Console.WriteLine($"形状类型: {shape.GetShapeType()}");
                Console.WriteLine($"形状合法: {shape.IsValid()}");
                if (shape.IsValid())
                {
                    Console.WriteLine($"面积: {shape.CalculateArea():F2}");
                }
            }
            else
            {
                Console.WriteLine("无效的形状");
            }

            // 询问用户是否继续
            Console.WriteLine("是否要继续输入形状？(y/n)");
            string response = Console.ReadLine().Trim().ToLower();

            if (response == "n" || response == "no")
            {
                break; // 退出循环
            }

        } while (true);

        Console.WriteLine("程序已退出。");
    }
}