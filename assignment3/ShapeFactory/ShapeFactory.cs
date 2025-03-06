using System;

public interface IShape
{
    bool IsValid();  // 判断形状是否合法
    double CalculateArea();  // 计算面积
    string GetShapeType();  // 获取形状类型
}

// 抽象类，提供默认的 GetShapeType 方法
public abstract class Shape : IShape
{
    public abstract bool IsValid();
    public abstract double CalculateArea();
    public virtual string GetShapeType() => GetType().Name;
}

// 三角形类
public class Triangle : Shape
{
    private double a, b, c;

    public Triangle(double a, double b, double c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public override bool IsValid() => a + b > c && a + c > b && b + c > a;

    public override double CalculateArea()
    {
        if (!IsValid()) return 0;
        double s = (a + b + c) / 2;
        return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
    }
}

// 长方形类
public class Rectangle : Shape
{
    protected double length, width;

    public Rectangle(double length, double width)
    {
        this.length = length;
        this.width = width;
    }

    public override bool IsValid() => length > 0 && width > 0 && length != width;

    public override double CalculateArea() => length * width;
}

// 正方形类（继承自长方形）
public class Square : Rectangle
{
    public Square(double side) : base(side, side) { }

    public override bool IsValid() => length > 0;
}

// **简单工厂类**
public class ShapeFactory
{
    private static Random random = new Random();

    public static IShape CreateRandomShape()
    {
        int type = random.Next(1, 4); // 1=Triangle, 2=Rectangle, 3=Square

        switch (type)
        {
            case 1:  // 生成三角形
                double a = random.Next(1, 10);
                double b = random.Next(1, 10);
                double c = random.Next(1, 10);
                return new Triangle(a, b, c);

            case 2:  // 生成长方形
                double length = random.Next(1, 10);
                double width = random.Next(1, 10);
                return new Rectangle(length, width);

            case 3:  // 生成正方形
                double side = random.Next(1, 10);
                return new Square(side);

            default:
                return null;
        }
    }
}

// **主程序**
public class Program
{
    public static void Main()
    {
        double totalArea = 0;
        IShape[] shapes = new IShape[10];

        for (int i = 0; i < 10; i++)
        {
            shapes[i] = ShapeFactory.CreateRandomShape();
            Console.WriteLine($"创建了 {shapes[i].GetShapeType()}，面积: {shapes[i].CalculateArea():F2}");

            if (shapes[i].IsValid())  
                totalArea += shapes[i].CalculateArea();
            else
                Console.WriteLine($"{shapes[i].GetShapeType()} 无效，跳过计算。");
        }

        Console.WriteLine($"总面积: {totalArea:F2}");
    }
}
