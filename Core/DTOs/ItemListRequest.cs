namespace Portfolio.Core.DTOs
{
    public enum SortOrder
    {
        None = 0,
        Ascending = 1,
        Descending = 2,
    }

    /// <summary>
    ///   <para>Defines a request model for retrieving a list of items by their unique IDs with optional sorting order.</para>
    ///   <para>Includes a SortOrder enum for specifying sorting direction. Default order returns the list in the order the IDs were provided.</para>
    ///   <para>
    ///   Directions:
    ///   <br/>None (default) = 0
    ///   <br/>Ascending = 1
    ///   <br/>Descending = 2
    ///   </para>
    /// </summary>
    public class ItemListRequest
    {
        public List<Guid> Ids { get; set; } = [];
        public SortOrder Order { get; set; } = SortOrder.None;
    }
}
