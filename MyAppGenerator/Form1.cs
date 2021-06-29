using MyAppGenerator.Helpers;
using MyAppGenerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MyAppGenerator.CsGenerators;
using System.Net.Http;

namespace MyAppGenerator
{
    public partial class MainForm : Form
    {

        /// <summary>
        /// Dictionary of Database objects types and Database objects.
        /// </summary>
        private Dictionary<string, List<string[]>> dbObjects = null;

        private string outputDirectory = "";

        public MainForm()
        {
            InitializeComponent();
        }





        private async void btnGenerator_Click(object sender, EventArgs e)
        {
            UtilityHelper.ConnectionString = txtConnectionString.Text;

            try
            {

                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Select an output directory";
                    dialog.SelectedPath = outputDirectory;
                    dialog.ShowNewFolderButton = true;
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        outputDirectory = dialog.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }

                #region Save App Setting 

                AppSetting appSetting = new AppSetting();
                appSetting.ProjectName = txtProjectName.Text.Trim();
                appSetting.ConnectionString = txtConnectionString.Text.Trim();
                appSetting.DataBase = txtDataBase.Text.Trim();
                appSetting.LoginName = txtLoginName.Text.Trim();
                appSetting.Password = txtPassword.Text.Trim();
                appSetting.SqlAuthentication = (rbtnWindowsAuthentication.Checked ? 1 : 2);
                appSetting.SqlServer = txtServer.Text.Trim();
                appSetting.Architecture = ddlArchitecture.SelectedIndex;
                appSetting.WithResourcesFile = chkWithResourcesFile.Checked;
                appSetting.WithTranslations = chkWithTranslations.Checked;
                appSetting.APICustomPath = txtAPICustomPath.Text.Trim();


                List<string> culutresList = null;

                if (chkWithResourcesFile.Checked)
                {
                    culutresList = new List<string>();

                    foreach (var i in chklistCulutres.CheckedItems)
                    {
                        string[] values = i.ToString().Split(' ');
                        culutresList.Add(values[0]);
                    }
                }

                appSetting.LanguagesResourcesFile = culutresList;
                appSetting.OutputDirectory = outputDirectory;
                appSetting.DomainCustomPath = txtDomainCustomPath.Text.Trim();
                appSetting.ApplicationCustomPath = txtApplicationCustomPath.Text.Trim();
                appSetting.InfrastructureCustomPath = txtInfrastructureCustomPath.Text.Trim();
                appSetting.KeyType = ddlKeyType.SelectedIndex;
                appSetting.AddDapperToDBContext = chAddDapperToDbContext.Checked;

                var filePath = AppDomain.CurrentDomain.BaseDirectory;

                File.WriteAllText(filePath + @"\AppSetting.txt", String.Empty);
                UtilityHelper.WriteToFile(filePath, "AppSetting.txt", JsonConvert.SerializeObject(appSetting));

                #endregion



                UtilityHelper.ConnectionString = txtConnectionString.Text;


                List<NodeType> tablesName = CheckedNames(schemaTreeView.Nodes);

                if (tablesName != null && tablesName.Count > 0)
                {
                    if (ddlArchitecture.SelectedIndex == -1 || ddlArchitecture.SelectedIndex == 0)
                    {

                        #region Onion Architecture

                        OnionArchitecture.ProjectName = txtProjectName.Text.Trim();
                        OnionArchitecture.useResourceFile = chkWithResourcesFile.Checked;
                        OnionArchitecture.tables = tablesName;
                        OnionArchitecture.GenerateOnionArchitecture(txtProjectName.Text.Trim(), outputDirectory);

                        #endregion

                    }
                    else if (ddlArchitecture.SelectedIndex == 1)
                    {

                        #region CQRS Architecture
                        CQRSArchitecture.ProjectName = txtProjectName.Text.Trim();
                        CQRSArchitecture.useResourceFile = chkWithResourcesFile.Checked;
                        CQRSArchitecture.tables = tablesName;
                        CQRSArchitecture.GenerateCQRSArchitecture(txtProjectName.Text.Trim(), outputDirectory);

                        #endregion

                    }
                    else if (ddlArchitecture.SelectedIndex == 2)
                    {

                        #region CQRS Architecture
                        CQRS2Architecture.ProjectName = txtProjectName.Text.Trim();
                        CQRS2Architecture.useResourceFile = chkWithResourcesFile.Checked;
                        CQRS2Architecture.tables = tablesName;
                        CQRS2Architecture.GenerateCQRS2Architecture(appSetting);

                        #endregion

                    }
                    else if (ddlArchitecture.SelectedIndex == 3)
                    {

                        #region DbContext Architecture
                        DbContextArchitecture.ProjectName = txtProjectName.Text.Trim();
                        DbContextArchitecture.useResourceFile = chkWithResourcesFile.Checked;
                        DbContextArchitecture.tables = tablesName;
                        DbContextArchitecture.GenerateDbContextArchitecture(appSetting);

                        #endregion

                    }
                    else if (ddlArchitecture.SelectedIndex == 4)
                    {

                        #region Clean Architecture Repository
                        CleanArchitectureRepository.ProjectName = txtProjectName.Text.Trim();
                        CleanArchitectureRepository.useResourceFile = chkWithResourcesFile.Checked;
                        CleanArchitectureRepository.tables = tablesName;
                        CleanArchitectureRepository.GenerateCleanArchitectureRepository(appSetting);

                        #endregion

                    }
                }

                // Inform the user we're done
                MessageBox.Show("Classes were generated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // progressBar.Value = 0;

            }

            //  LoadData();


        }

        List<NodeType> CheckedNames(System.Windows.Forms.TreeNodeCollection theNodes)
        {
            List<NodeType> aResult = new List<NodeType>();

            if (theNodes != null)
            {
                foreach (System.Windows.Forms.TreeNode aNode in theNodes)
                {
                    if (aNode.Checked)
                    {

                        if (!string.IsNullOrEmpty(Convert.ToString(aNode.Tag)))
                        {
                            NodeType nodeType = new NodeType();
                            nodeType.Title = aNode.Text;
                            nodeType.Type = (SchemaTypeEnum)aNode.Tag;
                            aResult.Add(nodeType);
                        }
                    }

                    aResult.AddRange(CheckedNames(aNode.Nodes));
                }
            }

            return aResult;
        }

        private void LoadData()
        {
            if (dbObjects != null)
            {
                dbObjects.Clear();
                dbObjects = null;
            }
            dbObjects = new Dictionary<string, List<string[]>>();
            SQLHelper sql = new SQLHelper(UtilityHelper.ConnectionString);
            //  List<string> listDB = new List<string>() { AppStatic.Database };

            var listDbItems = sql.GetTables();
            dbObjects.Add(UtilityHelper.Constants.Tables, listDbItems);

            listDbItems = sql.GetViews();
            dbObjects.Add(UtilityHelper.Constants.Views, listDbItems);

            listDbItems = sql.GetProcedures();
            dbObjects.Add(UtilityHelper.Constants.StoredProcedures, listDbItems);

            listDbItems = sql.GetTableValuedFunctions();
            dbObjects.Add(UtilityHelper.Constants.TableValuedFunctions, listDbItems);

            listDbItems = sql.GetTableTypes();
            dbObjects.Add(UtilityHelper.Constants.UserDefinedTableTypes, listDbItems);



        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Read App Setting File and load data

            var appSettingText = UtilityHelper.ReadFile(AppDomain.CurrentDomain.BaseDirectory + "AppSetting.txt");

            if (!string.IsNullOrEmpty(appSettingText))
            {
                var appSetting = JsonConvert.DeserializeObject<AppSetting>(appSettingText);
                if (appSetting != null)
                {
                    txtProjectName.Text = appSetting.ProjectName;
                    txtConnectionString.Text = appSetting.ConnectionString;
                    txtDataBase.Text = appSetting.DataBase;
                    txtLoginName.Text = appSetting.LoginName;
                    txtPassword.Text = appSetting.Password;


                    if (appSetting.SqlAuthentication == 1)
                    {
                        rbtnSQLServerAuthentication.Checked = false;
                        rbtnWindowsAuthentication.Checked = true;
                    }
                    else
                    {
                        rbtnSQLServerAuthentication.Checked = true;
                        rbtnWindowsAuthentication.Checked = false;
                    }

                    txtServer.Text = appSetting.SqlServer;
                    ddlArchitecture.SelectedIndex = appSetting.Architecture;
                    ddlKeyType.SelectedIndex = appSetting.KeyType;

                    chkWithResourcesFile.Checked = appSetting.WithResourcesFile;
                    chkWithTranslations.Checked = appSetting.WithTranslations;
                    chAddDapperToDbContext.Checked = appSetting.AddDapperToDBContext;

                    if (appSetting.LanguagesResourcesFile != null && appSetting.LanguagesResourcesFile.Count > 0)
                    {
                        for (int i = 0; i < appSetting.LanguagesResourcesFile.Count; i++)
                        {
                            for (int x = 0; x < chklistCulutres.Items.Count; x++)
                            {
                                string[] values = chklistCulutres.Items[x].ToString().Split(' ');
                                if (appSetting.LanguagesResourcesFile[i] == values[0])
                                {
                                    chklistCulutres.SetItemChecked(x, true);
                                }
                            }
                        }

                    }
                    outputDirectory = appSetting.OutputDirectory;

                    txtDomainCustomPath.Text = appSetting.DomainCustomPath;
                    txtApplicationCustomPath.Text = appSetting.ApplicationCustomPath;
                    txtInfrastructureCustomPath.Text = appSetting.InfrastructureCustomPath;


                    btnConnect_Click(btnConnect, new EventArgs());

                }
            }

            #endregion

        }


        private void EnableConnectionButton()
        {
            if (string.IsNullOrEmpty(txtServer.Text))
            {
                btnConnect.Enabled = false;
                return;
            }

            if (string.IsNullOrEmpty(txtDataBase.Text))
            {
                btnConnect.Enabled = false;
                return;
            }

            if (rbtnSQLServerAuthentication.Checked)
            {
                if (string.IsNullOrEmpty(txtLoginName.Text))
                {
                    btnConnect.Enabled = false;
                    return;
                }
            }

            btnConnect.Enabled = true;
        }


        private void chkWithResourcesFile_CheckedChanged(object sender, EventArgs e)
        {
            chklistCulutres.Enabled = chkWithResourcesFile.Checked;
        }


        private void rbtnWindowsAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            txtLoginName.Enabled = false;
            txtLoginName.BackColor = SystemColors.InactiveBorder;


            txtPassword.Enabled = false;
            txtPassword.BackColor = SystemColors.InactiveBorder;


            EnableConnectionButton();
        }

        private void rbtnSQLServerAuthentication_CheckedChanged(object sender, EventArgs e)
        {

            txtLoginName.Enabled = true;
            txtLoginName.BackColor = SystemColors.Window;


            txtPassword.Enabled = true;
            txtPassword.BackColor = SystemColors.Window;

            EnableConnectionButton();
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            EnableConnectionButton();
        }

        private void txtDataBase_TextChanged(object sender, EventArgs e)
        {
            EnableConnectionButton();
        }

        private void txtLoginName_TextChanged(object sender, EventArgs e)
        {
            EnableConnectionButton();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            EnableConnectionButton();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            try
            {
                schemaTreeView.Nodes.Clear();
                btnGenerator.Enabled = false;

                // Build the connection string
                if (rbtnWindowsAuthentication.Checked)
                {
                    UtilityHelper.ConnectionString = @"Server=" + txtServer.Text + "; Database=" + txtDataBase.Text + "; Integrated Security=SSPI;";
                }
                else
                {
                    UtilityHelper.ConnectionString = @"Server=" + txtServer.Text + "; Database=" + txtDataBase.Text + "; User ID=" + txtLoginName.Text + "; Password=" + txtPassword.Text + ";";
                }

                txtConnectionString.Text = UtilityHelper.ConnectionString;

                SqlConnection connection = null;
                try
                {
                    connection = new SqlConnection(UtilityHelper.ConnectionString);
                    connection.Open();
                    // MessageBox.Show("Connection Successed");
                    GetTables(connection);
                    GetViews(connection);
                    GetStoredProcedures(connection);

                    UpdateSchemaTreeView();

                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (connection != null)
                        connection.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnGenerator.Enabled = true;
            }

        }

        private void GetTables(SqlConnection connection)
        {
            UtilityHelper.TableList.Clear();

            string sqlTables = string.Format(@"select TABLE_CATALOG,TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
												from INFORMATION_SCHEMA.TABLES
												 where TABLE_TYPE = 'BASE TABLE'
													and (TABLE_NAME NOT IN ( 'dtProperties' ,
																		 'sysdiagrams',
																		 '__EFMigrationsHistory',
																		 'AspNetRoleClaims',
																		 'AspNetRoles',
																		 'AspNetUserClaims',
																		 'AspNetUserLogins',
																		 'AspNetUserRoles',
																		 'AspNetUsers',
																		 'AspNetUserTokens'))
												and TABLE_CATALOG = '{0}'
												order by TABLE_NAME", txtDataBase.Text.Trim());

            // Get a list of the entities in the database
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlTables, connection);
            dataAdapter.Fill(dataTable);

            // Process each table
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Table table = new Table();
                table.Name = (string)dataRow["TABLE_NAME"];
                //  QueryTable(connection, table);
                UtilityHelper.TableList.Add(table);
            }
        }


        private void GetViews(SqlConnection connection)
        {
            UtilityHelper.ViewList.Clear();

            string sqlTables = string.Format(@"select TABLE_CATALOG,TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
												from INFORMATION_SCHEMA.TABLES
												 where TABLE_TYPE = 'VIEW'
												and TABLE_CATALOG = '{0}'
												order by TABLE_NAME", txtDataBase.Text.Trim());

            // Get a list of the entities in the database
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlTables, connection);
            dataAdapter.Fill(dataTable);

            // Process each table
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Table table = new Table();
                table.Name = (string)dataRow["TABLE_NAME"];
                //  QueryTable(connection, table);
                UtilityHelper.ViewList.Add(table);
            }
        }


        private void GetStoredProcedures(SqlConnection connection)
        {
            UtilityHelper.SpList.Clear();
            string sqlTables = string.Format(@"SELECT SPECIFIC_CATALOG AS TABLE_CATALOG, SPECIFIC_SCHEMA AS TABLE_SCHEMA, SPECIFIC_NAME AS TABLE_NAME, ROUTINE_TYPE AS TABLE_TYPE
FROM            {0}.INFORMATION_SCHEMA.ROUTINES
WHERE        (ROUTINE_TYPE = 'PROCEDURE') AND (LEFT(ROUTINE_NAME, 3) NOT IN ('sp_', 'xp_', 'ms_'))", txtDataBase.Text.Trim());

            // Get a list of the entities in the database
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlTables, connection);
            dataAdapter.Fill(dataTable);

            // Process each table
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Table table = new Table();
                table.Name = (string)dataRow["TABLE_NAME"];
                //  QueryTable(connection, table);
                UtilityHelper.SpList.Add(table);
            }
        }


        private void UpdateSchemaTreeView()
        {
            schemaTreeView.Nodes.Clear();

            TreeNode rootNode = new TreeNode(txtDataBase.Text.Trim());


            //Add tables, columns and indexes
            TreeNode tableNode = new TreeNode("Tables");

            foreach (Table table in UtilityHelper.TableList)
            {
                //Table
                TreeNode currentTableNode = new TreeNode(table.Name);
                currentTableNode.Tag = SchemaTypeEnum.Table;
                tableNode.Nodes.Add(currentTableNode);

            }

            ////Add views
            ////NOTE for the Views we are going to add the View object to the tag property of the node so that 
            ////we can display the view definition when the view node is selected.
            TreeNode viewNode = new TreeNode("Views");

            foreach (Table view in UtilityHelper.ViewList)
            {
                TreeNode currentViewNode = new TreeNode(view.Name);
                currentViewNode.Tag = SchemaTypeEnum.View;
                viewNode.Nodes.Add(currentViewNode);
            }



            TreeNode spNode = new TreeNode("Stored Procedure");

            foreach (Table sp in UtilityHelper.SpList)
            {
                TreeNode currentViewNode = new TreeNode(sp.Name);
                currentViewNode.Tag = SchemaTypeEnum.StoredProcedure;
                spNode.Nodes.Add(currentViewNode);
            }

            rootNode.Nodes.Add(tableNode);
            rootNode.Nodes.Add(viewNode);
            rootNode.Nodes.Add(spNode);
            schemaTreeView.Nodes.Add(rootNode);

        }

        private void schemaTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {

            CheckTreeViewNode(e.Node, e.Node.Checked);

        }

        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }

    }
}
