using RazorEngine.Templating;

namespace CodeFirstGenerator.Entities
{

    /// <summary>
    /// 生成方案
    /// </summary>
    public class Solutions : IEntity
    {
        public long Id { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 描述信息
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 模板列表
        /// </summary>
        public virtual List<Template> Templates { get; set; } = new();

        public string TemplateIds => string.Join(",", Templates.Select(x => x.Id));
    }
}
