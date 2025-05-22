using System.ComponentModel.DataAnnotations;

namespace CodeFirstGenerator.Entities
{

    /// <summary>
    /// 属性特性
    /// </summary>
    public class PropertyAttribute : IEntity
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 参数
        /// </summary>
        [StringLength(500)]
        public string Arguments { get; set; } = "";
        
        /// <summary>
        /// 属性信息ID
        /// </summary>
        public long PropertyInfoId { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo? PropertyInfo { get; set; }
    }
}
