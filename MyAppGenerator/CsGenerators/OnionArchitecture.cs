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
    public static class OnionArchitecture
    {
        #region Create Folders 
        public static string OutputDirectory { get; set; }
        public static string ProjectName { get; set; }
        public static string DataPath { get; set; }
        public static string InfraDataInitializerPath { get; set; }
        public static string InfraDataFolderPath { get; set; }
        public static string InfraEntityConfigurationPath { get; set; }
        public static string InfraExtensionsPath { get; set; }
        public static string InfraInterfacesPath { get; set; }
        public static string InfraInterfacesDataPath { get; set; }
        public static string InfraInterfacesRepositoriesPath { get; set; }
        public static string InfraRepositoriesPath { get; set; }
        public static string InfraRepositoriesDataPath { get; set; }
        public static string DomainPath { get; set; }
        public static string DomainCommonPath { get; set; }
        public static string DomainEntitiesPath { get; set; }
        public static string DomainEnumsPath { get; set; }
        public static string DomainResourcesPath { get; set; }
        public static string ApplicationPath { get; set; }
        public static string ApplicationDtosPath { get; set; }
        public static string ApplicationMappingPath { get; set; }
        public static string ApplicationServicesPath { get; set; }
        public static string ApplicationServicesImplementationsPath { get; set; }
        public static string ApplicationServicesInterfacesPath { get; set; }
        public static string ApplicationValidationsPath { get; set; }

        public static List<NodeType> tables { get; set; }
        public static bool useResourceFile { get; set; }

        public static string DomainNameSpace = ProjectName + ".Domain";
        public static string DataAccessNameSpace = ProjectName + ".Infrastructure";
        public static string ApplicationNameSpace = ProjectName + ".Application";

        #endregion

        public static void GenerateOnionArchitecture(string projectName, string outputDirectory)
        {
            DomainNameSpace = projectName + ".Domain";
            DataAccessNameSpace = projectName + ".Infrastructure";
            ApplicationNameSpace = projectName + ".Application";

            CreateFoldersForOnionArchitecture(outputDirectory);
            CreateDomainClasses();
            CreateInfrastructureClasses();
            CreateApplicationClasses();
            CreateResourceFile();
        }
        private static void CreateFoldersForOnionArchitecture(string outputDirectory)
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


            InfraExtensionsPath = Path.Combine(outputDirectory, "Infrastructure/Extensions");
            UtilityHelper.CreateSubDirectory(InfraExtensionsPath, true);

            InfraInterfacesPath = Path.Combine(outputDirectory, "Infrastructure/Interfaces");
            UtilityHelper.CreateSubDirectory(InfraInterfacesPath, true);

            InfraInterfacesDataPath = Path.Combine(outputDirectory, "Infrastructure/Interfaces/Data");
            UtilityHelper.CreateSubDirectory(InfraInterfacesDataPath, true);

            InfraInterfacesRepositoriesPath = Path.Combine(outputDirectory, "Infrastructure/Interfaces/Repositories");
            UtilityHelper.CreateSubDirectory(InfraInterfacesRepositoriesPath, true);


            InfraRepositoriesPath = Path.Combine(outputDirectory, "Infrastructure/Repositories");
            UtilityHelper.CreateSubDirectory(InfraRepositoriesPath, true); //partial

            InfraRepositoriesDataPath = Path.Combine(outputDirectory, "Infrastructure/Repositories/Data");
            UtilityHelper.CreateSubDirectory(InfraRepositoriesDataPath, true); //partial


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

            ApplicationDtosPath = Path.Combine(ApplicationPath, "Dtos");
            UtilityHelper.CreateSubDirectory(ApplicationDtosPath, true);


            ApplicationMappingPath = Path.Combine(ApplicationPath, "Mapping");
            UtilityHelper.CreateSubDirectory(ApplicationMappingPath, true);

            ApplicationServicesPath = Path.Combine(ApplicationPath, "Services");
            UtilityHelper.CreateSubDirectory(ApplicationServicesPath, true);


            ApplicationServicesImplementationsPath = Path.Combine(ApplicationServicesPath, "Implementations");
            UtilityHelper.CreateSubDirectory(ApplicationServicesImplementationsPath, true);


            ApplicationServicesInterfacesPath = Path.Combine(ApplicationServicesPath, "Interfaces");
            UtilityHelper.CreateSubDirectory(ApplicationServicesInterfacesPath, true);


            ApplicationValidationsPath = Path.Combine(ApplicationPath, "Validations");
            UtilityHelper.CreateSubDirectory(ApplicationValidationsPath, true);


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

            #region Extensions Classes

            #region  EnumerablePagedListExtensions

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraExtensionsPath, "EnumerablePagedListExtensions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DataAccessNameSpace + @".Interfaces.Data;
using " + DataAccessNameSpace + @".Repositories;
using System;
using System.Collections.Generic;

namespace " + DataAccessNameSpace + @".Extensions
{
    /// <summary>
    /// Provides some extension methods for <see cref='IEnumerable{T}'/> to provide paging capability.
    /// </summary>
    public static class EnumerablePagedListExtensions
    {
        /// <summary>
        /// Converts the specified source to <see cref='IPagedList{T}'/> by the specified <paramref name='pageIndex'/> and <paramref name='pageSize'/>.
        /// </summary>
        /// <typeparam name='T'>The type of the source.</typeparam>
        /// <param name='source'>The source to paging.</param>
        /// <param name='pageIndex'>The index of the page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='indexFrom'>The start index value.</param>
        /// <returns>An instance of the inherited from <see cref='IPagedList{T}'/> interface.</returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom = 0) => new PagedList<T>(source, pageIndex, pageSize, indexFrom);

        /// <summary>
        /// Converts the specified source to <see cref='IPagedList{T}'/> by the specified <paramref name='converter'/>, <paramref name='pageIndex'/> and <paramref name='pageSize'/>
        /// </summary>
        /// <typeparam name='TSource'>The type of the source.</typeparam>
        /// <typeparam name='TResult'>The type of the result</typeparam>
        /// <param name='source'>The source to convert.</param>
        /// <param name='converter'>The converter to change the <typeparamref name='TSource'/> to <typeparamref name='TResult'/>.</param>
        /// <param name='pageIndex'>The page index.</param>
        /// <param name='pageSize'>The page size.</param>
        /// <param name='indexFrom'>The start index value.</param>
        /// <returns>An instance of the inherited from <see cref='IPagedList{T}'/> interface.</returns>
        public static IPagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom = 0) => new PagedList<TSource, TResult>(source, converter, pageIndex, pageSize, indexFrom);
    }
}

");

            }

            #endregion

            #region QueryablePageListExtensions





            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraExtensionsPath, "QueryablePageListExtensions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DataAccessNameSpace + @".Interfaces.Data;
using " + DataAccessNameSpace + @".Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Extensions
{
    public static class QueryablePageListExtensions
    {
        /// <summary>
        /// Converts the specified source to <see cref='IPagedList{T}'/> by the specified <paramref name='pageIndex'/> and <paramref name='pageSize'/>.
        /// </summary>
        /// <typeparam name='T'>The type of the source.</typeparam>
        /// <param name='source'>The source to paging.</param>
        /// <param name='pageIndex'>The index of the page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name='indexFrom'>The start index value.</param>
        /// <returns>An instance of the inherited from <see cref='IPagedList{T}'/> interface.</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($""indexFrom: { indexFrom} > pageIndex: { pageIndex}, must indexFrom <= pageIndex"");
            }

            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip((pageIndex - indexFrom) * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

            var pagedList = new PagedList<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                TotalCount = count,
                Items = items,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };

            return pagedList;
        }
    }
}

");

            }

            #endregion

            #endregion

            #region Interfaces


            #region Data

            //IDbInitializer
            #region IDbInitializer
            using (
               StreamWriter streamWriter =
                   new StreamWriter(Path.Combine(InfraInterfacesDataPath, "IDbInitializer.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
namespace " + DataAccessNameSpace + @".Interfaces.Data {
    public interface IDbInitializer
    {
        void Initialize();
    }
}

");

            }

            #endregion


            //IPagedList
            #region IPagedList

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraInterfacesDataPath, "IPagedList.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System.Collections.Generic;
namespace " + DataAccessNameSpace + @".Interfaces.Data
{
    /// <summary>
    /// Provides the interface(s) for paged list of any type.
    /// </summary>
    /// <typeparam name='T'>The type for paging.</typeparam>
        public interface IPagedList<T>
        {
            /// <summary>
            /// Gets the index start value.
            /// </summary>
            /// <value>The index start value.</value>
            int IndexFrom { get; }
            /// <summary>
            /// Gets the page index (current).
            /// </summary>
            int PageIndex { get; }
            /// <summary>
            /// Gets the page size.
            /// </summary>
            int PageSize { get; }
            /// <summary>
            /// Gets the total count of the list of type <typeparamref name='T'/>
            /// </summary>
            int TotalCount { get; }
            /// <summary>
            /// Gets the total pages.
            /// </summary>
            int TotalPages { get; }
            /// <summary>
            /// Gets the current page items.
            /// </summary>
            IList<T> Items { get; }
            /// <summary>
            /// Gets the has previous page.
            /// </summary>
            /// <value>The has previous page.</value>
            bool HasPreviousPage { get; }
            /// <summary>
            /// Gets the has next page.
            /// </summary>
            /// <value>The has next page.</value>
            bool HasNextPage { get; }




            }
        }

");

            }

            #endregion


            //IRepository
            #region IRepository
            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraInterfacesDataPath, "IRepository.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
namespace " + DataAccessNameSpace + @".Interfaces.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region VARIOUS
        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref='IQueryable{TEntity}'/>.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Gets the count based on a predicate.
        /// </summary>
        /// <param name='predicate'></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate = null);
        #endregion

        #region PAGEDLIST
        /// <summary>
        /// Gets the <see cref='IPagedList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                         int pageIndex = 0,
                                         int pageSize = 10,
                                         bool disableTracking = true);

        /// <summary>
        /// Gets the <see cref='IPagedList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                    int pageIndex = 0,
                                                    int pageSize = 20,
                                                    bool disableTracking = true,
                                                    CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref='IPagedList{TResult}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TResult}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                  Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  int pageIndex = 0,
                                                  int pageSize = 20,
                                                  bool disableTracking = true) where TResult : class;

        /// <summary>
        /// Gets the <see cref='IPagedList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                             Expression<Func<TEntity, bool>> predicate = null,
                                                             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                             int pageIndex = 0,
                                                             int pageSize = 20,
                                                             bool disableTracking = true,
                                                             CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
        #endregion

        #region List
        IList<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken));

        IList<TResult> GetList<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true) where TResult : class;

        Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
        #endregion

        #region FIRSTORDEFAULT 
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{T}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                  bool disableTracking = true);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                           Expression<Func<TEntity, bool>> predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                           bool disableTracking = true);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query. </remarks>
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true);
        #endregion

        #region FIND 
        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name='keyValues'>The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name='keyValues'>The values of the primary key for the entity to be found.</param>
        /// <returns>A <see cref='Task{TEntity}'/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        Task<TEntity> FindAsync(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name='keyValues'>The values of the primary key for the entity to be found.</param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref='Task{TEntity}'/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);


        bool Any(Expression<Func<TEntity, bool>> predicate = null);


        /// <summary>
        /// bool based on a predicate.
        /// </summary>
        /// <param name='predicate'></param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>true or false</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null,
            CancellationToken cancellationToken = default(CancellationToken));


        #endregion

        #region INSERT
        /// <summary>
        /// Inserts a new entity synchronously.
        /// </summary>
        /// <param name='entity'>The entity to insert.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        void Insert(params TEntity[] entities);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts a new entity asynchronously.
        /// </summary>
        /// <param name='entity'>The entity to insert.</param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref='Task'/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        /// <returns>A <see cref='Task'/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(params TEntity[] entities);

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref='Task'/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region UPDATE
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name='entity'>The entity.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        void Update(params TEntity[] entities);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        void Update(IEnumerable<TEntity> entities);
        #endregion

        #region DELETE
        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name='id'>The primary key value.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name='entity'>The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        void Delete(params TEntity[] entities);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        void Delete(IEnumerable<TEntity> entities);
        #endregion
    }
}

");

            }

            #endregion


            //IRepositoryFactory
            #region IRepositoryFactory
            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraInterfacesDataPath, "IRepositoryFactory.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"

namespace " + DataAccessNameSpace + @".Interfaces.Data
{
    /// <summary>
    /// Defines the interfaces for <see cref='IRepository{TEntity}'/> interfaces.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Gets the specified repository for the <typeparamref name='TEntity'/>.
        /// </summary>
        /// <typeparam name='TEntity'>The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref='IRepository{TEntity}'/> interface.</returns>
       // IRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;


       //   TEntity GetRepository<TEntity>() where TEntity : class;

    }
}

");

            }

            #endregion



            //IUnitOfWork
            #region IUnitOfWork
            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraInterfacesDataPath, "IUnitOfWork.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Linq;
using System.Threading.Tasks;
namespace " + DataAccessNameSpace + @".Interfaces.Data
{
   /// <summary>
    /// Defines the interface(s) for unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {


        #region Supported Repositories
       


        #endregion
        /// <summary>
        /// Gets the specified repository for the <typeparamref name='TEntity'/>.
        /// </summary>
        /// <typeparam name='TEntity'>The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref='IRepository{TEntity}'/> interface.</returns>
     //   IRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
        TEntity GetRepository<TEntity>() where TEntity : class;




        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name='ensureAutoHistory'><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name='ensureAutoHistory'><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>A <see cref='Task{TResult}'/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name='sql'>The raw SQL.</param>
        /// <param name='parameters'>The parameters.</param>
        /// <returns>The number of state entities written to database.</returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);

        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name='TEntity'/> data.
        /// </summary>
        /// <typeparam name='TEntity'>The type of the entity.</typeparam>
        /// <param name='sql'>The raw SQL.</param>
        /// <param name='parameters'>The parameters.</param>
        /// <returns>An <see cref='IQueryable{T}'/> that contains elements that satisfy the condition specified by raw SQL.</returns>
        IQueryable<TEntity> FromSqlRaw<TEntity>(string sql, params object[] parameters) where TEntity : class;
    }
}

");

            }

            #endregion


            #endregion

            #region Repositories

            foreach (var table in tables)
            {

                string className = "";

                #region  Model Classes
                className = "I" + table.Title;

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraInterfacesRepositoriesPath, className + "Repository.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Entities;");
                    streamWriter.WriteLine("using " + DataAccessNameSpace + ".Interfaces.Data;");
                    streamWriter.WriteLine(@"namespace " + DataAccessNameSpace + @".Interfaces.Repositories
{
    public interface " + className + "Repository : IRepository<" + table.Title + @">
    {
        //Task<List<KeyValue>> GetAllForDropDownLis();
    }
}"
                                           );









                }
                #endregion


            }

            #endregion

            #endregion

            #region Implementations

            #region Repositories

            foreach (var table in tables)
            {

                string className = "";

                #region  Model Classes
                className = table.Title;

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraRepositoriesPath, className + "Repository.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Entities;");
                    streamWriter.WriteLine("using " + DataAccessNameSpace + ".Interfaces.Repositories;");
                    streamWriter.WriteLine(@"using Microsoft.EntityFrameworkCore;
namespace " + DataAccessNameSpace + @".Repositories
{
   public class " + table.Title + @"Repository : Repository<" + table.Title + @">, I" + table.Title + @"Repository
    {


        public " + table.Title + @"Repository(DbContext dbContext) : base(dbContext)
        {

        }

        //public async Task<List<KeyValue>> GetAllForDropDownLis()
        //{
        //    return await _dbSet.AsNoTracking().Select(o => new KeyValue() { Id = o.BookTypeId, Value = o.Title }).ToListAsync();
        //}
    }
}"
                    );









                }
                #endregion


            }

            #endregion

            #region PagedList


            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraRepositoriesDataPath, "PagedList.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"using " + DataAccessNameSpace + @".Interfaces.Data;
                using System;
                using System.Collections.Generic;
                using System.Linq;

namespace  " + DataAccessNameSpace + @".Repositories
    {
        /// <summary>
        /// Represents the default implementation of the <see cref='IPagedList{T}'/> interface.
        /// </summary>
        /// <typeparam name='T'>The type of the data to page</typeparam>
        public class PagedList<T> : IPagedList<T>
        {
            /// <summary>
            /// Gets or sets the index of the page.
            /// </summary>
            /// <value>The index of the page.</value>
            public int PageIndex { get; set; }
            /// <summary>
            /// Gets or sets the size of the page.
            /// </summary>
            /// <value>The size of the page.</value>
            public int PageSize { get; set; }
            /// <summary>
            /// Gets or sets the total count.
            /// </summary>
            /// <value>The total count.</value>
            public int TotalCount { get; set; }
            /// <summary>
            /// Gets or sets the total pages.
            /// </summary>
            /// <value>The total pages.</value>
            public int TotalPages { get; set; }
            /// <summary>
            /// Gets or sets the index from.
            /// </summary>
            /// <value>The index from.</value>
            public int IndexFrom { get; set; }

            /// <summary>
            /// Gets or sets the items.
            /// </summary>
            /// <value>The items.</value>
            public IList<T> Items { get; set; }

            /// <summary>
            /// Gets the has previous page.
            /// </summary>
            /// <value>The has previous page.</value>
            public bool HasPreviousPage => PageIndex - IndexFrom > 0;

            /// <summary>
            /// Gets the has next page.
            /// </summary>
            /// <value>The has next page.</value>
            public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

            /// <summary>
            /// Initializes a new instance of the <see cref='PagedList{T}' /> class.
            /// </summary>
            /// <param name='source'>The source.</param>
            /// <param name='pageIndex'>The index of the page.</param>
            /// <param name='pageSize'>The size of the page.</param>
            /// <param name='indexFrom'>The index from.</param>
            internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
            {
                if (indexFrom > pageIndex)
                    throw new ArgumentException($""indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex"");


                if (source is IQueryable<T> querable)
                {
                    PageIndex = pageIndex;
                    PageSize = pageSize;
                    IndexFrom = indexFrom;
                    TotalCount = querable.Count();
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                    Items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
                }
                else
                {
                    PageIndex = pageIndex;
                    PageSize = pageSize;
                    IndexFrom = indexFrom;
                    TotalCount = source.Count();
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                    Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref='PagedList{T}' /> class.
            /// </summary>
            internal PagedList() => Items = new T[0];
        }


        /// <summary>
        /// Provides the implementation of the <see cref='IPagedList{T}'/> and converter.
        /// </summary>
        /// <typeparam name='TSource'>The type of the source.</typeparam>
        /// <typeparam name='TResult'>The type of the result.</typeparam>
        internal class PagedList<TSource, TResult> : IPagedList<TResult>
        {
            /// <summary>
            /// Gets the index of the page.
            /// </summary>
            /// <value>The index of the page.</value>
            public int PageIndex { get; }
            /// <summary>
            /// Gets the size of the page.
            /// </summary>
            /// <value>The size of the page.</value>
            public int PageSize { get; }
            /// <summary>
            /// Gets the total count.
            /// </summary>
            /// <value>The total count.</value>
            public int TotalCount { get; }
            /// <summary>
            /// Gets the total pages.
            /// </summary>
            /// <value>The total pages.</value>
            public int TotalPages { get; }
            /// <summary>
            /// Gets the index from.
            /// </summary>
            /// <value>The index from.</value>
            public int IndexFrom { get; }

            /// <summary>
            /// Gets the items.
            /// </summary>
            /// <value>The items.</value>
            public IList<TResult> Items { get; }

            /// <summary>
            /// Gets the has previous page.
            /// </summary>
            /// <value>The has previous page.</value>
            public bool HasPreviousPage => PageIndex - IndexFrom > 0;

            /// <summary>
            /// Gets the has next page.
            /// </summary>
            /// <value>The has next page.</value>
            public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

            /// <summary>
            /// Initializes a new instance of the <see cref='PagedList{TSource, TResult}' /> class.
            /// </summary>
            /// <param name='source'>The source.</param>
            /// <param name='converter'>The converter.</param>
            /// <param name='pageIndex'>The index of the page.</param>
            /// <param name='pageSize'>The size of the page.</param>
            /// <param name='indexFrom'>The index from.</param>
            public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom)
            {
                if (indexFrom > pageIndex)
                {
                    throw new ArgumentException($""indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex"");
                }

                if (source is IQueryable<TSource> querable)
                {
                    PageIndex = pageIndex;
                    PageSize = pageSize;
                    IndexFrom = indexFrom;
                    TotalCount = querable.Count();
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                    var items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                    Items = new List<TResult>(converter(items));
                }
                else
                {
                    PageIndex = pageIndex;
                    PageSize = pageSize;
                    IndexFrom = indexFrom;
                    TotalCount = source.Count();
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                    var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                    Items = new List<TResult>(converter(items));
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref='PagedList{TSource, TResult}' /> class.
            /// </summary>
            /// <param name='source'>The source.</param>
            /// <param name='converter'>The converter.</param>
            public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
            {
                PageIndex = source.PageIndex;
                PageSize = source.PageSize;
                IndexFrom = source.IndexFrom;
                TotalCount = source.TotalCount;
                TotalPages = source.TotalPages;

                Items = new List<TResult>(converter(source.Items));
            }
        }

        /// <summary>
        /// Provides some help methods for <see cref='IPagedList{T}'/> interface.
        /// </summary>
        public static class PagedList
        {
            /// <summary>
            /// Creates an empty of <see cref='IPagedList{T}'/>.
            /// </summary>
            /// <typeparam name='T'>The type for paging </typeparam>
            /// <returns>An empty instance of <see cref='IPagedList{T}'/>.</returns>
            public static IPagedList<T> Empty<T>() => new PagedList<T>();
            /// <summary>
            /// Creates a new instance of <see cref='IPagedList{TResult}'/> from source of <see cref='IPagedList{TSource}'/> instance.
            /// </summary>
            /// <typeparam name='TResult'>The type of the result.</typeparam>
            /// <typeparam name='TSource'>The type of the source.</typeparam>
            /// <param name='source'>The source.</param>
            /// <param name='converter'>The converter.</param>
            /// <returns>An instance of <see cref='IPagedList{TResult}'/>.</returns>
            public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) => new PagedList<TSource, TResult>(source, converter);
        }
    }
"
                );









            }

            #endregion


            #region Generic Repository


            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraRepositoriesDataPath, "Repository.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DataAccessNameSpace + @".Extensions;
using " + DataAccessNameSpace + @".Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Repositories
{
    /// <summary>
    /// Represents a default generic repository implements the <see cref='IRepository{TEntity}'/> interface.
    /// </summary>
    /// <typeparam name='TEntity'>The type of the entity.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref='Repository{TEntity}'/> class.
        /// </summary>
        /// <param name='dbContext'>The database context.</param>
        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        #region VARIOUS
        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>The <see cref='IQueryable{TEntity}'/>.</returns>
        public IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }

        /// <summary>
        /// Gets the count based on a predicate.
        /// </summary>
        /// <param name='predicate'></param>
        /// <returns></returns>
        public int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
        }

        #endregion

        #region PAGEDLIST
        /// <summary>
        /// Gets the <see cref='IPagedList{T}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                int pageIndex = 0,
                                                int pageSize = 10,
                                                bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy == null ? query.ToPagedList(pageIndex, pageSize)
                : orderBy(query).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets the <see cref='IPagedList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                           int pageIndex = 0,
                                                           int pageSize = 20,
                                                           bool disableTracking = true,
                                                           CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy?.Invoke(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
                ?? query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }

        /// <summary>
        /// Gets the <see cref='IPagedList{TResult}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TResult}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                         Expression<Func<TEntity, bool>> predicate = null,
                                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                         int pageIndex = 0,
                                                         int pageSize = 20,
                                                         bool disableTracking = true)
            where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy == null ? query.Select(selector).ToPagedList(pageIndex, pageSize)
                : orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets the <see cref='IPagedList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='pageIndex'>The index of page.</param>
        /// <param name='pageSize'>The size of the page.</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                                    Expression<Func<TEntity, bool>> predicate = null,
                                                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                                    int pageIndex = 0,
                                                                    int pageSize = 20,
                                                                    bool disableTracking = true,
                                                                    CancellationToken cancellationToken = default(CancellationToken))
            where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy?.Invoke(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
                ?? query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }
        #endregion

        #region LIST
        /// <summary>
        /// Gets the <see cref='IList{T}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public IList<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return
                orderBy?.Invoke(query).ToList()
                ?? query.ToList();
        }

        /// <summary>
        /// Gets the <see cref='IList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref='IList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                           bool disableTracking = true,
                                                           CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return
                orderBy?.Invoke(query).ToListAsync(cancellationToken)
                ?? query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the <see cref='IList{TResult}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IList{TResult}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public IList<TResult> GetList<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                         Expression<Func<TEntity, bool>> predicate = null,
                                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                         bool disableTracking = true)
            where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy?.Invoke(query).Select(selector).ToList()
                ?? query.Select(selector).ToList();
        }

        /// <summary>
        /// Gets the <see cref='IList{TEntity}'/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name='cancellationToken'>
        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref='IList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                                    Expression<Func<TEntity, bool>> predicate = null,
                                                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                                    bool disableTracking = true,
                                                                    CancellationToken cancellationToken = default(CancellationToken))
            where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy != null ?
                orderBy(query).Select(selector).ToListAsync(cancellationToken)
                : query.Select(selector).ToListAsync(cancellationToken);
        }
        #endregion

        #region FIRSTORDEFAULT
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                         bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy == null ? query.FirstOrDefault()
                : orderBy(query).FirstOrDefault();
        }


        /// <inheritdoc />
        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy != null ? await orderBy(query).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name='selector'>The selector for projection.</param>
        /// <param name='predicate'>A function to test each element for a condition.</param>
        /// <param name='orderBy'>A function to order elements.</param>
        /// <param name='include'>A function to include navigation properties</param>
        /// <param name='disableTracking'><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref='IPagedList{TEntity}'/> that contains elements that satisfy the condition specified by <paramref name='predicate'/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                  Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy != null ? orderBy(query).Select(selector).FirstOrDefault()
                : query.Select(selector).FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                  Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return orderBy != null
                ? await orderBy(query).Select(selector).FirstOrDefaultAsync()
                : await query.Select(selector).FirstOrDefaultAsync();
        }
        #endregion

        #region FIND 

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name='keyValues'>The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        public TEntity Find(params object[] keyValues)
        {
            var findResult = _dbSet.Find(keyValues);

            if (findResult == null)
                return null;

            //var isDeleted = findResult.GetType().GetProperty('IsDeleted');
            //if ((findResult.GetType().GetProperty('IsDeleted') != null) &&
            //    ((bool)isDeleted.GetValue(findResult) == true))
            //    return null;

            return _dbSet.Find(keyValues);
        }
        //=> _dbSet.Where(x => x.)
        //.Find(keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name='keyValues'>The values of the primary key for the entity to be found.</param>
        /// <returns>A <see cref='Task{TEntity}' /> that represents the asynchronous insert operation.</returns>
        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name='keyValues'>The values of the primary key for the entity to be found.</param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref='Task{TEntity}'/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        public async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(keyValues, cancellationToken);
        }



        /// <summary>
        /// bool based on a predicate.
        /// </summary>
        /// <param name='predicate'></param>
        /// <returns></returns>
        public bool Any(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet.Any() : _dbSet.Any(predicate);
        }


        /// <summary>
        /// bool based on a predicate.
        /// </summary>
        /// <param name='predicate'></param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>true or false</returns>
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return predicate == null ? _dbSet.AnyAsync(cancellationToken) : _dbSet.AnyAsync(predicate, cancellationToken);
        }

        #endregion

        #region INSERT
        /// <summary>
        /// Inserts a new entity synchronously.
        /// </summary>
        /// <param name='entity'>The entity to insert.</param>
        public void Insert(TEntity entity)
            => _dbSet.Add(entity);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        public void Insert(params TEntity[] entities)
            => _dbSet.AddRange(entities);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        public void Insert(IEnumerable<TEntity> entities)
            => _dbSet.AddRange(entities);

        /// <summary>
        /// Inserts a new entity asynchronously.
        /// </summary>
        /// <param name='entity'>The entity to insert.</param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref='Task'/> that represents the asynchronous insert operation.</returns>
        public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dbSet.AddAsync(entity, cancellationToken);

            // Shadow properties?
            //var property = _dbContext.Entry(entity).Property('Created');
            //if (property != null) {
            //property.CurrentValue = DateTime.Now;
            //}
        }

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        /// <returns>A <see cref='Task' /> that represents the asynchronous insert operation.</returns>
        public Task InsertAsync(params TEntity[] entities)
            => _dbSet.AddRangeAsync(entities);

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name='entities'>The entities to insert.</param>
        /// <param name='cancellationToken'>A <see cref='CancellationToken'/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref='Task'/> that represents the asynchronous insert operation.</returns>
        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
            => _dbSet.AddRangeAsync(entities, cancellationToken);

        #endregion

        #region UPDATE
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name='entity'>The entity.</param>
        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name='entity'>The entity.</param>
        public void UpdateAsync(TEntity entity)
            => _dbSet.Update(entity);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        public void Update(params TEntity[] entities)
            => _dbSet.UpdateRange(entities);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        public void Update(IEnumerable<TEntity> entities)
            => _dbSet.UpdateRange(entities);
        #endregion

        #region DELETE
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name='entity'>The entity to delete.</param>
        public void Delete(TEntity entity)
            => _dbSet.Remove(entity);

        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name='id'>The primary key value.</param>
        public void Delete(object id)
        {
            // using a stub entity to mark for deletion
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = _dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);

            if (property == null)
            {
                var ent = _dbSet.Find(id);
                if (ent != null)
                    Delete(ent);
                return;
            }

            var entity = Activator.CreateInstance<TEntity>();
            property.SetValue(entity, id);
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        public void Delete(params TEntity[] entities)
            => _dbSet.RemoveRange(entities);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name='entities'>The entities.</param>
        public void Delete(IEnumerable<TEntity> entities)
            => _dbSet.RemoveRange(entities);
        #endregion

        #region RAW SQL
        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name='TEntity' /> data.
        /// </summary>
        /// <param name='sql'>The raw SQL.</param>
        /// <param name='parameters'>The parameters.</param>
        /// <returns>An <see cref='IQueryable{TEntity}' /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
        {
            return _dbSet.FromSqlRaw(sql, parameters);
        }
        #endregion
    }
}"
                );









            }

            #endregion


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
using " + DataAccessNameSpace + @".Interfaces.Data;
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
using " + DomainNameSpace + @".Entities;
using " + DataAccessNameSpace + @".EntityConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace " + DataAccessNameSpace + @".Data
{
  public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
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


            #region UnitOfWork

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataFolderPath, "UnitOfWork.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using " + DataAccessNameSpace + @".Interfaces.Data;
using " + DataAccessNameSpace + @".Interfaces.Repositories;
using " + DataAccessNameSpace + @".Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace " + DataAccessNameSpace + @".Data {
  /// <summary>
    /// Represents the default implementation of the <see cref='IUnitOfWork'/> and <see cref='IUnitOfWork{TContext}'/> interface.
    /// </summary>
    /// <typeparam name='TContext'>The type of the db context.</typeparam>
    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork where TContext : DbContext
    {
        private bool _disposed = false;
        private Dictionary<Type, object> _repositories;
        private readonly TContext _context;

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name='TContext'/>.</returns>
        public TContext DbContext
        {
            get { return _context; }
        }


    

        /// <summary>
        /// Initializes a new instance of the <see cref='UnitOfWork{TContext}'/> class.
        /// </summary>
        /// <param name='context'>The context.</param>
        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

    


         #region SQL
        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name='TEntity' /> data.
        /// </summary>
        /// <typeparam name='TEntity'>The type of the entity.</typeparam>
        /// <param name='sql'>The raw SQL.</param>
        /// <param name='parameters'>The parameters.</param>
        /// <returns>An <see cref='IQueryable{T}' /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public IQueryable<TEntity> FromSqlRaw<TEntity>(string sql, params object[] parameters) where TEntity : class
        {
            return _context.Set<TEntity>().FromSqlRaw(sql, parameters);
        }

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name='sql'>The raw SQL.</param>
        /// <param name='parameters'>The parameters.</param>
        /// <returns>The number of state entities written to database.</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _context.Database.ExecuteSqlCommand(sql, parameters);
        }

        #endregion

        #region GET



        ///// <summary>
        ///// Gets the specified repository for the <typeparamref name='TEntity'/>.
        ///// </summary>
        ///// <typeparam name='TEntity'>The type of the entity.</typeparam>
        ///// <returns>An instance of type inherited from <see cref='IRepository{TEntity}'/> interface.</returns>
        //public IRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
        //{
        //    if (_repositories == null)
        //        _repositories = new Dictionary<Type, object>();

        //    var type = typeof(TEntity);
        //    if (!_repositories.ContainsKey(type))
        //        _repositories[type] = new Repository<TEntity>(_context);

        //    return (IRepository<TEntity>)_repositories[type];
        //}


        public  TEntity GetRepository<TEntity>() where TEntity : class
        {

            var interfaceType = typeof(TEntity);


            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();

              
            }
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetInterface(interfaceType.Name) != null).FirstOrDefault();

            if (type != null)
            {
                if (!_repositories.ContainsKey(interfaceType))
                {
                    _repositories[interfaceType] = Activator.CreateInstance(type, _context);
                }

                return (TEntity) _repositories[interfaceType];

            }


            return null;
        }


        #endregion

        #region SAVE
        /// <inheritdoc />
        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name='ensureAutoHistory'><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {

            return _context.SaveChanges();


        }

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name='ensureAutoHistory'><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>A <see cref='Task{TResult}'/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync()
        {

            return await _context.SaveChangesAsync();


        }

        #endregion

        #region Dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name='disposing'>The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {   // clear repositories
                _repositories?.Clear();
               
                // dispose the db context.
                _context.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}

");




            }


            #endregion


            #region Infrastructure Dependency Injection



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DataPath, "DependencyInjection.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using " + DataAccessNameSpace + @".Data;
using " + DataAccessNameSpace + @".Data.Initializer;
using " + DataAccessNameSpace + @".Interfaces.Data;
using " + DomainNameSpace + @".Entities;

namespace " + DataAccessNameSpace + @"
{
   public static class DependencyInjection
    {
        public static readonly LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public static IServiceCollection AddDataBaseAccess(this IServiceCollection services, IConfiguration configuration)
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
                services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();

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


                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationDtosPath, classNameReadDto + ".cs")))
                {
                    string clsName = sb.ToString().Replace("#className#", classNameReadDto).ToString();
                    streamWriter.WriteLine(clsName);
                }



                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationDtosPath, classNameCreateDto + ".cs")))
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

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationValidationsPath, classNameCreateDto + ".cs")))
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

                sb.AppendLine("\t}");
                sb.AppendLine("}");
                sb.AppendLine("}");


                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationMappingPath, className + "Profile.cs")))
                {
                    string clsName = sb.ToString().Replace("#className#", classNameReadDto).ToString();
                    streamWriter.WriteLine(clsName);
                }





                #endregion

                #region Appliucation Service Interface



                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationServicesInterfacesPath, "I" + className + "Service.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Common;");
                    streamWriter.WriteLine("using " + ApplicationNameSpace + ".Dtos;");
                    streamWriter.WriteLine(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace " + ApplicationNameSpace + @".Services.Interfaces
{
   public partial interface I" + table.Name + @"Service 
    {
        Task<ClientMessage<int>> Add" + table.Name + @"Async(" + table.Name + @"UpsertDto " + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);
        Task<ClientMessage<int>> Update" + table.Name + @"Async(" + table.Name + @"UpsertDto " + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);
        Task<ClientMessage<int>> Delete" + table.Name + @"Async(int id);
        Task<ClientMessage<" + table.Name + @"ReadDto>> Get" + table.Name + @"ByIdAsync(int id);
        Task<ClientMessage<List<" + table.Name + @"ReadDto>>> GetAll" + table.Name + @"Async();

    }
}"
                    );









                }
                #endregion

                #region Application Service Impelemtation

                var titleName = UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table);

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationServicesImplementationsPath, className + "Service.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(@"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + ApplicationNameSpace + @".Services.Interfaces;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Enums;
using " + DataAccessNameSpace + @".Interfaces.Data;
using " + DataAccessNameSpace + @".Interfaces.Repositories;
using " + DataAccessNameSpace + @".Repositories;
using " + DataAccessNameSpace + @";

namespace " + ApplicationNameSpace + @".Services.Implementations
{
   public partial class " + table.Name + @"Service : I" + table.Name + @"Service
    {
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public " + table.Name + @"Service(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<ClientMessage<int>> Add" + table.Name + @"Async(" + table.Name + @"UpsertDto " +
                                          UtilityHelper.FormatCamel(table.Name) + @"UpsertDto)
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

                if (await _unitOfWork.GetRepository<I" + table.Name + @"Repository>().AnyAsync(o => o." +
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

                    await _unitOfWork.GetRepository<I" + table.Name + @"Repository>().InsertAsync(" +
                                           UtilityHelper.FormatCamel(table.Name) + @");

                    int effectedRows = await _unitOfWork.SaveChangesAsync();
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

                    await _unitOfWork.GetRepository<I" + table.Name + @"Repository>().InsertAsync(" +
                                           UtilityHelper.FormatCamel(table.Name) + @");

                    int effectedRows = await _unitOfWork.SaveChangesAsync();
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

    
        public async Task<ClientMessage<int>> Update" + table.Name + "Async(" + table.Name + "UpsertDto " +
                                           UtilityHelper.FormatCamel(table.Name) + @"UpsertDto)
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

                if (await _unitOfWork.GetRepository<I" + table.Name + @"Repository>().AnyAsync(o => o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @".ToUpper() == " +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @".ToUpper() && o." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" != " +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @"))
                {
                    clientMessage.ClientStatusCode = AppEnums.OperationStatus.Error;
                    clientMessage.ClientMessageContent = new List<string>()
                    {
                        string.Format(SD.ExistData," + UtilityHelper.FormatCamel(table.Name) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @")
                    };
                }
                else
                {
                    " + table.Name + " " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name + @">(" +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

                    _unitOfWork.GetRepository<I" + table.Name + @"Repository>().Update(" + UtilityHelper.FormatCamel(table.Name) +
                                               @");

                    int effectedRows = await _unitOfWork.SaveChangesAsync();
                    if (effectedRows != 0)
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                        clientMessage.ReturnedData = effectedRows;
                    }
                }
");
                    }
                    else
                    {

                        streamWriter.WriteLine(@"

                    " + table.Name + " " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name + @">(" +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);

                    _unitOfWork.GetRepository<I" + table.Name + @"Repository>().Update(" + UtilityHelper.FormatCamel(table.Name) +
                                               @");

                    int effectedRows = await _unitOfWork.SaveChangesAsync();
                    if (effectedRows != 0)
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                        clientMessage.ReturnedData = effectedRows;
                    }
              
");

                    }


                    streamWriter.WriteLine(@"    }

            return clientMessage;
        }



        public async Task<ClientMessage<" + table.Name + @"ReadDto>> Get" + table.Name + @"ByIdAsync(int id)
        {
            ClientMessage<" + table.Name + @"ReadDto> clientMessage = new ClientMessage<" + table.Name + @"ReadDto>();
         
            var result = await _unitOfWork.GetRepository<I" + table.Name + @"Repository>().GetFirstOrDefaultAsync(o => o." + UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" == id, null, null, true);
            
            clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
            clientMessage.ReturnedData = _mapper.Map<" + table.Name + @"ReadDto>(result);

            return clientMessage;
        }


        public async Task<ClientMessage<List<" + table.Name + @"ReadDto>>> GetAll" + table.Name + @"Async()
        {
            ClientMessage<List<" + table.Name + @"ReadDto>> clientMessage = new ClientMessage<List<" + table.Name + @"ReadDto>>();

            var result = await _unitOfWork.GetRepository<I" + table.Name + @"Repository>().GetListAsync();

            clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
            clientMessage.ReturnedData = _mapper.Map<List<" + table.Name + @"ReadDto>>(result);

            return clientMessage;
        }

        
        public async Task<ClientMessage<int>> Delete" + table.Name + @"Async(int id)
        {
            ClientMessage<int> clientMessage = new ClientMessage<int>();

            _unitOfWork.GetRepository<I" + table.Name + @"Repository>().Delete(id);

            int effectedRows = await _unitOfWork.SaveChangesAsync();
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

            }


            #region Application Dependency Injection



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationPath, "DependencyInjection.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"

using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using " + ApplicationNameSpace + @".Services.Implementations;
using " + ApplicationNameSpace + @".Services.Interfaces;
using System.Reflection;


namespace " + ApplicationNameSpace + @"
{
   public static class DependencyInjection
    {
       
      public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


");
                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("services.AddTransient<I" + table.Name + "Service, " + table.Name + "Service>();");
                }

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
