using DBox_CS.Core.AppClass;
using DBox_CS.Core.Enums;
using DBox_CS.Core.Models;
using DBox_CS.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBox_CS.IntegrationService
{
    partial class Service1 : ServiceBase
    {
        
        private Timer Schedular;

       //private List<LocalFile> LocalFiles = new List<LocalFile>();
        static Dictionary<string, string> tempFolders;// List of folders to process files from
        static Dictionary<string, string> tempOutputFolders;// List of folders for output
        static string appFilesPath;
        static string decryptDirectory;
        static string sourceDirectory;
        static string sourceArchiveDirectory;
        static string tempDirectory;
        static string encryptTempDirectory;

        public Service1()
        {
            InitializeComponent();

            appFilesPath = ConfigurationManager.AppSettings["AppFilesPath"].ToString();
            sourceDirectory = ConfigurationManager.AppSettings["SourceFilePath"].ToString();
            sourceArchiveDirectory = ConfigurationManager.AppSettings["SourceArchiveFilePath"].ToString();
            decryptDirectory = Path.Combine(appFilesPath, "DownloadedFile\\Processed\\");
            tempDirectory = Path.Combine(appFilesPath, "DownloadedFile\\Temp\\");
            encryptTempDirectory = Path.Combine(appFilesPath, "DownloadedFile\\EncryptTemp\\");
            tempFolders = new Dictionary<string, string>
                {
                    { "emp",Path.Combine(appFilesPath, "EmployeeImport\\Temp\\") },
                    { "fin",Path.Combine(appFilesPath, "FinancialImport\\Temp\\") },
                    { "finc",Path.Combine(appFilesPath, "FinancialChangesImport\\Temp\\") },
                    { "eos",Path.Combine(appFilesPath, "EOSImport\\Temp\\") }
                };

            tempOutputFolders = new Dictionary<string, string>
                {
                    { "leave",Path.Combine(appFilesPath, "LeaveExport\\Temp\\") },
                };
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            this.ScheduleService();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            this.Schedular.Dispose();
        }


        public void ScheduleService()
        {
            //JRR Added Temp ScheduleService for Multiple Time Schedule.. Pls check if you want to change pls change as per you logic it Mr. Robin
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();

                DateTime scheduledTime = DateTime.MinValue;

                if (mode == "DAILY")
                {
                    // Get the Scheduled Times from AppSettings (comma-separated values)
                    string[] scheduledTimes = ConfigurationManager.AppSettings["ScheduledTimes"].Split(',');

                    List<DateTime> nextRunTimes = new List<DateTime>();

                    foreach (string time in scheduledTimes)
                    {
                        DateTime parsedTime = DateTime.Today.Add(TimeSpan.Parse(time.Trim())); // Set for today

                        if (DateTime.Now > parsedTime)
                        {
                            parsedTime = parsedTime.AddDays(1); // Move to tomorrow if already passed
                        }

                        nextRunTimes.Add(parsedTime);
                    }

                    // Get the next closest execution time
                    scheduledTime = nextRunTimes.OrderBy(t => t).First();
                    Common.LogAction($"Next Scheduled Execution At: {scheduledTime}");
                }
                else if (mode.ToUpper() == "INTERVAL")
                {
                    // Get the Interval in Minutes from AppSettings.
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);

                   // Common.LogAction($"Next Interval Execution At: {scheduledTime}");
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                if (!IsProcessExecution_inProgress)
                {
                    ExecuteUFIProcess(); // Run your process
                }
                else
                {
                    Common.LogAction("Skipped Scheduled Service as Manual Execution is in progress.");
                }

                // Reschedule for the next execution time
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
            }
            
        }

        private void SchedularCallback(object e)
        {
            this.ScheduleService();
        }


        private void ExecuteUFIProcess()
        {
            try
            {
                Main objMain = new Main(Main.ProcessIntitator.BackroudWindowService);
                objMain.appFilesPath = appFilesPath;
                objMain.tempOutputFolders = tempOutputFolders;

                string servicestoexecute = "";
                List<string> svcToExList = new List<string>();
                if(ConfigurationManager.AppSettings["ServicesToExecute"]!=null)
                {
                    servicestoexecute = ConfigurationManager.AppSettings["ServicesToExecute"].ToString(); //Valid values = "EmployeeExport,LeaveExport,DailyHoursImport"
                    if (!string.IsNullOrEmpty(servicestoexecute))
                    {
                        svcToExList = servicestoexecute.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(email => email.Trim().ToUpper())
                                    .ToList();
                    }
                }
                if(svcToExList.Count==0 || svcToExList.Contains("EMPLOYEEEXPORT"))
                {
                    //objMain.UploadeEmployeeToUF();
                    objMain.PushEmployeesToDBOX();
                }
                if (svcToExList.Contains("LEAVEEXPORT"))
                {
                    //objMain.UploadeLeaveToUF();
                }
                if (svcToExList.Contains("DAILYHOURSIMPORT"))
                {
                    //objMain.ImportDailyHoursFromUF();
                }
             

            }
            catch (Exception ex)
            {
                Common.LogAction($"An error occured at ExecuteUFIProcess. Details {ex.Message}");
                Common.LogException(ex);
                Console.WriteLine($"An error occured at ExecuteUFIProcess. Details {ex.Message}");
            }

        }





        #region TCP server Code
        //static Thread SocketThread;
        static bool IsProcessExecution_inProgress = false;
        //static System.Net.Sockets.TcpListener server;
        //static bool stopserver = false;
        //static Encoding encoding = Encoding.UTF8;
        //private void StartServer()
        //{
        //    try
        //    {
        //        stopserver = false;
        //        //System.Net.IPHostEntry ipHostInfo =  System.Net.Dns.GetHostEntry("localhost");
        //        //System.Net.IPAddress ipAddress = ipHostInfo.AddressList[1];

        //        if (server == null)
        //        {
        //            System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse("127.0.0.1");
        //            var ipEndPoint = new System.Net.IPEndPoint(ipAddress, 5555);
        //            server = new System.Net.Sockets.TcpListener(ipEndPoint);
        //        }
        //        server.Start();
        //        Common.LogAction("Start Server");

        //        // keep running
        //        while (!stopserver)
        //        {
        //            using (System.Net.Sockets.TcpClient clientsender = server.AcceptTcpClient())
        //            {
        //                string request = streamToMessage(clientsender.GetStream());

        //                if (request != null)
        //                {
        //                    //LogAction(string.Format("Client Sent message :{0}", request));
        //                    Common.LogAction(string.Format("Service process manually initiated"));

        //                    IsProcessExecution_inProgress = true;

        //                    try
        //                    {
        //                        StartEmployeeExportProcess();

        //                        ErrMsg = "1";//success msg
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ErrMsg = "0";//error msg

        //                    }
        //                    IsProcessExecution_inProgress = false;

        //                    sendMessage(ErrMsg, clientsender);
        //                }

        //                clientsender.Close();
        //            }

        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogException(ex);
        //    }
        //    finally
        //    {
        //        IsProcessExecution_inProgress = false;
        //        server.Stop();
        //        Common.LogAction("Stop Server");
        //    }
        //}

        //private void StopServer()
        //{
        //    Common.LogAction("Stop Server");
        //    try
        //    {
        //        stopserver = true;
        //        if (server != null)
        //        {
        //            server.Stop();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogAction("Stop Failed");
        //        Common.LogException(ex);
        //    }

        //}


        //private static void sendMessage(string message, System.Net.Sockets.TcpClient client)
        //{
        //    // messageToByteArray- discussed later
        //    byte[] bytes = messageToByteArray(message);
        //    client.GetStream().Write(bytes, 0, bytes.Length);
        //}

        //private static byte[] messageToByteArray(string message)
        //{
        //    // get the size of original message
        //    byte[] messageBytes = encoding.GetBytes(message);
        //    int messageSize = messageBytes.Length;
        //    // add content length bytes to the original size
        //    int completeSize = messageSize + 4;
        //    // create a buffer of the size of the complete message size
        //    byte[] completemsg = new byte[completeSize];

        //    // convert message size to bytes
        //    byte[] sizeBytes = BitConverter.GetBytes(messageSize);
        //    // copy the size bytes and the message bytes to our overall message to be sent 
        //    sizeBytes.CopyTo(completemsg, 0);
        //    messageBytes.CopyTo(completemsg, 4);
        //    return completemsg;
        //}

        //private static string streamToMessage(Stream stream)
        //{
        //    // size bytes have been fixed to 4
        //    byte[] sizeBytes = new byte[4];
        //    // read the content length
        //    stream.Read(sizeBytes, 0, 4);
        //    int messageSize = BitConverter.ToInt32(sizeBytes, 0);
        //    // create a buffer of the content length size and read from the stream
        //    byte[] messageBytes = new byte[messageSize];
        //    stream.Read(messageBytes, 0, messageSize);
        //    // convert message byte array to the message string using the encoding
        //    string message = encoding.GetString(messageBytes);
        //    string result = null;
        //    foreach (var c in message)
        //        if (c != '\0')
        //            result += c;

        //    return result;
        //}
        #endregion
    }
}
