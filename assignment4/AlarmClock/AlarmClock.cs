using System;
using System.Threading;

namespace AlarmClock
{
    // 定义闹钟类
    public class Clock
    {
        // 定义嘀嗒（Tick）事件和响铃（Alarm）事件
        public event Action Tick;
        public event Action Alarm;

        // 闹钟时间
        public DateTime AlarmTime { get; set; }

        // 开始运行闹钟
        public void Start()
        {
            while (true)
            {
                // 触发 Tick 事件
                Tick?.Invoke();

                // 判断是否响铃
                if (DateTime.Now >= AlarmTime)
                {
                    // 触发 Alarm 事件
                    Alarm?.Invoke();
                    break;
                }

                // 每秒嘀嗒一次
                Thread.Sleep(1000);
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // 创建闹钟对象
            Clock clock = new Clock();

            // 订阅嘀嗒事件
            clock.Tick += () =>
            {
                Console.WriteLine($"嘀嗒：{DateTime.Now:HH:mm:ss}");
            };

            // 订阅响铃事件
            clock.Alarm += () =>
            {
                Console.WriteLine("🔔 铃铃铃！时间到了！");
            };

            // 设置闹钟时间
            Console.Write("请输入闹钟时间（格式：HH:mm:ss）：");
            string inputTime = Console.ReadLine();
            if (DateTime.TryParse(inputTime, out DateTime alarmTime))
            {
                clock.AlarmTime = alarmTime;
                Console.WriteLine($"闹钟已设置为：{alarmTime:HH:mm:ss}");
                // 启动闹钟
                clock.Start();
            }
            else
            {
                Console.WriteLine("时间格式错误！");
            }
        }
    }
}

