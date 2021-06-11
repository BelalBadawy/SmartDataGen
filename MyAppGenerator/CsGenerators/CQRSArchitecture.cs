using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using MyAppGenerator.Helpers;
using MyAppGenerator.Models;

namespace MyAppGenerator.CsGenerators
{
    public static class CQRSArchitecture
    {

        #region Create Folders 
        public static string OutputDirectory { get; set; }
        public static string ProjectName { get; set; }
        public static string DataPath { get; set; }
        public static string InfraDataInitializerPath { get; set; }
        public static string InfraDataFolderPath { get; set; }
        public static string InfraEntityConfigurationPath { get; set; }

        public static string InfraInterfacesDataPath { get; set; }

        public static string DomainPath { get; set; }
        public static string DomainDataPath { get; set; }
        public static string DomainCommonPath { get; set; }
        public static string DomainEntitiesPath { get; set; }
        public static string DomainEnumsPath { get; set; }
        public static string DomainResourcesPath { get; set; }
        public static string ApplicationPath { get; set; }
        public static string ApplicationCommonPath { get; set; }
        public static string ApplicationCommonInterfacesPath { get; set; }
        public static string ApplicationCommonBehavioursPath { get; set; }


        public static List<NodeType> tables { get; set; }
        public static bool useResourceFile { get; set; }

        public static string DomainNameSpace = ProjectName + ".Domain";
        public static string DataAccessNameSpace = ProjectName + ".Infrastructure";
        public static string ApplicationNameSpace = ProjectName + ".Application";

        #endregion

        public static void GenerateCQRSArchitecture(string projectName, string outputDirectory)
        {
            DomainNameSpace = projectName + ".Domain";
            DataAccessNameSpace = projectName + ".Infrastructure";
            ApplicationNameSpace = projectName + ".Application";

            CreateFoldersForCleanArchitecture(outputDirectory);
            CreateDomainClasses();
            CreateInfrastructureClasses();
            CreateApplicationClasses();
            CreateResourceFile();
        }
        private static void CreateFoldersForCleanArchitecture(string outputDirectory)
        {

            OutputDirectory = outputDirectory;
            DataPath = Path.Combine(outputDirectory, "Infrastructure");
            UtilityHelper.CreateSubDirectory(DataPath, true);

            InfraDataFolderPath = Path.Combine(outputDirectory, "Infrastructure/Data");
            UtilityHelper.CreateSubDirectory(InfraDataFolderPath, true);


            InfraDataInitializerPath = Path.Combine(outputDirectory, "Infrastructure/Data/Initializer");
            UtilityHelper.CreateSubDirectory(InfraDataInitializerPath, true);

            InfraEntityConfigurationPath = Path.Combine(outputDirectory, "Infrastructure/EntityConfiguration");
            UtilityHelper.CreateSubDirectory(InfraEntityConfigurationPath, true);


            DomainPath = Path.Combine(outputDirectory, "Domain");
            UtilityHelper.CreateSubDirectory(DomainPath, true);


            DomainCommonPath = Path.Combine(DomainPath, "Common");
            UtilityHelper.CreateSubDirectory(DomainCommonPath, true);


            DomainEntitiesPath = Path.Combine(DomainPath, "Entities");
            UtilityHelper.CreateSubDirectory(DomainEntitiesPath, true);


            DomainEnumsPath = Path.Combine(DomainPath, "Enums");
            UtilityHelper.CreateSubDirectory(DomainEnumsPath, true);


            DomainResourcesPath = Path.Combine(DomainPath, "Resources");
            UtilityHelper.CreateSubDirectory(DomainResourcesPath, true);



            ApplicationPath = Path.Combine(outputDirectory, "Application");
            UtilityHelper.CreateSubDirectory(ApplicationPath, true);

            ApplicationCommonPath = Path.Combine(ApplicationPath, "Common");
            UtilityHelper.CreateSubDirectory(ApplicationCommonPath, true);


            ApplicationCommonInterfacesPath = Path.Combine(ApplicationCommonPath, "Interfaces");
            UtilityHelper.CreateSubDirectory(ApplicationCommonInterfacesPath, true);

            ApplicationCommonBehavioursPath = Path.Combine(ApplicationCommonPath, "Behaviours");
            UtilityHelper.CreateSubDirectory(ApplicationCommonBehavioursPath, true);

        }


        private static void CreateDomainClasses()
        {


            #region Common Classes


            #region AuditEntity Class

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DomainCommonPath, "AuditEntity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace " + DomainNameSpace + @".Common
{
    public class AuditEntity
    {
        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
                                        ");
            }

            #endregion

            #region BaseEntity Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "BaseEntity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace " + DomainNameSpace + @".Common
{
    public class BaseEntity : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int ShowOrder { get; set; }
    }
}
                                        ");


            }

            #endregion

            #region ClientMessage Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "ClientMessage.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using " + DomainNameSpace + @".Enums;

namespace " + DomainNameSpace + @".Common
{
   public class ClientMessage<T>
    {
        public ClientMessage()
        {
        }

        public ClientMessage(AppEnums.OperationStatus statusCode, List<string> message, List<ValidationResult> validationResults, T data)
        {
            _statusCode = statusCode;
            _message = message;
            _validationResults = validationResults;
            _data = data;
        }

        private AppEnums.OperationStatus _statusCode;
        public AppEnums.OperationStatus ClientStatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        private List<string> _message = new List<string>();
        public List<string> ClientMessageContent
        {
            get { return _message; }
            set { _message = value; }
        }


        private List<ValidationResult> _validationResults = new List<ValidationResult>();
        public List<ValidationResult> ValidationResults
        {
            get { return _validationResults; }
            set { _validationResults = value; }
        }

        private T _data;

        public T ReturnedData
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }
}

            ");


            }


            #endregion

            #region KeyValue Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "KeyValue.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace " + DomainNameSpace + @".Common
{
  public class KeyValue
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
");


            }


            #endregion

            #region PagedResult Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "PagedResult.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System.Collections.Generic;

namespace " + DomainNameSpace + @".Common
{
  public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int FilteredTotalCount { get; set; }
        public List<T> Data { get; set; }
    }
}
");


            }


            #endregion

            #region Data Table Models Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "DatatableModels.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Newtonsoft.Json;
using System.Collections.Generic;

namespace " + DomainNameSpace + @".Common
{
  ///This view model class has been referred from example created by Marien Monnier at Soft.it. All credits to Marien for this class

    /// <summary>
    /// A full result, as understood by jQuery DataTables.
    /// </summary>
    /// <typeparam name=""T""> The data type of each row.</typeparam>
    public class DtResult<T>
        {
            /// <summary>
            /// The draw counter that this object is a response to - from the draw parameter sent as part of the data request.
            /// Note that it is strongly recommended for security reasons that you cast this parameter to an integer, rather than simply echoing back to the client what it sent in the draw parameter, in order to prevent Cross Site Scripting (XSS) attacks.
            /// </summary>
            [JsonProperty(""draw"")]
            public int Draw { get; set; }

            /// <summary>
            /// Total records, before filtering (i.e. the total number of records in the database)
            /// </summary>
            [JsonProperty(""recordsTotal"")]
            public int RecordsTotal { get; set; }

            /// <summary>
            /// Total records, after filtering (i.e. the total number of records after filtering has been applied - not just the number of records being returned for this page of data).
            /// </summary>
            [JsonProperty(""recordsFiltered"")]
            public int RecordsFiltered { get; set; }

            /// <summary>
            /// The data to be displayed in the table.
            /// This is an array of data source objects, one for each row, which will be used by DataTables.
            /// Note that this parameter's name can be changed using the ajaxDT option's dataSrc property.
            /// </summary>
            [JsonProperty(""data"")]
            public IEnumerable<T> Data { get; set; }

            public string PartialView { get; set; }
        }

        /// <summary>
        /// The additional columns that you can send to jQuery DataTables for automatic processing.
        /// </summary>
        public abstract class DtRow
        {
            /// <summary>
            /// Set the ID property of the dt-tag tr node to this value
            /// </summary>
            [JsonProperty(""DT_RowId"")]
            public virtual string DtRowId => null;

            /// <summary>
            /// Add this class to the dt-tag tr node
            /// </summary>
            [JsonProperty(""DT_RowClass"")]
            public virtual string DtRowClass => null;

            /// <summary>
            /// Add this data property to the row's dt-tag tr node allowing abstract data to be added to the node, using the HTML5 data-* attributes.
            /// This uses the jQuery data() method to set the data, which can also then be used for later retrieval (for example on a click event).
            /// </summary>
            [JsonProperty(""DT_RowData"")]
            public virtual object DtRowData => null;
        }

        /// <summary>
        /// The parameters sent by jQuery DataTables in AJAX queries.
        /// </summary>
        public class DtParameters
        {
            /// <summary>
            /// Draw counter.
            /// This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
            /// This is used as part of the draw return parameter (see below).
            /// </summary>
            public int Draw { get; set; }

            /// <summary>
            /// An array defining all columns in the table.
            /// </summary>
            public DtColumn[] Columns { get; set; }

            /// <summary>
            /// An array defining how many columns are being ordering upon - i.e. if the array length is 1, then a single column sort is being performed, otherwise a multi-column sort is being performed.
            /// </summary>
            public DtOrder[] Order { get; set; }

            /// <summary>
            /// Paging first record indicator.
            /// This is the start point in the current data set (0 index based - i.e. 0 is the first record).
            /// </summary>
            public int Start { get; set; }

            /// <summary>
            /// Number of records that the table can display in the current draw.
            /// It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
            /// Note that this can be -1 to indicate that all records should be returned (although that negates any benefits of server-side processing!)
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// Global search value. To be applied to all columns which have searchable as true.
            /// </summary>
            public DtSearch Search { get; set; }

            /// <summary>
            /// Custom column that is used to further sort on the first Order column.
            /// </summary>
            public string SortOrder => Columns != null && Order != null && Order.Length > 0
                ? (Columns[Order[0].Column].Data +
                   (Order[0].Dir == DtOrderDir.Desc ? "" "" + Order[0].Dir : string.Empty))
                : null;

            /// <summary>
            /// For Posting Additional Parameters to Server
            /// </summary>
            public IEnumerable<string> AdditionalValues { get; set; }

        }

        /// <summary>
        /// A jQuery DataTables column.
        /// </summary>
        public class DtColumn
        {
            /// <summary>
            /// Column's data source, as defined by columns.data.
            /// </summary>
            public string Data { get; set; }

            /// <summary>
            /// Column's name, as defined by columns.name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Flag to indicate if this column is searchable (true) or not (false). This is controlled by columns.searchable.
            /// </summary>
            public bool Searchable { get; set; }

            /// <summary>
            /// Flag to indicate if this column is orderable (true) or not (false). This is controlled by columns.orderable.
            /// </summary>
            public bool Orderable { get; set; }

            /// <summary>
            /// Specific search value.
            /// </summary>
            public DtSearch Search { get; set; }
        }

        /// <summary>
        /// An order, as sent by jQuery DataTables when doing AJAX queries.
        /// </summary>
        public class DtOrder
        {
            /// <summary>
            /// Column to which ordering should be applied.
            /// This is an index reference to the columns array of information that is also submitted to the server.
            /// </summary>
            public int Column { get; set; }

            /// <summary>
            /// Ordering direction for this column.
            /// It will be dt-string asc or dt-string desc to indicate ascending ordering or descending ordering, respectively.
            /// </summary>
            public DtOrderDir Dir { get; set; }
        }

        /// <summary>
        /// Sort orders of jQuery DataTables.
        /// </summary>
        public enum DtOrderDir
        {
            Asc,
            Desc
        }

        /// <summary>
        /// A search, as sent by jQuery DataTables when doing AJAX queries.
        /// </summary>
        public class DtSearch
        {
            /// <summary>
            /// Global search value. To be applied to all columns which have searchable as true.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// true if the global filter should be treated as a regular expression for advanced searching, false otherwise.
            /// Note that normally server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically possible and at the discretion of your script.
            /// </summary>
            public bool Regex { get; set; }
        }
    }
");


            }


            #endregion

            #region SD Static Data Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "SD.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
namespace " + DomainNameSpace + @".Common
{
    public static class SD
    {
        #region User Roles

        public const string Admin = ""Admin"";
        public const string User = ""User"";

        #endregion



        #region Messages
        public const string SavedSuccessfully = ""Saved Successfully"";
        public const string ExistData = ""This [{0}] already exist."";
        public const string ErrorOccured = ""An error has been occured."";
        public const string NotExistData = ""This record does not exist."";
        public const string CanNotDeleteData = ""Sorry we can't delete this record"";
        public const string AllowedForUpload = ""Only {0} are allowed to be uploaded."";
        public const string IsRequiredData = ""{ 0} is required."";


        #endregion
    }
}
");


            }


            #endregion

            #region Enums Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEnumsPath, "AppEnums.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace " + DomainNameSpace + @".Enums
{
  public class AppEnums
    {
        public enum OperationStatus
        {
            Ok = 1,
            Error = 2,
            ValidationError = 3,
        }
    }
}
");


            }


            #endregion

            #region ApplicationUser Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEntitiesPath, "ApplicationUser.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                        using Microsoft.AspNetCore.Identity;
                                        using System.ComponentModel.DataAnnotations;

                                        namespace " + DomainNameSpace + @".Entities
                                        {
                                           public class ApplicationUser : IdentityUser<int>
                                            {
                                                [Required]
                                                [StringLength(200)]
                                                public string FullName { get; set; }

                                            }
                                        }
                                      ");


            }


            #endregion

            #endregion


            List<Table> tableList = new List<Table>();

            using (SqlConnection connection = new SqlConnection(UtilityHelper.ConnectionString))
            {
                try
                {
                    connection.Open();

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(UtilityHelper.GetSelectedTablesAndViews(connection.Database, tables.Where(o => o.Type != SchemaTypeEnum.StoredProcedure).Select(o => o.Title).ToList()), connection);
                    dataAdapter.Fill(dataTable);

                    // Process each table
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        Table table = new Table();
                        table.Name = (string)dataRow["TABLE_NAME"];
                        table.Type = (string)dataRow["TABLE_TYPE"];
                        UtilityHelper.QueryTable(connection, table);
                        tableList.Add(table);
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }

            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region  Model Classes
                className = table.Name;

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DomainEntitiesPath, className + ".cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using System.Collections.Generic;");
                    streamWriter.WriteLine("using System.ComponentModel.DataAnnotations;");

                    streamWriter.WriteLine();
                    streamWriter.WriteLine("namespace " + DomainNameSpace + ".Entities");
                    streamWriter.WriteLine("{");

                    streamWriter.WriteLine("\tpublic class " + className);

                    streamWriter.WriteLine("\t{");

                    // Create an explicit public constructor
                    //   streamWriter.WriteLine("\t\t#region Constructors");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("\t\t/// <summary>");
                    streamWriter.WriteLine("\t\t/// Initializes a new instance of the " + className + " class.");
                    streamWriter.WriteLine("\t\t/// </summary>");
                    streamWriter.WriteLine("\t\tpublic " + className + "()");
                    streamWriter.WriteLine("\t\t{");

                    string sameKey = "";


                    foreach (var t in tableList)
                    {
                        foreach (var f in t.ForeignKeys.Values)
                        {
                            var foreignKeysList = f.Where(o => o.PrimaryKeyTableName == table.Name).ToList();
                            for (int j = 0; j < foreignKeysList.Count; j++)
                            {
                                if (!string.IsNullOrEmpty(foreignKeysList[j].ForeignKeyTableName))
                                {
                                    if (sameKey != foreignKeysList[j].ForeignKeyTableName)
                                    {
                                        sameKey = foreignKeysList[j].ForeignKeyTableName;

                                        streamWriter.WriteLine("\t\t this." +
                                                               foreignKeysList[j].ForeignKeyTableName + "List =  new List<" +
                                                               sameKey + ">(); ");
                                        streamWriter.WriteLine();
                                    }
                                }
                            }
                        }
                    }

                    streamWriter.WriteLine();


                    foreach (var f in table.ForeignKeys.Values)
                    {
                        var foreignKeysList = f.ToList();
                        for (int j = 0; j < foreignKeysList.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(foreignKeysList[j].PrimaryKeyTableName))
                            {
                                if (sameKey != foreignKeysList[j].PrimaryKeyTableName)
                                {
                                    sameKey = foreignKeysList[j].PrimaryKeyTableName;
                                    //  public virtual Course Course { get; set; }
                                    streamWriter.WriteLine("\t\t this." +
                                                           foreignKeysList[j].PrimaryKeyTableName + "Class = " +
                                                          " new " + foreignKeysList[j].PrimaryKeyTableName +
                                                           "();");
                                }
                            }
                        }
                    }



                    streamWriter.WriteLine("\t\t}");
                    streamWriter.WriteLine();

                    //// Create the "partial" constructor
                    //int parameterCount = 0;

                    //if (table.Columns.Count(o => o.IsIdentity == false) > 0)
                    //{
                    //    streamWriter.WriteLine("\t\t/// <summary>");
                    //    streamWriter.WriteLine("\t\t/// Initializes a new instance of the " + className + " class.");
                    //    streamWriter.WriteLine("\t\t/// </summary>");
                    //    streamWriter.Write("\t\tpublic " + className + "(");



                    //    for (int i = 0; i < table.Columns.Count; i++)
                    //    {
                    //        Column column = table.Columns[i];
                    //        if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    //        {
                    //            streamWriter.Write(UtilityHelper.CreateMethodParameter(column));
                    //            if (i < (table.Columns.Count - 1))
                    //            {
                    //                streamWriter.Write(", ");
                    //            }
                    //            parameterCount++;
                    //        }
                    //    }
                    //    streamWriter.WriteLine(") ");
                    //    streamWriter.WriteLine("\t\t{");



                    //    foreach (Column column in table.Columns)
                    //    {
                    //        if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    //        {
                    //            streamWriter.WriteLine("\t\t\tthis." + UtilityHelper.FormatPascal(column.Name) + " = " +
                    //                                   UtilityHelper.FormatCamel(column.Name) + ";");
                    //        }
                    //    }
                    //    streamWriter.WriteLine("\t\t}");
                    //}

                    //// Create the "full featured" constructor, if we haven't already
                    //if (parameterCount < table.Columns.Count)
                    //{
                    //    streamWriter.WriteLine();
                    //    streamWriter.WriteLine("\t\t/// <summary>");
                    //    streamWriter.WriteLine("\t\t/// Initializes a new instance of the " + className + " class.");
                    //    streamWriter.WriteLine("\t\t/// </summary>");
                    //    streamWriter.Write("\t\tpublic " + className + "(");
                    //    for (int i = 0; i < table.Columns.Count; i++)
                    //    {
                    //        Column column = table.Columns[i];
                    //        streamWriter.Write(UtilityHelper.CreateMethodParameter(column));
                    //        if (i < (table.Columns.Count - 1))
                    //        {
                    //            streamWriter.Write(", ");
                    //        }
                    //    }
                    //    streamWriter.WriteLine(")  ");
                    //    streamWriter.WriteLine("\t\t{");



                    //    foreach (Column column in table.Columns)
                    //    {
                    //        streamWriter.WriteLine("\t\t\tthis." + UtilityHelper.FormatPascal(column.Name) + " = " + UtilityHelper.FormatCamel(column.Name) + ";");
                    //    }
                    //    streamWriter.WriteLine("\t\t}");
                    //}

                    //streamWriter.WriteLine();
                    //streamWriter.WriteLine("\t\t#endregion");
                    //streamWriter.WriteLine();

                    // Append the public properties
                    streamWriter.WriteLine("\t\t#region Properties");
                    for (int i = 0; i < table.Columns.Count; i++)
                    {

                        Column column = table.Columns[i];
                        string parameter = UtilityHelper.CreateMethodParameter(column);
                        string type = parameter.Split(' ')[0];
                        string name = parameter.Split(' ')[1];

                        streamWriter.WriteLine("\t\t/// <summary>");
                        streamWriter.WriteLine("\t\t/// Gets or sets the " + UtilityHelper.FormatPascal(name) + " value.");
                        streamWriter.WriteLine("\t\t/// </summary>");


                        if (table.Type != "VIEW")
                        {
                            streamWriter.WriteLine("\t\t" +
                                                   UtilityHelper.CreateDataAnnotationParameter(false, column,
                                                       table.ForeignKeys, useResourceFile, "", table.Name));
                        }

                        streamWriter.WriteLine("\t\tpublic " + type + " " + UtilityHelper.FormatPascal(name) + " { get; set; }");

                        if (i < (table.Columns.Count - 1))
                        {
                            streamWriter.WriteLine();
                        }
                    }

                    streamWriter.WriteLine();

                    sameKey = "";


                    foreach (var t in tableList)
                    {
                        foreach (var f in t.ForeignKeys.Values)
                        {
                            var foreignKeysList = f.Where(o => o.PrimaryKeyTableName == table.Name).ToList();
                            for (int j = 0; j < foreignKeysList.Count; j++)
                            {
                                if (!string.IsNullOrEmpty(foreignKeysList[j].ForeignKeyTableName))
                                {
                                    if (sameKey != foreignKeysList[j].ForeignKeyTableName)
                                    {
                                        sameKey = foreignKeysList[j].ForeignKeyTableName;
                                        //  public virtual ICollection<Course> Courses { get; set; }
                                        streamWriter.WriteLine("\t\tpublic virtual List<" +
                                                               foreignKeysList[j].ForeignKeyTableName + "> " +
                                                               foreignKeysList[j].ForeignKeyTableName +
                                                               "List { get; set; }");
                                        streamWriter.WriteLine();
                                    }
                                }
                            }
                        }
                    }

                    streamWriter.WriteLine();


                    foreach (var f in table.ForeignKeys.Values)
                    {
                        var foreignKeysList = f.ToList();
                        for (int j = 0; j < foreignKeysList.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(foreignKeysList[j].PrimaryKeyTableName))
                            {
                                if (sameKey != foreignKeysList[j].PrimaryKeyTableName)
                                {
                                    sameKey = foreignKeysList[j].PrimaryKeyTableName;
                                    //  public virtual Course Course { get; set; }
                                    // streamWriter.WriteLine("\t\t[BindNever]");
                                    streamWriter.WriteLine("\t\tpublic virtual " +
                                                           foreignKeysList[j].PrimaryKeyTableName + " " +
                                                           foreignKeysList[j].PrimaryKeyTableName +
                                                           "Class { get; set; }");
                                }
                            }
                        }
                    }









                    streamWriter.WriteLine();
                    streamWriter.WriteLine("\t\t#endregion");



                    streamWriter.WriteLine();

                    // Close out the class and namespace
                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                }
                #endregion


            }

        }
        private static void CreateInfrastructureClasses()
        {
            #region Interfaces


            #region Data

            //IDbInitializer
            #region IDbInitializer
            using (
               StreamWriter streamWriter =
                   new StreamWriter(Path.Combine(InfraDataFolderPath, "IDbInitializer.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
namespace " + DataAccessNameSpace + @".Data {
    public interface IDbInitializer
    {
        void Initialize();
    }
}

");

            }

            #endregion





            #endregion



            #endregion

            #region Implementations




            #region UserInitializer

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "UserInitializer.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;
namespace " + DataAccessNameSpace + @".Data.Initializer
{
    public class UserInitializer
    {


        public static void AddUser(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            if (!db.Roles.Any(r => r.Name ==""Admin""))
            {
                roleManager.CreateAsync(new IdentityRole<int>(""Admin"")).GetAwaiter().GetResult();
            }

            if (!db.Roles.Any(r => r.Name == ""User""))
            {
                roleManager.CreateAsync(new IdentityRole<int>(""User"")).GetAwaiter().GetResult();
            }


            if (!db.ApplicationUsers.Any(u => u.Email == ""admin@gmail.com""))
                {
                    userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = ""Belal"",
                        Email = ""admin@gmail.com"",
                        EmailConfirmed = true,
                        FullName = ""Belal Badawy""
                      

                    }, ""Admin123$"").GetAwaiter().GetResult();

                    ApplicationUser user = db.ApplicationUsers.Where(u => u.Email == ""admin@gmail.com"").FirstOrDefault();
                    userManager.AddToRoleAsync(user, ""Admin"").GetAwaiter().GetResult();
                }

            }


        }
    }

");

            }


            #endregion


            #region DbInitializer

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "DbInitializer.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace " + DataAccessNameSpace + @".Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }


            UserInitializer.AddUser(_db, _userManager, _roleManager);
            
          

        }
    }
}

");

            }


            #endregion

            #endregion

            #region EntityConfiguration

            List<Table> tableList = new List<Table>();

            using (SqlConnection connection = new SqlConnection(UtilityHelper.ConnectionString))
            {
                try
                {
                    connection.Open();

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(UtilityHelper.GetSelectedTablesAndViews(connection.Database, tables.Where(o => o.Type != SchemaTypeEnum.StoredProcedure).Select(o => o.Title).ToList()), connection);
                    dataAdapter.Fill(dataTable);

                    // Process each table
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        Table table = new Table();
                        table.Name = (string)dataRow["TABLE_NAME"];
                        table.Type = (string)dataRow["TABLE_TYPE"];
                        UtilityHelper.QueryTable(connection, table);
                        tableList.Add(table);
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }

            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region  Model Classes
                className = table.Name;

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraEntityConfigurationPath, className + "Configuration.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace " + DataAccessNameSpace + @".EntityConfiguration
{
    public partial class " + table.Name + @"Configuration : IEntityTypeConfiguration<" + table.Name + @">
    {
        public void Configure(EntityTypeBuilder<" + table.Name + @"> builder)
        {
            // table
            builder.ToTable(""" + table.Name + @""", ""dbo"");

                    // key
                   



                    ");

                    streamWriter.WriteLine(UtilityHelper.CreateBuilderPropertyForEntityFrameworkHasKey(table.PrimaryKeys));

                    //                    if (table.PrimaryKeys.Count == 1)
                    //                    {
                    //                        Column column = table.Columns.FirstOrDefault(o => o.IsIdentity);
                    //                        streamWriter.WriteLine("builder.HasKey(t => t." + column.Name + ");" +
                    //                                               @"
                    //// properties
                    //                        builder.Property(t => t." + column.Name + @")
                    //                            .HasColumnName(""" + column.Name + @""")
                    //                            .HasColumnType(""" + column.Type + @""")
                    //                            " + (column.IsNullable ? "" : ".IsRequired()") +
                    //                             (column.IsIdentity ? ".ValueGeneratedOnAdd(); " : "; "));
                    //                    }
                    //                    else if (table.PrimaryKeys.Count > 1)
                    //                    {
                    //                        streamWriter.WriteLine("builder.HasKey(t => new { t.BookId, t.BookCategoryId });");
                    //                    }

                    // Create the parameter list
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];

                        // Ignore any identity columns
                        streamWriter.WriteLine(UtilityHelper.CreateBuilderPropertyForEntityFramework(column));
                    }

                    string sameKey = "";



                    foreach (var f in table.ForeignKeys.Values)
                    {
                        //var foreignKeysList = f.Where(o => o.PrimaryKeyTableName == table.Name).ToList();
                        //for (int j = 0; j < foreignKeysList.Count; j++)
                        //{
                        //    if (!string.IsNullOrEmpty(foreignKeysList[j].ForeignKeyTableName))
                        //    {
                        //        if (sameKey != foreignKeysList[j].ForeignKeyTableName)
                        //        {
                        //            sameKey = foreignKeysList[j].ForeignKeyTableName;

                        streamWriter.WriteLine("builder.HasOne(t => t." + f[0].PrimaryKeyTableName + "Class)");
                        streamWriter.WriteLine(".WithMany(t => t." + f[0].ForeignKeyTableName + "List)");
                        streamWriter.WriteLine(".HasForeignKey(d => d." + f[0].Name + ")");
                        streamWriter.WriteLine(".HasConstraintName(" + @"""FK_" + f[0].ForeignKeyTableName + "_" + f[0].PrimaryKeyTableName + @""");");

                        //streamWriter.WriteLine("\t\t this." +
                        //                       foreignKeysList[j].ForeignKeyTableName + "List =  new List<" +
                        //                       sameKey + ">(); ");
                        //streamWriter.WriteLine();
                        //}
                        // }
                        //}
                    }

                    streamWriter.WriteLine("}");
                    streamWriter.WriteLine("}");
                    streamWriter.WriteLine("}");













                }
                #endregion


            }

            #endregion

            #region ApplicationDbContext

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataFolderPath, "ApplicationDbContext.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System.Reflection;
using " + DomainNameSpace + @".Entities;
//using " + DataAccessNameSpace + @".EntityConfiguration;
using " + ApplicationNameSpace + @".Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace " + DataAccessNameSpace + @".Data
{
  public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

");

                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("public virtual DbSet<" + table.Name + "> " + table.Name + " { get; set; }");
                }

                streamWriter.WriteLine(" protected override void OnModelCreating(ModelBuilder modelBuilder){");

                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine(" // modelBuilder.ApplyConfiguration(new " + table.Name + "Configuration());");
                }

                streamWriter.WriteLine("modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());");


                streamWriter.WriteLine("base.OnModelCreating(modelBuilder);");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");



            }


            #endregion

            #region Infrastructure Dependency Injection



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DataPath, "DependencyInjection.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"



using " + ApplicationNameSpace + @".Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using " + DataAccessNameSpace + @".Data;
using " + DataAccessNameSpace + @".Data.Initializer;
using  " + DomainNameSpace + @".Entities;

namespace " + DataAccessNameSpace + @"
{
   public static class DependencyInjection
    {
        public static readonly LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddDbContext<ApplicationDbContext>(options =>

                options.UseLoggerFactory(_myLoggerFactory).UseSqlServer(
                    configuration.GetConnectionString(""DefaultConnection""),
                serverOptions =>
                {
                    serverOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                }));



                services.AddIdentity<ApplicationUser, IdentityRole<int>>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();


                //services.AddScoped<IRepositoryFactory, UnitOfWork<ApplicationDbContext>>();
               services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

                services.AddScoped<IDbInitializer, DbInitializer>();

                return services;
            }
        }
    }"
                );









            }
            #endregion

        }
        private static void CreateApplicationClasses()
        {
            List<Table> tableList = new List<Table>();

            using (SqlConnection connection = new SqlConnection(UtilityHelper.ConnectionString))
            {
                try
                {
                    connection.Open();

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(UtilityHelper.GetSelectedTablesAndViews(connection.Database, tables.Where(o => o.Type != SchemaTypeEnum.StoredProcedure).Select(o => o.Title).ToList()), connection);
                    dataAdapter.Fill(dataTable);

                    // Process each table
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        Table table = new Table();
                        table.Name = (string)dataRow["TABLE_NAME"];
                        table.Type = (string)dataRow["TABLE_TYPE"];
                        UtilityHelper.QueryTable(connection, table);
                        tableList.Add(table);
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }


            StringBuilder sb = new StringBuilder();


            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                className = table.Name;

                string classNameReadDto = className + "ReadDto";
                string classNameCreateDto = className + "UpsertDto";
                //  string classNameUpdateDto = className + "UpdateDto";
                // string classNameUpdateDto = classNameCreateDto;

                var applicationClassPath = Path.Combine(ApplicationPath, className);
                UtilityHelper.CreateSubDirectory(applicationClassPath, true);

                var dtosPath = Path.Combine(applicationClassPath, "Dtos");
                UtilityHelper.CreateSubDirectory(dtosPath, true);

                var commandsPath = Path.Combine(applicationClassPath, "Commands");
                UtilityHelper.CreateSubDirectory(commandsPath, true);


                var mappingPath = Path.Combine(applicationClassPath, "Mapping");
                UtilityHelper.CreateSubDirectory(mappingPath, true);


                var queriesPath = Path.Combine(applicationClassPath, "Queries");
                UtilityHelper.CreateSubDirectory(queriesPath, true);


                var validationsPath = Path.Combine(applicationClassPath, "Validations");
                UtilityHelper.CreateSubDirectory(validationsPath, true);


                #region Dtos Classes


                sb.Clear();




                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("namespace " + ApplicationNameSpace + ".Dtos");
                sb.AppendLine("{");
                sb.AppendLine("\tpublic class #className#");
                sb.AppendLine("\t{");

                for (int i = 0; i < table.Columns.Count; i++)
                {

                    Column column = table.Columns[i];
                    string parameter = UtilityHelper.CreateMethodParameter(column);
                    string type = parameter.Split(' ')[0];
                    string name = parameter.Split(' ')[1];


                    sb.AppendLine("\t\tpublic " + type + " " + UtilityHelper.FormatPascal(name) + " { get; set; }");
                }



                sb.AppendLine("\t}");
                sb.AppendLine("}");


                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(dtosPath, classNameReadDto + ".cs")))
                {
                    string clsName = sb.ToString().Replace("#className#", classNameReadDto).ToString();
                    streamWriter.WriteLine(clsName);
                }



                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(dtosPath, classNameCreateDto + ".cs")))
                {
                    string clsName = sb.ToString().Replace("#className#", classNameCreateDto).ToString();
                    streamWriter.WriteLine(clsName);

                }


                //using (StreamWriter streamWriter = new StreamWriter(Path.Combine(UtilityHelper.ApplicationDtosPath, classNameUpdateDto + ".cs")))
                //{
                //    string clsName = sb.ToString().Replace("#className#", classNameUpdateDto).ToString();
                //    streamWriter.WriteLine(clsName);
                //}

                #endregion

                #region Validations Classes




                sb.Clear();

                sb.AppendLine("using System;");
                sb.AppendLine("using FluentValidation;");
                sb.AppendLine("using " + ApplicationNameSpace + ".Dtos;");
                sb.AppendLine("namespace " + ApplicationNameSpace + ".Validations");
                sb.AppendLine("{");
                sb.AppendLine("\tpublic class #className#  : AbstractValidator<#validatorName#>");
                sb.AppendLine("\t{");

                sb.AppendLine("\tpublic #className#()");
                sb.AppendLine("{");

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];

                    sb.AppendLine("\t\t" + UtilityHelper.CreateFluentValidationRules("DomainResource", table.Name, column, table.ForeignKeys, useResourceFile));
                }



                sb.AppendLine("}");

                sb.AppendLine("\t}");
                sb.AppendLine("}");

                var validatorNameCreateDto = classNameCreateDto;
                //   var validatorNameUpdateDto = classNameUpdateDto;

                classNameCreateDto = classNameCreateDto + "Validator";
                //  classNameUpdateDto = classNameUpdateDto + "Validator";

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(validationsPath, classNameCreateDto + ".cs")))
                {
                    string clsName = sb.ToString().Replace("#className#", classNameCreateDto).Replace("#validatorName#", validatorNameCreateDto).ToString();
                    streamWriter.WriteLine(clsName);

                }


                //using (StreamWriter streamWriter = new StreamWriter(Path.Combine(UtilityHelper.ApplicationValidationsPath, classNameUpdateDto + ".cs")))
                //{
                //    string clsName = sb.ToString().Replace("#className#", classNameUpdateDto).Replace("#validatorName#", validatorNameUpdateDto).ToString();
                //    streamWriter.WriteLine(clsName);


                //}

                #endregion

                #region Mapping Classes


                sb = new StringBuilder();

          
                sb.AppendLine("using System;");
                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using " + ApplicationNameSpace + ".Commands;");
                sb.AppendLine("using " + ApplicationNameSpace + ".Dtos;");
                sb.AppendLine("using " + DomainNameSpace + ".Entities;");
                sb.AppendLine("namespace " + ApplicationNameSpace + ".Mapping");
                sb.AppendLine("{");
                sb.AppendLine("\tpublic class " + className + "Profile : AutoMapper.Profile");
                sb.AppendLine("\t{");

                sb.AppendLine("\t\tpublic " + className + "Profile() {");
                sb.AppendLine("CreateMap<" + className + ", " + className + "ReadDto>().ReverseMap();");
                sb.AppendLine("CreateMap<" + className + ", " + className + "UpsertDto>().ReverseMap();");
                sb.AppendLine("CreateMap<" + className + "ReadDto, " + className + "UpsertDto>().ReverseMap();");
                //   sb.AppendLine("CreateMap<" + className + ", " + className + "UpdateDto>().ReverseMap();");

                sb.AppendLine("CreateMap<" + className + ", Create" + className + "Command>().ReverseMap();");
                sb.AppendLine("CreateMap<" + className + ", Update" + className + "Command>().ReverseMap();");

                sb.AppendLine("\t}");
                sb.AppendLine("}");
                sb.AppendLine("}");


                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(mappingPath, className + "Profile.cs")))
                {
                    string clsName = sb.ToString().Replace("#className#", classNameReadDto).ToString();
                    streamWriter.WriteLine(clsName);
                }





                #endregion

                #region Application Commands

                var titleName = UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table);

                #region Create Command

                var createCommand = "Create" + className + "Command";
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(commandsPath, createCommand + ".cs")))
                {

                    // Create the header for the class

                    streamWriter.WriteLine(@"

using AutoMapper;
using " + ApplicationNameSpace + @".Common.Interfaces;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Enums;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace " + ApplicationNameSpace + @".Commands
{

 public partial class " + createCommand + ": " + table.Name + @"UpsertDto , IRequest<ClientMessage<int>>
        {

        }

   public class " + createCommand + @"Handler : IRequestHandler<" + createCommand + @", ClientMessage<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

       public  " + createCommand + @"Handler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClientMessage<int>> Handle(" + createCommand + " " + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto, CancellationToken cancellationToken)
        {

          ClientMessage<int> clientMessage = new ClientMessage<int>();

            " + table.Name + @"UpsertDtoValidator dtoValidator = new " + table.Name + @"UpsertDtoValidator();

            ValidationResult validationResult = dtoValidator.Validate(" + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

            if (validationResult != null && validationResult.IsValid == false)
            {
                clientMessage.ClientStatusCode = AppEnums.OperationStatus.ValidationError;
                clientMessage.ValidationResults = validationResult.Errors.Select(modelError => new System.ComponentModel.DataAnnotations.ValidationResult(errorMessage: modelError.ErrorMessage)).ToList();

            }
            else
            {


");
                    if (!string.IsNullOrEmpty(titleName))
                    {
                        streamWriter.WriteLine(@"

                if (await _context." + table.Name + ".AnyAsync(o => o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + ".ToUpper() == " +
                                           UtilityHelper.FormatCamel(table.Name) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @".ToUpper()))
                {
                    clientMessage.ClientStatusCode = AppEnums.OperationStatus.Error;
                    clientMessage.ClientMessageContent = new List<string>()
                        {
                            string.Format(SD.ExistData," + UtilityHelper.FormatCamel(table.Name) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @")
                        };
                }
                else
                {
                    " + table.Name + " " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name + @">(" +
                                           UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

                    await _context." + table.Name + @".AddAsync(" +
                                           UtilityHelper.FormatCamel(table.Name) + @");

                    int effectedRows =  await _context.SaveChangesAsync(cancellationToken);
                    if (effectedRows != 0)
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                        clientMessage.ReturnedData = " + UtilityHelper.FormatCamel(table.Name) + @"." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @";
                    }
                }


");
                    }
                    else
                    {

                        streamWriter.WriteLine(@"

                 " + table.Name + " " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name + @">(" +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

                    await _context." + table.Name + @".AddAsync(" +
                                               UtilityHelper.FormatCamel(table.Name) + @");

                    int effectedRows =  await _context.SaveChangesAsync(cancellationToken);
                    if (effectedRows != 0)
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                        clientMessage.ReturnedData = " + UtilityHelper.FormatCamel(table.Name) + @"." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @";
                    }
");

                    }


                    streamWriter.WriteLine(@"   



            }

            return clientMessage;
        }

    }
}"
                    );


                }


                #endregion

                #region Update Command

                var updateCommand = "Update" + className + "Command";
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(commandsPath, updateCommand + ".cs")))
                {

                    // Create the header for the class

                    streamWriter.WriteLine(@"

using AutoMapper;
using " + ApplicationNameSpace + @".Common.Interfaces;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Enums;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace " + ApplicationNameSpace + @".Commands
{

 public partial class " + updateCommand + ": " + table.Name + @"UpsertDto , IRequest<ClientMessage<int>>
        {

        }

   public class " + updateCommand + @"Handler : IRequestHandler<" + updateCommand + @", ClientMessage<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

       public  " + updateCommand + @"Handler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClientMessage<int>> Handle(" + updateCommand + " " + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto, CancellationToken cancellationToken)
        {

          ClientMessage<int> clientMessage = new ClientMessage<int>();

            " + table.Name + @"UpsertDtoValidator dtoValidator = new " + table.Name + @"UpsertDtoValidator();

            ValidationResult validationResult = dtoValidator.Validate(" + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

            if (validationResult != null && validationResult.IsValid == false)
            {
                clientMessage.ClientStatusCode = AppEnums.OperationStatus.ValidationError;
                clientMessage.ValidationResults = validationResult.Errors.Select(modelError => new System.ComponentModel.DataAnnotations.ValidationResult(errorMessage: modelError.ErrorMessage)).ToList();

            }
            else
            {


");
                    if (!string.IsNullOrEmpty(titleName))
                    {
                        streamWriter.WriteLine(@"

                if (await _context." + table.Name + ".AnyAsync(o => o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + ".ToUpper() == " +
                                           UtilityHelper.FormatCamel(table.Name) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @".ToUpper() && o." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" != " +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @"))
                {
                    clientMessage.ClientStatusCode = AppEnums.OperationStatus.Error;
                    clientMessage.ClientMessageContent = new List<string>()
                        {
                            string.Format(SD.ExistData," + UtilityHelper.FormatCamel(table.Name) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @")
                        };
                }
                else
                {
                    " + table.Name + " " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name + @">(" +
                                           UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

                     _context." + table.Name + @".Update(" +
                                           UtilityHelper.FormatCamel(table.Name) + @");

                    int effectedRows =  await _context.SaveChangesAsync(cancellationToken);
                    if (effectedRows != 0)
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                        clientMessage.ReturnedData = " + UtilityHelper.FormatCamel(table.Name) + @"." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @";
                    }
                }


");
                    }
                    else
                    {

                        streamWriter.WriteLine(@"

                 " + table.Name + " " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name + @">(" +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

                     _context." + table.Name + @".Update(" +
                                               UtilityHelper.FormatCamel(table.Name) + @");

                    int effectedRows =  await _context.SaveChangesAsync(cancellationToken);
                    if (effectedRows != 0)
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                        clientMessage.ReturnedData =  effectedRows ;
                    }
");

                    }


                    streamWriter.WriteLine(@"   



            }

            return clientMessage;
        }

    }
}"
                    );


                }


                #endregion


                #region Delete Command

                var deleteCommand = "Delete" + className + "Command";
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(commandsPath, deleteCommand + ".cs")))
                {

                    // Create the header for the class

                    streamWriter.WriteLine(@"

using AutoMapper;
using " + ApplicationNameSpace + @".Common.Interfaces;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Enums;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace " + ApplicationNameSpace + @".Commands
{

 public partial class " + deleteCommand + @":  IRequest<ClientMessage<int>>
        {
            public int Id { get; set; }
        }

   public class " + deleteCommand + @"Handler : IRequestHandler<" + deleteCommand + @", ClientMessage<int>>
    {
        private readonly IApplicationDbContext _context;
       

       public  " + deleteCommand + @"Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClientMessage<int>> Handle(" + deleteCommand + @" request , CancellationToken cancellationToken)
        {

         ClientMessage<int> clientMessage = new ClientMessage<int>();

            var entity = await _context." + table.Name + @".FirstOrDefaultAsync(o => o." + UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @"== request.Id);


            if (entity == null)
            {
                clientMessage.ClientStatusCode = AppEnums.OperationStatus.Error;
                clientMessage.ClientMessageContent = new List<string>()
                {
                   ""Not Found record with id  "" +request.Id
                };

                return clientMessage;
            }

            _context.Books.Remove(entity);


            int effectedRows = await _context.SaveChangesAsync(cancellationToken);
            if (effectedRows != 0)
            {
                clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                clientMessage.ReturnedData = effectedRows;
            }
            return clientMessage;

           
        }

    }
}"
                    );


                }


                #endregion

                #endregion

                #region Application Queries



                #region Get By ID Query

                var getByIDQuery = "Get" + className + "ByIdQuery";
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(queriesPath, getByIDQuery + ".cs")))
                {

                    // Create the header for the class

                    streamWriter.WriteLine(@"

using AutoMapper;
using " + ApplicationNameSpace + @".Common.Interfaces;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Enums;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace " + ApplicationNameSpace + @".Queries
{

 public partial class " + getByIDQuery + ":IRequest<ClientMessage<" + table.Name + @"ReadDto>>
        {
            public int Id { get; set; }
        }

   public class " + getByIDQuery + @"Handler : IRequestHandler<" + getByIDQuery + @", ClientMessage<" + table.Name + @"ReadDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public " + getByIDQuery + @"Handler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClientMessage<" + table.Name + @"ReadDto>> Handle(" + getByIDQuery + @" request, CancellationToken cancellationToken)
        {
            ClientMessage<" + table.Name + @"ReadDto> clientMessage = new ClientMessage<" + table.Name + @"ReadDto>();

            var result = await _context." + table.Name + @".FirstOrDefaultAsync(o => o." + UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" == request.Id);

            clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
            clientMessage.ReturnedData = _mapper.Map<" + table.Name + @"ReadDto>(result);

            return clientMessage;
        }
    }
}"
                    );


                }


                #endregion


                #region Get ALL Query

                var getAllQuery = "GetAll" + className + "Query";
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(queriesPath, getAllQuery + ".cs")))
                {

                    // Create the header for the class

                    streamWriter.WriteLine(@"

using AutoMapper;
using " + ApplicationNameSpace + @".Common.Interfaces;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Enums;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace " + ApplicationNameSpace + @".Queries
{

 public partial class " + getAllQuery + ":IRequest<ClientMessage<List<" + table.Name + @"ReadDto>>>
        {
          
        }

   public class " + getAllQuery + @"Handler : IRequestHandler<" + getAllQuery + @", ClientMessage<List<" + table.Name + @"ReadDto>>>
    {
         private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public " + getAllQuery + @"Handler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClientMessage<List<" + table.Name + @"ReadDto>>> Handle(" + getAllQuery + @" request, CancellationToken cancellationToken)
        {
            ClientMessage<List<" + table.Name + @"ReadDto>> clientMessage = new ClientMessage<List<" + table.Name + @"ReadDto>>();

            var result = await _context." + table.Name + @".ToListAsync(cancellationToken);

            clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
            clientMessage.ReturnedData = _mapper.Map<List<" + table.Name + @"ReadDto>>(result);

            return clientMessage;
        }
    }
}"
                    );


                }


                #endregion


                #endregion
            }


            #region Common Claess

            #region LoggingBehaviour Class

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationCommonBehavioursPath, "LoggingBehaviour.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace " + ApplicationNameSpace + @".Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
      //  private readonly ICurrentUserService _currentUserService;
      //  private readonly IIdentityService _identityService;

       // public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
            //_currentUserService = currentUserService;
            //_identityService = identityService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId =  string.Empty;
            string userName = string.Empty;

            //if (!string.IsNullOrEmpty(userId))
            //{
            //    userName = await _identityService.GetUserNameAsync(userId);
            //}

            //_logger.LogInformation(""CleanArchDemo Request: Name UserId    UserName    Request   "", requestName, userId, userName, request);
            _logger.LogInformation("" Request: {0} {1} {2}  {3} "", requestName, userId, userName, request);
            }
        }
    }
                                        ");
            }

            #endregion




            #region IApplicationDbContext

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ApplicationCommonInterfacesPath, "IApplicationDbContext.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;



namespace " + ApplicationNameSpace + @".Common.Interfaces
{
 public interface IApplicationDbContext
    {
        
       DbSet<ApplicationUser> ApplicationUsers { get; set; }


");

                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine(" DbSet<" + table.Name + "> " + table.Name + " { get; set; }");
                }


                streamWriter.WriteLine("Task<int> SaveChangesAsync(CancellationToken cancellationToken);");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");



            }


            #endregion

            #endregion


            #region Application Dependency Injection



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationPath, "DependencyInjection.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"

using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;


namespace " + ApplicationNameSpace + @"
{
   public static class DependencyInjection
    {
       
      public static IServiceCollection AddApplication(this IServiceCollection services)
        {
           services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());


");
                //foreach (Table table in tableList)
                //{
                //    streamWriter.WriteLine("services.AddTransient<I" + table.Name + "Service, " + table.Name + "Service>();");
                //}

                streamWriter.WriteLine(@"
           


            return services;
        }
    }
}"
                );









            }
            #endregion




        }
        private static void CreateResourceFile()
        {

            string sqlTables = @"SELECT DISTINCT T.name AS TableName ,
       C.name AS ColumnName ,
	   T.name+'_'+C.name AS TableColumnName
FROM   sys.objects AS T
       JOIN sys.columns AS C ON T.object_id = C.object_id
       JOIN sys.types AS P ON C.system_type_id = P.system_type_id
WHERE  T.type_desc = 'USER_TABLE' AND p.name <> 'sysname'
ORDER BY T.name+'_'+C.name;";

            //List<string> keysList = new List<string>();
            Dictionary<string, string> keysList = new Dictionary<string, string>();

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(UtilityHelper.ConnectionString);
                connection.Open();

                // Get a list of the entities in the database
                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlTables, connection);
                dataAdapter.Fill(dataTable);

                // Process each table
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    keysList.Add((string)dataRow["TableColumnName"], (string)dataRow["ColumnName"]);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }



            #region Using FluentValidation
            using (
                ResXResourceWriter resx =
                    new ResXResourceWriter(DomainResourcesPath + @"/DomainResource.resx"))
            {

                resx.AddResource("EmailAddress", "{PropertyName} is not a valid email address.");
                resx.AddResource("Equal", "{PropertyName} should be equal to {0}");
                resx.AddResource("GreaterThan", "{PropertyName} must be greater than {0}.");
                resx.AddResource("GreaterThanOrEqual",
                    "{PropertyName} must be greater than or equal to {0}.");
                resx.AddResource("IsValidDateTime", "{PropertyName} is not in the correct format.");
                resx.AddResource("Length", "{PropertyName} must be less than {1} characters.");
                resx.AddResource("LessThan", "{PropertyName} must be less than {0}.");
                resx.AddResource("LessThanOrEqual", "{PropertyName} must be less than or equal to {0}.");
                resx.AddResource("NotEmpty", "{PropertyName} should not be empty.");
                resx.AddResource("NotEqual", "{PropertyName} should not be equal to {0}.");
                resx.AddResource("RegularExpression", "{PropertyName} is not in the correct format.");

                foreach (var key in keysList)
                {
                    resx.AddResource(key.Key, UtilityHelper.FixName(key.Value));
                }

            }

            using (
                ResXResourceWriter resx =
                    new ResXResourceWriter(DomainResourcesPath + @"/DomainResource.ar.resx"))
            {

                resx.AddResource("EmailAddress", "{PropertyName} البريد الإلكتروني غير صحيح.");
                resx.AddResource("Equal", "{PropertyName} يجب ان تساوي {0}");
                resx.AddResource("GreaterThan", "{PropertyName} يجب ان تكون أكبر من {0}.");
                resx.AddResource("GreaterThanOrEqual", "{PropertyName} يجب ان تكون أكبر من أو تساوي {0}.");
                resx.AddResource("IsValidDateTime", "{PropertyName} يجب أن يكون في الصغية الصحيحة.");
                resx.AddResource("Length", "{PropertyName}  عدد الحروف يجب ان يكون  أصغر من{1}.");
                resx.AddResource("LessThan", "{PropertyName} يجب أن يكون أقل من {0}.");
                resx.AddResource("LessThanOrEqual", "{PropertyName} يجب أن يكون أقل أو يساوي {0}.");
                resx.AddResource("NotEmpty", "{PropertyName} يجب أن يحتوي على قيمة.");
                resx.AddResource("NotEqual", "{PropertyName} يجب أن لا يساوي {0}.");
                resx.AddResource("RegularExpression", "{PropertyName} يجب أن يكون في الصغية الصحيحة.");

                foreach (var key in keysList)
                {
                    resx.AddResource(key.Key, UtilityHelper.FixName(key.Value));
                }
            }
            #endregion

        }
    }
}
