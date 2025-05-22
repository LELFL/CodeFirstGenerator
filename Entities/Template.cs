using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstGenerator.Entities
{
    /// <summary>
    /// 模板信息
    /// </summary>
    public class Template : IEntity
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// 模板类型
        /// </summary>
        [StringLength(500)]
        public TemplateType TemplateType { get; set; } = TemplateType.Razor;

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; } = default!;

        /// <summary>
        /// 生成路径(如${SlnDir}\Dtos\${EntityFloder}\${EntityName}.cs)
        /// </summary>
        public string OutputPath { get; set; } = default!;

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool Overwrite { get; set; } = true;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Razor模板ID
        /// </summary>
        public long RazorId { get; internal set; }

        //public virtual List<Solutions> Solutionss { get; set; } = new();
    }

    /// <summary>
    /// 模板类型
    /// </summary>
    public enum TemplateType
    {
        Razor = 1,
        CSharpScript = 2,
        JavaScript = 3,
    }
}
