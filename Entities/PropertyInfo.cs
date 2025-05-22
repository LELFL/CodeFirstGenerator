using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstGenerator.Entities
{
    /// <summary>
    /// 属性信息
    /// </summary>
    public class PropertyInfo : IEntity
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(100)]
        public string Description { get; set; } = default!;

        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(100)]
        public string Type { get; set; } = default!;

        /// <summary>
        /// 默认值（可为空）
        /// </summary>
        [StringLength(100)]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int? DecimalDigits { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// 特性
        /// </summary>
        public List<PropertyAttribute> Attributes { get; set; } = new();

        /// <summary>
        /// 实体信息Id
        /// </summary>
        public long EntityInfoId { get; set; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo? EntityInfo { get; set; }

        /// <summary>
        /// 是否引用属性
        /// </summary>
        public bool IsReference { get; set; }

        /// <summary>
        /// 名称(首字母小写)（前端显示用）
        /// </summary>
        [NotMapped]
        public string NameJs { get => ToCamelCase(Name); }


        private string ToCamelCase(string str)
        {
            return string.IsNullOrEmpty(str) ? str :
            char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
