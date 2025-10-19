namespace Portfolio.Core.DTOs
{
    public enum SortOrder
    {
        None,
        Ascending,
        Descending
    }

    public class ItemListRequest
    {
        public List<Guid> Ids { get; set; } = [];
        public SortOrder Order { get; set; } = SortOrder.None;
    }
}
