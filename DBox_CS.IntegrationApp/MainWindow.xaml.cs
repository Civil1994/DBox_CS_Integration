using System.Windows;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.ObjectModel;
using DBox_CS.Core.Models;
using DBox_CS.Core.AppClass;
using System.Net;
using System.Data.SqlClient;
using DBox_CS.Core.DALayer;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBox_CS.Core.BL;
using System.Configuration;
using Starksoft.Aspen.GnuPG;
using FluentFTP;
using System.Windows.Input;
using DBox_CS.Core.Utility;
using DBox_CS.Core.Enums;

namespace DBox_CS.IntegrationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Main objMain;
        static string appFilesPath;
        static Dictionary<string, string> tempOutputFolders;// List of folders for output
        static Dictionary<string, string> tempFolders;// List of folders to process files from
        static string tempDirectory;
        public MainWindow()
        {
            InitializeComponent();


            appFilesPath = ConfigurationManager.AppSettings["AppFilesPath"].ToString();

            tempDirectory = Path.Combine(appFilesPath, "DownloadedFile\\Temp\\");

            tempOutputFolders = new Dictionary<string, string>
                {
                    { "leave",Path.Combine(appFilesPath, "LeaveExport\\Temp\\") },
                };

            tempFolders = new Dictionary<string, string>
                {
                    { "dailyhours",Path.Combine(appFilesPath, "DailyHoursImport\\Temp\\") }
                };
        }





        /// <summary>
        /// Upload Process Below
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        #region Upload Employee to Unifocus       
        private void UploadEmpToUFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objMain == null)
                {
                    objMain = new Main(Main.ProcessIntitator.WindowsForm);
                }
                objMain.appFilesPath = appFilesPath;
                objMain.tempOutputFolders = tempOutputFolders;

                objMain.PushDocumentToDBOX();
                //objMain.PushEmployeesToDBOX();
            }
            catch (Exception ex)
            {
                Common.LogAction($"An error occured at UploadEmpToUFButton. Details {ex.Message}");
                Common.LogException(ex);
                MessageBox.Show($"An error occured at UploadEmpToUFButton. Details {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        #endregion



        private void UploadLeaveToUFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objMain == null)
                {
                    objMain = new Main(Main.ProcessIntitator.WindowsForm);
                }
                objMain.appFilesPath = appFilesPath;
                objMain.tempOutputFolders = tempOutputFolders;


                //objMain.UploadeLeaveToUF();
            }
            catch (Exception ex)
            {
                Common.LogAction($"An error occured at UploadLeaveToUFButton. Details {ex.Message}");
                Common.LogException(ex);
                MessageBox.Show($"An error occured at UploadLeaveToUFButton. Details {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objMain == null)
                {
                    objMain = new Main(Main.ProcessIntitator.WindowsForm);
                }
                objMain.appFilesPath = appFilesPath;
                objMain.tempDirectory = tempDirectory;
                objMain.tempFolders = tempFolders;


                //objMain.DownloadFiles();
            }
            catch (Exception ex)
            {
                Common.LogAction($"Error loading files from local directory: {ex.Message}");
                Common.LogException(ex);
                MessageBox.Show($"Error loading files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objMain == null)
                {
                    objMain = new Main(Main.ProcessIntitator.WindowsForm);
                }
                objMain.appFilesPath = appFilesPath;
                objMain.tempDirectory = tempDirectory;
                objMain.tempFolders = tempFolders;

                //objMain.ProcessFiles();
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while processing files: {ex.Message}";
                Common.LogAction(errorMessage);
                Common.LogException(ex);
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Download File From SFTP and FTP 
        //private bool DownloadFileFromSFTP(ref string errmsg, string fileType)
        //{
        //    string appFilesPath = ConfigurationManager.AppSettings["AppFilesPath"].ToString();
        //    try
        //    {
        //        // Retrieve the SFTP configuration from the database based on the FileType
        //        string host = string.Empty, username = string.Empty, password = string.Empty, destination = string.Empty;
        //        int port = 0;
        //        string connectionString = ConnectionFunctions.GetConnectionString();
        //        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        //        {
        //            sqlConnection.Open();
        //            using (SqlCommand command = new SqlCommand("SELECT Destination, Host, UserName, Password, Port FROM EFTSFTP WHERE FileType = @FileType", sqlConnection))
        //            {
        //                command.Parameters.AddWithValue("@FileType", fileType);

        //                SqlDataReader reader = command.ExecuteReader();
        //                if (reader.Read())
        //                {
        //                    destination = reader["Destination"].ToString();
        //                    host = reader["Host"].ToString();
        //                    username = reader["UserName"].ToString();
        //                    password = reader["Password"].ToString();
        //                    //port = reader["Port"] != DBNull.Value ? Convert.ToInt32(reader["Port"]) : 0;
        //                    port = Convert.ToInt32(reader["Port"]);
        //                    // Default to port 22 if port is 0 or invalid
        //                    if (port == 0)
        //                    {
        //                        port = 22;
        //                    }
        //                }
        //                else
        //                {
        //                    errmsg = $"No SFTP configuration found for FileType: {fileType}.";
        //                    return false;
        //                }
        //            }
        //        }

        //        // Local file path where the file will be saved
        //        string tempDirectory = appFilesPath + "DownloadedFile\\Temp\\"; // Adjust the path as needed
        //        string finalDirectory = appFilesPath + "DownloadedFile\\Processed\\";  // Folder where the file will be moved

        //        //string remoteFilePath = "/SFTPTest/EmployeeFile_06_12_2024_13_47_13.csv"; // Remote path of the file

        //        //string localDirectory = Path.GetDirectoryName(localFilePath);
        //        if (!Directory.Exists(tempDirectory))
        //        {
        //            Directory.CreateDirectory(tempDirectory);
        //            Common.LogAction("DownloadedFile path Created");
        //        }
        //        else
        //        {
        //            Common.LogAction("DownloadedFile path Available");
        //        }

        //        if (!Directory.Exists(finalDirectory))
        //        {
        //            Directory.CreateDirectory(finalDirectory);
        //            Common.LogAction("Processed path Created");
        //        }

        //        // Connect to the SFTP server and download the file
        //        using (var sftp = new SftpClient(host, port, username, password))
        //        {
        //            sftp.Connect();

        //            // List files in the remote directory
        //            var fileList = sftp.ListDirectory(destination)
        //                                .Where(file => !file.Name.StartsWith(".") && file.IsRegularFile)
        //                                .OrderByDescending(file => file.LastWriteTime)
        //                                .ToList();

        //            // Select the file based on specific criteria (e.g., most recent)
        //            var fileToDownload = fileList.FirstOrDefault();

        //            if (fileToDownload == null)
        //            {
        //                errmsg = "No files found in the remote directory.";
        //                return false;
        //            }

        //            // Local path where the file will be saved
        //            string localFilePath = Path.Combine(tempDirectory, fileToDownload.Name);

        //            // Download the selected file to the temporary folder
        //            using (var fileStream = new FileStream(localFilePath, FileMode.Create))
        //            {
        //                sftp.DownloadFile(fileToDownload.FullName, fileStream);
        //            }

        //            sftp.Disconnect();

        //            // Check if the file already exists in the final directory
        //            string finalFilePath = Path.Combine(finalDirectory, fileToDownload.Name);
        //            if (File.Exists(finalFilePath))
        //            {
        //                // Rename the file by appending a timestamp
        //                string newFileName = Path.GetFileNameWithoutExtension(fileToDownload.Name) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(fileToDownload.Name);
        //                finalFilePath = Path.Combine(finalDirectory, newFileName);
        //            }

        //            // Move the downloaded file to the final folder
        //            File.Move(localFilePath, finalFilePath);  // Move the file to the processed folder

        //            Common.LogAction($"File moved to: {finalFilePath}");

        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        errmsg = $"Error: {ex.Message}";
        //        return false;
        //    }
        //}


        //private bool DownloadFileFromFTP(ref string errmsg, string fileType)
        //{
        //    string appFilesPath = ConfigurationManager.AppSettings["AppFilesPath"].ToString();
        //    try
        //    {
        //        // Retrieve the FTP configuration from the database based on the FileType
        //        string host = string.Empty, username = string.Empty, password = string.Empty, destination = string.Empty;
        //        int port = 0;
        //        string connectionString = ConnectionFunctions.GetConnectionString();
        //        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        //        {
        //            sqlConnection.Open();
        //            using (SqlCommand command = new SqlCommand("SELECT Destination, Host, UserName, Password, Port FROM EFTSFTP WHERE FileType = @FileType", sqlConnection))
        //            {
        //                command.Parameters.AddWithValue("@FileType", fileType);

        //                SqlDataReader reader = command.ExecuteReader();
        //                if (reader.Read())
        //                {
        //                    destination = reader["Destination"].ToString();
        //                    host = reader["Host"].ToString();
        //                    username = reader["UserName"].ToString();
        //                    password = reader["Password"].ToString();
        //                    port = Convert.ToInt32(reader["Port"]);

        //                    // Default to port 21 if port is 0 or invalid (standard FTP port)
        //                    if (port == 0)
        //                    {
        //                        port = 21;
        //                    }
        //                }
        //                else
        //                {
        //                    errmsg = $"No FTP configuration found for FileType: {fileType}.";
        //                    return false;
        //                }
        //            }
        //        }

        //        // Local file path where the file will be saved
        //        //string tempDirectory = appFilesPath + "DownloadedFile\\Temp\\"; // Adjust the path as needed
        //        //string finalDirectory = appFilesPath + "DownloadedFile\\Processed\\";  // Folder where the file will be moved
        //        // Directories for temporary and final storage
        //        string tempDirectory = Path.Combine(appFilesPath, "DownloadedFile\\Temp\\");
        //        string finalDirectory = Path.Combine(appFilesPath, "DownloadedFile\\Processed\\");


        //        //string remoteFilePath = "/SFTPTest/EmployeeFile_06_12_2024_13_47_13.csv"; // Remote path of the file

        //        //string localDirectory = Path.GetDirectoryName(localFilePath);
        //        if (!Directory.Exists(tempDirectory))
        //        {
        //            Directory.CreateDirectory(tempDirectory);
        //            Common.LogAction("DownloadedFile path Created");
        //        }
        //        else
        //        {
        //            Common.LogAction("DownloadedFile path Available");
        //        }

        //        if (!Directory.Exists(finalDirectory))
        //        {
        //            Directory.CreateDirectory(finalDirectory);
        //            Common.LogAction("Processed path Created");
        //        }

        //        // File name filters
        //        string[] fileNameFilters = { "EmployeeFile", "EoS", "FinancialChangeFile", "FinancialFileNewHire" };

        //        // Connect to the FTP server
        //        using (var ftp = new FtpClient(host, username, password, port))
        //        {
        //            ftp.Connect();

        //            // Retrieve the file list from the remote directory
        //            var fileList = ftp.GetListing(destination, FtpListOption.AllFiles)
        //          .Where(file => file.Type == FtpObjectType.File && // Check if the item is a file
        //                         !file.Name.StartsWith(".") && // Ignore hidden/system files
        //                         fileNameFilters.Any(filter => file.Name.Contains(filter)))
        //          .OrderByDescending(file => file.Modified)
        //          .ToList();

        //            if (!fileList.Any())
        //            {
        //                errmsg = "No matching files found in the remote directory.";
        //                return false;
        //            }
        //            //var fileList = ftp.GetListing(destination, FtpListOption.AllFiles);
        //            //var fileToDownload = fileList.OrderByDescending(file => file.Modified).FirstOrDefault();
        //            //if (fileToDownload == null)
        //            //{
        //            //    errmsg = "No files found in the remote directory.";
        //            //    return false;
        //            //}

        //            foreach (var fileToDownload in fileList)
        //            {
        //                // Local file path
        //                string localFilePath = Path.Combine(tempDirectory, fileToDownload.Name);

        //                // Download the file to the local directory
        //                using (var fileStream = new FileStream(localFilePath, FileMode.Create))
        //                {
        //                    ftp.DownloadStream(fileStream, fileToDownload.FullName);
        //                }

        //                Common.LogAction($"File downloaded: {fileToDownload.Name}");

        //                //string localFilePath = Path.Combine(tempDirectory, fileToDownload.Name);

        //                //// Download the file to the local directory
        //                //using (var fileStream = new FileStream(localFilePath, FileMode.Create))
        //                //{
        //                //    ftp.DownloadStream(fileStream, fileToDownload.FullName); //remoteFilePath

        //                //}

        //                // Decrypt the file and move to the final directory
        //                //string decryptedFileName = Path.GetFileNameWithoutExtension(fileToDownload.Name); // Removes .gpg
        //                //string decryptedFilePath = Path.Combine(finalDirectory, decryptedFileName); // Save the decrypted file without .gpg

        //                string decryptedFilePath = Path.Combine(finalDirectory, fileToDownload.Name);

        //                FileCryptographyUtil.DecryptFile(localFilePath, decryptedFilePath, ref errmsg);


        //                if (File.Exists(decryptedFilePath))
        //                {
        //                    // Handle duplicate files in the final directory by appending a timestamp
        //                    string newFileName = Path.GetFileNameWithoutExtension(decryptedFilePath) + "_" +
        //                                         DateTime.Now.ToString("yyyyMMdd_HHmmss") +
        //                                         Path.GetExtension(decryptedFilePath);

        //                    decryptedFilePath = Path.Combine(finalDirectory, newFileName);
        //                }

        //                // After decryption, move the file to the appropriate folder based on the file name
        //                bool result = DecryptAndMoveFileBasedOnName(localFilePath, decryptedFilePath, ref errmsg);

        //                if (!result)
        //                {
        //                    Common.LogAction($"Decrypted file Not moved: {errmsg}"); 
        //                    return false;
        //                }
        //                // Move the decrypted file to the final folder
        //                // File.Move(localFilePath, decryptedFilePath);

        //                Common.LogAction($"Decrypted file moved to: {decryptedFilePath}");
        //                ftp.Disconnect();
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        errmsg = $"Error: {ex.Message}";
        //        return false;
        //    }
        //}
        #endregion

        #region WindowsActionButtons
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }




        #endregion

    }
}

