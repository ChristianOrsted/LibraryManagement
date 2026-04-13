# 智慧图书馆借阅管理系统

> Web开发与应用A 课程项目

## 项目简介

本系统是一个基于 **ASP.NET Core MVC** 框架开发的图书馆借阅管理系统，实现了用户注册登录、图书浏览检索、借阅归还、预约管理以及管理员后台等完整功能。

## 技术栈

| 层次 | 技术 |
|------|------|
| 框架 | ASP.NET Core 8.0 MVC |
| 语言 | C# 12 |
| 数据库 | MySQL 8.0 |
| ORM | Entity Framework Core 8.0 (Pomelo.EntityFrameworkCore.MySql) |
| 前端 | Bootstrap 5 + Bootstrap Icons + jQuery |
| 密码加密 | BCrypt.Net |
| 认证方式 | Session |

## 项目结构

```
WebProject1/
├── Controllers/          # 控制器
│   ├── HomeController.cs       # 首页
│   ├── AccountController.cs    # 注册/登录/注销
│   ├── BookController.cs       # 图书浏览/搜索/详情
│   ├── BorrowController.cs     # 借阅/归还/预约
│   └── AdminController.cs      # 管理后台 CRUD
├── Models/               # 数据模型
│   ├── User.cs                 # 用户
│   ├── Category.cs             # 图书分类
│   ├── Book.cs                 # 图书
│   ├── BookCopy.cs             # 图书副本
│   ├── BorrowRecord.cs         # 借阅记录
│   ├── Reservation.cs          # 预约记录
│   └── ViewModels.cs           # 视图模型
├── Data/                 # 数据层
│   ├── LibraryDbContext.cs     # EF Core 数据库上下文
│   └── DbInitializer.cs       # 数据库种子数据
├── Filters/              # 过滤器
│   └── AuthFilter.cs          # 登录验证 & 管理员权限过滤器
├── Views/                # 视图
│   ├── Home/                   # 首页视图
│   ├── Account/                # 登录/注册视图
│   ├── Book/                   # 图书检索/详情视图
│   ├── Borrow/                 # 借阅/预约视图
│   ├── Admin/                  # 管理后台视图
│   └── Shared/                 # 共享布局
├── wwwroot/              # 静态资源 (CSS, JS, 库文件)
├── Program.cs            # 程序入口及服务配置
└── appsettings.json      # 应用配置
```

## 数据库设计

系统包含 6 张核心表：

```
Category ──< Book ──< BookCopy ──< BorrowRecord >── User
                           └── Reservation >── User
```

- **Users** — 用户表（读者 + 管理员）
- **Categories** — 图书分类
- **Books** — 图书基本信息
- **BookCopies** — 图书实体副本（同一本书可有多册，体现真实图书馆业务）
- **BorrowRecords** — 借阅记录
- **Reservations** — 预约记录

## 功能说明

### 读者功能
- 用户注册与登录（BCrypt 密码加密）
- 图书浏览、关键词搜索（书名/作者/ISBN）、分类筛选、分页展示
- 查看图书详情与馆藏状态
- 借阅图书（每人最多借阅 5 本，借期 30 天）
- 归还图书
- 预约无库存图书、取消预约

### 管理员功能
- 管理后台 Dashboard（统计总览）
- 图书管理：增删改查
- 分类管理：增删改查
- 图书副本管理：添加副本、修改状态（可借/损坏）
- 用户管理：查看用户、切换角色
- 借阅记录管理：查看所有记录、强制归还

## 运行方法

### 1. 初始化数据库

确保本地 MySQL 服务已启动，创建数据库：

```sql
CREATE DATABASE library_db CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
```

项目首次运行时，`DbInitializer` 会自动创建表结构并填充示例数据。

### 2. 配置数据库连接

创建 `appsettings.Development.json`，填入你本地 MySQL 的凭据：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=library_db;User=你的用户名;Password=你的密码;"
  }
}
```

> 注意：`appsettings.Development.json` 已被 `.gitignore` 忽略，不会提交到仓库。

### 3. 运行项目

```bash
dotnet run
```

### 预置账号

| 角色 | 用户名 | 密码 |
|------|--------|------|
| 管理员 | admin | admin123 |
| 读者 | reader | reader123 |

## 课程知识点覆盖

### 课堂知识点
- **MVC 架构模式**：Controller 处理请求、Model 定义数据、View 渲染页面
- **Entity Framework Core**：ORM 映射、LINQ 查询、导航属性、外键关系
- **数据验证**：DataAnnotation（`[Required]`、`[StringLength]`、`[EmailAddress]` 等）
- **表单处理**：`[HttpPost]`、`[ValidateAntiForgeryToken]` 防 CSRF 攻击
- **Session 管理**：基于 Session 的用户认证状态维护
- **Tag Helpers**：`asp-for`、`asp-action`、`asp-controller`、`asp-route-*` 等
- **ViewBag / TempData**：控制器与视图之间的数据传递
- **Partial View**：`_Layout.cshtml` 布局页、`_ValidationScriptsPartial` 验证脚本

### 课外知识点
- **BCrypt 密码加密**：使用 BCrypt.Net 对用户密码进行安全哈希存储
- **ActionFilter 过滤器**：自定义 `AuthFilter` 和 `AdminFilter` 实现权限控制
- **Pomelo MySQL Provider**：使用第三方开源 EF Core MySQL 驱动
- **Bootstrap 5 响应式布局**：Card、Grid、Navbar、Table 等组件
- **Bootstrap Icons**：图标字体库美化界面
- **种子数据初始化**：`DbInitializer` 自动填充示例数据
