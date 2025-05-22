namespace ELF.Shared
{
    public interface IAmisPagedRequest
    {
        string? Page { get; set; }
        string? PerPage { get; set; }
        int Skip { get; }
        int Take { get; }
    }
}
