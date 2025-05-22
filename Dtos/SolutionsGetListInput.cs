using ELF.Shared;

namespace CodeFirstGenerator.Dtos
{
    public class SolutionsGetListInput : AmisPagedAndSortedRequestDto, IAmisPagedAndSortedRequest
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string? Description { get; set; }
    }
}