using WebProject1.Models;

namespace WebProject1.Data;

/// <summary>
/// 数据库种子数据初始化器
/// 应用首次启动时，自动创建数据库表并填充初始数据（管理员账号、示例图书等）
/// 这样开发和演示时不需要手动插入数据
/// </summary>
public static class DbInitializer
{
    public static void Initialize(LibraryDbContext context)
    {
        // 如果数据库不存在，则根据 DbContext 中的实体定义自动建表
        context.Database.EnsureCreated();

        // 如果已经有用户数据，说明种子数据已初始化过，直接跳过
        if (context.Users.Any()) return;

        // 创建管理员账号（密码: admin123，用 BCrypt 哈希存储）
        var admin = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Email = "admin@library.com",
            Role = "Admin",
            CreatedAt = DateTime.Now
        };

        // 创建普通读者账号（密码: reader123）
        var reader = new User
        {
            Username = "reader",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("reader123"),
            Email = "reader@library.com",
            Role = "Reader",
            CreatedAt = DateTime.Now
        };

        context.Users.AddRange(admin, reader);

        // 初始化图书分类
        var categories = new Category[]
        {
            new() { Name = "文学", Description = "小说、散文、诗歌等文学作品" },
            new() { Name = "科技", Description = "计算机、人工智能、工程技术等" },
            new() { Name = "历史", Description = "中国历史、世界历史、人物传记" },
            new() { Name = "哲学", Description = "中西方哲学、逻辑学、伦理学" },
            new() { Name = "教育", Description = "教材、考试辅导、学术论文" },
            new() { Name = "艺术", Description = "绘画、音乐、摄影、设计" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();

        // 初始化示例图书数据
        var books = new Book[]
        {
            new() { Title = "三体", Author = "刘慈欣", ISBN = "978-7-5366-9293-0", Publisher = "重庆出版社", PublishYear = 2008, Description = "地球往事三部曲之一，讲述地球人类文明和三体文明的信息交流、生死搏杀。", CategoryId = categories[0].Id },
            new() { Title = "活着", Author = "余华", ISBN = "978-7-5063-3074-3", Publisher = "作家出版社", PublishYear = 1993, Description = "讲述了农村人福贵悲惨的人生遭遇。", CategoryId = categories[0].Id },
            new() { Title = "深入理解计算机系统", Author = "Randal E. Bryant", ISBN = "978-7-111-54493-7", Publisher = "机械工业出版社", PublishYear = 2016, Description = "从程序员的视角详细阐述计算机系统的本质概念。", CategoryId = categories[1].Id },
            new() { Title = "算法导论", Author = "Thomas H. Cormen", ISBN = "978-7-111-40701-0", Publisher = "机械工业出版社", PublishYear = 2012, Description = "全面介绍了算法的设计与分析方法。", CategoryId = categories[1].Id },
            new() { Title = "人类简史", Author = "尤瓦尔·赫拉利", ISBN = "978-7-5086-4456-6", Publisher = "中信出版社", PublishYear = 2014, Description = "从十万年前有生命迹象开始到21世纪资本、科技交织的人类发展史。", CategoryId = categories[2].Id },
            new() { Title = "万历十五年", Author = "黄仁宇", ISBN = "978-7-108-00982-4", Publisher = "三联书店", PublishYear = 1997, Description = "以1587年为切入点，分析明代社会的症结。", CategoryId = categories[2].Id },
            new() { Title = "苏菲的世界", Author = "乔斯坦·贾德", ISBN = "978-7-5063-1122-3", Publisher = "作家出版社", PublishYear = 1999, Description = "以小说形式讲述西方哲学史。", CategoryId = categories[3].Id },
            new() { Title = "C# 高级编程", Author = "Christian Nagel", ISBN = "978-7-302-50262-0", Publisher = "清华大学出版社", PublishYear = 2018, Description = "全面讲解 C# 语言及 .NET 框架开发技术。", CategoryId = categories[1].Id },
        };
        context.Books.AddRange(books);
        context.SaveChanges();

        // 为每本书随机生成 2~4 个物理副本，并随机分配馆内位置
        var copies = new List<BookCopy>();
        string[] locations = { "A区-1排", "A区-2排", "B区-1排", "B区-2排", "C区-1排" };
        foreach (var book in books)
        {
            int copyCount = Random.Shared.Next(2, 5); // 每本书 2~4 个副本
            for (int i = 0; i < copyCount; i++)
            {
                copies.Add(new BookCopy
                {
                    BookId = book.Id,
                    Status = "Available",
                    Location = locations[Random.Shared.Next(locations.Length)]
                });
            }
        }
        context.BookCopies.AddRange(copies);
        context.SaveChanges();
    }
}
