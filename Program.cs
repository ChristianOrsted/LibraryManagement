using Microsoft.EntityFrameworkCore;
using WebProject1.Data;

// ============================
// ASP.NET Core 应用的入口文件
// 负责配置服务（依赖注入）和中间件管道
// ============================

var builder = WebApplication.CreateBuilder(args);

// 注册 MVC 服务：启用 Controller + View（即 MVC 模式）的支持
builder.Services.AddControllersWithViews();

// 配置 EF Core 数据库上下文，使用 Pomelo 驱动连接 MySQL
// 连接字符串从 appsettings.json 的 "ConnectionStrings:DefaultConnection" 读取
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// 配置 Session（会话）：用于在服务端记住用户的登录状态
// DistributedMemoryCache 是 Session 的内存存储后端
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // 30分钟无操作则 Session 过期
    options.Cookie.HttpOnly = true;                   // 防止 JavaScript 访问 Session Cookie（安全措施）
    options.Cookie.IsEssential = true;                // 即使用户未同意 Cookie 策略也发送（GDPR 相关）
});

// 注册 IHttpContextAccessor，允许在非 Controller 的地方访问当前 HTTP 请求上下文
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 应用启动时自动初始化数据库：建表 + 填充种子数据（管理员账号、示例图书等）
// CreateScope() 创建一个临时的依赖注入作用域，用完即释放
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    DbInitializer.Initialize(context);
}

// ============================
// 中间件管道配置（请求按从上到下的顺序经过这些中间件）
// ============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // 生产环境：全局异常处理，出错时跳转到错误页面
    app.UseHsts();                           // 强制浏览器使用 HTTPS（HTTP Strict Transport Security）
}

app.UseHttpsRedirection();  // 将 HTTP 请求自动重定向到 HTTPS
app.UseStaticFiles();       // 启用静态文件服务（wwwroot 目录下的 CSS/JS/图片等）
app.UseRouting();           // 启用路由匹配
app.UseSession();           // 启用 Session 中间件（必须在 UseRouting 之后、UseAuthorization 之前）
app.UseAuthorization();     // 启用授权中间件

// 配置默认路由规则：URL 格式为 /{Controller}/{Action}/{id?}
// 默认访问 HomeController 的 Index 方法
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();  // 启动应用，开始监听 HTTP 请求
