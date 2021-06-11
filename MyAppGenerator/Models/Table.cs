using System.Collections.Generic;

namespace MyAppGenerator.Models
{
    public class Table
    {
        public Table()
        {
            this.Columns = new List<Column>();
            this.PrimaryKeys = new List<Column>();
            this.ForeignKeys = new Dictionary<string, List<Column>>();
        }

        public string Name { get; set; }
        public string Type { get; set; }

        public string SchemaOwner { get; set; }

        public List<Column> Columns { get; private set; }


        public List<Column> PrimaryKeys { get; private set; }

        public Dictionary<string, List<Column>> ForeignKeys { get; private set; }

        public int ColumnsCount { get; set; }
    }
}
