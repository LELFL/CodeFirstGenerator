namespace ELF.Shared
{
    [Serializable]
    public class AmisPagedRequestDto : IAmisPagedRequest
    {
        public string? _page;
        public string? Page { get => _page == "0" ? "1" : _page; set => _page = value; }

        public string? _perPage;
        public string? PerPage { get => _perPage == "0" ? "0" : _perPage; set => _perPage = value; }

        public int Skip => (Convert.ToInt32(Page) - 1) * Convert.ToInt32(PerPage);

        public int Take => Convert.ToInt32(PerPage);
    }
}
