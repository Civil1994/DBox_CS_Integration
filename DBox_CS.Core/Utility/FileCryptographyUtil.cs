using DBox_CS.Core.AppClass;
using Starksoft.Aspen.GnuPG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Utility
{
    public class FileCryptographyUtil
    {
        #region PGP Encrypt&Decrypt

        const string strPassPhrase = "dbssfi01012025";

        ////Test server keys
        //static string privateKeyPath = ConfigurationManager.AppSettings["SecureFilesPath"].ToString() + "Keys\\cs_dbs_private_03012025.asc";
        //static string publicKeyPath = ConfigurationManager.AppSettings["SecureFilesPath"].ToString() + "Keys\\cs_dbs_public_03012025.asc";

        //PROD server keys
        static string privateKeyPath = ConfigurationManager.AppSettings["SecureFilesPath"].ToString() + "Keys\\cs_dbs_PROD_private_05022025.asc";
        static string publicKeyPath = ConfigurationManager.AppSettings["SecureFilesPath"].ToString() + "Keys\\cs_dbs_PROD_public_05022025.asc";

        static string ExeGpgPath = ConfigurationManager.AppSettings["GPGFilePath"].ToString() + "gpg.exe";
        #region new method not working
        public static bool DecryptFile(string inputFilePath, string outputFilePath, ref string ErrMsg)
        {
            try
            {
                // Check if required files exist
                if (!File.Exists(inputFilePath))
                {
                    ErrMsg = "Input file does not exist.";
                    Common.LogAction(ErrMsg);
                    return false;
                }

                if (!File.Exists(ExeGpgPath))
                {
                    ErrMsg = "GPG executable not found.";
                    Common.LogAction(ErrMsg);
                    return false;
                }

                // Construct the GPG decryption process
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ExeGpgPath, // Path to gpg.exe
                        Arguments = $"--batch --yes --passphrase \"{strPassPhrase}\" --pinentry-mode loopback -d \"{inputFilePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                // Start the process
                process.Start();

                // Capture decrypted output and error messages
                string decryptedOutput = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();

                // Wait for process to exit
                process.WaitForExit();

                // Check for errors
                if (process.ExitCode != 0)
                {
                    Common.LogAction($"GPG Decryption failed: {errorOutput}");
                    throw new Exception($"GPG Decryption failed: {errorOutput}");
                }

                // Write the decrypted output to the specified output file
                File.WriteAllText(outputFilePath, decryptedOutput);
                Common.LogAction($"GPG Decryption Success: {decryptedOutput}");
                return true;
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
                ErrMsg = $"Decryption failed: {ex.Message}";
                Common.LogAction(ErrMsg);
                return false;
            }
        }

        #endregion
        #region Old Working Method for Decryption
        //public static bool DecryptFile(string inputFilePath, string outputFilePath, ref string ErrMsg)
        //{
        //    //const string inputFilePath = "C:\\Jitendra\\SF_CS Integration\\Files\\Encrypted File\\ForEncryption.txt.gpg";
        //    //const string outputFilePath = "C:\\Jitendra\\SF_CS Integration\\Files\\Encrypted File\\ForEncryption2.txt";

        //    //const string ExeGpgPath = "C:\\Jitendra\\SF_CS Integration\\SF_CS_Integration\\SF_CS.IntegrationApp\\GpgFiles\\gpg.exe";

        //    try
        //    {
        //        // Check if required files exist
        //        if (!File.Exists(inputFilePath))
        //        {
        //            ErrMsg = "Input file does not exist.";
        //            return false;
        //        }
        //        if (!File.Exists(privateKeyPath))
        //        {
        //            ErrMsg = "Private key file does not exist.";
        //            return false;
        //        }
        //        if (!File.Exists(ExeGpgPath))
        //        {
        //            ErrMsg = "GPG executable not found.";
        //            return false;
        //        }
        //        // PGP Decryption Code Start
        //        Starksoft.Aspen.GnuPG.Gpg gpg = new Gpg();
        //        string privateKeyFileName, encryptedFile, decryptedFile, passphrase;
        //        privateKeyFileName = encryptedFile = decryptedFile = passphrase = "";

        //        // Specify the private key file
        //        privateKeyFileName = privateKeyPath;

        //        // Specify the encrypted file
        //        encryptedFile = inputFilePath;

        //        // Specify the output for the decrypted file
        //        decryptedFile = outputFilePath;

        //        // Specify the passphrase for the private key
        //        passphrase = strPassPhrase;

        //        // Create streams for the encrypted input file and the decrypted output file
        //        using (Stream privateKeyStream = new FileStream(privateKeyFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {
        //            using (Stream encryptedFileStream = new FileStream(encryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //            {

        //                //  make sure the stream is at the start.
        //                encryptedFileStream.Position = 0;

        //                using (Stream decryptedFileStream = new FileStream(decryptedFile, FileMode.Create))
        //                {
        //                    // Specify the directory containing gpg.exe
        //                    gpg.BinaryPath = ExeGpgPath;

        //                    // Import the private key
        //                    gpg.Import(privateKeyStream);

        //                    // Set the passphrase for the private key
        //                    gpg.Passphrase = passphrase;

        //                    // Perform decryption
        //                    gpg.Decrypt(encryptedFileStream, decryptedFileStream);

        //                    // Dispose of GPG resources
        //                    //gpg.Dispose();
        //                    //gpg = null;
        //                }

        //            }

        //        }
        //        // PGP Decryption Code End

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogException(ex);

        //        ErrMsg = $"Decryption failed: { ex.Message}";
        //        return false;
        //    }
        //}
        #endregion

        public static bool EncryptFile(string inputFilePath, string outputFilePath, ref string ErrMsg)
        {
            //const string inputFilePath = "C:\\Jitendra\\SF_CS Integration\\Files\\Encrypted File\\ForEncryption.txt";
            //const string outputFilePath = "C:\\Jitendra\\SF_CS Integration\\Files\\Encrypted File\\ForEncryption.txt.gpg";
            //const string publicKeyPath = "C:\\Jitendra\\SF_CS Integration\\ForPGP\\cs_dbs_public_03012025.asc";
            //const string ExeGpgPath = "C:\\Jitendra\\SF_CS Integration\\SF_CS_Integration\\SF_CS.IntegrationApp\\GpgFiles\\gpg.exe";

            try
            {
                // PGP Decryption Code Start
                Starksoft.Aspen.GnuPG.Gpg gpg = new Gpg();
                string publicKeyFileName, encryptedFile, decryptedFile;
                publicKeyFileName = encryptedFile = decryptedFile = "";

                // Specify the private key file
                publicKeyFileName = publicKeyPath;

                // Specify the encrypted file
                encryptedFile = outputFilePath;

                // Specify the output for the decrypted file
                decryptedFile = inputFilePath;


                // Create streams for the encrypted input file and the decrypted output file
                using (Stream publicKeyStream = new FileStream(publicKeyFileName, FileMode.Open))
                {
                    using (Stream decryptedFileStream = new FileStream(decryptedFile, FileMode.Open))
                    {

                        ////  make sure the stream is at the start.
                        //encryptedFileStream.Position = 0;

                        using (Stream encryptedFileStream = new FileStream(encryptedFile, FileMode.Create))
                        {
                            // Specify the directory containing gpg.exe
                            gpg.BinaryPath = ExeGpgPath;

                            gpg.Recipient = "support@civilsoft.net";
                            // Import the private key
                            gpg.Import(publicKeyStream);

                            // Set the passphrase for the private key
                            // gpg.Passphrase = passphrase;

                            // Perform decryption
                            gpg.Encrypt(decryptedFileStream, encryptedFileStream);

                            // Dispose of GPG resources
                            //gpg.Dispose();
                            //gpg = null;
                        }

                    }

                }
                // PGP Decryption Code End

                //Console.WriteLine($"File successfully Encrypted to: {outputFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Encryption failed: {ex.Message}");
                //MessageBox.Show($"Encryption failed: { ex.Message}");

                Common.LogException(ex);

                ErrMsg = $"Encryption failed: { ex.Message}";
                return false;
            }
        }
        #endregion
    }
}
