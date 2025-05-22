using ELF.Shared;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstGenerator.Controllers
{
    public class EntityInfoGetListInput : AmisPagedAndSortedRequestDto, IAmisPagedAndSortedRequest
    {

        /// <summary>
        /// using 引用名称
        /// </summary>
        [StringLength(500)]
        public string? Using { get; set; }

        /// <summary>
        /// 命名空间名称
        /// </summary>
        [StringLength(100)]
        public string? Namespace { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        [StringLength(100)]
        public string? ClassName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(100)]
        public string? Description { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [StringLength(500)]
        public string? FilePath { get; set; }

        /// <summary>
        /// 所在文件夹(如/Users)
        /// </summary>
        public string? Folder { get; set; }
    }
}