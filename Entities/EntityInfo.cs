using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstGenerator.Entities
{
    /// <summary>
    /// 实体信息
    /// </summary>
    public class EntityInfo : IEntity
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// using 引用名称
        /// </summary>
        [StringLength(500)]
        public string Using { get; set; } = default!;

        /// <summary>
        /// 命名空间名称
        /// </summary>
        [StringLength(100)]
        public string Namespace { get; set; } = default!;

        /// <summary>
        /// 类名
        /// </summary>
        [StringLength(100)]
        public string ClassName { get; set; } = default!;

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(100)]
        public string Description { get; set; } = default!;

        /// <summary>
        /// 文件路径
        /// </summary>
        [StringLength(500)]
        public string FilePath { get; set; } = default!;

        /// <summary>
        /// 类特性
        /// </summary>
        public List<EntityAttribute> Attributes { get; set; } = new();

        /// <summary>
        /// 属性集合
        /// </summary>
        public List<PropertyInfo> Properties { get; set; } = new();

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 原始代码
        /// </summary>
        public string? OriginalCode { get; set; }

        /// <summary>
        /// 所在文件夹(如/Users)
        /// </summary>
        public string Folder { get; set; } = default!;

        /// <summary>
        /// 忽略属性
        /// </summary>
        public string? IgnoreProperties { get; set; }

        [NotMapped]
        public string ClassNameJs { get => ToCamelCase(ClassName); }


        private string ToCamelCase(string str)
        {
            return string.IsNullOrEmpty(str) ? str :
            char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
