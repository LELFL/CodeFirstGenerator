namespace ELF.Shared
{
    public class AmisPagedAndSortedRequestDto : AmisPagedRequestDto, IAmisPagedAndSortedRequest
    {
        public string? Sorting
        {
            get
            {
                if (string.IsNullOrEmpty(OrderBy)) return null;
                if (string.IsNullOrEmpty(OrderDir)) OrderDir = "asc";
                return OrderBy + " " + OrderDir;
            }
            set { }
        }
        public string? OrderBy { get; set; }
        public string? OrderDir { get; set; }
    }
}
