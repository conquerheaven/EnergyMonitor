using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using EnergyMonitor.Controllers.Properties;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Diagnostics;


namespace EnergyMonitor.Controllers.Utils
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Util
    {
        private static Settings _defaultConfig = Settings.Default;

        /// <summary>
        /// 根据键获得值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>字符串值</returns>
        public static string GetConfigValue(string key)
        {
            return _defaultConfig[key] as string;
        }

        /// <summary>
        /// 根据键获得值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>对象值</returns>
        public static object GetConfigValueObj(string key)
        {
            return _defaultConfig[key];
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">接收者</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文</param>
        /// <returns></returns>
        public static bool SendMail(string mailTo, string subject, string body)
        {
            string mailServerName = GetConfigValue("mailServerName");
            string mailFrom = GetConfigValue("mailFrom");
            string mailUserName = GetConfigValue("mailUserName");
            string mailPassword = GetConfigValue("mailPassword");
            using (MailMessage message = new MailMessage(mailFrom, mailTo, subject, body))
            {
                message.IsBodyHtml = true;
                SmtpClient mailClient = new SmtpClient(mailServerName);
                mailClient.EnableSsl = true;
                mailClient.Credentials = new NetworkCredential(mailUserName, mailPassword);
                try
                {
                    mailClient.Send(message);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="mailTo">接收者</param>
        /// <param name="userID">用户ID</param>
        /// <param name="userName">接收者用户名</param>
        /// <param name="activeAction">激活Action</param>
        /// <param name="activeStr">激活码</param>
        /// <returns></returns>
        public static bool SendActiveMail(string activeWebSite, string mailTo, string userID, string userName,string activeAction, string activeStr)
        {
            string activeSubject = GetConfigValue("activeSubject");
            string activeBody = GetConfigValue("activeBody");
            if (activeWebSite == null)
            {
                activeWebSite = GetConfigValue("activeWebSite");
            }
            string activeUrl = activeWebSite + activeAction + "/" + userID + "/" + activeStr;
            activeBody = String.Format(activeBody, userName, activeUrl);
            return SendMail(mailTo, activeSubject, activeBody);
        }

        /// <summary>
        /// 得到页大小
        /// </summary>
        /// <param name="totalItem">总数据条数</param>
        /// <returns></returns>
        public static int GetTotalPage(int totalItem, int pageSize)
        {
            int totalPage = totalItem / pageSize;
            if (totalItem % pageSize != 0)
            {
                totalPage++;
            }
            return totalPage;
        }

        /// <summary>
        /// 解密发送来的字符串
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <returns>验证正确返回用户ID,否则返回null</returns>
        public static string DecryptStr(string decryptStr, string timeStamp)
        {
            string desKey = GetConfigValue("desKey").Substring(0,8);
            string desVector = GetConfigValue("desVector").Substring(0, 8);
            string decryptedStr = DecryptDES(decryptStr, desKey, desVector);
            if (decryptedStr != null && decryptedStr.Length > 40)
            {
                string msgAbstract = decryptedStr.Substring(0, 40);
                string userID = decryptedStr.Substring(40);
                string sourceStr = desKey + timeStamp + userID;
                string calMsgAbstract = SHA1Encrypt(sourceStr);
                if (msgAbstract == calMsgAbstract)
                {
                    return userID;
                }
            }
            return null;
        }

        /// <summary>
        /// 解密des字符串
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="decryptKey">密钥,必须为8个字符</param>
        /// <param name="decryptVector">密钥向量,必须为8个字符</param>
        /// <returns>解密后的字符串,错误返回null</returns>
        private static string DecryptDES(string decryptString, string decryptKey, string decryptVector)
        {
            try
            {
                byte[] inputByteArray = new byte[decryptString.Length / 2];
                for (int i = 0; i < inputByteArray.Length; i++)
                {
                    inputByteArray[i] = Convert.ToByte(decryptString.Substring(i * 2, 2), 16);
                }
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Encoding.UTF8.GetBytes(decryptVector);
                //临时密钥向量
                //byte[] rgbIV = { 0xfF, 0x4A, 0x33, 0x6B, 0xF2, 0x43, 0xDD, 0x17 };
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// SHA1加密字符串
        /// </summary>
        /// <param name="sourceStr">待加密字符串</param>
        /// <returns></returns>
        public static string SHA1Encrypt(string sourceStr)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(sourceStr);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            strRes = iSHA.ComputeHash(strRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        /// <summary>
        /// 得到页大小
        /// </summary>
        /// <returns></returns>
        public static int PageSize
        {
            get
            {
                return (int)Util.GetConfigValueObj("pageSize");
            }
        }

          /// <summary>
          /// 获得指定文件夹下指定格式的文件名
          /// </summary>
          /// <param name="DirFullPath"></param>
          /// <param name="SearchPattern"></param>
          /// <returns></returns>
        public static List<string> GetDirFiles(string DirFullPath, string SearchPattern)
        {
            if (Directory.Exists(DirFullPath) == true)
            {
                List<string> list = new List<string>();
                //获取当前目录下指定文件类型的文件列表
                string[] stringList = Directory.GetFiles(DirFullPath, SearchPattern);                    
                foreach (string str in stringList)
                {
                    string fileName;
                    fileName = System.IO.Path.GetFileNameWithoutExtension(str);
                    list.Add(fileName);

                }
                return list;
            }
            else
            {
                return null;
            }
        }


        /// Word转换成pdf
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <returns>true=转换成功</returns>
        public static bool DOCConvertToPDF(string sourcePath, string targetPath)
        {
            bool result = false;
            Word.WdExportFormat exportFormat = Word.WdExportFormat.wdExportFormatPDF;
            object paramMissing = Type.Missing;
            Word.ApplicationClass wordApplication = new Word.ApplicationClass();
            Word.Document wordDocument = null;
            try
            {
                object paramSourceDocPath = sourcePath;
                string paramExportFilePath = targetPath;
                Word.WdExportFormat paramExportFormat = exportFormat;
                bool paramOpenAfterExport = false;
                Word.WdExportOptimizeFor paramExportOptimizeFor = Word.WdExportOptimizeFor.wdExportOptimizeForPrint;
                Word.WdExportRange paramExportRange = Word.WdExportRange.wdExportAllDocument;
                int paramStartPage = 0;
                int paramEndPage = 0;
                Word.WdExportItem paramExportItem = Word.WdExportItem.wdExportDocumentContent;
                bool paramIncludeDocProps = true;
                bool paramKeepIRM = true;
                Word.WdExportCreateBookmarks paramCreateBookmarks = Word.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                bool paramDocStructureTags = true;
                bool paramBitmapMissingFonts = true;
                bool paramUseISO19005_1 = false;
                wordDocument = wordApplication.Documents.Open(
                    ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing);
                if (wordDocument != null)
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                        paramExportFormat, paramOpenAfterExport,
                        paramExportOptimizeFor, paramExportRange, paramStartPage,
                        paramEndPage, paramExportItem, paramIncludeDocProps,
                        paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                        paramBitmapMissingFonts, paramUseISO19005_1,
                        ref paramMissing);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                    wordDocument = null;
                }
                if (wordApplication != null)
                {
                    wordApplication.Quit(ref paramMissing, ref paramMissing, ref paramMissing);
                    wordApplication = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        /// <summary>
        /// Pdf2Swf 将pdf转化为swf
        /// </summary>
        public static void PDFConvertToSWF(string sourcePath, string targetPath)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe ";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string SWFTools = Util.GetConfigValue("SWFToolsPath");
            string backToRool = "cd " + SWFTools;
            p.StandardInput.WriteLine(backToRool);
            string drive= SWFTools.Substring(0,1);
            p.StandardInput.WriteLine(drive+":");            
            //string cmd = "pdf2swf.exe" + " -t " + sourcePath + " -s flashversion=9 -o " + targetPath;     
            //p.StandardInput.WriteLine(cmd); 
            string cmd1 = "pdf2swf.exe " + sourcePath + " -o " + targetPath + "  -f -T 9 -G -s poly2bitmap"; 
            p.StandardInput.WriteLine(cmd1);           
            p.Close();
        }
    }
}
