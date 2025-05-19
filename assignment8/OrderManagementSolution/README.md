# 订单管理系统 Web API

本项目是一个基于ASP.NET Core的订单管理系统Web API，提供订单的增删改查功能。

## 项目结构

- **OrderManagement.Domain**: 领域模型项目，包含Order和OrderDetails实体类
- **OrderManagement.Service**: 服务层项目，包含数据库上下文和订单服务类
- **OrderManagement.WebAPI**: Web API项目，包含API控制器和配置
- **OrderManagement.ConsoleApp**: 控制台应用程序，用于测试和演示
- **OrderManagement.WinFormsApp**: Windows Forms应用程序，提供图形界面

## 如何运行

### 准备工作

1. 确保安装了.NET 9.0 SDK
2. 确保安装并运行了MySQL服务器
3. 更新`appsettings.json`中的数据库连接字符串，设置正确的用户名和密码

### 启动API

```bash
cd OrderManagement.WebAPI
dotnet run
```

应用程序启动后，可以通过浏览器访问 `http://localhost:5000/` 查看Swagger UI界面。

## API端点

| 方法   | 路径                           | 描述                   |
|--------|--------------------------------|------------------------|
| GET    | /api/orders                    | 获取所有订单           |
| GET    | /api/orders/{id}               | 根据ID获取订单         |
| GET    | /api/orders/customer/{name}    | 根据客户名称查询订单   |
| GET    | /api/orders/orderNumber/{num}  | 根据订单号查询订单     |
| POST   | /api/orders                    | 创建新订单             |
| PUT    | /api/orders/{id}               | 更新订单               |
| DELETE | /api/orders/{id}               | 删除订单               |

## 使用Postman测试

1. 导入`OrderManagement.WebAPI.http`文件到Postman
2. 或者直接使用提供的API端点进行测试

示例：添加新订单

```json
POST http://localhost:5000/api/orders
Content-Type: application/json

{
  "orderNumber": "ORD-001",
  "customer": "张三",
  "orderDetails": [
    {
      "productName": "商品1",
      "amount": 100.00
    },
    {
      "productName": "商品2",
      "amount": 200.00
    }
  ]
}
```

## 数据库初始化

系统启动时会自动应用迁移，创建必要的数据库表。如果需要手动迁移，可以执行以下命令：

```bash
cd OrderManagement.WebAPI
dotnet ef database update
```

## 开发注意事项

- 所有API请求和响应均使用JSON格式
- 错误响应会包含详细的错误信息
- API支持CORS，允许从任何源进行跨域请求 