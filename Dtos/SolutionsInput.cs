using CodeFirstGenerator.Entities;

namespace CodeFirstGenerator.Dtos
{
    public class SolutionsInput
    {
        public string Name { get; set; } = default!;
        /// <summary>
        /// 描述信息
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 模板列表
        /// </summary>
        public string? TemplateIds { get; set; }
    }
}