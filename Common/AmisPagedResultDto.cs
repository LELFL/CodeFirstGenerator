namespace ELF.Shared
{
    [Serializable]
    public class AmisPagedResultDto<T>
    {
        private IReadOnlyList<T>? _items;

        public AmisPagedResultDto(int totalCount, List<T> entityDtos)
        {
            Total = totalCount;
            Items = entityDtos;
        }

        public IReadOnlyList<T> Items
        {
            get
            {
                return _items ?? (_items = new List<T>());
            }
            set
            {
                _items = value;
            }
        }
        public long Total { get; set; }
    }
}
