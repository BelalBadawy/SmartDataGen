using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyAppGenerator.Models;

namespace MyAppGenerator.Helpers
{
    public static class UtilityHelper
    {
        public static async Task<string> GetTranslationAsync(string textToTranslate, string langToTranslate)
        {
            string translation = textToTranslate;
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl={langToTranslate}&dt=t&q={textToTranslate}";
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var translationString = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(translationString))
                        {
                            var translationList = translationString.Split(',').ToList();
                            translation = translationList[0].Replace("[", "").Replace("\"", "");
                        }
                    }
                }
            }

            return translation;
        }

        public static string GetIDKeyType(AppSetting appSetting)
        {
            switch (appSetting.KeyType)
            {
                //int
                case 0:
                    return "int";
                //Uniqueidentifier
                case 1:
                    return "Guid";
            }

            return "int";
        }

        public static class Constants
        {
            public const string Tables = "Tables";
            public const string Views = "Views";
            public const string TableValuedFunctions = "Table Valued Functions";
            public const string UserDefinedTableTypes = "User-Defined Table Types";
            public const string StoredProcedures = "Stored Procedures";

            public const string Contains = "Contains";
            public new const string Equals = "Equals";
            public const string DoesNotContain = "Does not Contain";

            public const string FilteredText = " (filtered)";

        }

        public static string ConnectionString { get; set; }
        public static List<Table> Tables = new List<Table>();
        public static List<Table> TableList = new List<Table>();
        public static List<Table> ViewList = new List<Table>();
        public static List<Table> SpList = new List<Table>();


        public static string CreateMethodParameter(Column column)
        {
            return GetCsType(column) + " " + FormatCamel(column.Name); ;

        }

        public static void WriteToFile(string filePath, string fileName, string text)
        {
            //  string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            string path = Path.Combine(filePath, fileName);
            var fs = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            var sw = new System.IO.StreamWriter(fs);
            sw.WriteLine(text);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public static string ReadFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {

            }

            return null;
        }

        public static void CreateSubDirectory(string name)
        {
            if (Directory.Exists(name) == false)
            {
                Directory.CreateDirectory(name);
            }
        }

        public static void CreateSubDirectory(string name, bool deleteIfExists)
        {
            if (Directory.Exists(name))
            {
                //   Directory.Delete(name, true);
            }
            else
            {
                Directory.CreateDirectory(name);
            }
        }

        public static string ReadCustomRegionText(string filePath)
        {
            StringBuilder sb = new StringBuilder();


            if (File.Exists(filePath))
            {

                var lines = File.ReadAllLines(filePath);
                bool startCustomLines = false;
                foreach (var line in lines)
                {
                    if (startCustomLines)
                    {
                        sb.AppendLine(line);
                    }

                    if (line.Contains("#region Custom"))
                    {
                        startCustomLines = true;
                        sb.AppendLine(line);
                    }

                    if (line.Contains("#endregion Custom"))
                    {
                        startCustomLines = false;
                        //   sb.AppendLine(line);
                    }

                }

                File.Delete(filePath);
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("#region Custom");
                sb.AppendLine("#endregion Custom");
            }

            return sb.ToString();
        }


        static readonly List<string> ReservedKeywords = new List<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
            "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
            "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed",
            "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator",
            "out", "override", "params", "private", "protected", "public", "readonly", "ref",
            "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "volatile", "void", "while"
        };

        //public static string MakePlural(string word)
        //{
        //    try
        //    {

        //        if (string.IsNullOrEmpty(word))
        //            return string.Empty;



        //        if (word.Contains('_')) return MakePluralHelper(word, '_');
        //        if (word.Contains(' ')) return MakePluralHelper(word, ' ');
        //        if (word.Contains('-')) return MakePluralHelper(word, '-');

        //        var serv = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));

        //        return serv.Pluralize(word);
        //    }
        //    catch (Exception)
        //    {
        //        return word;
        //    }
        //}

        //private static string MakePluralHelper(string word, char split)
        //{
        //    if (string.IsNullOrEmpty(word))
        //        return string.Empty;
        //    var parts = word.Split(split);
        //    var serv = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));

        //    parts[parts.Length - 1] = serv.Pluralize(parts[parts.Length - 1]); // Pluralize just the last word
        //    return string.Join(split.ToString(), parts);
        //}


        public static string GetSelectedTablesAndViews(string database, List<string> tables)
        {
            string sqlTables;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i] != "Tables" && tables[i] != "Views" && tables[i] != database)
                    sb.AppendFormat("'{0}',", tables[i]);
            }


            if (sb.Length == 0)
            {
                sqlTables = string.Format(@"select TABLE_CATALOG,TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
												from INFORMATION_SCHEMA.TABLES
												  where (TABLE_TYPE = 'BASE TABLE'  or TABLE_TYPE ='VIEW')
												and (TABLE_NAME != 'dtProperties' and TABLE_NAME != 'sysdiagrams')
												and TABLE_CATALOG = '{0}'
												order by TABLE_NAME", database);

            }
            else
            {
                sqlTables = string.Format(@"select TABLE_CATALOG,TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
												from INFORMATION_SCHEMA.TABLES
												 where (TABLE_TYPE = 'BASE TABLE'  or TABLE_TYPE ='VIEW')
												and (TABLE_NAME != 'dtProperties' and TABLE_NAME != 'sysdiagrams')
                                                and  (TABLE_NAME in ({0}))
												and TABLE_CATALOG = '{1}'
												order by TABLE_NAME", sb.ToString().Substring(0, sb.Length - 1), database);
            }

            return sqlTables;

        }


        public static void QueryTable(SqlConnection connection, Table table)
        {
            // Get a list of the entities in the database
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(GetColumnsQuery(table.Name), connection);
            dataAdapter.Fill(dataTable);

            foreach (DataRow columnRow in dataTable.Rows)
            {
                Column column = new Column();
                column.Name = columnRow["COLUMN_NAME"].ToString();
                column.Type = columnRow["DATA_TYPE"].ToString();
                column.Precision = columnRow["NUMERIC_PRECISION"].ToString();
                column.Scale = columnRow["NUMERIC_SCALE"].ToString();

                // Determine the column's length
                if (columnRow["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                {
                    column.Length = columnRow["CHARACTER_MAXIMUM_LENGTH"].ToString();
                }
                else
                {
                    column.Length = columnRow["COLUMN_LENGTH"].ToString();
                }

                // Is the column a RowGuidCol column?
                if (columnRow["IS_ROWGUIDCOL"].ToString() == "1")
                {
                    column.IsRowGuidCol = true;
                }

                // Is the column an Identity column?
                if (columnRow["IS_IDENTITY"].ToString() == "1")
                {
                    column.IsIdentity = true;
                }

                // Is columnRow column a computed column?
                if (columnRow["IS_COMPUTED"].ToString() == "1")
                {
                    column.IsComputed = true;
                }

                if (columnRow["IS_NULLABLE"].ToString() == "NO")
                {
                    column.IsNullable = false;
                }
                else
                {
                    column.IsNullable = true;

                }

                table.Columns.Add(column);
            }

            // Get the list of primary keys
            DataTable primaryKeyTable = GetPrimaryKeyList(connection, table.Name);
            foreach (DataRow primaryKeyRow in primaryKeyTable.Rows)
            {
                string primaryKeyName = primaryKeyRow["COLUMN_NAME"].ToString();

                foreach (Column column in table.Columns)
                {
                    if (column.Name == primaryKeyName)
                    {
                        table.PrimaryKeys.Add(column);
                        break;
                    }
                }
            }

            // Get the list of foreign keys
            DataTable foreignKeyTable = GetForeignKeyList(connection, table.Name);
            foreach (DataRow foreignKeyRow in foreignKeyTable.Rows)
            {
                string name = foreignKeyRow["FK_NAME"].ToString();
                string pkColumnName = foreignKeyRow["PKCOLUMN_NAME"].ToString();
                string columnName = foreignKeyRow["FKCOLUMN_NAME"].ToString();
                string fkTableName = foreignKeyRow["FKTABLE_NAME"].ToString();
                string pkTableName = foreignKeyRow["PKTABLE_NAME"].ToString();

                if (table.ForeignKeys.ContainsKey(name) == false)
                {
                    table.ForeignKeys.Add(name, new List<Column>());
                }

                List<Column> foreignKeys = table.ForeignKeys[name];

                foreach (Column column in table.Columns)
                {
                    if (column.Name == columnName)
                    {
                        column.PrimaryKeyTableName = pkTableName;
                        column.ForeignKeyTableName = fkTableName;
                        column.PrimaryKeyColumnName = pkColumnName;
                        column.ForeignKeyColumnName = columnName;
                        foreignKeys.Add(column);
                        break;
                    }
                }

            }




        }

        public static DataTable GetPrimaryKeyList(SqlConnection connection, string tableName)
        {
            SqlParameter parameter;

            using (SqlCommand command = new SqlCommand("sp_pkeys", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                parameter = new SqlParameter("@table_name", SqlDbType.NVarChar, 128, ParameterDirection.Input, false, 0, 0, "table_name", DataRowVersion.Current, tableName);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@table_owner", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "table_owner", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "table_qualifier", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                return dataTable;
            }
        }

        public static DataTable GetForeignKeyList(SqlConnection connection, string tableName)
        {
            SqlParameter parameter;

            using (SqlCommand command = new SqlCommand("sp_fkeys", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                parameter = new SqlParameter("@pktable_name", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "pktable_name", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@pktable_owner", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "pktable_owner", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@pktable_qualifier", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "pktable_qualifier", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@fktable_name", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "fktable_name", DataRowVersion.Current, tableName);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@fktable_owner", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "fktable_owner", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@fktable_qualifier", SqlDbType.NVarChar, 128, ParameterDirection.Input, true, 0, 0, "fktable_qualifier", DataRowVersion.Current, DBNull.Value);
                command.Parameters.Add(parameter);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                return dataTable;
            }
        }
        public static string GetColumnsQuery(string tableName)
        {
            return string.Format(@"select INFORMATION_SCHEMA.COLUMNS.*,
	                             COL_LENGTH('{0}', INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME) AS COLUMN_LENGTH,
	                            COLUMNPROPERTY(OBJECT_ID('{0}'), INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, 'IsComputed') as IS_COMPUTED,
	                            COLUMNPROPERTY(OBJECT_ID('{0}'), INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, 'IsIdentity') as IS_IDENTITY,
	                            COLUMNPROPERTY(OBJECT_ID('{0}'), INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, 'IsRowGuidCol') as IS_ROWGUIDCOL
                                from INFORMATION_SCHEMA.COLUMNS
                                where INFORMATION_SCHEMA.COLUMNS.TABLE_NAME = '{0}' AND INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME NOT IN (
								''
								)", tableName);
        }

        /// <summary>
        /// Makes the plural.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakePlural(string word)
        {
            try
            {
                var pluralizationService = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));

                if (string.IsNullOrEmpty(word))
                    return string.Empty;

                if (pluralizationService == null)
                    return word;

                if (word.Contains('_')) return MakePluralHelper(word, '_');
                if (word.Contains(' ')) return MakePluralHelper(word, ' ');
                if (word.Contains('-')) return MakePluralHelper(word, '-');

                return pluralizationService.Pluralize(word);
            }
            catch (Exception)
            {
                return word;
            }
        }

        private static string MakePluralHelper(string word, char split)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            var pluralizationService = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));


            var parts = word.Split(split);
            parts[parts.Length - 1] = pluralizationService.Pluralize(parts[parts.Length - 1]); // Pluralise just the last word
            return string.Join(split.ToString(), parts);
        }

        /// <summary>
        /// Makes the singular.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeSingular(string word)
        {
            try
            {
                if (string.IsNullOrEmpty(word))
                    return string.Empty;

                var pluralizationService = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));


                if (pluralizationService == null)
                    return word;

                if (word.Contains('_')) return MakeSingularHelper(word, '_');
                if (word.Contains(' ')) return MakeSingularHelper(word, ' ');
                if (word.Contains('-')) return MakeSingularHelper(word, '-');

                return pluralizationService.Singularize(word);
            }
            catch (Exception)
            {
                return word;
            }
        }

        private static string MakeSingularHelper(string word, char split)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            var pluralizationService = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));


            var parts = word.Split(split);
            parts[parts.Length - 1] = pluralizationService.Singularize(parts[parts.Length - 1]); // Pluralise just the last word
            return string.Join(split.ToString(), parts);
        }

        public static string FormatCamel(string original)
        {
            if (original.Length > 0)
            {
                return Char.ToLower(original[0]) + original.Substring(1);
            }
            else
            {
                return String.Empty;
            }
        }



        public static string FormatPascal(string original)
        {
            if (original.Length > 0)
            {
                return Char.ToUpper(original[0]) + original.Substring(1);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Converts the string to title case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string ToTitleCase(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            var s = Regex.Replace(ToHumanCase(AddUnderscores(word)), @"\b([a-z])",
                match => match.Captures[0].Value.ToUpperInvariant());
            var digit = false;
            var sb = new StringBuilder(word.Length + 1);
            foreach (var c in s)
            {
                if (char.IsDigit(c))
                {
                    digit = true;
                    sb.Append(c);
                }
                else
                {
                    if (digit && char.IsLower(c))
                        sb.Append(char.ToUpperInvariant(c));
                    else
                        sb.Append(c);
                    digit = false;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts the string to human case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        public static string ToHumanCase(string lowercaseAndUnderscoredWord)
        {
            if (string.IsNullOrEmpty(lowercaseAndUnderscoredWord))
                return string.Empty;

            return MakeInitialCaps(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }


        /// <summary>
        /// Adds the underscores.
        /// </summary>
        /// <param name="pascalCasedWord">The pascal cased word.</param>
        /// <returns></returns>
        public static string AddUnderscores(string pascalCasedWord)
        {
            if (string.IsNullOrEmpty(pascalCasedWord))
                return string.Empty;

            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"),
                    @"([a-z\d])([A-Z])", "$1_$2"),
                @"[-\s]", "_")
                .ToLowerInvariant();
        }

        /// <summary>
        /// Makes the initial caps.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialCaps(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            return string.Concat(word.Substring(0, 1).ToUpperInvariant(), word.Substring(1).ToLowerInvariant());
        }

        /// <summary>
        /// Makes the initial character lowercase.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialLower(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            return string.Concat(word.Substring(0, 1).ToLowerInvariant(), word.Substring(1));
        }

        public static string MakeLowerIfAllCaps(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            return IsAllCaps(word) ? word.ToLowerInvariant() : word;
        }

        public static bool IsAllCaps(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            return word.All(char.IsUpper);
        }

        public static string ToDisplayName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            var sb = new StringBuilder(30);
            str = Regex.Replace(str, @"[^a-zA-Z0-9]", " "); // Anything that is not a letter or digit, convert to a space
            str = Regex.Replace(str, @"[A-Z]{2,}", " $+ "); // Any word that is upper case

            var hasUpperCased = false;
            var lastChar = '\0';
            foreach (var original in str.Trim())
            {
                var c = original;
                if (lastChar == '\0')
                {
                    c = char.ToUpperInvariant(original);
                }
                else
                {
                    var isLetter = char.IsLetter(original);
                    var isDigit = char.IsDigit(original);
                    var isWhiteSpace = !isLetter && !isDigit;

                    // Is this char is different to last time
                    var isDifferent = false;
                    if (isLetter && !char.IsLetter(lastChar))
                        isDifferent = true;
                    else if (isDigit && !char.IsDigit(lastChar))
                        isDifferent = true;
                    else if (char.IsUpper(original) && !char.IsUpper(lastChar))
                        isDifferent = true;

                    if (isDifferent || isWhiteSpace)
                        sb.Append(' '); // Add a space

                    if (hasUpperCased && isLetter)
                        c = char.ToLowerInvariant(original);
                }
                lastChar = original;
                if (!hasUpperCased && char.IsUpper(c))
                    hasUpperCased = true;
                sb.Append(c);
            }
            str = sb.ToString();
            str = Regex.Replace(str, @"\s+", " ").Trim(); // Multiple white space to one space
            str = Regex.Replace(str, @"\bid\b", "ID"); //  Make ID word uppercase
            return str;
        }

        public static string GetCsType(Column column)
        {
            switch (column.Type.ToLower())
            {
                case "binary":
                    return column.IsNullable ? "byte?[]" : "byte[]";
                case "bigint":

                    return column.IsNullable ? "long?" : "long";
                case "bit":
                    return column.IsNullable ? "bool?" : "bool";
                case "char":
                    return "string";
                case "date":
                case "datetime":
                case "datetime2":
                case "datetimeoffset":
                case "smalldatetime":
                    return column.IsNullable ? "DateTime?" : "DateTime";
                case "decimal":
                    return column.IsNullable ? "decimal?" : "decimal";
                case "filestream":
                    return column.IsNullable ? "byte?[]" : "byte[]";
                case "float":
                    return column.IsNullable ? "double?" : "double";
                case "image":
                    return column.IsNullable ? "byte?[]" : "byte[]";
                case "int":
                    return column.IsNullable ? "int?" : "int";
                case "money":
                    return column.IsNullable ? "decimal?" : "decimal";
                case "nchar":
                    return "string";
                case "ntext":
                    return "string";
                case "nvarchar":
                    return "string";
                case "numeric":
                    return column.IsNullable ? "decimal?" : "decimal";
                case "real":
                    return column.IsNullable ? "decimal?" : "decimal";

                case "smallint":
                    return column.IsNullable ? "short?" : "short";
                case "smallmoney":
                    return column.IsNullable ? "decimal?" : "decimal";

                case "sql_variant":
                case "rowversion":
                    return column.IsNullable ? "byte?[]" : "byte[]";

                case "sysname":
                    return "string";
                case "text":
                    return "string";
                case "timestamp":
                    return column.IsNullable ? "DateTime?" : "DateTime";
                case "tinyint":
                    return column.IsNullable ? "byte?" : "byte";
                case "varbinary":
                    return column.IsNullable ? "byte?[]" : "byte[]";

                case "varchar":
                    return "string";
                case "uniqueidentifier":
                    return column.IsNullable ? "Guid?" : "Guid";
                case "xml":
                    return "string";
                default:  // Unknow data type
                    throw (new Exception("Invalid SQL Server data type specified: " + column.Type));
            }
        }

        public static string GetSqlDbType(string sqlDbType)
        {
            switch (sqlDbType.ToLower())
            {
                case "binary":
                    return "Binary";
                case "bigint":
                    return "BigInt";
                case "bit":
                    return "Bit";
                case "char":
                    return "Char";
                case "date":
                case "datetime":
                case "datetime2":
                case "datetimeoffset":

                    return "DateTime";
                case "decimal":
                    return "Decimal";
                case "float":
                    return "Float";
                case "image":
                    return "Image";
                case "int":
                    return "Int";
                case "money":
                    return "Money";
                case "nchar":
                    return "NChar";
                case "ntext":
                    return "NText";
                case "nvarchar":
                    return "NVarChar";
                case "numeric":
                    return "Decimal";
                case "real":
                    return "Real";
                case "smalldatetime":
                    return "SmallDateTime";
                case "smallint":
                    return "SmallInt";
                case "smallmoney":
                    return "SmallMoney";
                case "sql_variant":
                    return "Variant";
                case "sysname":
                    return "VarChar";
                case "text":
                    return "Text";
                case "timestamp":
                    return "Timestamp";
                case "tinyint":
                    return "TinyInt";
                case "varbinary":
                    return "VarBinary";
                case "varchar":
                    return "VarChar";
                case "uniqueidentifier":
                    return "UniqueIdentifier";
                case "xml":
                    return "Xml";
                default:  // Unknow data type
                    throw (new Exception("Invalid SQL Server data type specified: " + sqlDbType));
            }
        }

        public static string FormatVariableName(string tableName)
        {
            string variableName;

            if (Char.IsLower(tableName[0]))
            {
                variableName = tableName;
            }
            else
            {
                variableName = FormatCamel(tableName);
            }

            // Attept to removing a trailing 's' or 'S', unless, the last two characters are both 's' or 'S'.
            if (variableName[variableName.Length - 1] == 'S' && variableName[variableName.Length - 2] != 'S')
            {
                variableName = variableName.Substring(0, variableName.Length - 1);
            }
            else if (variableName[variableName.Length - 1] == 's' && variableName[variableName.Length - 2] != 's')
            {
                variableName = variableName.Substring(0, variableName.Length - 1);
            }

            if (variableName == "bool"
                || variableName == "byte"
                || variableName == "char"
                || variableName == "decimal"
                || variableName == "double"
                || variableName == "float"
                || variableName == "long"
                || variableName == "object"
                || variableName == "sbyte"
                || variableName == "short"
                || variableName == "string"
                || variableName == "uint"
                || variableName == "ulong"
                || variableName == "ushort")
            {
                variableName += "Value";
            }

            return variableName;
        }

        public static string CreateBuilderPropertyForEntityFramework(Column column)
        {
            StringBuilder sb = new StringBuilder();



            string propLower = column.Name.ToLower();


            if (column.IsIdentity == false)
            {

                sb.Append("builder.Property(t => t." + column.Name + ")");
                sb.Append(".HasColumnName(\"" + column.Name + "\")");
                if (column.Name.ToUpper() != "RowVersion".ToUpper())
                {
                    if (column.Type == "char" || column.Type == "varchar" || column.Type == "nchar" ||
                        column.Type == "nvarchar")
                    {
                        sb.Append(".HasColumnType(\"" + column.Type +
                                  (column.Length == "-1" ? "(max)" : "(" + column.Length + ")") + "\")");
                        if (column.Type == "char" || column.Type == "varchar")
                        {
                            sb.Append(".HasMaxLength" + (column.Length == "-1" ? "(8000)" : "(" + column.Length + ")"));
                        }
                        else if (column.Type == "nchar" || column.Type == "nvarchar")
                        {
                            sb.Append(".HasMaxLength" + (column.Length == "-1" ? "(4000)" : "(" + column.Length + ")"));
                        }
                    }
                    else
                    {
                        sb.Append(".HasColumnType(\"" + column.Type + "\")");
                    }

                    if (column.IsNullable == false)
                    {
                        sb.Append(".IsRequired()");
                    }
                }
                else
                {
                    sb.Append(".IsConcurrencyToken().ValueGeneratedOnAddOrUpdate()");
                }

                sb.Append(";");
            }



            return sb.ToString();
        }

        public static string CreateBuilderPropertyForGlobalFilterEntityFrameworkHasKey(List<Column> columns)
        {
            if (columns.Any(o => o.Name.Contains("IsDeleted")))
                return " builder.HasQueryFilter(p => !p.IsDeleted);";

            return "";

        }
        public static string CreateBuilderPropertyForEntityFrameworkHasKey(List<Column> columns)
        {
            StringBuilder sb = new StringBuilder();

            if (columns.Count == 1)
            {
                Column column = columns.FirstOrDefault();

                sb.Append("builder.HasKey(t => t." + column.Name + ");");

                sb.Append("builder.Property(t => t." + column.Name + ")");
                sb.Append(".HasColumnName(\"" + column.Name + "\")");
                sb.Append(".HasColumnType(\"" + column.Type + "\")");
                sb.Append(".IsRequired()");
                if (column.IsIdentity)
                {
                    sb.Append((column.IsIdentity ? ".ValueGeneratedOnAdd(); " : "; "));
                }
                else
                {
                    sb.Append("; ");
                }

                return sb.ToString();

            }
            else if (columns.Count > 1)
            {
                //sb.Append(("builder.HasKey(t => new { t.BookId, t.BookCategoryId });");
                sb.Append("builder.HasKey(t => new {");
                foreach (var c in columns)
                {
                    sb.Append("t." + c.Name + " ,");
                }


                return sb.ToString().Substring(0, sb.Length - 1) + "});";
            }


            return "";

        }

        public static string CreateDataAnnotationParameter(bool dataAnnotation, Column column, Dictionary<string, List<Column>> foriegnKeysList, bool useResourceFile, string resourceFileNameSpace, string tableName)
        {
            //if (column.IsNullable && !stringSqlType.Contains(column.Type.ToLower()))
            //{
            //    return GetCsType(column) + "? " + FormatCamel(column.Name); ;
            //}
            //else
            //{
            //    return GetCsType(column) + " " + FormatCamel(column.Name); ;
            //}


            StringBuilder sb = new StringBuilder();

            sb.AppendLine();

            string propLower = column.Name.ToLower();

            if (!dataAnnotation)
            {
                if (column.Type == "date" || column.Type == "datetime" || column.Type == "datetime2" || column.Type == "datetimeoffset" || column.Type == "smalldatetime" || column.Type == "time")
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.DateTime)]");
                }

                if (propLower.Contains("phone") || propLower.Contains("telephone"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.PhoneNumber)]");
                }
                if (propLower.Contains("html") || propLower.Contains("content"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.Html)]");
                }

                if (propLower.Contains("email"))
                {
                    if (column.Type == "char" || column.Type == "varchar" || column.Type == "nchar" ||
                        column.Type == "nvarchar")
                    {
                        sb.AppendLine();
                        sb.Append("\t\t[DataType(DataType.EmailAddress)]");
                        sb.AppendLine();


                        //sb.Append("[RegularExpression(\"^[a - z0 - 9_\\\\+-] + (\\\\.[a - z0 - 9_\\\\+-] +)*@[a-z0-9-]+(\\\\.[a-z0-9]+)*\\\\.([a-z]{2,4})$\", ErrorMessage = \"Invalid email format.\")]");

                        if (useResourceFile)
                        {
                            sb.AppendLine("[EmailAddress(ErrorMessageResourceType = typeof(" + resourceFileNameSpace + "), ErrorMessageResourceName = \"EmailAddress\")]");
                        }
                        else
                        {
                            sb.AppendLine("[EmailAddress(ErrorMessage = \"Invalid Email Address\")]");
                        }

                    }
                }

                if (propLower.Contains("password"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.Password)]");
                }

            }
            else
            {

                if (useResourceFile)
                {
                    //[Display(Name="Active", ResourceType = typeof(Resources.DataEntitiesResource))]
                    sb.Append("\t\t[Display(Name=\"" + tableName + "_" + column.Name + "\", ResourceType = typeof(" + resourceFileNameSpace + "))]");
                }
                else
                {
                    sb.Append("\t\t[Display(Name=\"" + FixName(column.Name) + "\")]");
                }


                if (column.IsNullable == false)
                {
                    sb.AppendLine();
                    if (useResourceFile)
                    {
                        sb.Append("\t\t[Required(ErrorMessageResourceType = typeof(" + resourceFileNameSpace + "), ErrorMessageResourceName = \"Required\")]");
                    }
                    else
                    {
                        sb.Append("\t\t[Required(ErrorMessage=\"" + FixName(column.Name) + " is required\")]");
                    }
                }

                if (column.Type == "char" || column.Type == "varchar" || column.Type == "nchar" || column.Type == "nvarchar")
                {
                    sb.AppendLine();
                    if (useResourceFile)
                    {
                        sb.Append("\t\t[StringLength(" + (column.Length == "-1" ? "4000" : column.Length) +
                                  ", ErrorMessageResourceType = typeof(" + resourceFileNameSpace +
                                  "), ErrorMessageResourceName = \"StringLength\")]");
                    }
                    else
                    {
                        sb.Append("\t\t[StringLength(" + (column.Length == "-1" ? "4000" : column.Length) +
                                  ",ErrorMessage = \"" + FixName(column.Name) + " must be less than " +
                                  (column.Length == "-1" ? "1000000" : column.Length) + " characters\")]");
                    }
                }

                if (column.Type == "date" || column.Type == "datetime" || column.Type == "datetime2" || column.Type == "datetimeoffset" || column.Type == "smalldatetime" || column.Type == "time")
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.DateTime)]");
                }

                if (propLower.Contains("phone") || propLower.Contains("telephone"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.PhoneNumber)]");
                }
                if (propLower.Contains("html") || propLower.Contains("content"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.Html)]");
                }

                if (propLower.Contains("email"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.EmailAddress)]");
                    sb.AppendLine();
                    //sb.Append("[RegularExpression(\"^[a - z0 - 9_\\\\+-] + (\\\\.[a - z0 - 9_\\\\+-] +)*@[a-z0-9-]+(\\\\.[a-z0-9]+)*\\\\.([a-z]{2,4})$\", ErrorMessage = \"Invalid email format.\")]");

                    if (useResourceFile)
                    {
                        sb.AppendLine("[EmailAddress(ErrorMessageResourceType = typeof(" + resourceFileNameSpace + "), ErrorMessageResourceName = \"EmailAddress\")]");
                    }
                    else
                    {
                        sb.AppendLine("[EmailAddress(ErrorMessage = \"Invalid Email Address\")]");
                    }

                }

                if (propLower.Contains("password"))
                {
                    sb.AppendLine();
                    sb.Append("\t\t[DataType(DataType.Password)]");
                }

                foreach (var f in foriegnKeysList.Values)
                {
                    if (f.Any(o => o.Name == column.Name))
                    {
                        if (column.IsRowGuidCol == false)
                        {
                            if (column.IsNullable == false)
                            {
                                sb.AppendLine();


                                if (useResourceFile)
                                {
                                    sb.AppendLine("[Range(1, " + GetCsType(column) + ".MaxValue, ErrorMessageResourceType = typeof(" + resourceFileNameSpace + "), ErrorMessageResourceName = \"Range\")]");
                                }
                                else
                                {
                                    sb.AppendLine("[Range(1, " + GetCsType(column) + ".MaxValue, ErrorMessage = \"{0} must be between {1} and {2}.\")]");
                                }
                            }
                        }
                    }
                }
            }

            //if (propLower.Contains("url"))
            //{
            //    sb.AppendLine();
            //    sb.Append("\t\t[DataType(DataType.Url)]");
            //    sb.AppendLine();
            //    sb.Append("\t\t[Url(ErrorMessage = \"Invalid URL Address\")]");
            //}


            if (propLower.Contains("rowversion"))
            {
                sb.AppendLine();
                sb.Append("\t\t[Timestamp]");
                sb.AppendLine();
            }
            return sb.ToString();

        }

        public static string CreateFluentValidationRules(string modelResourceName, string tableName, Column column, Dictionary<string, List<Column>> foriegnKeysList, bool useResourceFile)
        {

            //string ruleFor = "RuleFor(o => o."+ column.Name + ").WithName(Resource." + tableName + "_" + column.Name + ")";



            StringBuilder sbColName = new StringBuilder();
            StringBuilder sb = new StringBuilder();


            // sb.Append("\t\t[Display(Name="" + FixName(column.Name) + "\")]");

            if (column.Type == "uniqueidentifier")
            {
                if (column.IsNullable == false)
                {
                    //  sb.Append("RuleFor<Guid>(x => x." + column.Name + ").NotEqual(Guid.Empty).WithMessage(\"" + column.Name + " cannot be empty\");");
                    if (useResourceFile)
                    {
                        sb.Append("RuleFor<Guid>(x => x." + column.Name + ").NotEqual(Guid.Empty).WithName(o => localizer[\"" + tableName + "_" + column.Name + "\"]);");

                    }
                    else
                    {
                        sb.Append("RuleFor<Guid>(x => x." + column.Name + ").NotEqual(Guid.Empty);");
                    }



                    return sb.ToString() + ";";
                }
            }


            sbColName.Append("RuleFor(o => o." + column.Name + ")");
            if (column.IsNullable == false)
            {
                if (column.Type == "char" || column.Type == "varchar" || column.Type == "nchar" ||
                    column.Type == "nvarchar")
                {
                    // sb.AppendLine();
                    if (useResourceFile)
                    {
                        //sb.Append(".NotEmpty().WithMessage(o => " + modelResourceName + ".NotEmpty)");
                        sb.Append(".NotEmpty().WithName(o => localizer[\"" + tableName + "_" + column.Name + "\"])");
                    }
                    else
                    {
                        sb.Append(".NotEmpty()");
                    }
                }
            }


            string propLower = column.Name.ToLower();



            if (column.Type == "date" || column.Type == "datetime" || column.Type == "datetime2" || column.Type == "datetimeoffset" || column.Type == "smalldatetime" || column.Type == "time")
            {
                //sb.AppendLine();
                //sb.Append("\t\t[DataType(DataType.DateTime)]");
                if (column.IsNullable == false)
                {
                    //if (useResourceFile)
                    //{
                    //    sb.Append(".IsValidDateTime().WithMessage(() => " + modelResourceName + ".IsValidDateTime)");
                    //}
                    //else
                    //{
                    //    sb.Append(".IsValidDateTime()");
                    //}
                }
            }

            if (column.Type == "char" || column.Type == "varchar" || column.Type == "nchar" || column.Type == "nvarchar")
            {
                //sb.AppendLine();

                if (useResourceFile)
                {
                    sb.Append(".MaximumLength(" + column.Length + ").WithName(o => localizer[\"" + tableName + "_" + column.Name + "\"])");
                }
                else
                {
                    sb.Append(".MaximumLength(" + (column.Length == "-1" ? "(8000)" : column.Length) + ")");
                    //if (column.Type == "char" || column.Type == "varchar")
                    //{
                    //    sb.Append(".MaximumLength(" + (column.Length == "-1" ? "(8000)" : column.Length) + ")");
                    //}
                    //else if (column.Type == "nchar" || column.Type == "nvarchar")
                    //{
                    //    sb.Append(".MaximumLength(" + (column.Length == "-1" ? "(4000)" : column.Length) + ")");
                    //}
                }



            }
            //if (propLower.Contains("phone") || propLower.Contains("telephone"))
            //{
            //    sb.AppendLine();
            //    sb.Append("\t\t[DataType(DataType.PhoneNumber)]");
            //}
            //if (propLower.Contains("html") || propLower.Contains("content"))
            //{
            //    sb.AppendLine();
            //    sb.Append("\t\t[DataType(DataType.Html)]");
            //}

            if (propLower.Contains("email"))
            {
                if (column.Type == "char" || column.Type == "varchar" || column.Type == "nchar" ||
                    column.Type == "nvarchar")
                {

                    if (useResourceFile)
                    {
                        sb.Append(".EmailAddress().WithName(o => localizer[\"" + tableName + "_" + column.Name + "\"])");
                    }
                    else
                    {
                        sb.Append(".EmailAddress()");
                    }

                }
            }

            //if (propLower.Contains("url"))
            //{
            //    sb.AppendLine();
            //    sb.Append("\t\t[DataType(DataType.Url)]");
            //    sb.AppendLine();
            //    sb.Append("\t\t[Url(ErrorMessage = \"Invalid URL Address\")]");
            //}

            //if (propLower.Contains("password"))
            //{
            //    sb.AppendLine();
            //    sb.Append("\t\t[DataType(DataType.Password)]");
            //}

            foreach (var f in foriegnKeysList.Values)
            {
                if (f.Any(o => o.Name == column.Name))
                {
                    if (column.IsRowGuidCol == false)
                    {
                        if (column.IsNullable == false)
                        {
                            // sb.AppendLine();

                            if (column.Type == "int" || column.Type == "bigint")
                            {
                                if (useResourceFile)
                                {
                                    sb.Append(".GreaterThan(0).WithName(o => localizer[\"" + tableName + "_" + column.Name + "\"])");
                                }
                                else
                                {
                                    sb.Append(".GreaterThan(0)");
                                }
                            }
                            else
                            {
                                sb.Append(".NotNull().NotEmpty().WithName(o => localizer[\"" + tableName + "_" + column.Name + "\"])");
                            }

                        }
                    }
                }
            }

            if (sb.Length > 0)
            {
                return sbColName.ToString() + sb.ToString() + ";";
            }

            return "";

        }

        public static string FixName(string propName)
        {
            if (propName.ToLower() != "id" && propName.ToLower().EndsWith("id"))
            {
                propName = propName.Replace("Id", "").Replace("ID", "");
            }
            return System.Text.RegularExpressions.Regex.Replace(propName, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static string GetColumnNameTitleForCheckExistInDataBase(Table table)
        {
            foreach (var c in table.Columns)
            {
                if (c.Name.ToLower().Contains("title"))
                {
                    return c.Name;
                }

                if (c.Name.ToLower().Contains("name"))
                {
                    return c.Name;
                }

                if (c.Name.ToLower().Contains("content"))
                {
                    return c.Name;
                }

                if (c.Name.ToLower().Contains("description"))
                {
                    return c.Name;
                }


            }

            return "";
        }




        public static string GetIDColumnNameForCheckExistInDataBase(Table table)
        {
            var id = table.PrimaryKeys.FirstOrDefault();
            if (id != null)
            {
                return id.Name;
            }

            return "";
        }


        public static string GetEntityInterfaces(List<Column> columns, string idType)
        {
            StringBuilder sb = new StringBuilder();

            if (columns.Any(o => o.Name.ToLower().Contains("Id".ToLower())))
            {
                var idColumn = columns.FirstOrDefault(o => o.Name.ToLower().Contains("Id".ToLower()));
                if (GetCsType(idColumn).ToLower() == idType.ToLower())
                {
                    sb.Append(" IBaseEntity ");
                }
            }



            if (columns.Any(o => o.Name.ToLower().Contains("CreatedBy".ToLower()))
                && columns.Any(o => o.Name.ToLower().Contains("CreatedAt".ToLower()))
                && columns.Any(o => o.Name.ToLower().Contains("LastModifiedBy".ToLower()))
                && columns.Any(o => o.Name.ToLower().Contains("LastModifiedAt".ToLower()))
            )
            {
                sb.Append(sb.Length > 0 ? ", IAuditable " : " IAuditable ");
            }

            if (columns.Any(o => o.Name.ToLower().Contains("SoftDeleted".ToLower())))
            {
                sb.Append(sb.Length > 0 ? ", ISoftDelete " : " ISoftDelete ");
            }

            if (columns.Any(o => o.Name.ToLower().Contains("RowVersion".ToLower())))
            {
                sb.Append(sb.Length > 0 ? ", IDataConcurrency " : " IDataConcurrency ");
            }

            if (sb.Length > 0)
            {
                return " : " + sb;
            }
            else
            {
                return "";
            }

        }
    }
}
