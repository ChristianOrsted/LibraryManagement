namespace WebProject1.Models;

/// <summary>
/// 错误页面的 ViewModel —— 当系统发生未处理异常时，由 HomeController.Error 传给错误视图
/// 主要用于在错误页面上显示请求 ID，方便排查问题
/// </summary>
public class ErrorViewModel
{
    // 请求 ID：每个 HTTP 请求的唯一标识，可用于在日志中定位具体的出错请求
    public string? RequestId { get; set; }

    // 只读属性（=> 表示表达式体成员）：当 RequestId 不为空时返回 true
    // 视图中用此属性决定是否显示请求 ID 信息
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
