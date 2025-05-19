@echo off
echo 模拟数据库环境...

rem 设置工作目录
cd %~dp0

rem 假设我们已经有了数据库
echo 假设MySQL数据库已经创建并且连接正常
echo 假设已经创建了表结构
echo 假设已经添加了初始数据

echo 正在启动应用程序...

rem 运行应用程序
dotnet run --project OrderConsole

pause 