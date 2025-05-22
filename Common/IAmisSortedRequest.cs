namespace ELF.Shared
{
    public interface IAmisSortedRequest: ISortedResultRequest
    {
        string? OrderBy { get; set; }
        string? OrderDir { get; set; }
    }
}
