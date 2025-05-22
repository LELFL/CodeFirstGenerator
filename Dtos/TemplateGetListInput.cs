using CodeFirstGenerator.Entities;
using ELF.Shared;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstGenerator.Dtos
{
    public class TemplateGetListInput : AmisPagedAndSortedRequestDto, IAmisPagedAndSortedRequest
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public TemplateType[]? TemplateType { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 生成路径(如${SlnDir}\Dtos\${EntityFloder}\${EntityName}.cs)
        /// </summary>
        public string? OutputPath { get; set; }

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool? Overwrite { get; set; }

        ///// <summary>
        ///// 最后修改时间
        ///// </summary>
        //public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}