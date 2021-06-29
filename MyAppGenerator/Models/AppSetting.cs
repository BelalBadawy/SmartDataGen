using System.Collections.Generic;

namespace MyAppGenerator.Models
{
    public class AppSetting
    {
        public string SqlServer { get; set; }
        public string DataBase { get; set; }
        public int SqlAuthentication { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public string ProjectName { get; set; }
        public int Architecture { get; set; }
        public bool WithResourcesFile { get; set; }
        public bool WithTranslations { get; set; }
        public List<string> LanguagesResourcesFile { get; set; }
        public string OutputDirectory { get; set; }

        public string DomainCustomPath { get; set; }
        public string ApplicationCustomPath { get; set; }
        public string InfrastructureCustomPath { get; set; }
        public string ResourcesCustomPath { get; set; }

        public int KeyType { get; set; }
        public bool AddDapperToDBContext { get; set; }

        public string APICustomPath { get; set; }


    }
}
