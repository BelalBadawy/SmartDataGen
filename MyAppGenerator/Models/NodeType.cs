namespace MyAppGenerator.Models
{
    public enum SchemaTypeEnum
    {
        Table,
        View,
        StoredProcedure
    }

    public class NodeType
    {
        public string Title { get; set; }
        public SchemaTypeEnum Type { get; set; }
    }
}
