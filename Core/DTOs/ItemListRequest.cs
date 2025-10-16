namespace Portfolio.Core.DTOs
{
    public class ItemListRequest
    {
        public List<Guid> Ids { get; set; } = [];
        public bool IsDescending { get; set; } = false;

    }
}
