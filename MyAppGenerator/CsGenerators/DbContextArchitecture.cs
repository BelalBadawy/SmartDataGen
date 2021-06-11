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
    public static class DbContextArchitecture
    {
        public static DateTime createdDateTime = new DateTime();
        private static AppSetting _appSetting { get; set; }

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

        public static string AuthorizationPath { get; set; }

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
        public static string ApplicationExtensionsPath { get; set; }
        public static string ResourcesPath { get; set; }
        public static string ResourcesDataPath { get; set; }
        public static string ResourcesLanguagesPath { get; set; }
        public static string ResourcesModelsPath { get; set; }
        public static string ResourcesServicesPath { get; set; }


        public static List<NodeType> tables { get; set; }
        public static bool useResourceFile { get; set; }

        public static string DomainNameSpace = ProjectName + ".Domain";
        public static string DataAccessNameSpace = ProjectName + ".Infrastructure";
        public static string ApplicationNameSpace = ProjectName + ".Application";


        #endregion

        static List<Table> tableList = new List<Table>();

        public static void GenerateDbContextArchitecture(AppSetting appSetting)
        {
            _appSetting = appSetting;
            DomainNameSpace = appSetting.ProjectName + ".Domain";
            DataAccessNameSpace = appSetting.ProjectName + ".Infrastructure";
            ApplicationNameSpace = appSetting.ProjectName + ".Application";

            CreateFoldersForDbContextArchitecture(appSetting.OutputDirectory);
            CreateDomainClasses();
            CreateInfrastructureClasses();
            CreateApplicationClasses();
            CreateAuthorizationClasses();
            CreateResourceFile();
        }

        private static void CreateFoldersForDbContextArchitecture(string outputDirectory)
        {
            createdDateTime = DateTime.Now;
            OutputDirectory = outputDirectory;

            DataPath = (string.IsNullOrEmpty(_appSetting.InfrastructureCustomPath) ? Path.Combine(outputDirectory, "Infrastructure") : _appSetting.InfrastructureCustomPath);
            UtilityHelper.CreateSubDirectory(DataPath, true);

            InfraDataFolderPath = Path.Combine(DataPath, "Data");
            UtilityHelper.CreateSubDirectory(InfraDataFolderPath, true);

            InfraDataInitializerPath = Path.Combine(DataPath, "Data/Initializer");
            UtilityHelper.CreateSubDirectory(InfraDataInitializerPath, true);

            InfraEntityConfigurationPath = Path.Combine(DataPath, "EntityConfiguration");
            UtilityHelper.CreateSubDirectory(InfraEntityConfigurationPath, true);


            DomainPath = (string.IsNullOrEmpty(_appSetting.DomainCustomPath) ? Path.Combine(outputDirectory, "Domain") : _appSetting.DomainCustomPath);
            UtilityHelper.CreateSubDirectory(DomainPath, true);


            DomainCommonPath = Path.Combine(DomainPath, "Common");
            UtilityHelper.CreateSubDirectory(DomainCommonPath, true);


            DomainEntitiesPath = Path.Combine(DomainPath, "Entities");
            UtilityHelper.CreateSubDirectory(DomainEntitiesPath, true);


            DomainEnumsPath = Path.Combine(DomainPath, "Enums");
            UtilityHelper.CreateSubDirectory(DomainEnumsPath, true);


            DomainResourcesPath = Path.Combine(DomainPath, "Resources");
            UtilityHelper.CreateSubDirectory(DomainResourcesPath, true);



            ApplicationPath = (string.IsNullOrEmpty(_appSetting.ApplicationCustomPath) ? Path.Combine(outputDirectory, "Application") : _appSetting.ApplicationCustomPath);
            UtilityHelper.CreateSubDirectory(ApplicationPath, true);

            ApplicationDtosPath = Path.Combine(ApplicationPath, "Dtos");
            UtilityHelper.CreateSubDirectory(ApplicationDtosPath, true);


            ApplicationMappingPath = Path.Combine(ApplicationPath, "Mapping");
            UtilityHelper.CreateSubDirectory(ApplicationMappingPath, true);

            ApplicationServicesPath = Path.Combine(ApplicationPath, "Services");
            UtilityHelper.CreateSubDirectory(ApplicationServicesPath, true);


            ApplicationExtensionsPath = Path.Combine(ApplicationPath, "Extensions");
            UtilityHelper.CreateSubDirectory(ApplicationExtensionsPath, true);

            ApplicationServicesImplementationsPath = Path.Combine(ApplicationServicesPath, "Implementations");
            UtilityHelper.CreateSubDirectory(ApplicationServicesImplementationsPath, true);


            ApplicationServicesInterfacesPath = Path.Combine(ApplicationServicesPath, "Interfaces");
            UtilityHelper.CreateSubDirectory(ApplicationServicesInterfacesPath, true);


            ApplicationValidationsPath = Path.Combine(ApplicationPath, "Validations");
            UtilityHelper.CreateSubDirectory(ApplicationValidationsPath, true);

            ResourcesPath = (string.IsNullOrEmpty(_appSetting.ResourcesCustomPath) ? Path.Combine(outputDirectory, "Resources") : _appSetting.ResourcesCustomPath);
            UtilityHelper.CreateSubDirectory(ResourcesPath, true);

            ResourcesDataPath = Path.Combine(ResourcesPath, "Data");
            UtilityHelper.CreateSubDirectory(ResourcesDataPath, true);

            ResourcesLanguagesPath = Path.Combine(ResourcesPath, "Languages");
            UtilityHelper.CreateSubDirectory(ResourcesLanguagesPath, true);

            ResourcesModelsPath = Path.Combine(ResourcesPath, "Models");
            UtilityHelper.CreateSubDirectory(ResourcesModelsPath, true);

            ResourcesServicesPath = Path.Combine(ResourcesPath, "Services");
            UtilityHelper.CreateSubDirectory(ResourcesServicesPath, true);

            AuthorizationPath = Path.Combine(outputDirectory, "Authorization");
            UtilityHelper.CreateSubDirectory(AuthorizationPath, true);

            using (SqlConnection connection = new SqlConnection(UtilityHelper.ConnectionString))
            {
                try
                {
                    connection.Open();

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(
                        UtilityHelper.GetSelectedTablesAndViews(connection.Database,
                            tables.Where(o => o.Type != SchemaTypeEnum.StoredProcedure).Select(o => o.Title).ToList()),
                        connection);
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
        }

        private static void CreateDomainClasses()
        {

            //  List<Table> tableList = new List<Table>();

            //using (SqlConnection connection = new SqlConnection(UtilityHelper.ConnectionString))
            //{
            //    try
            //    {
            //        connection.Open();

            //        DataTable dataTable = new DataTable();
            //        SqlDataAdapter dataAdapter = new SqlDataAdapter(
            //            UtilityHelper.GetSelectedTablesAndViews(connection.Database,
            //                tables.Where(o => o.Type != SchemaTypeEnum.StoredProcedure).Select(o => o.Title).ToList()),
            //            connection);
            //        dataAdapter.Fill(dataTable);

            //        // Process each table
            //        foreach (DataRow dataRow in dataTable.Rows)
            //        {
            //            Table table = new Table();
            //            table.Name = (string)dataRow["TABLE_NAME"];
            //            table.Type = (string)dataRow["TABLE_TYPE"];
            //            UtilityHelper.QueryTable(connection, table);
            //            tableList.Add(table);
            //        }

            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    finally
            //    {
            //        connection.Close();
            //    }
            //}



            string customText = "";

            #region Common Classes

            #region IAuditEntity Interface

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "IAuditEntity.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DomainCommonPath, "IAuditEntity.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"

using System;
using System.ComponentModel.DataAnnotations;

namespace " + DomainNameSpace + @".Common
{
    public interface IAuditEntity
    {
//
       ");
                streamWriter.WriteLine(GetIAuditEntity());
                streamWriter.WriteLine(customText);
                streamWriter.WriteLine(@" }
                                        }");
            }

            #endregion

            #region BaseEntity Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "BaseEntity.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "BaseEntity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace " + DomainNameSpace + @".Common
{
    public class BaseEntity 
    {
      ");
                streamWriter.WriteLine(GetBaseEntity());
                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 

        }
}
                                        ");


            }

            #endregion

            #region ClientMessage Class

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "ClientMessage.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
                streamWriter.WriteLine(GetCreatedDateTime());
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
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
");


            }


            #endregion

            #region SD Static Data Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "SD.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "SD.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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

 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
    }
}
");


            }


            #endregion

            #region PagedResult Class

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "PagedResult.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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
                streamWriter.WriteLine(GetCreatedDateTime());

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

            #region Enums Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEnumsPath, "AppEnums.cs"));
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEnumsPath, "AppEnums.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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

        public enum MsgType
        {
            Success = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }


			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
    }
}
");


            }


            #endregion

            #region ApplicationUser Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEntitiesPath, "ApplicationUser.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEntitiesPath, "ApplicationUser.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
     using System;
     using System.Collections.Generic;
     using Microsoft.AspNetCore.Identity;
     using System.ComponentModel.DataAnnotations;

     namespace " + DomainNameSpace + @".Entities
     {
        public class ApplicationUser : IdentityUser" +
                                       (_appSetting.KeyType == 0 ? "<int>" : "<Guid>") + @"
                                            {
                                               // [Required]
                                             //   [StringLength(200)]
                                           //     public string FullName { get; set; }

                                            //    public Guid? TenantId { get; set; }


			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
                                            }
                                        }
                                      ");


            }


            #endregion


            #region AppClaim Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEntitiesPath, "AppClaim.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEntitiesPath, "AppClaim.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
     using System;

     namespace " + DomainNameSpace + @".Entities
     {
          public class AppClaim
    {
        #region Constructors

        public AppClaim()
        {

        }

        #endregion
        #region Properties
        public Guid AppClaimId { get; set; }

        public Guid? ParentId { get; set; }
        
        public string ClaimTitle { get; set; }

        public string UrlLink { get; set; }
        
        public int ShowOrder { get; set; }

        public bool IsActive { get; set; }


        public string DisplayName { get; set; }

        #endregion
                                               

			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
                                            }
                                        }
                                      ");


            }


            #endregion

            #region Permissions Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "EntityPermissions.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "EntityPermissions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Reflection;

namespace " + DomainNameSpace + @".Common
{
    public static class EntityPermissions 
    {
      ");
                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("public static class " + table.Name);
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("public const string List = \"Permissions." + table.Name + ".List\";");
                    streamWriter.WriteLine("public const string View = \"Permissions." + table.Name + ".View\";");
                    streamWriter.WriteLine("public const string Create = \"Permissions." + table.Name + ".Create\";");
                    streamWriter.WriteLine("public const string Edit = \"Permissions." + table.Name + ".Edit\";");
                    streamWriter.WriteLine("public const string Delete = \"Permissions." + table.Name + ".Delete\";");
                    streamWriter.WriteLine("}");

                }
                streamWriter.WriteLine(customText);
                streamWriter.WriteLine(@" 

        }
}
                                        ");


            }

            #endregion

            #endregion






            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region Model Classes

                className = table.Name;



                customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEntitiesPath, className + ".cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(DomainEntitiesPath, className + ".cs")))
                {
                    streamWriter.WriteLine(GetCreatedDateTime());
                    // Create the header for the class
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using System.Collections.Generic;");
                    streamWriter.WriteLine("using System.ComponentModel.DataAnnotations;");
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Common;");


                    streamWriter.WriteLine();
                    streamWriter.WriteLine("namespace " + DomainNameSpace + ".Entities");

                    streamWriter.WriteLine("{");
                    if (className != "AppClaim")
                    {
                        streamWriter.WriteLine("\tpublic class " + className + ": BaseEntity");
                    }
                    else
                    {
                        streamWriter.WriteLine("\tpublic class " + className);
                    }


                    streamWriter.WriteLine("\t{");

                    // Create an explicit public constructor
                    streamWriter.WriteLine("\t\t#region Constructors");
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

                                        streamWriter.WriteLine("\t\t " +
                                                               foreignKeysList[j].ForeignKeyTableName +
                                                               "List =  new List<" +
                                                               sameKey + ">(); ");
                                        streamWriter.WriteLine();
                                    }
                                }
                            }
                        }
                    }

                    streamWriter.WriteLine();


                    //foreach (var f in table.ForeignKeys.Values)
                    //{
                    //    var foreignKeysList = f.ToList();
                    //    for (int j = 0; j < foreignKeysList.Count; j++)
                    //    {
                    //        if (!string.IsNullOrEmpty(foreignKeysList[j].PrimaryKeyTableName))
                    //        {
                    //            if (sameKey != foreignKeysList[j].PrimaryKeyTableName)
                    //            {
                    //                sameKey = foreignKeysList[j].PrimaryKeyTableName;
                    //                //  public virtual Course Course { get; set; }
                    //                streamWriter.WriteLine("\t\t this." +
                    //                                       foreignKeysList[j].PrimaryKeyTableName + "Class = " +
                    //                                      " new " + foreignKeysList[j].PrimaryKeyTableName +
                    //                                       "();");
                    //            }
                    //        }
                    //    }
                    //}



                    streamWriter.WriteLine("\t\t}");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("\t\t#endregion");

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
                        streamWriter.WriteLine("\t\t/// Gets or sets the " + UtilityHelper.FormatPascal(name) +
                                               " value.");
                        streamWriter.WriteLine("\t\t/// </summary>");


                        if (table.Type != "VIEW")
                        {
                            streamWriter.WriteLine("\t\t" +
                                                   UtilityHelper.CreateDataAnnotationParameter(false, column,
                                                       table.ForeignKeys, useResourceFile, "", table.Name));
                        }

                        var nullableType = " ";
                        if (column.IsNullable)
                        {
                            if (!type.Contains("?"))
                                nullableType = "? ";
                        }

                        streamWriter.WriteLine("\t\tpublic " + (type == "AspNetUsers" ? "ApplicationUser" : type) + nullableType + UtilityHelper.FormatPascal(name) +
                                               " { get; set; }");

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

                    sameKey = "";
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
                                    streamWriter.WriteLine("\t\tpublic virtual " + (foreignKeysList[j].PrimaryKeyTableName == "AspNetUsers" ? "ApplicationUser" : foreignKeysList[j].PrimaryKeyTableName) + " " +
                                                           foreignKeysList[j].PrimaryKeyTableName +
                                                           "Class { get; set; }");
                                }
                            }
                        }
                    }









                    streamWriter.WriteLine();
                    streamWriter.WriteLine("\t\t#endregion");


                    streamWriter.WriteLine(customText);


                    streamWriter.WriteLine();

                    // Close out the class and namespace
                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                }

                #endregion


            }

        }

        private static void CreateAuthorizationClasses()
        {



            string customText = "";

            #region Common Classes

            #region Permissions Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(AuthorizationPath, "Permissions.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(AuthorizationPath, "Permissions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Reflection;

namespace " + DataAccessNameSpace + @".Authorization
{
    public static class Permissions 
    {
      ");
                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("public static class " + table.Name);
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("public const string List = \"Permissions." + table.Name + ".List\";");
                    streamWriter.WriteLine("public const string View = \"Permissions." + table.Name + ".View\";");
                    streamWriter.WriteLine("public const string Create = \"Permissions." + table.Name + ".Create\";");
                    streamWriter.WriteLine("public const string Edit = \"Permissions." + table.Name + ".Edit\";");
                    streamWriter.WriteLine("public const string Delete = \"Permissions." + table.Name + ".Delete\";");
                    streamWriter.WriteLine("}");

                }
                streamWriter.WriteLine(customText);
                streamWriter.WriteLine(@" 

        }
}
                                        ");


            }

            #endregion

            #region Custom Claim Types

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(AuthorizationPath, "CustomClaimTypes.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(AuthorizationPath, "CustomClaimTypes.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"

namespace " + DataAccessNameSpace + @".Authorization
{
    public class CustomClaimTypes
    {
        public const string Permission = ""permission"";
    }
}
       ");
             
            }

            #endregion

            #region Permission Authorization Handler

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(AuthorizationPath, "PermissionAuthorizationHandler.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(AuthorizationPath, "PermissionAuthorizationHandler.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
       
        public PermissionAuthorizationHandler()
        {
        }

        #region From db Context

        //UserManager<ApplicationUser> _userManager;
        //RoleManager<IdentityRole<Guid>> _roleManager;

        //public PermissionAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        //{
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //}

        //protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        //{
        //    if (context.User == null)
        //    {
        //        return;
        //    }

        //    // Get all the roles the user belongs to and check if any of the roles has the permission required
        //    // for the authorization to succeed.
        //    var user = await _userManager.GetUserAsync(context.User);
        //    if (user == null)
        //    {
        //        return;
        //    }
        //    var userRoleNames = await _userManager.GetRolesAsync(user);
        //    var userRoles = _roleManager.Roles.Where(x => userRoleNames.Contains(x.Name));

        //    foreach (var role in userRoles)
        //    {
        //        var roleClaims = await _roleManager.GetClaimsAsync(role);
        //        var permissions = roleClaims.Where(x => x.Type == CustomClaimTypes.Permission &&
        //                                                x.Value == requirement.Permission &&
        //                                                x.Issuer == ""LOCAL AUTHORITY"")
        //            .Select(x => x.Value);

                //        if (permissions.Any())
                //        {
                //            context.Succeed(requirement);
                //            return;
                //        }
                //    }
                //}

                #endregion


                #region From Token





        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // If user does not have the scope claim, get out of here
            if (context.User.HasClaim(c => c.Type == CustomClaimTypes.Permission &&
                                           c.Value == requirement.Permission &&
                                           c.Issuer == ""http://localhost:55445""))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
       ");

            }

            #endregion


            #region Permission Policy Provider

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(AuthorizationPath, "PermissionPolicyProvider.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(AuthorizationPath, "PermissionPolicyProvider.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace " + DataAccessNameSpace + @".Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // There can only be one policy provider in ASP.NET Core.
            // We only handle permissions related policies, for the rest
            /// we will use the default provider.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
        //{
            //return FallbackPolicyProvider.GetDefaultPolicyAsync();
       // }

        // Dynamically creates a policy with a requirement that contains the permission.
        // The policy name must match the permission that is needed.
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            //if (policyName.StartsWith(""Permissions"", StringComparison.OrdinalIgnoreCase))
                //{
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult(policy.Build());
                //  }

                // Policy is not for permissions, try the default provider.
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }
        }
    }
       ");

            }

            #endregion


            #region Permission Requirement

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(AuthorizationPath, "PermissionRequirement.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(AuthorizationPath, "PermissionRequirement.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Authorization;
namespace " + DataAccessNameSpace + @".Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
       ");

            }

            #endregion

            #region BaseEntity Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "BaseEntity.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "BaseEntity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace " + DomainNameSpace + @".Common
{
    public class BaseEntity 
    {
      ");
                streamWriter.WriteLine(GetBaseEntity());
                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 

        }
}
                                        ");


            }

            #endregion

            #region ClientMessage Class

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "ClientMessage.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
                streamWriter.WriteLine(GetCreatedDateTime());
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
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
");


            }


            #endregion

            #region SD Static Data Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "SD.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "SD.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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

 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
    }
}
");


            }


            #endregion

            #region PagedResult Class

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "PagedResult.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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
                streamWriter.WriteLine(GetCreatedDateTime());

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

            #region Enums Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEnumsPath, "AppEnums.cs"));
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEnumsPath, "AppEnums.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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

        public enum MsgType
        {
            Success = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }


			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
    }
}
");


            }


            #endregion

            #region ApplicationUser Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEntitiesPath, "ApplicationUser.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEntitiesPath, "ApplicationUser.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
     using System;
     using System.Collections.Generic;
     using Microsoft.AspNetCore.Identity;
     using System.ComponentModel.DataAnnotations;

     namespace " + DomainNameSpace + @".Entities
     {
        public class ApplicationUser : IdentityUser" +
                                       (_appSetting.KeyType == 0 ? "<int>" : "<Guid>") + @"
                                            {
                                               // [Required]
                                             //   [StringLength(200)]
                                           //     public string FullName { get; set; }

                                            //    public Guid? TenantId { get; set; }


			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
                                            }
                                        }
                                      ");


            }


            #endregion


            #region AppClaim Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEntitiesPath, "AppClaim.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainEntitiesPath, "AppClaim.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
     using System;

     namespace " + DomainNameSpace + @".Entities
     {
          public class AppClaim
    {
        #region Constructors

        public AppClaim()
        {

        }

        #endregion
        #region Properties
        public Guid AppClaimId { get; set; }

        public Guid? ParentId { get; set; }
        
        public string ClaimTitle { get; set; }

        public string UrlLink { get; set; }
        
        public int ShowOrder { get; set; }

        public bool IsActive { get; set; }


        public string DisplayName { get; set; }

        #endregion
                                               

			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" 
                                            }
                                        }
                                      ");


            }


            #endregion

            #region Permissions Class

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainCommonPath, "EntityPermissions.cs"));

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "EntityPermissions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(GetCreatedDateTime());
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Reflection;

namespace " + DomainNameSpace + @".Common
{
    public static class EntityPermissions 
    {
      ");
                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("public static class " + table.Name);
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("public const string List = \"Permissions." + table.Name + ".List\";");
                    streamWriter.WriteLine("public const string View = \"Permissions." + table.Name + ".View\";");
                    streamWriter.WriteLine("public const string Create = \"Permissions." + table.Name + ".Create\";");
                    streamWriter.WriteLine("public const string Edit = \"Permissions." + table.Name + ".Edit\";");
                    streamWriter.WriteLine("public const string Delete = \"Permissions." + table.Name + ".Delete\";");
                    streamWriter.WriteLine("}");

                }
                streamWriter.WriteLine(customText);
                streamWriter.WriteLine(@" 

        }
}
                                        ");


            }

            #endregion

            #endregion






            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region Model Classes

                className = table.Name;



                customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DomainEntitiesPath, className + ".cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(DomainEntitiesPath, className + ".cs")))
                {
                    streamWriter.WriteLine(GetCreatedDateTime());
                    // Create the header for the class
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using System.Collections.Generic;");
                    streamWriter.WriteLine("using System.ComponentModel.DataAnnotations;");
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Common;");


                    streamWriter.WriteLine();
                    streamWriter.WriteLine("namespace " + DomainNameSpace + ".Entities");

                    streamWriter.WriteLine("{");
                    if (className != "AppClaim")
                    {
                        streamWriter.WriteLine("\tpublic class " + className + ": BaseEntity");
                    }
                    else
                    {
                        streamWriter.WriteLine("\tpublic class " + className);
                    }


                    streamWriter.WriteLine("\t{");

                    // Create an explicit public constructor
                    streamWriter.WriteLine("\t\t#region Constructors");
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

                                        streamWriter.WriteLine("\t\t " +
                                                               foreignKeysList[j].ForeignKeyTableName +
                                                               "List =  new List<" +
                                                               sameKey + ">(); ");
                                        streamWriter.WriteLine();
                                    }
                                }
                            }
                        }
                    }

                    streamWriter.WriteLine();


                    //foreach (var f in table.ForeignKeys.Values)
                    //{
                    //    var foreignKeysList = f.ToList();
                    //    for (int j = 0; j < foreignKeysList.Count; j++)
                    //    {
                    //        if (!string.IsNullOrEmpty(foreignKeysList[j].PrimaryKeyTableName))
                    //        {
                    //            if (sameKey != foreignKeysList[j].PrimaryKeyTableName)
                    //            {
                    //                sameKey = foreignKeysList[j].PrimaryKeyTableName;
                    //                //  public virtual Course Course { get; set; }
                    //                streamWriter.WriteLine("\t\t this." +
                    //                                       foreignKeysList[j].PrimaryKeyTableName + "Class = " +
                    //                                      " new " + foreignKeysList[j].PrimaryKeyTableName +
                    //                                       "();");
                    //            }
                    //        }
                    //    }
                    //}



                    streamWriter.WriteLine("\t\t}");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("\t\t#endregion");

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
                        streamWriter.WriteLine("\t\t/// Gets or sets the " + UtilityHelper.FormatPascal(name) +
                                               " value.");
                        streamWriter.WriteLine("\t\t/// </summary>");


                        if (table.Type != "VIEW")
                        {
                            streamWriter.WriteLine("\t\t" +
                                                   UtilityHelper.CreateDataAnnotationParameter(false, column,
                                                       table.ForeignKeys, useResourceFile, "", table.Name));
                        }

                        var nullableType = " ";
                        if (column.IsNullable)
                        {
                            if (!type.Contains("?"))
                                nullableType = "? ";
                        }

                        streamWriter.WriteLine("\t\tpublic " + (type == "AspNetUsers" ? "ApplicationUser" : type) + nullableType + UtilityHelper.FormatPascal(name) +
                                               " { get; set; }");

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

                    sameKey = "";
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
                                    streamWriter.WriteLine("\t\tpublic virtual " + (foreignKeysList[j].PrimaryKeyTableName == "AspNetUsers" ? "ApplicationUser" : foreignKeysList[j].PrimaryKeyTableName) + " " +
                                                           foreignKeysList[j].PrimaryKeyTableName +
                                                           "Class { get; set; }");
                                }
                            }
                        }
                    }









                    streamWriter.WriteLine();
                    streamWriter.WriteLine("\t\t#endregion");


                    streamWriter.WriteLine(customText);


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
            string customText = "";

            #region Interfaces


            #region Data

            //IDbInitializer

            #region IDbInitializer

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(InfraDataFolderPath, "IDbInitializer.cs"));

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataFolderPath, "IDbInitializer.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
namespace " + DataAccessNameSpace + @".Data {
    public interface IDbInitializer
    {
        void Initialize();

   
			 ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" }
}

");

            }

            #endregion





            #endregion



            #endregion

            #region Implementations




            #region UserInitializer

            customText =
                UtilityHelper.ReadCustomRegionText(Path.Combine(InfraDataInitializerPath, "UserInitializer.cs"));


            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "UserInitializer.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
namespace " + DataAccessNameSpace + @".Data.Initializer
{
    public class UserInitializer
    {


        public static void AddUser(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @">> roleManager)
        {
            if (!db.Roles.Any(r => r.Name ==""Admin""))
            {
                roleManager.CreateAsync(new IdentityRole<" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @">(""Admin"")).GetAwaiter().GetResult();
            }

            if (!db.Roles.Any(r => r.Name == ""User""))
            {
                roleManager.CreateAsync(new IdentityRole<" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @">(""User"")).GetAwaiter().GetResult();
            }


            if (!db.ApplicationUsers.Any(u => u.Email == ""admin@gmail.com""))
                {
                    userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = ""Belal"",
                        Email = ""admin@gmail.com"",
                        EmailConfirmed = true,
                    
                      

                    }, ""Admin123$"").GetAwaiter().GetResult();

                    ApplicationUser user = db.ApplicationUsers.Where(u => u.Email == ""admin@gmail.com"").FirstOrDefault();
                    
                     var newRoleName = ""Admin"";

                roleManager.CreateAsync(new IdentityRole<Guid>(newRoleName)).GetAwaiter().GetResult();

                var newRole = roleManager.FindByNameAsync(newRoleName).GetAwaiter().GetResult();

                if (newRole != null)
                {
                    List<AppClaim> existsAppClaims = new List<AppClaim>();

                    existsAppClaims = db.AppClaim.ToListAsync().GetAwaiter().GetResult();

                    var claims = roleManager.GetClaimsAsync(newRole).GetAwaiter().GetResult();

                    foreach (var ca in existsAppClaims)
                    {
                        if (!string.IsNullOrEmpty(ca.ClaimTitle))
                        {
                            if (!claims.Any(o => o.Value.ToUpper() == ca.ClaimTitle.ToUpper()))
                            {
                                roleManager.AddClaimAsync(newRole, new Claim(""permission"", ca.ClaimTitle.ToUpper())).GetAwaiter().GetResult();
                            }
                        }
                    }
                }

                userManager.AddToRoleAsync(user, newRoleName).GetAwaiter().GetResult();
            }
        }
       ");

                streamWriter.WriteLine(customText);

                streamWriter.WriteLine(@" }

    }

");

            }


            #endregion


            #region AppClaims Initializer

            customText =
                UtilityHelper.ReadCustomRegionText(Path.Combine(InfraDataInitializerPath, "AppClaimsInitializer.cs"));


            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "AppClaimsInitializer.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace " + DataAccessNameSpace + @".Data.Initializer
{
    public class AppClaimsInitializer
    {
        public static void AppClaimsAsync(ApplicationDbContext db)
        {
            List<AppClaim> appClaimsList = new List<AppClaim>();

           

       ");


                foreach (Table table in tableList)
                {
                    string claimName = "claim" + table.Name;

                    streamWriter.WriteLine("#region " + UtilityHelper.FixName(table.Name));
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + " = new AppClaim();");
                    streamWriter.WriteLine(claimName + ".AppClaimId = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + ".DisplayName = \"" + UtilityHelper.FixName(table.Name) + "\";");
                    streamWriter.WriteLine(claimName + ".ClaimTitle =\"" + table.Name + "\";");
                    streamWriter.WriteLine(claimName + ".ParentId = null;");
                    streamWriter.WriteLine(claimName + ".UrlLink = null;");
                    streamWriter.WriteLine(claimName + ".ShowOrder = 1; ");
                    streamWriter.WriteLine(claimName + ".IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(" + claimName + ");");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "List = new AppClaim();");
                    streamWriter.WriteLine(claimName + "List.AppClaimId = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "List.DisplayName = \"List\";");
                    streamWriter.WriteLine(claimName + "List.ClaimTitle =\"Permissions." + table.Name + ".List\";");
                    streamWriter.WriteLine(claimName + "List.ParentId = claim" + table.Name + ".AppClaimId;");
                    streamWriter.WriteLine(claimName + "List.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "List.ShowOrder = 1; ");
                    streamWriter.WriteLine(claimName + "List.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "List);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "View = new AppClaim();");
                    streamWriter.WriteLine(claimName + "View.AppClaimId = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "View.DisplayName =  \"View\";");
                    streamWriter.WriteLine(claimName + "View.ClaimTitle =\"Permissions." + table.Name + ".View\";");
                    streamWriter.WriteLine(claimName + "View.ParentId = claim" + table.Name + ".AppClaimId;");
                    streamWriter.WriteLine(claimName + "View.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "View.ShowOrder = 2; ");
                    streamWriter.WriteLine(claimName + "View.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "View);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "Create = new AppClaim();");
                    streamWriter.WriteLine(claimName + "Create.AppClaimId = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "Create.DisplayName =  \"Create\";");
                    streamWriter.WriteLine(claimName + "Create.ClaimTitle =\"Permissions." + table.Name + ".Create\";");
                    streamWriter.WriteLine(claimName + "Create.ParentId = claim" + table.Name + ".AppClaimId;");
                    streamWriter.WriteLine(claimName + "Create.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "Create.ShowOrder = 3; ");
                    streamWriter.WriteLine(claimName + "Create.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "Create);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "Edit = new AppClaim();");
                    streamWriter.WriteLine(claimName + "Edit.AppClaimId = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "Edit.DisplayName = \"Edit\";");
                    streamWriter.WriteLine(claimName + "Edit.ClaimTitle =\"Permissions." + table.Name + ".Edit\";");
                    streamWriter.WriteLine(claimName + "Edit.ParentId = claim" + table.Name + ".AppClaimId;");
                    streamWriter.WriteLine(claimName + "Edit.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "Edit.ShowOrder = 4; ");
                    streamWriter.WriteLine(claimName + "Edit.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "Edit);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "Delete = new AppClaim();");
                    streamWriter.WriteLine(claimName + "Delete.AppClaimId = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "Delete.DisplayName = \"Delete\";");
                    streamWriter.WriteLine(claimName + "Delete.ClaimTitle =\"Permissions." + table.Name + ".Delete\";");
                    streamWriter.WriteLine(claimName + "Delete.ParentId = claim" + table.Name + ".AppClaimId;");
                    streamWriter.WriteLine(claimName + "Delete.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "Delete.ShowOrder = 5; ");
                    streamWriter.WriteLine(claimName + "Delete.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "Delete);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("#endregion");
                    streamWriter.WriteLine("");
                }

                streamWriter.WriteLine("");
                streamWriter.WriteLine(@" List<AppClaim> existsAppClaims = new List<AppClaim>();
           
            existsAppClaims = db.AppClaim.ToListAsync().GetAwaiter().GetResult();

            foreach (var apc in appClaimsList)
            {
                if (!existsAppClaims.Any(u => u.ClaimTitle == apc.ClaimTitle))
                {
                    db.AppClaim.AddAsync(apc).GetAwaiter().GetResult();
                }
            }

            db.SaveChangesAsync().GetAwaiter().GetResult();");


                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" }

    }
}

");

            }


            #endregion

            #region DbInitializer

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(InfraDataInitializerPath, "DbInitializer.cs"));

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "DbInitializer.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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
        private readonly RoleManager<IdentityRole<" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @">> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @">> roleManager)
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

            AppClaimsInitializer.AppClaimsAsync(_db);
            UserInitializer.AddUser(_db, _userManager, _roleManager);
           
        }
     ");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" }

}

");

            }


            #endregion

            #endregion

            #region EntityConfiguration





            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region Model Classes

                className = table.Name;
                if (className != "AspNetUsers")
                {
                    customText = UtilityHelper.ReadCustomRegionText(Path.Combine(InfraEntityConfigurationPath,
                        className + "Configuration.cs"));

                    using (StreamWriter streamWriter =
                        new StreamWriter(Path.Combine(InfraEntityConfigurationPath, className + "Configuration.cs")))
                    {
                        streamWriter.WriteLine(GetCreatedDateTime());
                        // Create the header for the class
                        streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using System.Collections.Generic;
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
                        streamWriter.WriteLine(
                            UtilityHelper.CreateBuilderPropertyForGlobalFilterEntityFrameworkHasKey(table.Columns));

                        streamWriter.WriteLine(
                            UtilityHelper.CreateBuilderPropertyForEntityFrameworkHasKey(table.PrimaryKeys));

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
                            streamWriter.WriteLine(".WithMany(t => t." + f[0].ForeignKeyTableName +
                                                   "List).OnDelete(DeleteBehavior.Restrict)");
                            streamWriter.WriteLine(".HasForeignKey(d => d." + f[0].Name + ")");
                            streamWriter.WriteLine(".HasConstraintName(" + @"""FK_" + f[0].ForeignKeyTableName + "_" +
                                                   f[0].PrimaryKeyTableName + @""");");

                            //streamWriter.WriteLine("\t\t this." +
                            //                       foreignKeysList[j].ForeignKeyTableName + "List =  new List<" +
                            //                       sameKey + ">(); ");
                            //streamWriter.WriteLine();
                            //}
                            // }
                            //}
                        }


                        streamWriter.WriteLine(customText);

                        streamWriter.WriteLine("}");



                        streamWriter.WriteLine("}");
                        streamWriter.WriteLine("}");













                    }
                }

                #endregion


            }

            #endregion

            #region ApplicationDbContext

            customText =
                UtilityHelper.ReadCustomRegionText(Path.Combine(InfraDataFolderPath, "ApplicationDbContext.cs"));

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataFolderPath, "ApplicationDbContext.cs")))
            {

                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"

using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Common;
using " + ApplicationNameSpace + @".Common.Interfaces;");


                streamWriter.WriteLine((_appSetting.AddDapperToDBContext == true ? " " : "//") + "using Dapper;");
                streamWriter.WriteLine((_appSetting.AddDapperToDBContext == true ? " " : "//") + " using Microsoft.Data.SqlClient;");

                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;

namespace " + DataAccessNameSpace + @".Data{");

                if (_appSetting.KeyType == 0)
                {
                    streamWriter.WriteLine("public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext");
                }
                else
                {
                    streamWriter.WriteLine("public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext");
                }

                streamWriter.WriteLine(@"
{

 private IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }


public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

");

                foreach (Table table in tableList)
                {
                    if (table.Name != "AspNetUsers")
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

                if (_appSetting.AddDapperToDBContext == false)
                {
                    streamWriter.WriteLine("/*");
                }

                streamWriter.WriteLine(@"
                    
public T ExecuteReturnScaler<T>(string sql, bool isStoredProcedure, object param = null)
{
 using (SqlConnection sqlCon = new SqlConnection(this.Database.GetDbConnection().ConnectionString))
 {
     // sqlCon.Open();
     if (isStoredProcedure)
     {
         return (T)Convert.ChangeType(
             sqlCon.ExecuteScalar<T>(sql, param,
                 commandType: System.Data.CommandType.StoredProcedure), typeof(T));
     }
     else
     {
         return (T)Convert.ChangeType(sqlCon.ExecuteScalar<T>(sql, param, commandType: System.Data.CommandType.Text), typeof(T));

     }
 }
}

public void ExecuteWithoutReturn(string sql, bool isStoredProcedure, object param = null)
{
 using (SqlConnection sqlCon = new SqlConnection(this.Database.GetDbConnection().ConnectionString))
 {
     //  sqlCon.Open();
     if (isStoredProcedure)
     {
         sqlCon.Execute(sql, param, commandType: System.Data.CommandType.StoredProcedure);
     }
     else
     {
         sqlCon.Execute(sql, param, commandType: System.Data.CommandType.Text);

     }
 }
}

public async Task<IEnumerable<T>> ReturnListAsync<T>(string sql, bool isStoredProcedure, object param = null)
{
 using (SqlConnection sqlCon = new SqlConnection(this.Database.GetDbConnection().ConnectionString))
 {
     //  sqlCon.Open();
     if (isStoredProcedure)
     {
         return sqlCon.QueryAsync<T>(sql, param, commandType: System.Data.CommandType.StoredProcedure).Result.ToList();
     }
     else
     {
         return sqlCon.QueryAsync<T>(sql, param, commandType: System.Data.CommandType.Text).Result.ToList();
     }
 }

}

                    ");

                if (_appSetting.AddDapperToDBContext == false)
                {
                    streamWriter.WriteLine("*/");
                }

                streamWriter.WriteLine(@"  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get all the entities that inherit from BaseEntity
            // and have a state of Added or Modified
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            // For each entity we will set the Audit properties
            foreach (var entityEntry in entries)
            {
                // If the entity state is Added let's set
                // the CreatedAt and CreatedBy properties
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                    if (_httpContextAccessor.HttpContext.User != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        ((BaseEntity)entityEntry.Entity).CreatedBy =" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @".Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    }
                }
                else
                {
                    // If the state is Modified then we don't want
                    // to modify the CreatedAt and CreatedBy properties
                    // so we set their state as IsModified to false
                    Entry((BaseEntity)entityEntry.Entity).Property(p => p.CreatedDate).IsModified = false;
                    Entry((BaseEntity)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;

                    // In any case we always want to set the properties
                    // ModifiedAt and ModifiedBy
                    ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;
                    if (_httpContextAccessor.HttpContext.User != null &&
                        _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        ((BaseEntity)entityEntry.Entity).UpdatedBy = " + (_appSetting.KeyType == 0 ? "int" : "Guid") + @".Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    }
                }

               
            }

            // After we set all the needed properties
            // we call the base implementation of SaveChangesAsync
            // to actually save our entities in the database

            return await base.SaveChangesAsync(cancellationToken);
        }");
                streamWriter.WriteLine(customText);

                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");



            }


            #endregion

            #region Infrastructure Dependency Injection


            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(DataPath, "DependencyInjection.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DataPath, "DependencyInjection.cs")))
            {

                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class

                streamWriter.WriteLine(@"



using " + ApplicationNameSpace + @".Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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



     services.AddIdentity<ApplicationUser, IdentityRole<" + (_appSetting.KeyType == 0 ? "int" : "Guid") + @">>()
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddDefaultTokenProviders();


     //services.AddScoped<IRepositoryFactory, UnitOfWork<ApplicationDbContext>>();
    services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

     services.AddScoped<IDbInitializer, DbInitializer>();

     return services;
 }
");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" }
}"
                    );









            }
            #endregion




            #region Identity Migrations



            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(DataPath, "IdentityMigrationsDB.cs")))
            {

                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                if (_appSetting.KeyType == 0)
                {

                    #region  INT
                    streamWriter.WriteLine(@"
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace " + ApplicationNameSpace + @".Infrastructure.Migrations
{
    public partial class initiladb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: ""dbo"");

            migrationBuilder.CreateTable(
                name: ""AspNetRoles"",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation(""SqlServer:Identity"", ""1, 1""),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(""PK_AspNetRoles"", x => x.Id);
                });

                    migrationBuilder.CreateTable(
                        name: ""AspNetUsers"",
                        columns: table => new
                        {
                            Id = table.Column<int>(nullable: false)
                                .Annotation(""SqlServer:Identity"", ""1, 1""),
                            UserName = table.Column<string>(maxLength: 256, nullable: true),
                            NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                            Email = table.Column<string>(maxLength: 256, nullable: true),
                            NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                            EmailConfirmed = table.Column<bool>(nullable: false),
                            PasswordHash = table.Column<string>(nullable: true),
                            SecurityStamp = table.Column<string>(nullable: true),
                            ConcurrencyStamp = table.Column<string>(nullable: true),
                            PhoneNumber = table.Column<string>(nullable: true),
                            PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                            TwoFactorEnabled = table.Column<bool>(nullable: false),
                            LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                            LockoutEnabled = table.Column<bool>(nullable: false),
                            AccessFailedCount = table.Column<int>(nullable: false),
                            FullName = table.Column<string>(maxLength: 200, nullable: false),
                            TenantId = table.Column<int>(nullable: true)
                        },
                        constraints: table =>
                        {
                            table.PrimaryKey(""PK_AspNetUsers"", x => x.Id);
                        });


                    migrationBuilder.CreateTable(
                        name: ""AspNetRoleClaims"",
                        columns: table => new
                        {
                            Id = table.Column<int>(nullable: false)
                                .Annotation(""SqlServer:Identity"", ""1, 1""),
                            RoleId = table.Column<int>(nullable: false),
                            ClaimType = table.Column<string>(nullable: true),
                            ClaimValue = table.Column<string>(nullable: true)
                        },
                        constraints: table =>
                        {
                            table.PrimaryKey(""PK_AspNetRoleClaims"", x => x.Id);
                            table.ForeignKey(
                                name: ""FK_AspNetRoleClaims_AspNetRoles_RoleId"",
                                column: x => x.RoleId,
                                principalTable: ""AspNetRoles"",
                                principalColumn: ""Id"",
                                onDelete: ReferentialAction.Cascade);
                        });

                    migrationBuilder.CreateTable(
                        name: ""AspNetUserClaims"",
                        columns: table => new
                        {
                            Id = table.Column<int>(nullable: false)
                                .Annotation(""SqlServer:Identity"", ""1, 1""),
                            UserId = table.Column<int>(nullable: false),
                            ClaimType = table.Column<string>(nullable: true),
                            ClaimValue = table.Column<string>(nullable: true)
                        },
                        constraints: table =>
                        {
                            table.PrimaryKey(""PK_AspNetUserClaims"", x => x.Id);
                            table.ForeignKey(
                                name: ""FK_AspNetUserClaims_AspNetUsers_UserId"",
                                column: x => x.UserId,
                                principalTable: ""AspNetUsers"",
                                principalColumn: ""Id"",
                                onDelete: ReferentialAction.Cascade);
                        });

                    migrationBuilder.CreateTable(
                        name: ""AspNetUserLogins"",
                        columns: table => new
                        {
                            LoginProvider = table.Column<string>(nullable: false),
                            ProviderKey = table.Column<string>(nullable: false),
                            ProviderDisplayName = table.Column<string>(nullable: true),
                            UserId = table.Column<int>(nullable: false)
                        },
                        constraints: table =>
                        {
                            table.PrimaryKey(""PK_AspNetUserLogins"", x => new { x.LoginProvider, x.ProviderKey });
                            table.ForeignKey(
                                name: ""FK_AspNetUserLogins_AspNetUsers_UserId"",
                                column: x => x.UserId,
                                principalTable: ""AspNetUsers"",
                                principalColumn: ""Id"",
                                onDelete: ReferentialAction.Cascade);
                        });

                    migrationBuilder.CreateTable(
                        name: ""AspNetUserRoles"",
                        columns: table => new
                        {
                            UserId = table.Column<int>(nullable: false),
                            RoleId = table.Column<int>(nullable: false)
                        },
                        constraints: table =>
                        {
                            table.PrimaryKey(""PK_AspNetUserRoles"", x => new { x.UserId, x.RoleId });
                            table.ForeignKey(
                                name: ""FK_AspNetUserRoles_AspNetRoles_RoleId"",
                                column: x => x.RoleId,
                                principalTable: ""AspNetRoles"",
                                principalColumn: ""Id"",
                                onDelete: ReferentialAction.Cascade);
                            table.ForeignKey(
                                name: ""FK_AspNetUserRoles_AspNetUsers_UserId"",
                                column: x => x.UserId,
                                principalTable: ""AspNetUsers"",
                                principalColumn: ""Id"",
                                onDelete: ReferentialAction.Cascade);
                        });

                    migrationBuilder.CreateTable(
                        name: ""AspNetUserTokens"",
                        columns: table => new
                        {
                            UserId = table.Column<int>(nullable: false),
                            LoginProvider = table.Column<string>(nullable: false),
                            Name = table.Column<string>(nullable: false),
                            Value = table.Column<string>(nullable: true)
                        },
                        constraints: table =>
                        {
                            table.PrimaryKey(""PK_AspNetUserTokens"", x => new { x.UserId, x.LoginProvider, x.Name });
                            table.ForeignKey(
                                name: ""FK_AspNetUserTokens_AspNetUsers_UserId"",
                                column: x => x.UserId,
                                principalTable: ""AspNetUsers"",
                                principalColumn: ""Id"",
                                onDelete: ReferentialAction.Cascade);
                        });

                    migrationBuilder.CreateIndex(
                        name: ""IX_AspNetRoleClaims_RoleId"",
                        table: ""AspNetRoleClaims"",
                        column: ""RoleId"");

                    migrationBuilder.CreateIndex(
                        name: ""RoleNameIndex"",
                        table: ""AspNetRoles"",
                        column: ""NormalizedName"",
                        unique: true,
                        filter: ""[NormalizedName] IS NOT NULL"");

                    migrationBuilder.CreateIndex(
                        name: ""IX_AspNetUserClaims_UserId"",
                        table: ""AspNetUserClaims"",
                        column: ""UserId"");

                    migrationBuilder.CreateIndex(
                        name: ""IX_AspNetUserLogins_UserId"",
                        table: ""AspNetUserLogins"",
                        column: ""UserId"");

                    migrationBuilder.CreateIndex(
                        name: ""IX_AspNetUserRoles_RoleId"",
                        table: ""AspNetUserRoles"",
                        column: ""RoleId"");

                    migrationBuilder.CreateIndex(
                        name: ""EmailIndex"",
                        table: ""AspNetUsers"",
                        column: ""NormalizedEmail"");

                    migrationBuilder.CreateIndex(
                        name: ""UserNameIndex"",
                        table: ""AspNetUsers"",
                        column: ""NormalizedUserName"",
                        unique: true,
                        filter: ""[NormalizedUserName] IS NOT NULL"");
                }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: ""AspNetRoleClaims"");

            migrationBuilder.DropTable(
                name: ""AspNetUserClaims"");

            migrationBuilder.DropTable(
                name: ""AspNetUserLogins"");

            migrationBuilder.DropTable(
                name: ""AspNetUserRoles"");

            migrationBuilder.DropTable(
                name: ""AspNetUserTokens"");


            migrationBuilder.DropTable(
                name: ""AspNetRoles"");

            migrationBuilder.DropTable(
                name: ""AspNetUsers"");
        }
    }
}


");
                    #endregion

                }
                else
                {
                    #region  GUID
                    streamWriter.WriteLine(@"
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace " + ApplicationNameSpace + @".Infrastructure.Migrations
{
    public partial class initiladb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: ""dbo"");

            migrationBuilder.CreateTable(
                name: ""AspNetRoles"",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(""PK_AspNetRoles"", x => x.Id);
                });

                migrationBuilder.CreateTable(
                    name: ""AspNetUsers"",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        UserName = table.Column<string>(maxLength: 256, nullable: true),
                        NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                        Email = table.Column<string>(maxLength: 256, nullable: true),
                        NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                        EmailConfirmed = table.Column<bool>(nullable: false),
                        PasswordHash = table.Column<string>(nullable: true),
                        SecurityStamp = table.Column<string>(nullable: true),
                        ConcurrencyStamp = table.Column<string>(nullable: true),
                        PhoneNumber = table.Column<string>(nullable: true),
                        PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                        TwoFactorEnabled = table.Column<bool>(nullable: false),
                        LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                        LockoutEnabled = table.Column<bool>(nullable: false),
                        AccessFailedCount = table.Column<int>(nullable: false),
                        FullName = table.Column<string>(maxLength: 200, nullable: false),
                        TenantId = table.Column<Guid>(nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(""PK_AspNetUsers"", x => x.Id);
                    });


                migrationBuilder.CreateTable(
                    name: ""AspNetRoleClaims"",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation(""SqlServer:Identity"", ""1, 1""),
                        RoleId = table.Column<Guid>(nullable: false),
                        ClaimType = table.Column<string>(nullable: true),
                        ClaimValue = table.Column<string>(nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(""PK_AspNetRoleClaims"", x => x.Id);
                        table.ForeignKey(
                            name: ""FK_AspNetRoleClaims_AspNetRoles_RoleId"",
                            column: x => x.RoleId,
                            principalTable: ""AspNetRoles"",
                            principalColumn:""Id"",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: ""AspNetUserClaims"",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation(""SqlServer:Identity"", ""1, 1""),
                        UserId = table.Column<Guid>(nullable: false),
                        ClaimType = table.Column<string>(nullable: true),
                        ClaimValue = table.Column<string>(nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(""PK_AspNetUserClaims"", x => x.Id);
                        table.ForeignKey(
                            name: ""FK_AspNetUserClaims_AspNetUsers_UserId"",
                            column: x => x.UserId,
                            principalTable: ""AspNetUsers"",
                            principalColumn: ""Id"",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: ""AspNetUserLogins"",
                    columns: table => new
                    {
                        LoginProvider = table.Column<string>(nullable: false),
                        ProviderKey = table.Column<string>(nullable: false),
                        ProviderDisplayName = table.Column<string>(nullable: true),
                        UserId = table.Column<Guid>(nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(""PK_AspNetUserLogins"", x => new { x.LoginProvider, x.ProviderKey });
                        table.ForeignKey(
                            name: ""FK_AspNetUserLogins_AspNetUsers_UserId"",
                            column: x => x.UserId,
                            principalTable: ""AspNetUsers"",
                            principalColumn: ""Id"",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name:""AspNetUserRoles"",
                    columns: table => new
                    {
                        UserId = table.Column<Guid>(nullable: false),
                        RoleId = table.Column<Guid>(nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(""PK_AspNetUserRoles"", x => new { x.UserId, x.RoleId });
                        table.ForeignKey(
                            name: ""FK_AspNetUserRoles_AspNetRoles_RoleId"",
                            column: x => x.RoleId,
                            principalTable: ""AspNetRoles"",
                            principalColumn: ""Id"",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: ""FK_AspNetUserRoles_AspNetUsers_UserId"",
                            column: x => x.UserId,
                            principalTable: ""AspNetUsers"",
                            principalColumn: ""Id"",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: ""AspNetUserTokens"",
                    columns: table => new
                    {
                        UserId = table.Column<Guid>(nullable: false),
                        LoginProvider = table.Column<string>(nullable: false),
                        Name = table.Column<string>(nullable: false),
                        Value = table.Column<string>(nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(""PK_AspNetUserTokens"", x => new { x.UserId, x.LoginProvider, x.Name });
                        table.ForeignKey(
                            name: ""FK_AspNetUserTokens_AspNetUsers_UserId"",
                            column: x => x.UserId,
                            principalTable: ""AspNetUsers"",
                            principalColumn: ""Id"",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: ""IX_AspNetRoleClaims_RoleId"",
                    table:""AspNetRoleClaims"",
                    column: ""RoleId"");

                migrationBuilder.CreateIndex(
                    name: ""RoleNameIndex"",
                    table: ""AspNetRoles"",
                    column: ""NormalizedName"",
                    unique: true,
                    filter: ""[NormalizedName] IS NOT NULL"");

                migrationBuilder.CreateIndex(
                    name: ""IX_AspNetUserClaims_UserId"",
                    table: ""AspNetUserClaims"",
                    column: ""UserId"");

                migrationBuilder.CreateIndex(
                    name: ""IX_AspNetUserLogins_UserId"",
                    table: ""AspNetUserLogins"",
                    column: ""UserId"");

                migrationBuilder.CreateIndex(
                    name: ""IX_AspNetUserRoles_RoleId"",
                    table: ""AspNetUserRoles"",
                    column: ""RoleId"");

                migrationBuilder.CreateIndex(
                    name: ""EmailIndex"",
                    table: ""AspNetUsers"",
                    column: ""NormalizedEmail"");

                migrationBuilder.CreateIndex(
                    name: ""UserNameIndex"",
                    table: ""AspNetUsers"",
                    column: ""NormalizedUserName"",
                    unique: true,
                    filter: ""[NormalizedUserName] IS NOT NULL"");
            }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: ""AspNetRoleClaims"");

            migrationBuilder.DropTable(
                name: ""AspNetUserClaims"");

            migrationBuilder.DropTable(
                name: ""AspNetUserLogins"");

            migrationBuilder.DropTable(
                name: ""AspNetUserRoles"");

            migrationBuilder.DropTable(
                name: ""AspNetUserTokens"");


            migrationBuilder.DropTable(
                name: ""AspNetRoles"");

            migrationBuilder.DropTable(
                name: ""AspNetUsers"");
        }
    }
}


");
                    #endregion
                }

            }


            #endregion





        }
        private static void CreateApplicationClasses()
        {
            string customText = "";

            //List<Table> tableList = new List<Table>();

            //using (SqlConnection connection = new SqlConnection(UtilityHelper.ConnectionString))
            //{
            //    try
            //    {
            //        connection.Open();

            //        DataTable dataTable = new DataTable();
            //        SqlDataAdapter dataAdapter = new SqlDataAdapter(UtilityHelper.GetSelectedTablesAndViews(connection.Database, tables.Where(o => o.Type != SchemaTypeEnum.StoredProcedure).Select(o => o.Title).ToList()), connection);
            //        dataAdapter.Fill(dataTable);

            //        // Process each table
            //        foreach (DataRow dataRow in dataTable.Rows)
            //        {
            //            Table table = new Table();
            //            table.Name = (string)dataRow["TABLE_NAME"];
            //            table.Type = (string)dataRow["TABLE_TYPE"];
            //            UtilityHelper.QueryTable(connection, table);
            //            tableList.Add(table);
            //        }

            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    finally
            //    {
            //        connection.Close();
            //    }
            //}


            StringBuilder sb = new StringBuilder();


            #region IApplicationDbContext

            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationPath, "IApplicationDbContext.cs"));


            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationPath, "IApplicationDbContext.cs")))

            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace " + ApplicationNameSpace + @".Common.Interfaces
{
public interface IApplicationDbContext
{

DbSet<ApplicationUser> ApplicationUsers { get; set; }


");

                foreach (Table table in tableList)
                {
                    if (table.Name != "AspNetUsers")
                        streamWriter.WriteLine(" DbSet<" + table.Name + "> " + table.Name + " { get; set; }");
                }


                streamWriter.WriteLine("Task<int> SaveChangesAsync(CancellationToken cancellationToken);");


                streamWriter.WriteLine("// using dapper for storedPr.");
                streamWriter.WriteLine((_appSetting.AddDapperToDBContext ? "" : "//") + " T ExecuteReturnScaler<T>(string sql, bool isStoredProcedure, object param = null);");
                streamWriter.WriteLine((_appSetting.AddDapperToDBContext ? "" : "//") + " void ExecuteWithoutReturn(string sql, bool isStoredProcedure, object param = null);");
                streamWriter.WriteLine((_appSetting.AddDapperToDBContext ? "" : "//") + " Task <IEnumerable<T>> ReturnListAsync<T>(string sql, bool isStoredProcedure, object param = null);");

                streamWriter.WriteLine(customText);

                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");



            }


            #endregion


            foreach (Table table in tableList)
            {
                if (table.Name != "AspNetUsers")
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



                    sb.AppendLine(GetCreatedDateTime());
                    sb.AppendLine("using System;");
                    sb.AppendLine("using " + DomainNameSpace + ".Entities;");
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



                    var sameKey = "";


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
                                        sb.AppendLine("\t\tpublic virtual List<" +
                                                      foreignKeysList[j].ForeignKeyTableName + "> " +
                                                      foreignKeysList[j].ForeignKeyTableName +
                                                      "List { get; set; }");
                                        sb.AppendLine("");
                                    }
                                }
                            }
                        }
                    }

                    sb.AppendLine("");

                    sameKey = "";
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
                                    sb.AppendLine("\t\tpublic virtual " +
                                                  (foreignKeysList[j].PrimaryKeyTableName == "AspNetUsers"
                                                      ? "ApplicationUser"
                                                      : foreignKeysList[j].PrimaryKeyTableName)

                                                  + " " +
                                                  foreignKeysList[j].PrimaryKeyTableName +
                                                  "Class { get; set; }");
                                    sb.AppendLine("");
                                }
                            }
                        }
                    }



                    customText =
                        UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationDtosPath, classNameReadDto + ".cs"));

                    using (StreamWriter streamWriter =
                        new StreamWriter(Path.Combine(ApplicationDtosPath, classNameReadDto + ".cs")))
                    {
                        string clsName = sb.ToString().Replace("#className#", classNameReadDto).ToString();
                        streamWriter.WriteLine(clsName);

                        streamWriter.WriteLine(customText);

                        streamWriter.WriteLine("\t}");
                        streamWriter.WriteLine("}");
                    }


                    customText =
                        UtilityHelper.ReadCustomRegionText(
                            Path.Combine(ApplicationDtosPath, classNameCreateDto + ".cs"));

                    using (StreamWriter streamWriter =
                        new StreamWriter(Path.Combine(ApplicationDtosPath, classNameCreateDto + ".cs")))
                    {
                        string clsName = sb.ToString().Replace("#className#", classNameCreateDto).ToString();
                        streamWriter.WriteLine(clsName);
                        streamWriter.WriteLine(customText);

                        streamWriter.WriteLine("\t}");
                        streamWriter.WriteLine("}");

                    }


                    //using (StreamWriter streamWriter = new StreamWriter(Path.Combine(UtilityHelper.ApplicationDtosPath, classNameUpdateDto + ".cs")))
                    //{
                    //    string clsName = sb.ToString().Replace("#className#", classNameUpdateDto).ToString();
                    //    streamWriter.WriteLine(clsName);
                    //}

                    #endregion

                    #region Validations Classes



                    sb.Clear();
                    sb.AppendLine(GetCreatedDateTime());
                    sb.AppendLine("using System;");
                    sb.AppendLine("using FluentValidation;");

                    sb.AppendLine("using " + ApplicationNameSpace + ".Dtos;");

                    if (_appSetting.WithResourcesFile)
                    {
                        sb.AppendLine("using  Resources;");
                        sb.AppendLine("using Microsoft.Extensions.Localization;");
                    }

                    sb.AppendLine("namespace " + ApplicationNameSpace + ".Validations");
                    sb.AppendLine("{");
                    sb.AppendLine("\tpublic class #className#  : AbstractValidator<#validatorName#>");
                    sb.AppendLine("\t{");

                    if (_appSetting.WithResourcesFile)
                    {
                        sb.AppendLine("\tpublic #className#(IStringLocalizer<SharedResource> localizer)");
                    }
                    else
                    {
                        sb.AppendLine("\tpublic #className#()");
                    }

                    sb.AppendLine("{");

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];

                        sb.AppendLine("\t\t" + UtilityHelper.CreateFluentValidationRules("DomainResource", table.Name,
                            column, table.ForeignKeys, useResourceFile));
                    }



                    sb.AppendLine("}");



                    var validatorNameCreateDto = classNameCreateDto;
                    //   var validatorNameUpdateDto = classNameUpdateDto;

                    classNameCreateDto = classNameCreateDto + "Validator";
                    //  classNameUpdateDto = classNameUpdateDto + "Validator";
                    customText =
                        UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationValidationsPath,
                            classNameCreateDto + ".cs"));
                    using (StreamWriter streamWriter =
                        new StreamWriter(Path.Combine(ApplicationValidationsPath, classNameCreateDto + ".cs")))
                    {
                        string clsName = sb.ToString().Replace("#className#", classNameCreateDto)
                            .Replace("#validatorName#", validatorNameCreateDto).ToString();

                        streamWriter.WriteLine(clsName);

                        streamWriter.WriteLine(customText);

                        streamWriter.WriteLine("\t}");
                        streamWriter.WriteLine("}");
                    }


                    //using (StreamWriter streamWriter = new StreamWriter(Path.Combine(UtilityHelper.ApplicationValidationsPath, classNameUpdateDto + ".cs")))
                    //{
                    //    string clsName = sb.ToString().Replace("#className#", classNameUpdateDto).Replace("#validatorName#", validatorNameUpdateDto).ToString();
                    //    streamWriter.WriteLine(clsName);


                    //}

                    #endregion

                    #region Mapping Classes


                    sb = new StringBuilder();

                    sb.AppendLine(GetCreatedDateTime());

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

                    // sb.AppendLine("\t}");


                    customText =
                        UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationMappingPath,
                            className + "Profile.cs"));

                    using (StreamWriter streamWriter =
                        new StreamWriter(Path.Combine(ApplicationMappingPath, className + "Profile.cs")))
                    {
                        string clsName = sb.ToString().Replace("#className#", classNameReadDto).ToString();
                        streamWriter.WriteLine(clsName);

                        streamWriter.WriteLine(customText);

                        streamWriter.WriteLine("}");
                        streamWriter.WriteLine("}");
                        streamWriter.WriteLine("}");
                    }





                    #endregion

                    #region Application Service Interface

                    customText = UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationServicesInterfacesPath,
                        "I" + className + "Service.cs"));


                    using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationServicesInterfacesPath,
                        "I" + className + "Service.cs")))
                    {
                        string keyType = "";
                        //foreach (var p in table.PrimaryKeys)
                        //{
                        //    keyType = UtilityHelper.GetCsType(p);
                        //}

                        var pk = table.PrimaryKeys.FirstOrDefault();
                        if (pk != null)
                        {
                            keyType = UtilityHelper.GetCsType(pk);
                        }

                        streamWriter.WriteLine(GetCreatedDateTime());
                        // Create the header for the class
                        streamWriter.WriteLine("using " + DomainNameSpace + ".Common;");
                        streamWriter.WriteLine("using " + ApplicationNameSpace + ".Dtos;");
                        streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace " + ApplicationNameSpace + @".Services.Interfaces
{
public partial interface I" + table.Name + @"Service 
{
Task<ClientMessage<" + keyType + @">> AddAsync(" + table.Name + @"UpsertDto " +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto);
Task<ClientMessage<int>> AddRangeAsync(List<" + table.Name + @"UpsertDto> " + UtilityHelper.FormatCamel(table.Name) +
                                               @"UpsertDto);
Task<ClientMessage<int>> UpdateAsync(" + table.Name + @"UpsertDto " + UtilityHelper.FormatCamel(table.Name) +
                                               @"UpsertDto);
Task<ClientMessage<int>> DeleteAsync(" + keyType + @" id);
Task<" + table.Name + @"ReadDto> GetByIdAsync(" + keyType + @" id);
Task<" + table.Name + @"ReadDto> GetByIdAsync(" + keyType + @" id, Func<IQueryable<" + table.Name + @">, IIncludableQueryable<" + table.Name + @", object>> include = null, bool disableTracking = true);
Task<List<" + table.Name + @"ReadDto>> GetAllAsync();
Task<PagedResult<" + table.Name +
                                               @"ReadDto>> GetAllAsync(string searchValue, string sortColumnName, bool orderAscendingDirection, int pageIndex, int pageSize);
Task<List<" + table.Name + @"ReadDto>> GetAllAsync(Expression<Func<" + table.Name +
                                               @", bool>> predicate, bool disableTracking = true);
Task<List<" + table.Name + @"ReadDto>> GetAllAsync(Expression<Func<" + table.Name + @", bool>> predicate = null,
            Func<IQueryable<" + table.Name + @">, IOrderedQueryable<" + table.Name + @">> orderBy = null,
            Func<IQueryable<" + table.Name + @">, IIncludableQueryable<" + table.Name + @", object>> include = null,
            bool disableTracking = true);

");

                        streamWriter.WriteLine(customText);


                        streamWriter.WriteLine(@" }
}"
                        );









                    }

                    #endregion

                    #region Application Service Impelemtation

                    var titleName = UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table);

                    customText = UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationServicesImplementationsPath,
                        className + "Service.cs"));

                    using (StreamWriter streamWriter =
                        new StreamWriter(Path.Combine(ApplicationServicesImplementationsPath,
                            className + "Service.cs")))
                    {

                        string keyType = "";
                        //foreach (var p in table.PrimaryKeys)
                        //{
                        //    keyType = UtilityHelper.GetCsType(p);
                        //}
                        var pk = table.PrimaryKeys.FirstOrDefault();
                        if (pk != null)
                        {
                            keyType = UtilityHelper.GetCsType(pk);
                        }



                        streamWriter.WriteLine(GetCreatedDateTime());
                        // Create the header for the class

                        if (_appSetting.WithResourcesFile)
                        {
                            streamWriter.WriteLine("using Microsoft.Extensions.Localization;");
                            streamWriter.WriteLine(" using Resources;");
                        }

                        streamWriter.WriteLine(@"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Validations;
using " + ApplicationNameSpace + @".Services.Interfaces;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Enums;
using " + ApplicationNameSpace + @".Common.Interfaces;
using " + ApplicationNameSpace + @".Extensions;

namespace " + ApplicationNameSpace + @".Services.Implementations
{
public partial class " + table.Name + @"Service : I" + table.Name + @"Service
{

private readonly IApplicationDbContext _context;
private readonly IMapper _mapper;" +
                                               (_appSetting.WithResourcesFile ? " private readonly IStringLocalizer<SharedResource> _localizer;" : "") + @"

public " + table.Name + @"Service(IApplicationDbContext context,IMapper mapper " + (_appSetting.WithResourcesFile ? ", IStringLocalizer<SharedResource> localizer" : "") + @")
{
  _context = context;
  _mapper = mapper;
" + (_appSetting.WithResourcesFile ? "   _localizer = localizer;" : "") + @"
}


public async Task<ClientMessage<" + keyType /*(_appSetting.KeyType == 0 ? "int" : "Guid")*/ + @">> AddAsync(" + table.Name +
                                               @"UpsertDto " +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto)
{

ClientMessage<" + keyType /*(_appSetting.KeyType == 0 ? "int" : "Guid")*/ + @"> clientMessage = new ClientMessage<" +
                                               keyType     /* (_appSetting.KeyType == 0 ? "int" : "Guid") */+ @">();

 " + table.Name + @"UpsertDtoValidator dtoValidator = new " + table.Name + "UpsertDtoValidator" + (_appSetting.WithResourcesFile ? "(_localizer);" : "();") + @"

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

     if (await _context." + table.Name + @".AnyAsync(o => o." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   ".ToUpper() == " +
                                                   UtilityHelper.FormatCamel(table.Name) + "UpsertDto." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   @".ToUpper()))
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

         int effectedRows = await _context.SaveChangesAsync(default);
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

         int effectedRows = await _context.SaveChangesAsync(default);
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

  public async Task<ClientMessage<int>> AddRangeAsync(List<" + table.Name + @"UpsertDto> " +
                                               UtilityHelper.FormatCamel(table.Name) + @"ListUpsertDto)
        {

            ClientMessage<int> clientMessage = new ClientMessage<int>();

            " + table.Name + @"UpsertDtoValidator dtoValidator = new " + table.Name + "UpsertDtoValidator" + (_appSetting.WithResourcesFile ? " (_localizer);" : "();") + @"

            ValidationResult validationResult = null;
            foreach (var v in " + UtilityHelper.FormatCamel(table.Name) + @"ListUpsertDto)
            {
                validationResult = dtoValidator.Validate(v);

                if (validationResult != null && validationResult.IsValid == false)
                {
                    clientMessage.ClientStatusCode = AppEnums.OperationStatus.ValidationError;
                    clientMessage.ValidationResults = validationResult.Errors.Select(modelError => new System.ComponentModel.DataAnnotations.ValidationResult(errorMessage: modelError.ErrorMessage)).ToList();
                    return clientMessage;
                }
            }



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

                foreach (var v in " + UtilityHelper.FormatCamel(table.Name) + @"ListUpsertDto)
                {
                    if (await _context." + table.Name + @".AnyAsync(o => o." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   ".ToUpper() == " +
                                                   "v." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   @".ToUpper()))
                    {
                        clientMessage.ClientStatusCode = AppEnums.OperationStatus.Error;
                        clientMessage.ClientMessageContent = new List<string>()
                        {
                            string.Format(SD.ExistData, v." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @")
                        };
                        return clientMessage;
                    }
                }
            ");
                        }

                        streamWriter.WriteLine(@"

                foreach (var v in " + UtilityHelper.FormatCamel(table.Name) + @"ListUpsertDto)
                {
                    " + table.Name + @" " + UtilityHelper.FormatCamel(table.Name) + @" = _mapper.Map<" + table.Name +
                                               @">(v);

                    await _context." + table.Name + @".AddAsync(" + UtilityHelper.FormatCamel(table.Name) + @");
                }

                int effectedRows = await _context.SaveChangesAsync(default);
                if (effectedRows != 0)
                {
                    clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
                    clientMessage.ReturnedData = effectedRows;
                }






            }

            return clientMessage;
        }


public async Task<ClientMessage<int>> UpdateAsync(" + table.Name + "UpsertDto " +
                                               UtilityHelper.FormatCamel(table.Name) + @"UpsertDto)
{
 ClientMessage<int> clientMessage = new ClientMessage<int>();

 " + table.Name + @"UpsertDtoValidator dtoValidator = new " + table.Name + "UpsertDtoValidator" + (_appSetting.WithResourcesFile ? " (_localizer);" : "();") + @"

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

     if (await _context." + table.Name + @".AnyAsync(o => o." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   @".ToUpper() == " +
                                                   UtilityHelper.FormatCamel(table.Name) + @"UpsertDto." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   @".ToUpper() && o." +
                                                   UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) +
                                                   @" != " +
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

         _context." + table.Name + @".Update(" + UtilityHelper.FormatCamel(table.Name) +
                                                   @");

         int effectedRows = await _context.SaveChangesAsync(default);
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

         _context." + table.Name + @".Update(" + UtilityHelper.FormatCamel(table.Name) +
                                                   @");

         int effectedRows = await _context.SaveChangesAsync(default);
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


public async Task<ClientMessage<int>> DeleteAsync(" + keyType /*(_appSetting.KeyType == 0 ? "int" : "Guid")*/ + @" id)
{
 ClientMessage<int> clientMessage = new ClientMessage<int>();

var entity = await _context." + table.Name + @"
     .Where(o => o." + UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" == id)
     .SingleOrDefaultAsync();

 _context." + table.Name + @".Remove(entity);

 int effectedRows = await _context.SaveChangesAsync(default);
 if (effectedRows != 0)
 {
     clientMessage.ClientStatusCode = AppEnums.OperationStatus.Ok;
     clientMessage.ReturnedData = effectedRows;
 }

 return clientMessage;
}

public async Task<" + table.Name + @"ReadDto> GetByIdAsync(" + keyType + @" id)
{
 var result = await _context." + table.Name + @".FirstOrDefaultAsync(o => o." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" == id);

 return _mapper.Map<" + table.Name + @"ReadDto>(result);
}

 public async Task<" + table.Name + @"ReadDto> GetByIdAsync(" + keyType + @" id, Func<IQueryable<" + table.Name + @">, IIncludableQueryable<" + table.Name + @", object>> include = null, bool disableTracking = true)
        {
            IQueryable<" + table.Name + @"> query = _context." + table.Name + @".AsQueryable();

            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            var result = await query.FirstOrDefaultAsync(o => o." + UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @" == id);

            return _mapper.Map<" + table.Name + @"ReadDto>(result);
        }

public async Task<List<" + table.Name + @"ReadDto>> GetAllAsync()
{
 var result = await _context." + table.Name + @".ToListAsync();
 return _mapper.Map<List<" + table.Name + @"ReadDto>>(result);
}

public async Task<PagedResult<" + table.Name +
                                               @"ReadDto>> GetAllAsync(string searchValue, string sortColumnName, bool orderAscendingDirection, int pageIndex, int pageSize)
{

 var result = _context." + table.Name + @".AsQueryable();

");
                        if (!string.IsNullOrEmpty(titleName))
                        {
                            streamWriter.WriteLine(@"
             if (!string.IsNullOrEmpty(searchValue))
             {
                 result = result.Where(r => r." +
                                                   UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                                   @".ToUpper().Contains(searchValue.ToUpper()));
             }
           ");
                        }

                        streamWriter.WriteLine(@"

         result = orderAscendingDirection ? result.OrderByDynamic(sortColumnName, DtOrderDir.Asc) : result.AsQueryable().OrderByDynamic(sortColumnName, DtOrderDir.Desc);

 var pagedResult = new PagedResult<" + table.Name + @"ReadDto>();
 pagedResult.TotalCount = await result.CountAsync();
 pagedResult.FilteredTotalCount = await _context." + table.Name + @".CountAsync();
 pagedResult.Data = _mapper.Map<List<" + table.Name +
                                               @"ReadDto>>(await result.Skip(pageIndex).Take(pageSize).ToListAsync());

 return pagedResult;
}



public async Task<List<" + table.Name + @"ReadDto>> GetAllAsync(Expression<Func<" + table.Name +
                                               @", bool>> predicate = null, bool disableTracking = true)
        {
            IQueryable<" + table.Name + @"> query = _context." + table.Name + @".AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (disableTracking)
                query = query.AsNoTracking();

 var result = await query.ToListAsync();
 return _mapper.Map<List<" + table.Name + @"ReadDto>>(result);
        }



  public async Task<List<" + table.Name + @"ReadDto>> GetAllAsync(Expression<Func<" + table.Name +
                                               @", bool>> predicate = null, Func<IQueryable<" + table.Name +
                                               @">, IOrderedQueryable<" + table.Name +
                                               @">> orderBy = null, Func<IQueryable<" + table.Name +
                                               @">, IIncludableQueryable<" + table.Name +
                                               @", object>> include = null, bool disableTracking = true)
        {
            IQueryable<" + table.Name + @"> query = _context." + table.Name + @".AsQueryable();

            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);


            var result = await query.ToListAsync();

            return _mapper.Map<List<" + table.Name + @"ReadDto>>(result);

        }
 


  ");

                        streamWriter.WriteLine(customText);


                        streamWriter.WriteLine(@" }
}"
                        );









                    }

                    #endregion
                }
            }


            #region Application Extensions LinqExtensions



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationExtensionsPath, "LinqExtensions.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
                // Create the header for the class

                streamWriter.WriteLine(@"

using System;
using System.Linq;
using System.Linq.Expressions;
using " + DomainNameSpace + @".Common;

namespace " + ApplicationNameSpace + @".Extensions
{
public static class LinqExtensions
{
public static IQueryable<T> OrderByDynamic<T>(
 this IQueryable<T> query,
 string orderByMember,
 DtOrderDir ascendingDirection)
{
 var param = Expression.Parameter(typeof(T), ""c"");

     var body = orderByMember.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

     var queryable = ascendingDirection == DtOrderDir.Asc ?
         (IOrderedQueryable<T>)Queryable.OrderBy(query.AsQueryable(), (dynamic)Expression.Lambda(body, param)) :
         (IOrderedQueryable<T>)Queryable.OrderByDescending(query.AsQueryable(), (dynamic)Expression.Lambda(body, param));

     return queryable;
 }

 public static IQueryable<T> WhereDynamic<T>(
     this IQueryable<T> sourceList, string query)
 {

     if (string.IsNullOrEmpty(query))
     {
         return sourceList;
     }

     try
     {

         var properties = typeof(T).GetProperties()
             .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);

         //Expression
         sourceList = sourceList.Where(c =>
             properties.Any(p => p.GetValue(c) != null && p.GetValue(c).ToString()
                 .Contains(query, StringComparison.InvariantCultureIgnoreCase)));
     }
     catch (Exception e)
     {
         Console.WriteLine(e);
     }

     return sourceList;
 }
}
}"
                );









            }
            #endregion

            #region Application Dependency Injection


            customText = UtilityHelper.ReadCustomRegionText(Path.Combine(ApplicationPath, "DependencyInjection.cs"));

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationPath, "DependencyInjection.cs")))
            {
                streamWriter.WriteLine(GetCreatedDateTime());
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
                    if (table.Name != "AspNetUsers")
                        streamWriter.WriteLine("services.AddTransient<I" + table.Name + "Service, " + table.Name + "Service>();");
                }

                streamWriter.WriteLine(@"



 return services;
}

");

                streamWriter.WriteLine(customText);


                streamWriter.WriteLine(@" }
}"
                    );









            }
            #endregion




        }


        private static async void CreateResourceFile()
        {
            if (_appSetting.WithResourcesFile)
            {
                string customText = "";

                #region Common Classes

                #region LocalizationDbContext

                customText =
                    UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesDataPath, "LocalizationDbContext.cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ResourcesDataPath, "LocalizationDbContext.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
using Microsoft.EntityFrameworkCore;
using Resources.Models;

namespace Resources.Data
{
    public class LocalizationDbContext : DbContext
    {
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<Resource> Resources { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(""ResourcesTest"");
        }
    }

}
");
                }

                #endregion

                #region LangProvider

                customText =
                    UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesLanguagesPath, "LangProvider.cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ResourcesLanguagesPath, "LangProvider.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
 using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Resources.Models;

namespace Resources.Languages
{
    public static class LangProvider
    {
        public static List<Resource> GetList(string cultureFiles)
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            List<Resource> resourcesFiles = new List<Resource>();

            foreach (string fileName in Directory.GetFiles(directory + ""/Languages/"", ""*_"" + cultureFiles + "".json""))
            {
                resourcesFiles.AddRange(JsonConvert.DeserializeObject<List<Resource>>(File.ReadAllText(fileName), jsonSerializerSettings));
            }

            return resourcesFiles;

        }
    }
}

                ");
                }

                #endregion

                #region Models

                customText = UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesModelsPath, "Culture.cs"));

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ResourcesModelsPath, "Culture.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
using System.Collections.Generic;

namespace Resources.Models
{
    public class Culture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Resource> Resources { get; set; }
    }
}

");
                }


                customText = UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesModelsPath, "Resource.cs"));

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ResourcesModelsPath, "Resource.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
namespace Resources.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual Culture Culture { get; set; }
    }
}

");
                }


                #endregion

                #region Services

                customText =
                    UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesServicesPath, "EFStringLocalizer.cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ResourcesServicesPath, "EFStringLocalizer.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Resources.Data;

namespace Resources.Services
{
    public class EFStringLocalizer : IStringLocalizer
    {
        private readonly LocalizationDbContext _db;

        public EFStringLocalizer(LocalizationDbContext db)
        {
            _db = db;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value ==
                    null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new EFStringLocalizer(_db);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            return _db.Resources
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string GetString(string name)
        {
            return _db.Resources
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .FirstOrDefault(r => r.Key == name)?.Value;
        }
    }

    public class EFStringLocalizer<T> : IStringLocalizer<T>
    {
        private readonly LocalizationDbContext _db;

        public EFStringLocalizer(LocalizationDbContext db)
        {
            _db = db;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value ==
                    null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new EFStringLocalizer(_db);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            return _db.Resources
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string GetString(string name)
        {
            return _db.Resources
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .FirstOrDefault(r => r.Key == name)?.Value;
        }
    }
}
");
                }


                customText =
                    UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesServicesPath,
                        "EFStringLocalizerFactory.cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ResourcesServicesPath, "EFStringLocalizerFactory.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
using System;
using Microsoft.Extensions.Localization;
using Resources.Data;
using Resources.Languages;
using Resources.Models;

namespace Resources.Services
{
    public class EFStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly LocalizationDbContext _db;

        public EFStringLocalizerFactory()
        {
            _db = new LocalizationDbContext();
            // Here we define all available languages to the app
            // available languages are those that have a json and cs file in
            // the Languages folder
            _db.AddRange(
              " + GetSelectedCultures() + @"
                );
            _db.SaveChanges();
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new EFStringLocalizer(_db);
        }
        public IStringLocalizer Create(string baseName, string location)
        {
            return new EFStringLocalizer(_db);
        }
    }
}

");
                }


                #endregion


                #region SharedResource

                customText =
                    UtilityHelper.ReadCustomRegionText(Path.Combine(ResourcesPath, "SharedResource.cs"));

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ResourcesPath, "SharedResource.cs")))
                {
                    // Create the header for the class

                    streamWriter.WriteLine(GetCreatedDateTime());
                    streamWriter.WriteLine(@"
namespace Resources
{
    public class SharedResource
    {
    }
}

");
                }

                #endregion

                #endregion

                string sqlTables = @"SELECT DISTINCT T.name AS TableName ,
        C.name AS ColumnName ,
        T.name+'_'+C.name AS TableColumnName
        FROM   sys.objects AS T
        JOIN sys.columns AS C ON T.object_id = C.object_id
        JOIN sys.types AS P ON C.system_type_id = P.system_type_id
        WHERE  T.type_desc = 'USER_TABLE' AND p.name <>  'sysname'
AND  T.name NOT IN  ('__EFMigrationsHistory','AspNetRoleClaims','AspNetRoles','AspNetUserClaims','AspNetUserLogins','AspNetUserRoles','AspNetUsers','AspNetUserTokens')
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

                //if (_appSetting.LanguagesResourcesFile.Contains("ar"))
                //{
                //    using (StreamWriter streamWriter =
                //        new StreamWriter(Path.Combine(ResourcesLanguagesPath, "validation_ar.json")))
                //    {
                //        // Create the header for the class
                //        streamWriter.WriteLine("[");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""EmailAddress"",""Value"": ""{PropertyName} البريد الإلكتروني غير صحيح.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""Equal"",""Value"": ""{PropertyName} يجب ان تساوي {0}""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""GreaterThan"",""Value"": ""{PropertyName} يجب ان تكون أكبر من {0}""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""GreaterThanOrEqual"",""Value"": ""{PropertyName} يجب ان تكون أكبر من أو تساوي {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""IsValidDateTime"",""Value"": ""{PropertyName} يجب أن يكون في الصغية الصحيحة.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""Length"",""Value"": ""{PropertyName}  عدد الحروف يجب ان يكون  أصغر من{1""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""LessThan"",""Value"": ""{PropertyName} يجب أن يكون أقل من {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""LessThanOrEqual"",""Value"": ""{PropertyName} يجب أن يكون أقل أو يساوي {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""NotEmpty"",""Value"": ""{PropertyName} يجب أن يحتوي على قيمة.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""NotEqual"",""Value"": ""{PropertyName} يجب أن لا يساوي {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""RegularExpression"",""Value"": ""{PropertyName} يجب أن يكون في الصغية الصحيحة.""}");
                //        streamWriter.WriteLine("]");
                //    }
                //}

                //if (_appSetting.LanguagesResourcesFile.Contains("en"))
                //{

                //    using (StreamWriter streamWriter =
                //        new StreamWriter(Path.Combine(ResourcesLanguagesPath, "validation_en.json")))
                //    {
                //        streamWriter.WriteLine("[");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""EmailAddress"",""Value"": ""{PropertyName} is not a valid email address.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""Equal"",""Value"": ""{PropertyName} should be equal to {0}""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""GreaterThan"",""Value"": ""{PropertyName} must be greater than {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""GreaterThanOrEqual"",""Value"": ""{PropertyName} must be greater than or equal to {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""IsValidDateTime"",""Value"": ""{PropertyName} is not in the correct format.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""Length"",""Value"": ""{PropertyName} must be less than {1} characters.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""LessThan"",""Value"": ""{PropertyName} must be less than {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""LessThanOrEqual"",""Value"": ""{PropertyName} must be less than or equal to {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""NotEmpty"",""Value"": ""{PropertyName} should not be empty.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""NotEqual"",""Value"": ""{PropertyName} should not be equal to {0}.""},");
                //        streamWriter.WriteLine(
                //            @" {""Key"": ""RegularExpression"",""Value"": ""{PropertyName} is not in the correct format.""}");
                //        streamWriter.WriteLine("]");
                //    }
                //}

                foreach (var l in _appSetting.LanguagesResourcesFile)
                {
                    using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ResourcesLanguagesPath, "db_" + l + ".json")))
                    {
                        // Create the header for the class
                        streamWriter.WriteLine("[");

                        foreach (var key in keysList)
                        {
                            if (l == "en")
                            {
                                streamWriter.WriteLine(@" {""Key"": """ + key.Key + @""",""Value"": """ +
                                                       UtilityHelper.FixName(key.Value) + @"""},");
                            }
                            else
                            {
                                if (_appSetting.WithTranslations)
                                {
                                    streamWriter.WriteLine(@" {""Key"": """ + key.Key + @""",""Value"": """ +
                                                           await UtilityHelper.GetTranslationAsync(
                                                               UtilityHelper.FixName(key.Value), l) + @"""},");

                                }
                                else
                                {
                                    streamWriter.WriteLine(@" {""Key"": """ + key.Key + @""",""Value"": """ +
                                                           UtilityHelper.FixName(key.Value) + @"""},");
                                }

                            }
                        }

                        streamWriter.WriteLine("]");
                    }
                }

                #region Resource Files
                //using (
                //    ResXResourceWriter resx =
                //        new ResXResourceWriter(DomainResourcesPath + "/DomainResource.resx"))
                //{

                //    resx.AddResource("EmailAddress", "{PropertyName} is not a valid email address.");
                //    resx.AddResource("Equal", "{PropertyName} should be equal to {0}");
                //    resx.AddResource("GreaterThan", "{PropertyName} must be greater than {0}.");
                //    resx.AddResource("GreaterThanOrEqual",
                //        "{PropertyName} must be greater than or equal to {0}.");
                //    resx.AddResource("IsValidDateTime", "{PropertyName} is not in the correct format.");
                //    resx.AddResource("Length", "{PropertyName} must be less than {1} characters.");
                //    resx.AddResource("LessThan", "{PropertyName} must be less than {0}.");
                //    resx.AddResource("LessThanOrEqual", "{PropertyName} must be less than or equal to {0}.");
                //    resx.AddResource("NotEmpty", "{PropertyName} should not be empty.");
                //    resx.AddResource("NotEqual", "{PropertyName} should not be equal to {0}.");
                //    resx.AddResource("RegularExpression", "{PropertyName} is not in the correct format.");

                //    foreach (var key in keysList)
                //    {
                //        resx.AddResource(key.Key, UtilityHelper.FixName(key.Value));
                //    }

                //}

                //using (
                //    ResXResourceWriter resx =
                //        new ResXResourceWriter(DomainResourcesPath + @"/DomainResource.ar.resx"))
                //{

                //    resx.AddResource("EmailAddress", "{PropertyName} البريد الإلكتروني غير صحيح.");
                //    resx.AddResource("Equal", "{PropertyName} يجب ان تساوي {0}");
                //    resx.AddResource("GreaterThan", "{PropertyName} يجب ان تكون أكبر من {0}.");
                //    resx.AddResource("GreaterThanOrEqual", "{PropertyName} يجب ان تكون أكبر من أو تساوي {0}.");
                //    resx.AddResource("IsValidDateTime", "{PropertyName} يجب أن يكون في الصغية الصحيحة.");
                //    resx.AddResource("Length", "{PropertyName}  عدد الحروف يجب ان يكون  أصغر من{1}.");
                //    resx.AddResource("LessThan", "{PropertyName} يجب أن يكون أقل من {0}.");
                //    resx.AddResource("LessThanOrEqual", "{PropertyName} يجب أن يكون أقل أو يساوي {0}.");
                //    resx.AddResource("NotEmpty", "{PropertyName} يجب أن يحتوي على قيمة.");
                //    resx.AddResource("NotEqual", "{PropertyName} يجب أن لا يساوي {0}.");
                //    resx.AddResource("RegularExpression", "{PropertyName} يجب أن يكون في الصغية الصحيحة.");

                //    foreach (var key in keysList)
                //    {
                //        resx.AddResource(key.Key, UtilityHelper.FixName(key.Value));
                //    }
                //}

                #endregion
                #endregion
            }
        }

        private static string GetIAuditEntity()
        {
            if (_appSetting.KeyType == 0)
            {
                return @" [Required]
                public int CreatedBy { get; set; }

                [Required]
                public DateTime CreatedDate { get; set; }

                public int? ModifiedBy { get; set; }

                public DateTime? ModifiedDate { get; set; }

                public bool IsDeleted { get; set; }

                public int? DeletedBy { get; set; }

                public DateTime? DeletedDate { get; set; }";
            }
            else
            {
                return @" [Required]
                public Guid CreatedBy { get; set; }

                [Required]
                public DateTime CreatedDate { get; set; }

                public Guid? ModifiedBy { get; set; }

                public DateTime? ModifiedDate { get; set; }

                public bool IsDeleted { get; set; }

                public Guid? DeletedBy { get; set; }

                public DateTime? DeletedDate { get; set; }";
            }
        }
        private static string GetBaseEntity()
        {
            if (_appSetting.KeyType == 0)
            {
                return @" 
                //  [Key]
              //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
              //  public int Id { get; set; }

             //   public int TenantId { get; set; }
            
              //  public bool IsActive { get; set; }
              //  public int ShowOrder { get; set; }

                public int? CreatedBy { get; set; }
                [DataType(DataType.DateTime)]
                public DateTime? CreatedDate { get; set; }

                public int? UpdatedBy { get; set; }
                [DataType(DataType.DateTime)]
                public DateTime? UpdatedDate { get; set; }
            

              */";
            }
            else
            {
                return @" 
                // [Key]
             //   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
              //  public Guid Id { get; set; }

              // public Guid TenantId { get; set; }
            
                //public bool IsActive { get; set; }
                //public int ShowOrder { get; set; }

                
                public Guid? CreatedBy { get; set; }
                [DataType(DataType.DateTime)]
                public DateTime? CreatedDate { get; set; }

                public Guid? UpdatedBy { get; set; }
                [DataType(DataType.DateTime)]
                public DateTime? UpdatedDate { get; set; }
            

              
        ";
            }
        }
        private static string GetCreatedDateTime()
        {
            // return "// Created DateTime  " + createdDateTime.ToString("yyyy-MM-dd hh-mm-ss tt");
            return "";
        }

        private static string GetSelectedCultures()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var l in _appSetting.LanguagesResourcesFile)
            {
                sb.AppendLine(@"  new Culture
                {
                    Name = """ + l + @""",
                    Resources = LangProvider.GetList(""" + l + @""")
            },");
            }

            return sb.ToString().Substring(0, sb.ToString().Length - 1);
        }
    }
}
