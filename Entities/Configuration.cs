using System.ComponentModel.DataAnnotations;

namespace CodeFirstGenerator.Entities
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class Configuration : IEntity
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 解决方案路径
        /// </summary>
        public string? SlnDir { get; set; } = default!;

        /// <summary>
        /// 枚举类型，多个用逗号分隔
        /// </summary>
        public string? EnumTypes { get; set; }
    }
}
