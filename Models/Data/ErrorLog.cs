using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.Data
{
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; } //詳情
        public DateTime Timestamp { get; set; } //時間戳
        public SecurityLevelType SecurityLevel { get; set; } //程度
        public int? UserId { get; set; } //使用者追蹤
        public int? HostId { get; set; } //哪台server

        public string? StackTrace { get; set; } = string.Empty;//哪一段錯誤
        public string? ServicePath { get; set; } = string.Empty;//哪個服務
        public string? RequestPath { get; set; } = string.Empty;//服務的哪個端點
    }
}
