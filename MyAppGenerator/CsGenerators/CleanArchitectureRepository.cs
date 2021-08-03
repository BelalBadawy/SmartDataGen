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
    public static class CleanArchitectureRepository
    {
        public static List<Table> tableList = null;
        private static AppSetting _appSetting { get; set; }

        #region Create Folders 
        public static string OutputDirectory { get; set; }
        public static string ProjectName { get; set; }
        public static string DataPath { get; set; }

        #region Infrastructure Folders
        public static string InfraCommonFolderPath { get; set; }
        public static string InfraDataInitializerPath { get; set; }
        public static string InfraDataFolderPath { get; set; }

        public static string InfraEntityConfigurationPath { get; set; }
        public static string InfraExtensionsPath { get; set; }
        public static string InfraInterfacesPath { get; set; }
        public static string InfraIdentityPath { get; set; }
        public static string InfraIdentityServicesPath { get; set; }
        public static string InfraInterfacesDataPath { get; set; }
        public static string InfraInterfacesRepositoriesPath { get; set; }
        public static string InfraRepositoriesPath { get; set; }
        // public static string InfraRepositoriesDataPath { get; set; }
        #endregion

        #region Domain Folders
        public static string DomainPath { get; set; }
        public static string DomainCommonPath { get; set; }
        public static string DomainEntitiesPath { get; set; }
        public static string DomainEnumsPath { get; set; }
        public static string DomainResourcesPath { get; set; }

        public static string DomainInterfacesPath { get; set; }
        public static string DomainModelsPath { get; set; }

        #endregion

        #region Application Folders

        public static string ApplicationPath { get; set; }
        public static string ApplicationDtosPath { get; set; }
        public static string ApplicationExceptionsPath { get; set; }
        public static string ApplicationInterfacesPath { get; set; }
        public static string ApplicationInterfacesRepositoriesPath { get; set; }

        public static string ApplicationMappingPath { get; set; }
        public static string ApplicationServicesPath { get; set; }
        public static string ApplicationServicesImplementationsPath { get; set; }
        public static string ApplicationServicesInterfacesPath { get; set; }
        public static string ApplicationValidationsPath { get; set; }


        #endregion


        #region API Folders

        public static string ApiPath { get; set; }
        public static string ApiControllersPath { get; set; }
        public static string ApiFiltersPath { get; set; }
        public static string ApiInfrastructurePath { get; set; }
        public static string ApiMiddlewarePath { get; set; }

        #endregion

        public static List<NodeType> tables { get; set; }
        public static bool useResourceFile { get; set; }

        public static string DomainNameSpace = ProjectName + ".Domain";
        public static string DataAccessNameSpace = ProjectName + ".Infrastructure";
        public static string ApplicationNameSpace = ProjectName + ".Application";
        public static string ApiNameSpace = ProjectName + ".API";

        #endregion

        public static void GenerateCleanArchitectureRepository(AppSetting appSetting)
        {
            _appSetting = appSetting;
            DomainNameSpace = appSetting.ProjectName + ".Domain";
            DataAccessNameSpace = appSetting.ProjectName + ".Infrastructure";
            ApplicationNameSpace = appSetting.ProjectName + ".Application";
            ApiNameSpace = appSetting.ProjectName + ".API";


            CreateFoldersForCleanArchitectureRepository(appSetting.OutputDirectory);
            GetSelectedTales();
            CreateDomainClasses();
            CreateInfrastructureClasses();
            CreateApplicationClasses();
            CreateAPIClasses();
            CreateResourceFile();
        }

        private static void GetSelectedTales()
        {
            tableList = new List<Table>();

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
        }

        private static void CreateFoldersForCleanArchitectureRepository(string outputDirectory)
        {

            OutputDirectory = outputDirectory;

            #region Infrastructure Folders

            DataPath = Path.Combine(outputDirectory, "Infrastructure");
            UtilityHelper.CreateSubDirectory(DataPath, true);


            InfraCommonFolderPath = Path.Combine(outputDirectory, "Infrastructure/Common");
            UtilityHelper.CreateSubDirectory(InfraCommonFolderPath, true);

            InfraDataFolderPath = Path.Combine(outputDirectory, "Infrastructure/Data");
            UtilityHelper.CreateSubDirectory(InfraDataFolderPath, true);

            InfraDataInitializerPath = Path.Combine(outputDirectory, "Infrastructure/Data/Initializer");
            UtilityHelper.CreateSubDirectory(InfraDataInitializerPath, true);

            InfraEntityConfigurationPath = Path.Combine(outputDirectory, "Infrastructure/Data/EntityConfiguration");
            UtilityHelper.CreateSubDirectory(InfraEntityConfigurationPath, true);

            InfraRepositoriesPath = Path.Combine(outputDirectory, "Infrastructure/Data/Repositories");
            UtilityHelper.CreateSubDirectory(InfraRepositoriesPath, true); //partial


            InfraExtensionsPath = Path.Combine(outputDirectory, "Infrastructure/Extensions");
            UtilityHelper.CreateSubDirectory(InfraExtensionsPath, true);


            InfraIdentityPath = Path.Combine(outputDirectory, "Infrastructure/Identity");
            UtilityHelper.CreateSubDirectory(InfraIdentityPath, true);


            InfraIdentityServicesPath = Path.Combine(outputDirectory, "Infrastructure/Identity/Services");
            UtilityHelper.CreateSubDirectory(InfraIdentityServicesPath, true);



            #endregion

            #region Domain Folders

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

            DomainInterfacesPath = Path.Combine(DomainPath, "Interfaces");
            UtilityHelper.CreateSubDirectory(DomainInterfacesPath, true);


            DomainModelsPath = Path.Combine(DomainPath, "Models");
            UtilityHelper.CreateSubDirectory(DomainModelsPath, true);

            #endregion

            #region Application Folders

            ApplicationPath = Path.Combine(outputDirectory, "Application");
            UtilityHelper.CreateSubDirectory(ApplicationPath, true);

            ApplicationDtosPath = Path.Combine(ApplicationPath, "Dtos");
            UtilityHelper.CreateSubDirectory(ApplicationDtosPath, true);



            ApplicationExceptionsPath = Path.Combine(ApplicationPath, "Exceptions");
            UtilityHelper.CreateSubDirectory(ApplicationExceptionsPath, true);


            ApplicationInterfacesPath = Path.Combine(ApplicationPath, "Interfaces");
            UtilityHelper.CreateSubDirectory(ApplicationInterfacesPath, true);



            ApplicationInterfacesRepositoriesPath = Path.Combine(ApplicationInterfacesPath, "Repositories");
            UtilityHelper.CreateSubDirectory(ApplicationInterfacesRepositoriesPath, true);




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
            #endregion


            #region API Folders

            ApiPath = Path.Combine(outputDirectory, "API");
            UtilityHelper.CreateSubDirectory(ApiPath, true);

            ApiControllersPath = Path.Combine(ApiPath, "Controllers");
            UtilityHelper.CreateSubDirectory(ApiControllersPath, true);

            ApiFiltersPath = Path.Combine(ApiPath, "Filters");
            UtilityHelper.CreateSubDirectory(ApiFiltersPath, true);

            ApiInfrastructurePath = Path.Combine(ApiPath, "Infrastructure");
            UtilityHelper.CreateSubDirectory(ApiInfrastructurePath, true);

            ApiMiddlewarePath = Path.Combine(ApiPath, "Middleware");
            UtilityHelper.CreateSubDirectory(ApiMiddlewarePath, true);

            #endregion

        }




        private static void CreateDomainClasses()
        {

            #region Common Classes

            #region BaseEntity Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "BaseEntity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            using System.ComponentModel.DataAnnotations;
                                            using System.ComponentModel.DataAnnotations.Schema;

                                            namespace " + DomainNameSpace + @".Common
                                            {
                                                public abstract class BaseEntity
                                                {
                                                    [Key]
                                                    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                                                     public virtual " + UtilityHelper.GetIDKeyType(_appSetting) + @" Id { get; set; }
                                                }
                                            }
                                        ");


            }

            #endregion

            #region Custom Claim Types Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "CustomClaimTypes.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;

                                            namespace " + DomainNameSpace + @".Common
                                            {
                                               public class CustomClaimTypes
                                                {
                                                     public const string Permission = ""permission"";
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


            #region Jwt Settings Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "JwtSettings.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;

                                            namespace " + DomainNameSpace + @".Common
                                            {
                                              public class JwtSettings
                                                {
                                                    public string Key { get; set; }
                                                    public string Issuer { get; set; }
                                                    public string Audience { get; set; }
                                                    public double DurationInMinutes { get; set; }
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


            #region Response Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "Response.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
namespace " + DomainNameSpace + @".Common
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }

        public Response(List<string> errors)
        {
            Succeeded = false;
            Errors = errors;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
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
namespace " + DomainNameSpace + @".Common
{
  public class KeyValue
    {
        public " + UtilityHelper.GetIDKeyType(_appSetting) + @" Id { get; set; }
        public string Value { get; set; }
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
        public const string ErrorOccurred = ""An error has been occured."";
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



            #region Permissions Class



            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainCommonPath, "AppPermissions.cs")))
            {

                streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Reflection;

namespace " + DomainNameSpace + @".Common
{
    public static class AppPermissions 
    {
      ");
                string className = "";

                foreach (Table table in tableList)
                {
                    className = UtilityHelper.MakeSingular(table.Name);

                    streamWriter.WriteLine("public static class " + className);
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("public const string List = \"Permissions." + className + ".List\";");
                    streamWriter.WriteLine("public const string View = \"Permissions." + className + ".View\";");
                    streamWriter.WriteLine("public const string Create = \"Permissions." + className + ".Create\";");
                    streamWriter.WriteLine("public const string Edit = \"Permissions." + className + ".Edit\";");
                    streamWriter.WriteLine("public const string Delete = \"Permissions." + className + ".Delete\";");
                    streamWriter.WriteLine("}");

                }

                streamWriter.WriteLine(@" 

        }
}
                                        ");


            }

            #endregion
            #endregion


            #region Interfaces

            #region IAuditable Interface
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainInterfacesPath, "IAuditable.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            using System.ComponentModel.DataAnnotations;

                                            namespace " + DomainNameSpace + @".Interfaces
                                            {
                                                public interface IAuditable
    {
        public Guid? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public Guid? LastModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedAt { get; set; }
    }
                                            }
                                        ");


            }

            #endregion

            #region IBaseEntity Interface
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainInterfacesPath, "IBaseEntity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            using System.ComponentModel.DataAnnotations;

                                            namespace " + DomainNameSpace + @".Interfaces
                                            {
                                               public interface IBaseEntity
                                                {
                                                    public " + UtilityHelper.GetIDKeyType(_appSetting) + @" Id { get; set; }
                                                }
                                            }
                                        ");


            }

            #endregion

            #region IDataConcurrency Interface
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainInterfacesPath, "IDataConcurrency.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            namespace " + DomainNameSpace + @".Interfaces
                                            {
                                               public interface IDataConcurrency
                                                {
                                                    public byte[] RowVersion { get; set; }
                                                }
                                            }
                                        ");


            }

            #endregion

            #region ISoftDelete Interface
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainInterfacesPath, "ISoftDelete.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            namespace " + DomainNameSpace + @".Interfaces
                                            {
                                              public interface ISoftDelete
                                                {
                                                    public bool SoftDeleted { get; set; }
                                                }
                                            }
                                        ");


            }

            #endregion


            #endregion

            #region Models

            #region AuthenticationResponse Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainModelsPath, "AuthenticationResponse.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            namespace " + DomainNameSpace + @".Models
                                            {
                                              public class AuthenticationResponse
                                                {
                                                    public string Id { get; set; }
                                                    public string UserName { get; set; }
                                                    public string Email { get; set; }
                                                    public string Token { get; set; }
                                                }
                                            }
                                        ");


            }

            #endregion

            #region LoginModel Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainModelsPath, "LoginModel.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            namespace " + DomainNameSpace + @".Models
                                            {
                                             public class LoginModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }
    }
                                            }
                                        ");


            }

            #endregion

            #region Log User Activity Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainModelsPath, "LogUserActivity.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            namespace " + DomainNameSpace + @".Models
                                            {
                                                public class LogUserActivity
                                                {
		                                            public Guid Id { get; set; }
                                                    public Guid UserId { get; set; }
                                                    public DateTime CreatedDateTime { get; set; }
                                                    public string UrlData { get; set; }
                                                    public string UserData { get; set; }
                                                    public string IPAddress { get; set; }
                                                    public string Browser { get; set; }
                                                    public string HttpMethod { get; set; }
	                                            }
                                            }
                                        ");


            }

            #endregion

            #region RegistrationModel Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(DomainModelsPath, "RegistrationModel.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                            using System;
                                            using System.ComponentModel.DataAnnotations;
                                            namespace " + DomainNameSpace + @".Models
                                            {
                                              public class RegistrationModel
                                                {
                                                    [Required]
                                               
                                                    public string FullName { get; set; }

                                                 
                                                    [Required]
                                                    [EmailAddress]
                                                    public string Email { get; set; }

                                                    //[Required]
                                                    //[MinLength(6)]
                                                    //public string UserName { get; set; }

                                                    [Required]
                                                    [MinLength(6)]
                                                    public string Password { get; set; }
                                                }
                                            }
                                        ");


            }

            #endregion

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

  public enum MsgType
        {
            Success = 1,
            Error = 2,
            Warning = 3,
            Info = 4
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
                                        using System;
                                        using Microsoft.AspNetCore.Identity;
                                        using System.ComponentModel.DataAnnotations;

                                        namespace " + DomainNameSpace + @".Entities
                                        {
                                           public class ApplicationUser : IdentityUser<" + UtilityHelper.GetIDKeyType(_appSetting) + @">
                                            {
                                                [Required]
                                                [StringLength(200)]
                                                public string FullName { get; set; }

                                            }
                                        }
                                      ");


            }


            #endregion






            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region  Model Classes
                className = UtilityHelper.MakeSingular(table.Name);

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DomainEntitiesPath, className + ".cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using System.Collections.Generic;");
                    streamWriter.WriteLine("using System.ComponentModel.DataAnnotations;");
                    streamWriter.WriteLine("using System.ComponentModel.DataAnnotations.Schema;");
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Interfaces;");

                    streamWriter.WriteLine();
                    streamWriter.WriteLine("namespace " + DomainNameSpace + ".Entities");
                    streamWriter.WriteLine("{");

                    streamWriter.WriteLine("\tpublic class " + className + UtilityHelper.GetEntityInterfaces(table.Columns, UtilityHelper.GetIDKeyType(_appSetting)));

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


                    //foreach (var t in tableList)
                    //{
                    //    foreach (var f in t.ForeignKeys.Values)
                    //    {
                    //        var foreignKeysList = f.Where(o => o.PrimaryKeyTableName == table.Name).ToList();
                    //        for (int j = 0; j < foreignKeysList.Count; j++)
                    //        {
                    //            if (!string.IsNullOrEmpty(foreignKeysList[j].ForeignKeyTableName))
                    //            {
                    //                if (sameKey != foreignKeysList[j].ForeignKeyTableName)
                    //                {
                    //                    sameKey = foreignKeysList[j].ForeignKeyTableName;

                    //                    streamWriter.WriteLine("\t\t this." +
                    //                                           foreignKeysList[j].ForeignKeyTableName + "List =  new List<" + sameKey + ">(); ");
                    //                    streamWriter.WriteLine();
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

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
                                                               UtilityHelper.MakeSingular(foreignKeysList[j].ForeignKeyTableName) + "> " +
                                                             UtilityHelper.MakePlural(foreignKeysList[j].ForeignKeyTableName) +
                                                               " { get; set; }");
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
                                if (foreignKeysList[j].PrimaryKeyTableName == foreignKeysList[j].ForeignKeyTableName)
                                {
                                    // Self-referencing entity with one to many relationship generates
                                    streamWriter.WriteLine("[ForeignKey(\"" +
                                                           foreignKeysList[j].ForeignKeyColumnName + "\")]");
                                    streamWriter.WriteLine("\t\tpublic virtual " +
                                                           UtilityHelper.MakeSingular(foreignKeysList[j]
                                                               .PrimaryKeyTableName) + " Parent " +
                                                           " { get; set; }");

                                }

                                if (sameKey != foreignKeysList[j].PrimaryKeyTableName)
                                {
                                    sameKey = foreignKeysList[j].PrimaryKeyTableName;
                                    //  public virtual Course Course { get; set; }
                                    // streamWriter.WriteLine("\t\t[BindNever]");
                                    streamWriter.WriteLine("[ForeignKey(\"" +
                                                           foreignKeysList[j].ForeignKeyColumnName + "\")]");
                                    streamWriter.WriteLine("\t\tpublic virtual " +
                                                           foreignKeysList[j].PrimaryKeyTableName + " " +
                                                           foreignKeysList[j].PrimaryKeyTableName +
                                                           " { get; set; }");
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
            #region Common Classes

            #region  DateTimeService

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraCommonFolderPath, "DateTimeService.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + ApplicationNameSpace + @".Interfaces;
using System;

namespace " + DataAccessNameSpace + @".Common
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}

");

            }

            #endregion


            #region  InMemorySessionWrapper

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraCommonFolderPath, "InMemorySessionWrapper.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + ApplicationNameSpace + @".Interfaces;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace " + DataAccessNameSpace + @".Common
{
   public class InMemorySessionWrapper : ISessionWrapper
    {

        private IHttpContextAccessor _httpContextAccessor;
        public InMemorySessionWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public T GetFromSession<T>(string key)
        {
            var value = _httpContextAccessor?.HttpContext?.Session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public void RemoveFromSession(string key)
        {
            _httpContextAccessor?.HttpContext?.Session.Remove(key);
        }

        public void SetInSession<T>(string key, T value)
        {
            _httpContextAccessor?.HttpContext?.Session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}

");

            }

            #endregion
            #endregion

            #region Data Classes

            #region EntityConfiguration

            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region  Model Classes
                className = UtilityHelper.MakeSingular(table.Name);

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraEntityConfigurationPath, className + "Configuration.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace " + DataAccessNameSpace + @".Data.EntityConfiguration
{
    public partial class " + className + @"Configuration : IEntityTypeConfiguration<" + className + @">
    {
        public void Configure(EntityTypeBuilder<" + className + @"> builder)
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
                        var primaryKeys = table.PrimaryKeys.Select(o => o.Name).ToList();

                        if (!primaryKeys.Contains(column.Name))
                        {
                            // Ignore any identity columns
                            streamWriter.WriteLine(UtilityHelper.CreateBuilderPropertyForEntityFramework(column));
                        }

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
                        if (f[0].PrimaryKeyTableName == f[0].ForeignKeyTableName)
                        {
                            // Self-referencing entity with one to many relationship generates


                            streamWriter.WriteLine("builder.HasOne(t => t.Parent)");
                            streamWriter.WriteLine(".WithMany(t => t." +
                                                   UtilityHelper.MakePlural(f[0].ForeignKeyTableName) + ")");
                            streamWriter.WriteLine(".HasForeignKey(d => d." + f[0].Name + ")");
                            streamWriter.WriteLine(".HasConstraintName(" + @"""FK_" + f[0].ForeignKeyTableName + "_" +
                                                   f[0].PrimaryKeyTableName + @""");");
                        }
                        else
                        {
                            streamWriter.WriteLine("builder.HasOne(t => t." +
                                                   UtilityHelper.MakeSingular(f[0].PrimaryKeyTableName) + ")");
                            streamWriter.WriteLine(".WithMany(t => t." +
                                                   UtilityHelper.MakePlural(f[0].ForeignKeyTableName) + ")");
                            streamWriter.WriteLine(".HasForeignKey(d => d." + f[0].Name + ")");
                            streamWriter.WriteLine(".HasConstraintName(" + @"""FK_" + f[0].ForeignKeyTableName + "_" +
                                                   f[0].PrimaryKeyTableName + @""");");
                        }

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

            #region Initializer

            #region IDbInitializer

            using (
                  StreamWriter streamWriter =
                      new StreamWriter(Path.Combine(InfraDataInitializerPath, "IDbInitializer.cs")))
            {

                streamWriter.WriteLine(@"
namespace " + DataAccessNameSpace + @".Data.Initializer {
   public interface IDbInitializer
    {
        void Initialize();
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

                streamWriter.WriteLine(@"
using System;
using " + DomainNameSpace + @".Entities;
using Microsoft.AspNetCore.Identity;

namespace " + DataAccessNameSpace + @".Data.Initializer
    {
        public class DbInitializer : IDbInitializer
        {
            private readonly ApplicationDbContext _db;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole<" + UtilityHelper.GetIDKeyType(_appSetting) + @">> _roleManager;

            public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<" + UtilityHelper.GetIDKeyType(_appSetting) + @">> roleManager)
            {
                _db = db;
                _roleManager = roleManager;
                _userManager = userManager;
            }

            public async void Initialize()
            {
                //try
                //{
                //    if (_db.Database.GetPendingMigrations().Count() > 0)
                //    {
                //        _db.Database.Migrate();
                //    }
                //}
                //catch (Exception ex)
                //{

                //}

                AppClaimsInitializer.AppClaimsAsync(_db);
                UserInitializer.AddUser(_db, _userManager, _roleManager);

            }
        }
    }

");

            }

            #endregion

            #region AppClaims Initializer



            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "AppClaimsInitializer.cs")))
            {

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
                    streamWriter.WriteLine(claimName + ".Id = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + ".DisplayName = \"" + UtilityHelper.FixName(table.Name) + "\";");
                    streamWriter.WriteLine(claimName + ".ClaimTitle =\"" + table.Name + "\";");
                    streamWriter.WriteLine(claimName + ".ParentId = null;");
                    streamWriter.WriteLine(claimName + ".UrlLink = null;");
                    streamWriter.WriteLine(claimName + ".DisplayOrder = 1; ");
                    streamWriter.WriteLine(claimName + ".IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(" + claimName + ");");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "List = new AppClaim();");
                    streamWriter.WriteLine(claimName + "List.Id = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "List.DisplayName = \"List\";");
                    streamWriter.WriteLine(claimName + "List.ClaimTitle =\"Permissions." + table.Name + ".List\";");
                    streamWriter.WriteLine(claimName + "List.ParentId = claim" + table.Name + ".Id;");
                    streamWriter.WriteLine(claimName + "List.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "List.DisplayOrder = 1; ");
                    streamWriter.WriteLine(claimName + "List.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "List);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "View = new AppClaim();");
                    streamWriter.WriteLine(claimName + "View.Id = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "View.DisplayName =  \"View\";");
                    streamWriter.WriteLine(claimName + "View.ClaimTitle =\"Permissions." + table.Name + ".View\";");
                    streamWriter.WriteLine(claimName + "View.ParentId = claim" + table.Name + ".Id;");
                    streamWriter.WriteLine(claimName + "View.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "View.DisplayOrder = 2; ");
                    streamWriter.WriteLine(claimName + "View.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "View);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "Create = new AppClaim();");
                    streamWriter.WriteLine(claimName + "Create.Id = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "Create.DisplayName =  \"Create\";");
                    streamWriter.WriteLine(claimName + "Create.ClaimTitle =\"Permissions." + table.Name + ".Create\";");
                    streamWriter.WriteLine(claimName + "Create.ParentId = claim" + table.Name + ".Id;");
                    streamWriter.WriteLine(claimName + "Create.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "Create.DisplayOrder = 3; ");
                    streamWriter.WriteLine(claimName + "Create.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "Create);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "Edit = new AppClaim();");
                    streamWriter.WriteLine(claimName + "Edit.Id = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "Edit.DisplayName = \"Edit\";");
                    streamWriter.WriteLine(claimName + "Edit.ClaimTitle =\"Permissions." + table.Name + ".Edit\";");
                    streamWriter.WriteLine(claimName + "Edit.ParentId = claim" + table.Name + ".Id;");
                    streamWriter.WriteLine(claimName + "Edit.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "Edit.DisplayOrder = 4; ");
                    streamWriter.WriteLine(claimName + "Edit.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "Edit);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("AppClaim " + claimName + "Delete = new AppClaim();");
                    streamWriter.WriteLine(claimName + "Delete.Id = Guid.NewGuid();");
                    streamWriter.WriteLine(claimName + "Delete.DisplayName = \"Delete\";");
                    streamWriter.WriteLine(claimName + "Delete.ClaimTitle =\"Permissions." + table.Name + ".Delete\";");
                    streamWriter.WriteLine(claimName + "Delete.ParentId = claim" + table.Name + ".Id;");
                    streamWriter.WriteLine(claimName + "Delete.UrlLink = null;");
                    streamWriter.WriteLine(claimName + "Delete.DisplayOrder = 5; ");
                    streamWriter.WriteLine(claimName + "Delete.IsActive = true;");
                    streamWriter.WriteLine("appClaimsList.Add(claim" + table.Name + "Delete);");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("#endregion");
                    streamWriter.WriteLine("");
                }

                streamWriter.WriteLine("");
                streamWriter.WriteLine(@" List<AppClaim> existsAppClaims = new List<AppClaim>();
           
            existsAppClaims = db.AppClaims.ToListAsync().GetAwaiter().GetResult();

            foreach (var apc in appClaimsList)
            {
                if (!existsAppClaims.Any(u => u.ClaimTitle == apc.ClaimTitle))
                {
                    db.AppClaims.AddAsync(apc).GetAwaiter().GetResult();
                }
            }

            db.SaveChangesAsync().GetAwaiter().GetResult();");



                streamWriter.WriteLine(@" }

    }
}

");

            }


            #endregion
            #region UserInitializer

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataInitializerPath, "UserInitializer.cs")))
            {

                streamWriter.WriteLine(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using " + DomainNameSpace + @".Entities;


namespace " + DataAccessNameSpace + @".Data.Initializer
    {
       public class UserInitializer
    {
        public static void AddUser(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!db.Roles.Any(r => r.Name == ""Admin""))
            {
                roleManager.CreateAsync(new IdentityRole<Guid>(""Admin"")).GetAwaiter().GetResult();
            }

                if (!db.Roles.Any(r => r.Name == ""User""))
                {
                    roleManager.CreateAsync(new IdentityRole<Guid>(""User"")).GetAwaiter().GetResult();
                }


               if (!db.ApplicationUsers.Any(u => u.Email == ""admin@gmail.com""))
               {
                   userManager.CreateAsync(new ApplicationUser
                   {
                       UserName = ""Belal"",
                       Email = ""admin@gmail.com"",
                       EmailConfirmed = true,
                       FullName = ""Belal Badawy"",


                   }, ""Admin123$"").GetAwaiter().GetResult();
 }

                   ApplicationUser user = db.ApplicationUsers.Where(u => u.Email == ""admin@gmail.com"").FirstOrDefault();

                   var newRoleName = ""Admin"";

                    var existRole = roleManager.FindByNameAsync(newRoleName).GetAwaiter().GetResult();
            if (existRole == null)
            {
                roleManager.CreateAsync(new IdentityRole<Guid>(newRoleName)).GetAwaiter().GetResult();
            }

            var newRole = (existRole != null ? existRole : roleManager.FindByNameAsync(newRoleName).GetAwaiter().GetResult());

            if (newRole != null)
            {
                List<AppClaim> existsAppClaims = new List<AppClaim>();

                existsAppClaims = db.AppClaims.ToListAsync().GetAwaiter().GetResult();

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

userManager.AddToRoleAsync(user, newRoleName).GetAwaiter().GetResult();
}

}
}

");

            }

            #endregion

            #endregion

            #region Repositories

            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";

                #region  Model Classes
                className = UtilityHelper.MakeSingular(table.Name);

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(InfraRepositoriesPath, className + "RepositoryAsync.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Entities;
using " + ApplicationNameSpace + @".Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
namespace " + DataAccessNameSpace + @".Data.Repositories
{
     public class " + className + @"RepositoryAsync : Repository<" + className + @">, I" + className + @"RepositoryAsync
    {
        public " + className + @"RepositoryAsync(DbContext context) : base(context)
        {

        }
    }

}
                    ");



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

using " + ApplicationNameSpace + @".Interfaces;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Interfaces;
using " + DataAccessNameSpace + @".Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Data
{
 public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<" + UtilityHelper.GetIDKeyType(_appSetting) + @">, " + UtilityHelper.GetIDKeyType(_appSetting) + @">
    {
        private readonly IDateTimeService _dateTime;

        private IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            IDateTimeService dateTime
        ) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _httpContextAccessor = httpContextAccessor;

        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

");

                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("public virtual DbSet<" + UtilityHelper.MakeSingular(table.Name) + "> " + UtilityHelper.MakePlural(table.Name) + " { get; set; }");
                }

                streamWriter.WriteLine(" protected override void OnModelCreating(ModelBuilder modelBuilder){");

                streamWriter.WriteLine("#region Entities Configuration");

                foreach (Table table in tableList)
                {

                    if (table.Columns.Any(o => o.Name.ToUpper() == "RowVersion".ToUpper()))
                    {
                        streamWriter.WriteLine("modelBuilder.Entity<" + UtilityHelper.MakeSingular(table.Name) + ">().Property(p => p.RowVersion).IsRowVersion();");
                    }

                    streamWriter.WriteLine(" // modelBuilder.ApplyConfiguration(new " + table.Name + "Configuration());");
                }

                streamWriter.WriteLine("#endregion");


                streamWriter.WriteLine("modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());");


                streamWriter.WriteLine(@" foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }");

                streamWriter.WriteLine("base.OnModelCreating(modelBuilder);");
                streamWriter.WriteLine("}");

                streamWriter.WriteLine(@"public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

           // string userId = _httpContextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            string userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Sid);

            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = _dateTime.NowUtc;
                        entry.Entity.CreatedBy = (string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId));
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = (string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId));
                        break;
                    case EntityState.Deleted:
                        entry.Entity.LastModifiedAt = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = (string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId));
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.SoftDeleted = true;
                        break;
                }
            }

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var item in ex.Entries)
                {
                    if (item.Entity is IDataConcurrency)
                    {
                        var currentValues = item.CurrentValues;
                        var dbValues = item.GetDatabaseValues();

                        foreach (var prop in currentValues.Properties)
                        {
                            var currentValue = currentValues[prop];
                            var dbValue = dbValues[prop];
                        }

                        // Refresh the original values to bypass next concurrency check
                        item.OriginalValues.SetValues(dbValues);
                    }
                    else
                    {
                        throw new ApplicationException(""Don’t know handling of concurrency conflict "" + item.Metadata.Name);
                    }
        }
    }
            catch (DbUpdateException e)
            {
                //This either returns a error string, or null if it can’t handle that error
                var sqlException = e.GetBaseException();
                if (sqlException != null)
                {
                    throw new ApplicationException(sqlException.Message, sqlException.InnerException); //return the error string
}
throw new ApplicationException(""couldn’t handle that error""); //return the error string
                //couldn’t handle that error, so rethrow
            }

            return 0;
        }");



                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");



            }


            #endregion

            #region Repository

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataFolderPath, "Repository.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + ApplicationNameSpace + @".Interfaces;
using " + DomainNameSpace + @".Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Data
{
 ");

                streamWriter.WriteLine(@" public class Repository<T> : IGenericRepositoryAsync<T> where T : class
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<T> DbSet;

        public Repository(DbContext context)
        {
            DbContext = context ?? throw new ArgumentException(nameof(context));
            DbSet = DbContext.Set<T>();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await DbSet.CountAsync(predicate);
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet;

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null ? (await orderBy(query).FirstOrDefaultAsync()) : (await query.FirstOrDefaultAsync());
        }

        public virtual async Task<T> GetAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet;
            query = query.AsNoTracking();

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
        }

        public async Task<PagedResult<T>> GetPagedListAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int pageIndex = 0, int pageSize = 10, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet;
            query = query.AsNoTracking();

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            if (predicate != null) query = query.Where(predicate);

            //  return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();

            var pagedResult = new PagedResult<T>();
            pagedResult.TotalCount = await DbSet.CountAsync();
            pagedResult.FilteredTotalCount = await query.CountAsync();
            pagedResult.Data = (orderBy != null ? await orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync() : await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());

            return pagedResult;
        }


        public async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public async Task UpdateAsync(T entity)
        {
            DbSet.Update(entity);
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            DbSet.UpdateRange(entities);
        }

        public async Task DeleteAsync(object id)
        {
            T entityToDelete = await DbSet.FindAsync(id);
            DbSet.Remove(entityToDelete);
        }

        public async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
        }

        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }


        public void Dispose()
        {
            DbContext?.Dispose();
        }


    }
}");




            }


            #endregion

            #region UnitOfWork

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraDataFolderPath, "UnitOfWork.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + ApplicationNameSpace + @".Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Data
{
 ");

                streamWriter.WriteLine(@"   public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private Dictionary<Type, object> _repositories;
        public DbContext _context { get; }
        public UnitOfWork(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Initializes an instance of the repository
        /// </summary>
        /// <typeparam name=""TEntity"">The entity type to initialize with</typeparam>
        /// <returns>An initialized repository</returns>
        public RepositoryType Repository<RepositoryType>() where RepositoryType : class
                {
                    // return (RepositoryType) GetOrAddRepository(typeof(RepositoryType), new RepositoryType(Context));

                    var interfaceType = typeof(RepositoryType);


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

                        return (RepositoryType)_repositories[interfaceType];

                    }

                    return null;
                }



                public async Task<int> CommitAsync()
                {
                    return await _context.SaveChangesAsync();
                }

                /// <summary>
                /// Releases the allocated resources for this context
                /// </summary>
                public void Dispose()
                {
                    _context?.Dispose();
                }

                //internal object GetOrAddRepository(Type type, object repo)
                //{
                //    // Initialize dictionary if it is null
                //    _repositories ??= new Dictionary<(Type type, string Name), object>();

                //    // Pull out the repository if it exists
                //    if (_repositories.TryGetValue((type, repo.GetType().FullName), out var repository)) return repository;

                //    // Add the repository to the dictionary
                //    _repositories.Add((type, repo.GetType().FullName), repo);
                //    return repo;
                //}


            }


        }");




            }


            #endregion

            #endregion

            #region Extensions Classes

            //            #region  EnumerablePagedListExtensions

            //            using (
            //                StreamWriter streamWriter =
            //                    new StreamWriter(Path.Combine(InfraExtensionsPath, "EnumerablePagedListExtensions.cs")))
            //            {
            //                // Create the header for the class
            //                streamWriter.WriteLine(@"
            //using " + DataAccessNameSpace + @".Interfaces.Data;
            //using " + DataAccessNameSpace + @".Repositories;
            //using System;
            //using System.Collections.Generic;

            //namespace " + DataAccessNameSpace + @".Extensions
            //{
            //    /// <summary>
            //    /// Provides some extension methods for <see cref='IEnumerable{T}'/> to provide paging capability.
            //    /// </summary>
            //    public static class EnumerablePagedListExtensions
            //    {
            //        /// <summary>
            //        /// Converts the specified source to <see cref='IPagedList{T}'/> by the specified <paramref name='pageIndex'/> and <paramref name='pageSize'/>.
            //        /// </summary>
            //        /// <typeparam name='T'>The type of the source.</typeparam>
            //        /// <param name='source'>The source to paging.</param>
            //        /// <param name='pageIndex'>The index of the page.</param>
            //        /// <param name='pageSize'>The size of the page.</param>
            //        /// <param name='indexFrom'>The start index value.</param>
            //        /// <returns>An instance of the inherited from <see cref='IPagedList{T}'/> interface.</returns>
            //        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom = 0) => new PagedList<T>(source, pageIndex, pageSize, indexFrom);

            //        /// <summary>
            //        /// Converts the specified source to <see cref='IPagedList{T}'/> by the specified <paramref name='converter'/>, <paramref name='pageIndex'/> and <paramref name='pageSize'/>
            //        /// </summary>
            //        /// <typeparam name='TSource'>The type of the source.</typeparam>
            //        /// <typeparam name='TResult'>The type of the result</typeparam>
            //        /// <param name='source'>The source to convert.</param>
            //        /// <param name='converter'>The converter to change the <typeparamref name='TSource'/> to <typeparamref name='TResult'/>.</param>
            //        /// <param name='pageIndex'>The page index.</param>
            //        /// <param name='pageSize'>The page size.</param>
            //        /// <param name='indexFrom'>The start index value.</param>
            //        /// <returns>An instance of the inherited from <see cref='IPagedList{T}'/> interface.</returns>
            //        public static IPagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom = 0) => new PagedList<TSource, TResult>(source, converter, pageIndex, pageSize, indexFrom);
            //    }
            //}

            //");

            //            }

            //            #endregion

            //            #region QueryablePageListExtensions





            //            using (
            //                StreamWriter streamWriter =
            //                    new StreamWriter(Path.Combine(InfraExtensionsPath, "QueryablePageListExtensions.cs")))
            //            {
            //                // Create the header for the class
            //                streamWriter.WriteLine(@"
            //using " + DataAccessNameSpace + @".Interfaces.Data;
            //using " + DataAccessNameSpace + @".Repositories;
            //using Microsoft.EntityFrameworkCore;
            //using System;
            //using System.Linq;
            //using System.Threading;
            //using System.Threading.Tasks;

            //namespace " + DataAccessNameSpace + @".Extensions
            //{
            //    public static class QueryablePageListExtensions
            //    {
            //        /// <summary>
            //        /// Converts the specified source to <see cref='IPagedList{T}'/> by the specified <paramref name='pageIndex'/> and <paramref name='pageSize'/>.
            //        /// </summary>
            //        /// <typeparam name='T'>The type of the source.</typeparam>
            //        /// <param name='source'>The source to paging.</param>
            //        /// <param name='pageIndex'>The index of the page.</param>
            //        /// <param name='pageSize'>The size of the page.</param>
            //        /// <param name='cancellationToken'>
            //        ///     A <see cref='CancellationToken' /> to observe while waiting for the task to complete.
            //        /// </param>
            //        /// <param name='indexFrom'>The start index value.</param>
            //        /// <returns>An instance of the inherited from <see cref='IPagedList{T}'/> interface.</returns>
            //        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0, CancellationToken cancellationToken = default(CancellationToken))
            //        {
            //            if (indexFrom > pageIndex)
            //            {
            //                throw new ArgumentException($""indexFrom: { indexFrom} > pageIndex: { pageIndex}, must indexFrom <= pageIndex"");
            //            }

            //            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            //            var items = await source.Skip((pageIndex - indexFrom) * pageSize)
            //                .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

            //            var pagedList = new PagedList<T>()
            //            {
            //                PageIndex = pageIndex,
            //                PageSize = pageSize,
            //                IndexFrom = indexFrom,
            //                TotalCount = count,
            //                Items = items,
            //                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            //            };

            //            return pagedList;
            //        }
            //    }
            //}

            //");

            //            }

            //            #endregion

            #region LinqExtensions

            using (
        StreamWriter streamWriter =
        new StreamWriter(Path.Combine(InfraExtensionsPath, "LinqExtensions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Common;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace " + DataAccessNameSpace + @".Extensions
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
    }

");

            }

            #endregion

            #region SoftDeleteQueryExtension

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraExtensionsPath, "SoftDeleteQueryExtension.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Reflection;


namespace " + DataAccessNameSpace + @".Extensions
{
     public static class SoftDeleteQueryExtension
    {
        public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData)
        {
            var methodToCall = typeof(SoftDeleteQueryExtension).GetMethod(nameof(GetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(null, new object[] { });
            entityData.SetQueryFilter((LambdaExpression)filter);
            entityData.AddIndex(entityData.FindProperty(nameof(ISoftDelete.SoftDeleted)));
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.SoftDeleted;
            return filter;
        }
    }
}

");

            }

            #endregion

            #endregion


            #region Identity

            #region AuthService

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraIdentityServicesPath, "AuthService.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using " + ApplicationNameSpace + @".Interfaces;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DomainNameSpace + @".Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace " + DataAccessNameSpace + @".Identity.Services
{
 public class AuthService : IAuthService
    {
        public Guid? UserId => null;


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;


        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null)
            {
                //  throw new Exception($""User with { loginModel.Email } not found."");
                return new Response<AuthenticationResponse>($""User with {loginModel.Email} not found."");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginModel.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new Response<AuthenticationResponse>($""Credentials for '{loginModel.Email} aren't valid'."");
                //throw new Exception($""Credentials for '{loginModel.Email} aren't valid'."");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            AuthenticationResponse response = new AuthenticationResponse
            {
                Id = user.Id.ToString(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName
            };

            return new Response<AuthenticationResponse>(response);
        }

        public async Task<Response<Guid>> RegisterAsync(RegistrationModel request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FullName = request.FullName,
                EmailConfirmed = false,
                UserName = request.Email
            };

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    return new Response<Guid>(user.Id);
                }
                else
                {
                    return new Response<Guid>($""{ string.Join(""; "", result.Errors.Select(o => o.Description).ToList())}"");
                }
            }
            else
            {
                return new Response<Guid>($""Email {request.Email } already exists."");
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var roleClaims = new List<Claim>();

             roleClaims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var userClaims = await _userManager.GetClaimsAsync(user);

            if (userClaims != null && userClaims.Count > 0)
            {
                foreach (var uc in userClaims)
                {
                    roleClaims.Add(new Claim(CustomClaimTypes.Permission, uc.Value));
                }
            }


            var roles = await _userManager.GetRolesAsync(user);


            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim(""roles"", roles[i]));
                var role = await _roleManager.FindByNameAsync(roles[i]);
                if (role != null)
                {
                    var _roleClaims = await _roleManager.GetClaimsAsync(role);
                    if (_roleClaims != null && _roleClaims.Count > 0)
                    {
                        foreach (var c in _roleClaims)
                        {
                            roleClaims.Add(new Claim(CustomClaimTypes.Permission, c.Value));
                        }
                    }
                }
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(""uid"", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                //  expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}

");

            }

            #endregion



            #region PermissionAuthorizationHandler

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraIdentityPath, "PermissionAuthorizationHandler.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"


using " + DomainNameSpace + @".Common;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Identity
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
                                           c.Value.ToUpper() == requirement.Permission.ToUpper()
                                           //  && c.Issuer == ""http://localhost:55445""
                                           ))
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


            #region PermissionChecker

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraIdentityPath, "PermissionChecker.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"

using " + ApplicationNameSpace + @".Interfaces;
using " + DomainNameSpace + @".Common;
using Microsoft.AspNetCore.Http;

namespace " + DataAccessNameSpace + @".Identity
{
   
   public class PermissionChecker : IPermissionChecker
    {
        private readonly IHttpContextAccessor _context;
        public PermissionChecker(IHttpContextAccessor context)
        {
            _context = context;
        }
        #region From Token

        public bool HasClaim(string requiredClaim)
        {
            if (_context.HttpContext.User == null)
            {
                return false;
            }

            // If user does not have the scope claim, get out of here
            if (_context.HttpContext.User.HasClaim(c => c.Type == CustomClaimTypes.Permission &&
                                                        c.Value.ToUpper() == requiredClaim.ToUpper()))
            {

                return true;
            }

            return false;
        }
        #endregion
    }
}

");

            }

            #endregion


            #region PermissionPolicyProvider

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraIdentityPath, "PermissionPolicyProvider.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace " + DataAccessNameSpace + @".Identity
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


            #region PermissionRequirement

            using (
                StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(InfraIdentityPath, "PermissionRequirement.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Authorization;

namespace " + DataAccessNameSpace + @".Identity
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

            #endregion



            //            #region UnitOfWork

            //            using (
            //                StreamWriter streamWriter =
            //                    new StreamWriter(Path.Combine(InfraDataFolderPath, "UnitOfWork.cs")))
            //            {
            //                // Create the header for the class
            //                streamWriter.WriteLine(@"
            //using System;
            //using System.Collections.Generic;
            //using System.Linq;
            //using System.Threading.Tasks;
            //using " + DataAccessNameSpace + @".Interfaces.Data;
            //using " + DataAccessNameSpace + @".Interfaces.Repositories;
            //using " + DataAccessNameSpace + @".Repositories;
            //using Microsoft.EntityFrameworkCore;
            //using Microsoft.EntityFrameworkCore.Storage;

            //namespace " + DataAccessNameSpace + @".Data {
            //  /// <summary>
            //    /// Represents the default implementation of the <see cref='IUnitOfWork'/> and <see cref='IUnitOfWork{TContext}'/> interface.
            //    /// </summary>
            //    /// <typeparam name='TContext'>The type of the db context.</typeparam>
            //    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork where TContext : DbContext
            //    {
            //        private bool _disposed = false;
            //        private Dictionary<Type, object> _repositories;
            //        private readonly TContext _context;

            //        /// <summary>
            //        /// Gets the db context.
            //        /// </summary>
            //        /// <returns>The instance of type <typeparamref name='TContext'/>.</returns>
            //        public TContext DbContext
            //        {
            //            get { return _context; }
            //        }




            //        /// <summary>
            //        /// Initializes a new instance of the <see cref='UnitOfWork{TContext}'/> class.
            //        /// </summary>
            //        /// <param name='context'>The context.</param>
            //        public UnitOfWork(TContext context)
            //        {
            //            _context = context ?? throw new ArgumentNullException(nameof(context));
            //        }




            //         #region SQL
            //        /// <summary>
            //        /// Uses raw SQL queries to fetch the specified <typeparamref name='TEntity' /> data.
            //        /// </summary>
            //        /// <typeparam name='TEntity'>The type of the entity.</typeparam>
            //        /// <param name='sql'>The raw SQL.</param>
            //        /// <param name='parameters'>The parameters.</param>
            //        /// <returns>An <see cref='IQueryable{T}' /> that contains elements that satisfy the condition specified by raw SQL.</returns>
            //        public IQueryable<TEntity> FromSqlRaw<TEntity>(string sql, params object[] parameters) where TEntity : class
            //        {
            //            return _context.Set<TEntity>().FromSqlRaw(sql, parameters);
            //        }

            //        /// <summary>
            //        /// Executes the specified raw SQL command.
            //        /// </summary>
            //        /// <param name='sql'>The raw SQL.</param>
            //        /// <param name='parameters'>The parameters.</param>
            //        /// <returns>The number of state entities written to database.</returns>
            //        public int ExecuteSqlCommand(string sql, params object[] parameters)
            //        {
            //            return _context.Database.ExecuteSqlCommand(sql, parameters);
            //        }

            //        #endregion

            //        #region GET



            //        ///// <summary>
            //        ///// Gets the specified repository for the <typeparamref name='TEntity'/>.
            //        ///// </summary>
            //        ///// <typeparam name='TEntity'>The type of the entity.</typeparam>
            //        ///// <returns>An instance of type inherited from <see cref='IRepository{TEntity}'/> interface.</returns>
            //        //public IRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
            //        //{
            //        //    if (_repositories == null)
            //        //        _repositories = new Dictionary<Type, object>();

            //        //    var type = typeof(TEntity);
            //        //    if (!_repositories.ContainsKey(type))
            //        //        _repositories[type] = new Repository<TEntity>(_context);

            //        //    return (IRepository<TEntity>)_repositories[type];
            //        //}


            //        public  TEntity GetRepository<TEntity>() where TEntity : class
            //        {

            //            var interfaceType = typeof(TEntity);


            //            if (_repositories == null)
            //            {
            //                _repositories = new Dictionary<Type, object>();


            //            }
            //            var type = AppDomain.CurrentDomain.GetAssemblies()
            //                .SelectMany(s => s.GetTypes())
            //                .Where(x => x.GetInterface(interfaceType.Name) != null).FirstOrDefault();

            //            if (type != null)
            //            {
            //                if (!_repositories.ContainsKey(interfaceType))
            //                {
            //                    _repositories[interfaceType] = Activator.CreateInstance(type, _context);
            //                }

            //                return (TEntity) _repositories[interfaceType];

            //            }


            //            return null;
            //        }


            //        #endregion

            //        #region SAVE
            //        /// <inheritdoc />
            //        /// <summary>
            //        /// Saves all changes made in this context to the database.
            //        /// </summary>
            //        /// <param name='ensureAutoHistory'><c>True</c> if save changes ensure auto record the change history.</param>
            //        /// <returns>The number of state entries written to the database.</returns>
            //        public int SaveChanges()
            //        {

            //            return _context.SaveChanges();


            //        }

            //        /// <summary>
            //        /// Asynchronously saves all changes made in this unit of work to the database.
            //        /// </summary>
            //        /// <param name='ensureAutoHistory'><c>True</c> if save changes ensure auto record the change history.</param>
            //        /// <returns>A <see cref='Task{TResult}'/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
            //        public async Task<int> SaveChangesAsync()
            //        {

            //            return await _context.SaveChangesAsync();


            //        }

            //        #endregion

            //        #region Dispose
            //        /// <summary>
            //        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            //        /// </summary>
            //        public void Dispose()
            //        {
            //            Dispose(true);
            //            GC.SuppressFinalize(this);
            //        }

            //        /// <summary>
            //        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            //        /// </summary>
            //        /// <param name='disposing'>The disposing.</param>
            //        protected virtual void Dispose(bool disposing)
            //        {
            //            if (!_disposed && disposing)
            //            {   // clear repositories
            //                _repositories?.Clear();

            //                // dispose the db context.
            //                _context.Dispose();
            //            }

            //            _disposed = true;
            //        }
            //        #endregion
            //    }
            //}

            //");




            //            }


            //            #endregion


            #region Infrastructure Dependency Injection



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DataPath, "DependencyInjection.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"

using " + ApplicationNameSpace + @".Interfaces;
using " + ApplicationNameSpace + @".Interfaces.Repositories;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using " + DataAccessNameSpace + @".Common;
using " + DataAccessNameSpace + @".Data;
using " + DataAccessNameSpace + @".Data.Repositories;
using " + DataAccessNameSpace + @".Identity;
using " + DataAccessNameSpace + @".Identity.Services;
using " + DataAccessNameSpace + @".Data.Initializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
           
          services.AddScoped<DbContext, ApplicationDbContext>();

            services.Configure<JwtSettings>(configuration.GetSection(""JwtSettings""));


            services.AddDbContext<ApplicationDbContext>(options =>

                options.UseLoggerFactory(_myLoggerFactory).UseSqlServer(
                    configuration.GetConnectionString(""DefaultConnection""),
                serverOptions =>
                {
                    serverOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                }), ServiceLifetime.Scoped);



                services.AddIdentity<ApplicationUser, IdentityRole<" + UtilityHelper.GetIDKeyType(_appSetting) + @">>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

");

                foreach (Table table in tableList)
                {
                    streamWriter.WriteLine("services.AddScoped<I" + UtilityHelper.MakeSingular(table.Name) + "RepositoryAsync, " + UtilityHelper.MakeSingular(table.Name) + "RepositoryAsync>();");
                }

                streamWriter.WriteLine(@"
                services.AddTransient<UserManager<ApplicationUser>>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddTransient<IAuthService, AuthService>();
                services.AddTransient<IDateTimeService, DateTimeService>();
                services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
                services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
                services.AddScoped<IPermissionChecker, PermissionChecker>();
                services.AddScoped<IDbInitializer, DbInitializer>();
                services.AddSingleton<ISessionWrapper, InMemorySessionWrapper>();

                services.Configure<IdentityOptions>(opt =>
                {
                    opt.Password.RequiredLength = 5;
                    opt.Password.RequireLowercase = true;
                    opt.Password.RequireNonAlphanumeric = true;
                    opt.Password.RequiredUniqueChars = 1;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    opt.Lockout.MaxFailedAccessAttempts = 3;
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedAccount = true;
                    opt.SignIn.RequireConfirmedEmail = true;
                    opt.SignIn.RequireConfirmedPhoneNumber = false;

                });


              

                services.AddAuthentication(options =>
                {
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }) .AddCookie(options =>
                    {
                        options.Cookie.HttpOnly = true;
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                        options.LoginPath = ""/Account/Login"";
                        options.AccessDeniedPath = ""/Account/AccessDenied"";
                        options.SlidingExpiration = true;
            })
                   .AddJwtBearer(o =>
                   {
                       o.RequireHttpsMetadata = false;
                       o.SaveToken = false;
                       o.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ClockSkew = TimeSpan.Zero,
                           ValidIssuer = configuration[""JwtSettings:Issuer""],
                           ValidAudience = configuration[""JwtSettings:Audience""],
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[""JwtSettings:Key""]))
                       };

                       o.Events = new JwtBearerEvents()
                       {
                           OnAuthenticationFailed = context =>
                           {
                               context.Response.OnStarting(async () =>
                               {
                                   context.NoResult();
                                   context.Response.Headers.Add(""Token-Expired"", ""true"");
                                   context.Response.ContentType = ""text/plain"";
                                   context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                   await context.Response.WriteAsync(""Un-Authorized"");
                               });

                               return Task.CompletedTask;
                           },
                           OnChallenge = context =>
                           {
                                context.HandleResponse();
                               context.Response.StatusCode = 401;
                               context.Response.ContentType = ""application/json"";
                           // Ensure we always have an error and error description.
                           if (string.IsNullOrEmpty(context.Error))
                                   context.Error = ""invalid_token"";
                               if (string.IsNullOrEmpty(context.ErrorDescription))
                                   context.ErrorDescription = ""This request requires a valid JWT access token to be provided"";


                               var result = JsonConvert.SerializeObject(""401 Not authorized"");
                                context.Response.WriteAsync(result).Wait(); 
                               return Task.CompletedTask;
                           },
                           OnForbidden = context =>
                           {
                               context.Response.StatusCode = 403;
                               context.Response.ContentType = ""application/json"";
                               var result = JsonConvert.SerializeObject(""403 Not authorized"");
                               return context.Response.WriteAsync(result);
                           },
                       };
                   });

                





                return services;
            }
        }
    }"
                );









            }
            #endregion



            #region EF Code



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(DataPath, "efCode.txt")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"
dotnet tool update --global dotnet-ef

dotnet ef database drop --project ""BS.Infrastructure"" --startup-project ""BS.API""
                dotnet ef database update
                dotnet ef migrations add InitialMigration--project ""BS.Infrastructure""--startup - project ""BS.API""
                dotnet ef database update--project ""BS.Infrastructure"" --startup - project ""BS.API"" --verbose

");









            }
            #endregion
        }
        private static void CreateApplicationClasses()
        {


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




            #region Exceptions Classes

            #region BadRequestException

            using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ApplicationExceptionsPath, "BadRequestException.cs")))
            {

                streamWriter.WriteLine(@"using System;

namespace " + ApplicationNameSpace + @".Exceptions
{
    public class BadRequestException: ApplicationException
    {
        public BadRequestException(string message): base(message)
        {

        }
    }
}
");
            }

            #endregion

            #region NotFoundException

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationExceptionsPath, "NotFoundException.cs")))
            {
                streamWriter.WriteLine(@"using System;

namespace " + ApplicationNameSpace + @".Exceptions
{
    public class NotFoundException: ApplicationException
    {
         public NotFoundException(string name, object key) : base($""{name} ({key}) is not found"")
        {

        }
    }
}
");

            }

            #endregion

            #region ValidationException

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationExceptionsPath, "ValidationException.cs")))
            {
                streamWriter.WriteLine(@"using System;
using System.Collections.Generic;
using FluentValidation.Results;
    
namespace " + ApplicationNameSpace + @".Exceptions
{
    public class ValidationException : ApplicationException
    {
        public List<string> ValidationErrors { get; set; }

        public ValidationException(ValidationResult validationResult)
        {
            ValidationErrors = new List<string>();

            foreach (var validationError in validationResult.Errors)
            {
                ValidationErrors.Add(validationError.ErrorMessage);
            }
        }
    }
}
");

            }

            #endregion

            #endregion


            #region Interfaces Classes

            #region IAuthService

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationInterfacesPath, "IAuthService.cs")))
            {

                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Models;
using System;
using System.Threading.Tasks;

namespace " + ApplicationNameSpace + @".Interfaces
{
    public interface IAuthService
    {
        Guid? UserId { get; }

        Task<Response<AuthenticationResponse>> AuthenticateAsync(LoginModel loginModel);
        Task<Response<Guid>> RegisterAsync(RegistrationModel registrationModel);
    }
}
");
            }

            #endregion

            #region IDateTimeService

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationInterfacesPath, "IDateTimeService.cs")))
            {

                streamWriter.WriteLine(@"
using System;
namespace " + ApplicationNameSpace + @".Interfaces
{
     public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}
");
            }

            #endregion

            #region IGenericRepositoryAsync

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationInterfacesPath, "IGenericRepositoryAsync.cs")))
            {

                streamWriter.WriteLine(@"
using " + DomainNameSpace + @".Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace " + ApplicationNameSpace + @".Interfaces
{
     public interface IGenericRepositoryAsync<T> where T : class
    {
        #region Async

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes);

        Task<PagedResult<T>> GetPagedListAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int pageIndex = 0, int pageSize = 10, params Expression<Func<T, object>>[] includes);

        Task<T> GetAsync(object id);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes);

        #region Add Functions

        Task<T> AddAsync(T entity);

        Task AddAsync(IEnumerable<T> entities);

        #endregion

        #region Update Functions

        Task UpdateAsync(T entity);
        Task UpdateAsync(IEnumerable<T> entities);

        #endregion

        #region Delete Functions
        Task DeleteAsync(object id);
        Task DeleteAsync(T entity);
        Task DeleteAsync(IEnumerable<T> entities);

        #endregion
        #endregion

    }
}
");
            }

            #endregion

            #region IPermissionChecker

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationInterfacesPath, "IPermissionChecker.cs")))
            {

                streamWriter.WriteLine(@"
using System;
namespace " + ApplicationNameSpace + @".Interfaces
{
   public interface IPermissionChecker
    {
        bool HasClaim(string requiredClaim);
    }
}
");
            }

            #endregion

            #region IUnitOfWork

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationInterfacesPath, "IUnitOfWork.cs")))
            {

                streamWriter.WriteLine(@"
using System;
using System.Threading.Tasks;
namespace " + ApplicationNameSpace + @".Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        RepositoryType Repository<RepositoryType>() where RepositoryType : class;
        Task<int> CommitAsync();
    }

}
");
            }

            #endregion


            #region ISessionWrapper

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApplicationInterfacesPath, "ISessionWrapper.cs")))
            {

                streamWriter.WriteLine(@"
using System;
namespace " + ApplicationNameSpace + @".Interfaces
{
    public interface ISessionWrapper
    {

        public T GetFromSession<T>(string key);
        public void SetInSession<T>(string key, T value);
        public void RemoveFromSession(string key);
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

                className = UtilityHelper.MakeSingular(table.Name);

                #region Repositories Classes

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationInterfacesRepositoriesPath,
                    "I" + className + "RepositoryAsync.cs")))
                {
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Entities;");
                    streamWriter.WriteLine("namespace " + ApplicationNameSpace + ".Interfaces.Repositories");
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("\tpublic interface " + "I" + className +
                                           "RepositoryAsync : IGenericRepositoryAsync<" + className + ">");
                    streamWriter.WriteLine("\t{");
                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                }


                #endregion

                #region Dtos Classes


                #region ReadDto

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ApplicationDtosPath, className + "ReadDto.cs")))
                {

                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using System.Collections.Generic;");
                    streamWriter.WriteLine("namespace " + ApplicationNameSpace + ".Dtos");
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("\tpublic class " + className + "ReadDto");
                    streamWriter.WriteLine("\t{");

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];
                        string parameter = UtilityHelper.CreateMethodParameter(column);
                        string type = parameter.Split(' ')[0];
                        string name = parameter.Split(' ')[1];
                        streamWriter.WriteLine("\t\tpublic " + type + " " + UtilityHelper.FormatPascal(name) +
                                               " { get; set; }");
                    }

                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                }

                #endregion

                #region UpsertDto

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ApplicationDtosPath, className + "UpsertDto.cs")))
                {
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using System.Collections.Generic;");
                    streamWriter.WriteLine("namespace " + ApplicationNameSpace + ".Dtos");
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("\tpublic class " + className + "UpsertDto");
                    streamWriter.WriteLine("\t{");

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];
                        string parameter = UtilityHelper.CreateMethodParameter(column);
                        string type = parameter.Split(' ')[0];
                        string name = parameter.Split(' ')[1];
                        if (
                            name.ToLower() != "CreatedBy".ToLower()
                            && name.ToLower() != "CreatedAt".ToLower()
                            && name.ToLower() != "LastModifiedBy".ToLower()
                            && name.ToLower() != "LastModifiedAt".ToLower()
                            && name.ToLower() != "SoftDeleted".ToLower()
                            && name.ToLower() != "RowVersion".ToLower()

                        )
                        {
                            streamWriter.WriteLine("\t\tpublic " + type + " " + UtilityHelper.FormatPascal(name) +
                                                   " { get; set; }");
                        }
                    }

                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                }

                #endregion



                #endregion

                #region Validations Classes




                sb.Clear();
                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ApplicationValidationsPath, className + "UpsertDtoValidator.cs")))
                {
                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using FluentValidation;");
                    streamWriter.WriteLine("using " + ApplicationNameSpace + ".Dtos;");
                    streamWriter.WriteLine("namespace " + ApplicationNameSpace + ".Validations");
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("\tpublic class " + className + "UpsertDtoValidator : AbstractValidator<" +
                                           className + "UpsertDto> ");
                    streamWriter.WriteLine("\t{");

                    streamWriter.WriteLine("\tpublic " + className + "UpsertDtoValidator()");
                    streamWriter.WriteLine("{");

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];
                        string parameter = UtilityHelper.CreateMethodParameter(column);
                        string type = parameter.Split(' ')[0];
                        string name = parameter.Split(' ')[1];
                        if (
                            name.ToLower() != "CreatedBy".ToLower()
                            && name.ToLower() != "CreatedAt".ToLower()
                            && name.ToLower() != "LastModifiedBy".ToLower()
                            && name.ToLower() != "LastModifiedAt".ToLower()
                            && name.ToLower() != "SoftDeleted".ToLower()
                            && name.ToLower() != "RowVersion".ToLower()

                        )
                        {
                            streamWriter.WriteLine("\t\t" + UtilityHelper.CreateFluentValidationRules("DomainResource",
                                table.Name,
                                column, table.ForeignKeys, useResourceFile));
                        }
                    }



                    streamWriter.WriteLine("}");

                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                }

                //var validatorNameCreateDto = classNameCreateDto;
                ////   var validatorNameUpdateDto = classNameUpdateDto;

                //classNameCreateDto = classNameCreateDto + "Validator";
                ////  classNameUpdateDto = classNameUpdateDto + "Validator";

                //using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationValidationsPath, classNameCreateDto + ".cs")))
                //{
                //    string clsName = sb.ToString().Replace("#className#", classNameCreateDto).Replace("#validatorName#", validatorNameCreateDto).ToString();
                //    streamWriter.WriteLine(clsName);

                //}


                //using (StreamWriter streamWriter = new StreamWriter(Path.Combine(UtilityHelper.ApplicationValidationsPath, classNameUpdateDto + ".cs")))
                //{
                //    string clsName = sb.ToString().Replace("#className#", classNameUpdateDto).Replace("#validatorName#", validatorNameUpdateDto).ToString();
                //    streamWriter.WriteLine(clsName);


                //}

                #endregion

                #region Mapping Classes

                using (StreamWriter streamWriter =
                    new StreamWriter(Path.Combine(ApplicationMappingPath, className + "Profile.cs")))
                {

                    streamWriter.WriteLine("using System;");
                    streamWriter.WriteLine("using AutoMapper;");
                    streamWriter.WriteLine("using " + ApplicationNameSpace + ".Dtos;");
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Entities;");
                    streamWriter.WriteLine("namespace " + ApplicationNameSpace + ".Mapping");
                    streamWriter.WriteLine("{");
                    streamWriter.WriteLine("\tpublic class " + className + "Profile : AutoMapper.Profile");
                    streamWriter.WriteLine("\t{");

                    streamWriter.WriteLine("\t\tpublic " + className + "Profile() {");
                    streamWriter.WriteLine("CreateMap<" + className + ", " + className + "ReadDto>().ReverseMap();");
                    streamWriter.WriteLine("CreateMap<" + className + ", " + className + "UpsertDto>().ReverseMap();");
                    streamWriter.WriteLine("CreateMap<" + className + "ReadDto, " + className +
                                           "UpsertDto>().ReverseMap();");
                    //  streamWriter.WriteLine("CreateMap<" + className + ", " + className + "UpsertDto>().ReverseMap();");
                    streamWriter.WriteLine("\t}");
                    streamWriter.WriteLine("}");
                    streamWriter.WriteLine("}");

                }



                #endregion

                #region Appliucation Service Interface

                string keyType = "";

                var pk = table.PrimaryKeys.FirstOrDefault();
                if (pk != null)
                {
                    keyType = UtilityHelper.GetCsType(pk);
                }

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationServicesInterfacesPath,
                    "I" + className + "Service.cs")))
                {
                    // Create the header for the class
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Common;");
                    streamWriter.WriteLine("using " + DomainNameSpace + ".Entities;");
                    streamWriter.WriteLine("using " + ApplicationNameSpace + ".Dtos;");
                    streamWriter.WriteLine(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace " + ApplicationNameSpace + @".Services.Interfaces
{
   public partial interface I" + className + @"Service 
    {
        Task<Response<" + keyType + @">> AddAsync(" + className + @"UpsertDto " +
                                           UtilityHelper.FormatCamel(className) + @"UpsertDto);
        Task<Response<bool>> UpdateAsync(" + className + @"UpsertDto " + UtilityHelper.FormatCamel(className) +
                                           @"UpsertDto);
        Task<Response<bool>> DeleteAsync(" + keyType + @" id);
        Task<Response<" + className + @"ReadDto>> GetByIdAsync(" + keyType + @" id);
        Task<Response<List<" + className + @"ReadDto>>> GetAllAsync(Expression<Func<" + className +
                                           @", bool>> predicate = null,
            Func<IQueryable<" + className + @">, IOrderedQueryable<" + className +
                                           @">> orderBy = null, params Expression<Func<" + className +
                                           @", object>>[] includes);

        Task<Response<PagedResult<" + className + @"ReadDto>>> GetPagedListAsync(Expression<Func<" + className +
                                           @", bool>> predicate = null,
            Func<IQueryable<" + className + @">, IOrderedQueryable<" + className +
                                           @">> orderBy = null, int pageIndex = 0,
            int pageSize = 10, params Expression<Func<" + className + @", object>>[] includes);

    }
}"
                    );









                }

                #endregion

                #region Application Service Impelemtation

                var titleName = UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table);

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationServicesImplementationsPath,
                    className + "Service.cs")))
                {
                    // Create the header for the class


                    streamWriter.WriteLine(@"
using AutoMapper;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Interfaces;
using " + ApplicationNameSpace + @".Interfaces.Repositories;
using " + ApplicationNameSpace + @".Services.Interfaces;
using " + ApplicationNameSpace + @".Validations;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Entities;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace " + ApplicationNameSpace + @".Services.Implementations
{
    public partial class " + className + @"Service : I" + className + @"Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPermissionChecker _permissionChecker;

        public " + className + @"Service(IUnitOfWork unitOfWork, IMapper mapper, IPermissionChecker permissionChecker)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _permissionChecker = permissionChecker;
        }
");

                    #region Add




                    streamWriter.WriteLine(@"  public async Task<Response<" + keyType + @">> AddAsync(" + className +
                                           @"UpsertDto  " + UtilityHelper.FormatCamel(className) + @"UpsertDto)
        {
            if (_permissionChecker.HasClaim(AppPermissions." + className + @".Create))
            {
                        " + className + @"UpsertDtoValidator dtoValidator = new " + className + @"UpsertDtoValidator();

                        ValidationResult validationResult = dtoValidator.Validate(" +
                                           UtilityHelper.FormatCamel(className) + @"UpsertDto);

                        if (validationResult != null && validationResult.IsValid == false)
                        {
                            return new Response<" + keyType +
                                           @">(validationResult.Errors.Select(modelError => modelError.ErrorMessage).ToList());
                        }
                        else
                        {
");

                    var hasCodeColumn = table.Columns.Any(o => o.Name.ToUpper() == "CODE");

                    if (!string.IsNullOrEmpty(titleName) && hasCodeColumn == false)
                    {
                        streamWriter.WriteLine(@"

                            if (await _unitOfWork.Repository<I" + className + @"RepositoryAsync>()
                                .AnyAsync(o => o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               ".ToUpper() == " +
                                               UtilityHelper.FormatCamel(className) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               @".ToUpper()))
                            {
                                return new Response<" + keyType + @">(string.Format(SD.ExistData, " +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @"));
                            }

");
                    }


                    if (!string.IsNullOrEmpty(titleName) && hasCodeColumn)
                    {
                        streamWriter.WriteLine(@"

                            var exitRecord = await _unitOfWork.Repository<I" + className + @"RepositoryAsync>()
                                .FirstOrDefaultAsync(o => o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               ".ToUpper() == " +
                                               UtilityHelper.FormatCamel(className) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               @".ToUpper() ||
                                       o.Code.ToUpper() == " + UtilityHelper.FormatCamel(className) + "UpsertDto." +
                                               "Code.ToUpper()) ;");

                        streamWriter.WriteLine(@"
 if (exitRecord != null)
                    {
            if (exitRecord.Code.ToUpper() == " + UtilityHelper.FormatCamel(className) + "UpsertDto." + @"Code.ToUpper())
                        {
                            return new Response<Guid>(string.Format(SD.ExistData, " + UtilityHelper.FormatCamel(className) + "UpsertDto." + @"Code));
                        }

 if (exitRecord." + UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + ".ToUpper() == " + UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                        UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @".ToUpper())
                        {
                                return new Response<" + keyType + @">(string.Format(SD.ExistData, " +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @"));
}
    }                       
");

                    }

                    streamWriter.WriteLine(className + @" " + UtilityHelper.FormatCamel(className) +
                                           @" = _mapper.Map<" +
                                           className + @">(" + UtilityHelper.FormatCamel(className) + @"UpsertDto);

                                var addedEntity = await _unitOfWork.Repository<I" + className + @"RepositoryAsync>()
                                    .AddAsync(" + UtilityHelper.FormatCamel(className) + @");

                                int effectedRows = await _unitOfWork.CommitAsync();
                                if (effectedRows != 0)
                                {
                                    return new Response<" + keyType + @">(addedEntity.Id);
                                }
                          
                        }

                        return new Response<" + keyType + @">(SD.ErrorOccurred);
                    }

                    return new Response<" + keyType + @">(""not authorized"");
                }
");

                    #endregion

                    #region Update

                    streamWriter.WriteLine(@"public async Task<Response<bool>> UpdateAsync(" + className +
                                           @"UpsertDto " + UtilityHelper.FormatCamel(className) + @"UpsertDto)
                {
                    if (_permissionChecker.HasClaim(AppPermissions." + className + @".Edit))
                    {
                        " + className + @"UpsertDtoValidator dtoValidator = new " + className + @"UpsertDtoValidator();

                        ValidationResult validationResult = dtoValidator.Validate(" +
                                           UtilityHelper.FormatCamel(className) + @"UpsertDto);

                        if (validationResult != null && validationResult.IsValid == false)
                        {
                            return new Response<bool>(validationResult.Errors.Select(modelError => modelError.ErrorMessage).ToList());
                        }
                        else
                        {
");

                    if (!string.IsNullOrEmpty(titleName) && hasCodeColumn == false)
                    {
                        streamWriter.WriteLine(@"
                            if (await _unitOfWork.Repository<I" + className + @"RepositoryAsync>().AnyAsync(o => o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               @".ToUpper() == " +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               @".ToUpper() && o." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) +
                                               @" != " +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @"))
                            {
                                return new Response<bool>(string.Format(SD.ExistData," +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @" ));
                            }");
                    }

                    if (!string.IsNullOrEmpty(titleName) && hasCodeColumn)
                    {
                        streamWriter.WriteLine(@"

                            var exitRecord = await _unitOfWork.Repository<I" + className + @"RepositoryAsync>()
                                .FirstOrDefaultAsync(o => (o." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               ".ToUpper() == " +
                                               UtilityHelper.FormatCamel(className) + "UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) +
                                               @".ToUpper() ||
                                       o.Code.ToUpper() == " + UtilityHelper.FormatCamel(className) + "UpsertDto." +
                                               "Code.ToUpper())  && o." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) +
                                               @" != " +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @") ;");

                        streamWriter.WriteLine(@"
 if (exitRecord != null)
                    {
            if (exitRecord.Code.ToUpper() == " + UtilityHelper.FormatCamel(className) + "UpsertDto." + @"Code.ToUpper())
                        {
                            return new Response<bool>(string.Format(SD.ExistData, " + UtilityHelper.FormatCamel(className) + "UpsertDto." + @"Code));
                        }

 if (exitRecord." + UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + ".ToUpper() == " + UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @".ToUpper())
                        {
                                return new Response<bool>(string.Format(SD.ExistData, " +
                                               UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                               UtilityHelper.GetColumnNameTitleForCheckExistInDataBase(table) + @"));
}
    }                       
");
                    }


                    streamWriter.WriteLine(@"
                    var entityToUpdate = await _unitOfWork.Repository<I" + className +
                                           @"RepositoryAsync>().FirstOrDefaultAsync(x => x." +
                                           UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + " == " +
                                           UtilityHelper.FormatCamel(className) + @"UpsertDto." +
                                           UtilityHelper.GetIDColumnNameForCheckExistInDataBase(table) + @");

                                _mapper.Map(" + UtilityHelper.FormatCamel(className) + @"UpsertDto, entityToUpdate);

                                await _unitOfWork.Repository<I" + className +
                                           @"RepositoryAsync>().UpdateAsync(entityToUpdate);

                                int effectedRows = await _unitOfWork.CommitAsync();
                                if (effectedRows != 0)
                                {
                                    return new Response<bool>(true);
                                }
                           
                        }

                        return new Response<bool>(SD.ErrorOccurred);
                    }

                    return new Response<bool>(""not authorized"");
                }
");

                    #endregion

                    #region Delete

                    streamWriter.WriteLine(@" public async Task<Response<bool>> DeleteAsync(" + keyType + @" id)
                    {
                        if (_permissionChecker.HasClaim(AppPermissions." + className + @".Delete))
                        {
                            await _unitOfWork.Repository<I" + className + @"RepositoryAsync>().DeleteAsync(id);

                            int effectedRows = await _unitOfWork.CommitAsync();
                            if (effectedRows != 0)
                            {
                                return new Response<bool>(true);
                            }

                            return new Response<bool>(SD.ErrorOccurred);
                        }

                        return new Response<bool>(""not authorized"");
                    }
                ");

                    #endregion

                    #region GetByIdAsync

                    streamWriter.WriteLine(@" public async Task<Response<" + className + @"ReadDto>> GetByIdAsync(" +
                                           keyType + @" id)
                    {
                        if (_permissionChecker.HasClaim(AppPermissions." + className + @".View))
                        {
                            var result = await _unitOfWork.Repository<I" + className +
                                           @"RepositoryAsync>().GetAsync(id);

                            return new Response<" + className + @"ReadDto>(_mapper.Map<" + className +
                                           @"ReadDto>(result));
                        }

                        return new Response<" + className + @"ReadDto>(""not authorized"");
                    }
                ");

                    #endregion

                    #region GetAllAsync

                    streamWriter.WriteLine(@" public async Task<Response<List<" + className +
                                           @"ReadDto>>> GetAllAsync(Expression<Func<" + className +
                                           @", bool>> predicate = null,
                            Func<IQueryable<" + className + @">, IOrderedQueryable<" + className +
                                           @">> orderBy = null, params Expression<Func<" + className +
                                           @", object>>[] includes)
                    {
                        if (_permissionChecker.HasClaim(AppPermissions." + className + @".List))
                        {
                        var result = await _unitOfWork.Repository<I" + className +
                                           @"RepositoryAsync>().GetAllAsync(predicate, orderBy, includes);
                        return new Response<List<" + className + @"ReadDto>>(_mapper.Map<List<" + className +
                                           @"ReadDto>>(result));
                    }

                    return new Response<List<" + className + @"ReadDto>>(""not authorized"");
                }
                ");

                    #endregion

                    #region GetAllAsync

                    streamWriter.WriteLine(@" public async Task<Response<PagedResult<" + className +
                                           @"ReadDto>>> GetPagedListAsync(Expression<Func<" + className +
                                           @", bool>> predicate = null,
                            Func<IQueryable<" + className + @">, IOrderedQueryable<" + className +
                                           @">> orderBy = null, int pageIndex = 0, int pageSize = 10, params Expression<Func<" +
                                           className + @", object>>[] includes)
                    {
                        if (_permissionChecker.HasClaim(AppPermissions." + className + @".List))
                        {

                            var pagedResult = new PagedResult<" + className + @"ReadDto>();

                            var result = await _unitOfWork.Repository<I" + className +
                                           @"RepositoryAsync>().GetPagedListAsync(predicate, orderBy, pageIndex, pageSize, includes);
                            if (result != null)
                            {
                                pagedResult.TotalCount = result.TotalCount;
                                pagedResult.FilteredTotalCount = result.FilteredTotalCount;

                                if (result.Data != null && result.Data.Count > 0)
                                {
                                    pagedResult.Data = _mapper.Map<List<" + className + @"ReadDto>>(result.Data);
                                }
                            }

                            return new Response<PagedResult<" + className + @"ReadDto>>(pagedResult);
                        }

                        return new Response<PagedResult<" + className + @"ReadDto>>(""not authorized"");
                    }
                ");

                    #endregion

                    streamWriter.WriteLine("}}");
                }

                #endregion


            }

            #region Application Dependency Injection



            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApplicationPath, "DependencyInjection.cs")))
            {
                // Create the header for the class

                streamWriter.WriteLine(@"

using " + ApplicationNameSpace + @".Services.Implementations;
using " + ApplicationNameSpace + @".Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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
                var className = "";
                foreach (Table table in tableList)
                {
                    className = UtilityHelper.MakeSingular(table.Name);
                    streamWriter.WriteLine("services.AddTransient<I" + className + "Service, " + className + "Service>();");
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

        private static void CreateAPIClasses()
        {

            #region Common Classes

            #region Exception Handler Middleware Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiMiddlewarePath, "ExceptionHandlerMiddleware.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                          using System;
using System.Net;
using System.Threading.Tasks;
using " + ApplicationNameSpace + @".Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace " + ApiNameSpace + @".Middleware
{
    public class ExceptionHandlerMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionHandlerMiddleware> _logger;

            public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    await ConvertException(context, ex);
                }
            }

            private Task ConvertException(HttpContext context, Exception exception)
            {
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

                context.Response.ContentType = ""application/json"";

                var result = string.Empty;

                switch (exception)
                {
                    case ValidationException validationException:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        result = JsonConvert.SerializeObject(validationException.ValidationErrors);
                        break;
                    case BadRequestException badRequestException:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        result = badRequestException.Message;
                        break;
                    case NotFoundException notFoundException:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                    case Exception ex:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        break;
                }

                context.Response.StatusCode = (int)httpStatusCode;

                if (result == string.Empty)
                {
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    _logger.LogError(result);
                }

                return context.Response.WriteAsync(result);
            }
        }
    }

                                        ");


            }

            #endregion

            #region MiddlewareExtensions Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiMiddlewarePath, "MiddlewareExtensions.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Builder;

namespace " + ApiNameSpace + @".Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
                                        ");


            }

            #endregion

            #region SwaggerBasicAuthMiddleware Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiMiddlewarePath, "SwaggerBasicAuthMiddleware.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace " + ApiNameSpace + @".Middleware
{

 public class SwaggerBasicAuthMiddleware
    {

        private readonly RequestDelegate next;

        public SwaggerBasicAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Make sure we are hitting the swagger path, and not doing it locally as it just gets annoying :-)
            //if (context.Request.Path.StartsWithSegments(""/swagger"") && !this.IsLocalRequest(context))
            if (context.Request.Path.StartsWithSegments(""/swagger""))
            {
                string authHeader = context.Request.Headers[""Authorization""];
                if (authHeader != null && authHeader.StartsWith(""Basic ""))
                {
                    // Get the encoded username and password
                    var encodedUsernamePassword =
                        authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                    // Decode from Base64 to string
                    var decodedUsernamePassword =
                        Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    // Split username and password
                    var username = decodedUsernamePassword.Split(':', 2)[0];
                    var password = decodedUsernamePassword.Split(':', 2)[1];

                    // Check if login is correct
                    if (IsAuthorized(username, password))
                    {
                        await next.Invoke(context);
                        return;
                    }
                }

                // Return authentication type (causes browser to show login dialog)
                context.Response.Headers[""WWW-Authenticate""] = ""Basic"";

                // Return unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context);
            }
        }

        public bool IsAuthorized(string username, string password)
        {
            // Check that username and password are correct
            return username.Equals(""MinapharmUser"", StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals(""Min@pharmP@$$word2020"");
        }

        public bool IsLocalRequest(HttpContext context)
        {
            //Handle running using the Microsoft.AspNetCore.TestHost and the site being run entirely locally in memory without an actual TCP/IP connection
            if (context.Connection.RemoteIpAddress == null && context.Connection.LocalIpAddress == null)
            {
                return true;
            }

            if (context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress))
            {
                return true;
            }

            if (IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
            {
                return true;
            }

            return false;
        }
    }

    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>();
        }
    }  

}
                                        ");


            }

            #endregion

            #region BaseApiController Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiInfrastructurePath, "BaseApiController.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace " + ApiNameSpace + @".Infrastructure
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(""api/v{version:apiVersion}/[controller]/[action]"")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class BaseApiController : ControllerBase
    {
        public BaseApiController()
        {

        }
    }
}

");


            }


            #endregion

            #region Filters Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiFiltersPath, "Filters.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
                                           using " + DomainNameSpace + @".Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Security.Claims;

namespace " + ApiNameSpace + @".Filters
{
    public class UserActivitiesAttribute : IActionFilter
    {
        private readonly ILogger<UserActivitiesAttribute> _logger;

        public UserActivitiesAttribute(ILogger<UserActivitiesAttribute> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string areaName = string.IsNullOrEmpty(Convert.ToString(context.RouteData.Values[""area""])) ? """" : context.RouteData.Values[""area""].ToString().ToLower();
            string controllerName = context.RouteData.Values[""controller""].ToString().ToLower();
            string actionName = context.RouteData.Values[""action""].ToString().ToLower();

            #region Log User Activity
            try
            {
                LogUserActivity logActivity = new LogUserActivity();
                logActivity.Id = Guid.NewGuid();
                logActivity.CreatedDateTime = DateTime.UtcNow;
                logActivity.IPAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                logActivity.Browser = context.HttpContext.Request.Headers[""User-Agent""].ToString();
                logActivity.UrlData = string.Format(""{0}://{1}{2}{3}"", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host, context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);


                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    logActivity.UserId = Guid.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }

                logActivity.HttpMethod = context.HttpContext.Request.Method;

                if (context.HttpContext.Request.Path != ""/"")
                {
                    logActivity.UserData +=
                        JsonConvert.SerializeObject(
                            new
                            {
                                context.HttpContext.Request.Path
                            });
                }
                if (context.HttpContext.Request.QueryString.HasValue)
                {
                    logActivity.UserData +=
                        JsonConvert.SerializeObject(
                            new
                            {
                                context.HttpContext.Request.QueryString
                            });
                }
                logActivity.UserData +=
                    JsonConvert.SerializeObject(
                        new
                        {
                            context.RouteData.Values
                        });


                if (context.HttpContext.Request.HasFormContentType)
                {
                    logActivity.UserData =
                        JsonConvert.SerializeObject(
                            new
                            {
                                context.HttpContext.Request.Form
                            });
                }


                //var _logUserActivityServices = commonServices.ServiceProvider.GetRequiredService<ILogActivityService>();

                // await  _logUserActivityServices.AddAsync(logActivity);
                _logger.LogInformation(JsonConvert.SerializeObject(logActivity));
                //FileHelper.WriteToFile(JsonConvert.SerializeObject(logActivity));

            }
            catch (Exception ex)
            {
            }

            #endregion

        }
    }
}

                                        ");


            }

            #endregion

            #region Program Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiPath, "Program.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace " + ApiNameSpace + @"
{
    public class Program
    {

        public async static Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(""appsettings.json"")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
               // .WriteTo.File(""Logs/log-.txt"", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                   // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                   // await Identity.Seed.UserCreator.SeedAsync(userManager);
                    Log.Information(""Application Starting"");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, ""An error occured while starting the application"");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    

   
}
}

");


            }


            #endregion


            #region Startup Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiPath, "Startup.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using " + ApiNameSpace + @".Filters;
using " + ApiNameSpace + @".Middleware;
using " + ApplicationNameSpace + @";
using " + DataAccessNameSpace + @";
using " + DataAccessNameSpace + @".Data.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace " + ApiNameSpace + @"
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApplication();
            services.AddInfrastructure(Configuration);


            services.AddHttpContextAccessor();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options => options.GroupNameFormat = ""'v'VVV"");

            services.AddAntiforgery();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(UserActivitiesAttribute));
            });

            services.AddCors(options =>
            {
                options.AddPolicy(""Open"", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(""v1"", new OpenApiInfo { Title = """ + ApiNameSpace + @""", Version = ""v1"" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = ""JWT Authorization header using the Bearer scheme.Example: \""Authorization: Bearer { token}\"""",
                    Name = ""Authorization"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = ""bearer"",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ""Bearer""
                    }
                };

            options.AddSecurityDefinition(""Bearer"", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { ""Bearer"" } }
                };

            options.AddSecurityRequirement(securityRequirement);


        });
        }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

             app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(""/swagger/v1/swagger.json"", """ + ApiNameSpace + @" v1""));
       


        dbInitializer.Initialize();
        app.UseHttpsRedirection();
        //    app.UseSession();
        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseCustomExceptionHandler();

        app.UseCors(""Open"");

     

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
}


            ");


            }


            #endregion

            #region AppSetting Class

            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiPath, "appsettings.json")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Server=(localdb)\\mssqllocaldb;Database=" + _appSetting.DataBase + @";Trusted_Connection=True;MultipleActiveResultSets=true""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft"": ""Warning"",
      ""Microsoft.Hosting.Lifetime"": ""Information""
    }
  },
  ""Serilog"": {
    ""MinimumLevel"": {
      ""Default"": ""Information"",
      ""Override"": {
        ""Default"": ""Information"",
        ""Microsoft"": ""Warning"",
        ""Microsoft.Hosting.Lifetime"": ""Information""
      }
    },
    ""WriteTo"": [
      //{ ""Name"": ""Console"" },
      {
        ""Name"": ""File"",
        ""Args"": {
          ""path"": ""Logs/log-.txt"",
          ""outputTemplate"": ""{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"",
          // ""outputTemplate"": ""{Timestamp:o} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}"",
          ""rollingInterval"": ""Day"",
          ""retainedFileCountLimit"": 7
        }
      }
    ],
    ""Enrich"": [ ""FromLogContext"", ""WithMachineName"" ],
    ""Properties"": {
      ""Application"": ""AspNetCoreSerilogDemo""
    }
  },
  ""AllowedHosts"": ""*"",

  ""JwtSettings"": {
    ""Key"": ""OQ6NPHeCv4rgLWGkSlaLf8soegJZhRrLY7OGaavJAhsdW5cN4PByKiq38W2Dn3DvYqggcTsDvGNXaLiVIw-U3hdNewXcCtEbe8f9ezgSnhpZIjAUaCUrCZswz6itxb-KEIAp-aJaF1AztCv1jG7mzn_S2YvbrLQvTE2f60i87VPUvByKkkz6yJO2ab_Vx_XSBT77BQN1hyVStPMGPcTP0IIDlGyz2XVYUygPcBnfK6cONPTptjPMbTubpxyHyUCZ6-1DpyI7gRhPXUM36IagcHsCsLmwkIQdkGgR6kpay5LAcBYGRxDjs-lXeFS2Vd9D_cv3Lzq3N4QTqHrOBnLwWg"",
    ""Issuer"": ""https://localhost:44339"",
    ""Audience"": ""https://localhost:44339"",
    ""DurationInMinutes"": 60
  }
}

");


            }


            #endregion




            #endregion

            #region ApiControllersPath Class
            using (StreamWriter streamWriter =
                new StreamWriter(Path.Combine(ApiControllersPath, "AccountController.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine(@"
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  " + ApiNameSpace + @".Infrastructure;
using  " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Interfaces;
using " + ApplicationNameSpace + @".Services.Interfaces;
using " + DomainNameSpace + @".Common;
using " + DomainNameSpace + @".Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace  " + ApiNameSpace + @".Controllers
{
    [ApiVersion(""1.0"")]
    public class AccountController : BaseApiController
        {
            private readonly IAuthService _authService;
            private readonly ILogger<AccountController> _logger;

            public AccountController(IAuthService authService, ILogger<AccountController> logger)
            {
                _authService = authService;
                _logger = logger;
                _logger.LogInformation($""Enter the { nameof(AccountController)} controller"");
            }

            [AllowAnonymous]
            [HttpPost]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> RegisterAccount(RegistrationModel registrationModel)
            {

                if (registrationModel == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterAsync(registrationModel);

                if (response != null)
                {
                    if (response.Succeeded)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest(response);
                    }
                }
                else
                {
                    response = new Response<Guid>(SD.ErrorOccurred);
                    return BadRequest(response);
                }
            }

            [AllowAnonymous]
            [HttpPost]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Login(LoginModel loginModel)
            {

                if (loginModel == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.AuthenticateAsync(loginModel);

                if (response != null)
                {
                    if (response.Succeeded)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest(response);
                    }
                }
                else
                {

                    return BadRequest(response);
                }
            }
        }
    }

   ");


            }


            #endregion

            foreach (Table table in tableList)
            {
                var nodeType = tables.FirstOrDefault(o => o.Title == table.Name);

                string className = "";



                #region  Model Classes
                className = UtilityHelper.MakeSingular(table.Name);

                string Camel_className = UtilityHelper.FormatCamel(className);

                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(ApiControllersPath, className + "Controller.cs")))
                {

                    streamWriter.WriteLine(@"
using " + ApiNameSpace + @".Infrastructure;
using " + ApplicationNameSpace + @".Dtos;
using " + ApplicationNameSpace + @".Services.Interfaces;
using " + DomainNameSpace + @".Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
");


                    streamWriter.WriteLine(@"
namespace " + ApiNameSpace + @".Controllers
{
    [ApiVersion(""1.0"")]
    public class " + className + @"Controller : BaseApiController
    {
        string cacheKeyGetAll = ""Get_All_" + className + @""";
        private readonly I" + className + @"Service _" + Camel_className + @"Service;
        private readonly ILogger<" + className + @"Controller> _logger;
         private readonly IMemoryCache _cache;
        public " + className + @"Controller(I" + className + @"Service " + Camel_className + @"Service, ILogger<" + className + @"Controller> logger, IMemoryCache cache)
        {
            _" + Camel_className + @"Service = " + Camel_className + @"Service;
            _logger = logger;
            _cache = cache;
            _logger.LogInformation($""Enter the {nameof(" + className + @"Controller)} controller"");

        }

        [Authorize(AppPermissions." + className + @".List)]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<" + className + @"ReadDto>))]
        public async Task<ActionResult> GetAll()
        {

           
             Response<List<" + UtilityHelper.MakeSingular(table.Name) + @"ReadDto>> dataList = null;

                    if (_cache.TryGetValue(cacheKeyGetAll, out dataList))
                    {
                        return Ok(dataList);
                    }


                    dataList = await _" + Camel_className + @"Service.GetAllAsync();

                    _cache.Set(cacheKeyGetAll, dataList);

                    return Ok(dataList);
                 
        }
        
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedResult<" + className + @"ReadDto>))]
        public async Task<ActionResult> GetAllPagedList(int pageIndex, int pageSize)
        {
            return Ok(await _" + Camel_className + @"Service.GetPagedListAsync(null, (o => o.OrderBy(x => x.DisplayOrder)), pageIndex, pageSize));
        }
        
        [HttpGet(""{id:Guid}"", Name = """ + "GetById" + className + @""")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(" + className + @"ReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> GetById(Guid id)
        {
            var " + Camel_className + @" = await _" + Camel_className + @"Service.GetByIdAsync(id);

            if (" + Camel_className + @" == null)
            {
                return NotFound();
            }

            return Ok(" + Camel_className + @");
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(" + className + @"UpsertDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(" + className + @"UpsertDto " + Camel_className + @"UpsertDto)
        {

            if (" + Camel_className + @"UpsertDto == null)
            {
                return BadRequest(ModelState);
            }

            Response<Guid> response = await _" + Camel_className + @"Service.AddAsync(" + Camel_className + @"UpsertDto);

            if (response != null)
            {
                if (response.Succeeded)
                {
                      _cache.Remove(cacheKeyGetAll);
                    return CreatedAtRoute( """ + "GetById" + className + @""", new { id = response.Data }, response.Data);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            else
            {
                response = new Response<Guid>(SD.ErrorOccurred);
                return BadRequest(response);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(" + className + @"UpsertDto " + Camel_className + @"UpsertDto)
        {

            if (" + Camel_className + @"UpsertDto == null)
            {
                return BadRequest(ModelState);
            }

            Response<bool> response = await _" + Camel_className + @"Service.UpdateAsync(" + Camel_className + @"UpsertDto);

            if (response != null)
            {
                if (response.Succeeded)
                {
                    _cache.Remove(cacheKeyGetAll);
                    return NoContent();
                }
                else
                {
                    return BadRequest(response);
                }
            }
            else
            {
                response = new Response<bool>(SD.ErrorOccurred);
                return BadRequest(response);
            }
        }

        [HttpDelete(""{id}"")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {

            if (id == Guid.Empty)
            {
                return BadRequest(""id is required."");
            }

            Response<bool> response = await _" + Camel_className + @"Service.DeleteAsync(id);

            if (response != null)
            {
                if (response.Succeeded)
                {
                     _cache.Remove(cacheKeyGetAll);
                    return NoContent();
                }
                else
                {
                    return BadRequest(response);
                }
            }
            else
            {
                response = new Response<bool>(SD.ErrorOccurred);
                return BadRequest(response);
            }
        }
    }
}

");

                }
                #endregion


            }

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
