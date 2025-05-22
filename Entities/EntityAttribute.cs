using System.ComponentModel.DataAnnotations;

namespace CodeFirstGenerator.Entities
{
    /// <summary>
    /// 类特性
    /// </summary>
    public class EntityAttribute : IEntity
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
        /// 实体信息
        /// </summary>
        public long EntityInfoId { get; set; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo? EntityInfo { get; set; }
    }
}
