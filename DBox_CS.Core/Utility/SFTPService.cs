using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;

namespace DBox_CS.Core.Utility
{
    public class SFTPService
    {
        private bool RetVal = false;
        string errmsg = "";
        String sQry = String.Empty;
        private int result = 0;

        public bool DownloadFilesFromSFTP(string ftpConfigFileTypeId, string localFolderPath, ref string errmsg, bool bMovetoArchive=false, string archiveFolder="")
        {
            errmsg = "";
            try
            {
                Common.LogAction("Downloading File from SFTP");
                string source, destination, RemoteDestination, LocalDestination, host, username, password;
                source = destination = RemoteDestination = LocalDestination = host = username = password = "";
                int port = 0;
                int nFilecnt = 0;
                string archiveremotePath = "";
                LocalDestination = localFolderPath;
                DataTable dtSFTP = new DataTable();

                #region fetch SFTP connection details
                sQry = "Select Destination, Host, UserName, Password, Port from EFTSFTP WITH (NOLOCK) Where FileType = '" + ftpConfigFileTypeId + "'";
                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref dtSFTP, sQry, ref errmsg);

                if (RetVal == false)
                {
                    errmsg = "SFTP details fetching failed. Error: " + errmsg;
                    return false;
                }

                if (dtSFTP.Rows.Count > 0)
                {
                    source = dtSFTP.Rows[0]["Destination"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Destination"].ToString();
                    host = dtSFTP.Rows[0]["Host"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Host"].ToString();
                    username = dtSFTP.Rows[0]["UserName"] == DBNull.Value ? "" : dtSFTP.Rows[0]["UserName"].ToString();


                    //encrypted pass
                    password = dtSFTP.Rows[0]["Password"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Password"].ToString();

                    //decrypted pass
                    if (!string.IsNullOrEmpty(password))
                        password = cryptoutil.Decrypt(password);

                    port = Convert.ToInt32(dtSFTP.Rows[0]["Port"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Port"]);
                }
                else
                {
                    errmsg = "SFTP details not not configured..";
                    return false;
                }

                if(bMovetoArchive && string.IsNullOrEmpty(archiveFolder) )
                {
                    errmsg = "SFTP arhive folder name required to move files to archive";
                    return false;
                }
                if (bMovetoArchive)
                {
                    string trimmed = source.TrimEnd('/');
                    string[] parts = trimmed.Split('/');
                    parts[parts.Length - 1] = archiveFolder;
                    string result = string.Join("/", parts) + "/";
                    archiveremotePath = result;
                }



                #endregion



                #region Downlaod last written txt file at SFTP Source Folder whose name not matches with lastimportedfile
                using (SftpClient client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    Common.LogAction("SFTP connected");
                    client.ChangeDirectory(source);
                    Common.LogAction("SFTP Sourecdirectory changed");
                    var files = client.ListDirectory(source);
                    var lastWrittenSFTPFile = files.Where(f => f.Name.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(f => f.LastWriteTime).First();
                    Common.LogAction("SFTP Lastwritten file " + lastWrittenSFTPFile);
                    if (lastWrittenSFTPFile != null)
                    {
                        string remoteFileName = lastWrittenSFTPFile.Name;

                        using (Stream file1 = File.Create(LocalDestination + remoteFileName))
                        {
                            client.DownloadFile(source + remoteFileName, file1);
                            nFilecnt++;

                            Common.LogAction("SFTP File '"+ remoteFileName+"' downloaded");

                            if (bMovetoArchive)
                            {
                                client.RenameFile(source + remoteFileName, archiveremotePath + remoteFileName);

                                Common.LogAction("SFTP File '" + remoteFileName + "' moved to Archive folder");
                            }

                        }
                        Common.LogAction(nFilecnt.ToString() + " files got downloaded");

                    }

                }
                #endregion

                if (nFilecnt <= 0)
                {
                    errmsg = "No new file found on Source folder.";
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Common.LogAction(errmsg);
                Common.LogException(ex);
                return false;
            }
            finally
            {
                Common.LogAction("SFTP Download execution finished");
            }

        }

        public bool UploadFileToSFTP(string ftpConfigFileTypeId, string sfilePath, ref string errmsg)
        {

            errmsg = "";

            try
            {
                Common.LogAction("Uploading File " + sfilePath + " to SFTP");
                if (!File.Exists(sfilePath))
                {
                    errmsg="File not found for SFTP upload";
                    return false;
                }

                string source, destination, host, username, password;
                source = destination = host = username = password = "";
                int port = 0;
                source = sfilePath;

                ////destination = "/home/users/sftp.gisco/TEST/CivilSoft/Integration/GlInterface/Source/";
                //destination = "/home/users/sftp.gisco/PROD/CivilSoft/Integration/GlInterface/Source/";
                //host = "138.1.44.177";
                //username = "sftp.gisco";
                //password = @"Osi#Civilsoft@2024";
                //port = 5003;

                #region fetch SFTP connection details
                DataTable dtSFTP = new DataTable();

                sQry = "Select Destination, Host, UserName, Password, Port from EFTSFTP WITH (NOLOCK) Where FileType = '" + ftpConfigFileTypeId + "'";
                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref dtSFTP, sQry, ref errmsg);
                if (RetVal == false)
                {
                    errmsg = "SFTP details fetching failed. Error: " + errmsg;
                    return false;
                }
                if (dtSFTP.Rows.Count > 0)
                {
                    destination = dtSFTP.Rows[0]["Destination"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Destination"].ToString();
                    host = dtSFTP.Rows[0]["Host"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Host"].ToString();
                    username = dtSFTP.Rows[0]["UserName"] == DBNull.Value ? "" : dtSFTP.Rows[0]["UserName"].ToString();

                    //encrypted pass
                    password = dtSFTP.Rows[0]["Password"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Password"].ToString();

                    //decrypted pass
                    if (!string.IsNullOrEmpty(password))
                        password = cryptoutil.Decrypt(password);

                    port = Convert.ToInt32(dtSFTP.Rows[0]["Port"] == DBNull.Value ? "" : dtSFTP.Rows[0]["Port"]);
                }
                else
                {
                    errmsg = "SFTP details not not configured..";
                    Common.LogAction(errmsg);
                    return false;
                }

                #endregion


                #region Upload file to SFTP
                using (SftpClient client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    client.ChangeDirectory(destination);
                    using (FileStream fs = new FileStream(source, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(fs, Path.GetFileName(source));

                        Common.LogAction("File uploaded successfully to SFTP");
                    }
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                errmsg = "SFTP File Upload Error. Details: " + ex.Message;
                Common.LogAction(errmsg);
                Common.LogException(ex);
                return false;
            }
            finally
            {
                Common.LogAction("SFTP Upload execution finished");
            }

        }
    }
}
