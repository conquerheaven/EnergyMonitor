using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;
using System.Web;
using System.IO;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Controllers.Admin.Filters;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 能耗审计类
    /// </summary>
    [AdminFilter]
    public class EnergyComptrollerController : Controller
    {        

        public EnergyComptrollerController() { }
        /// <summary>
        /// 返回已经上传的审计文件列表
        /// </summary>
        /// <returns></returns>
        //public ActionResult Comptroller(string b)
        //{
        //    var wordPath = Server.MapPath("~/Content/uploads");
        //    List<string> fileList = (Util.GetDirFiles(wordPath, "*.doc*").Union(Util.GetDirFiles(wordPath, "*.pdf"))).ToList();
        //    ViewBag.fileListSize = fileList.Count();
        //    ViewBag.selectedWordName = null;
        //    ViewBag.previewFilePath = null;
        //    return View(fileList);
 
        //}

       // /// <summary>  
       ///// 上传的word文件预览事件  
       ///// </summary>  
       ///// <param name="uploadWord"></param>  
       ///// <returns></returns>  
       // public ActionResult Comptrolleringone(string wordNameTwo)  
       //{
       //    string wordName = wordNameTwo + ".doc";
       //    string wordPath = Path.Combine("~/Content/uploads/", wordName);
       //    var trueWordPath = Server.MapPath(wordPath);
       //    if (!System.IO.File.Exists(trueWordPath))
       //    {
       //        wordName = wordNameTwo + ".docx";
       //        wordPath = Path.Combine("~/Content/uploads/", wordName);
       //        trueWordPath = Server.MapPath(wordPath);
       //    };
       //     string wordFileName = Path.GetFileNameWithoutExtension(trueWordPath);
       //     string savePath = Server.MapPath("~/Content/uploads");           
       //     ApplicationClass word = new ApplicationClass();  
       //     Type wordType = word.GetType();  
       //     Word.Documents docs = word.Documents;  
       //     Type docsType = docs.GetType();
       //     Word.Document doc = (Word.Document)docsType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, docs, new Object[] { (object)trueWordPath, true, true });  
       //     Type docType = doc.GetType();  
       //     string strSaveFileName = Path.Combine(savePath , wordFileName + ".html");  
       //     object saveFileName = (object)strSaveFileName;

       //     docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, doc, new object[] { saveFileName, Word.WdSaveFormat.wdFormatDocument });
       //     docType.InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, doc, null);  
       //     wordType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);
       //     Process.Start(strSaveFileName);
       //     var wordUploadPath = Server.MapPath("~/Content/uploads");
       //     List<string> wordList = Util.GetDirFiles(wordUploadPath, "*.doc*").ToList();
       //     ViewBag.wordListSize = wordPath.Count();
       //     ViewBag.selectedWordName = wordFileName;
       //     return View(wordList);

       //}      

        /// <summary>  
        /// 上传的word文件预览事件  
        /// </summary>  
        /// <param name="uploadWord"></param>  
        /// <returns></returns>  
         [AuthenticationFilter]
        public ActionResult Comptroller()
        {
             var wordPath = Server.MapPath("~/Content/uploads");
            List<string> fileList = (Util.GetDirFiles(wordPath, "*.doc*").Union(Util.GetDirFiles(wordPath, "*.pdf"))).ToList();
            ViewBag.fileListSize = fileList.Count();
            ViewBag.previewTxt = null;
            //List<string> fileList = null;
            //ViewBag.fileListSize = 0;
            //if (wordNameTwo != "")
            //{
            //    string pdfName = wordNameTwo + ".swf";
            //    string pdfPath = Path.Combine("~/Content/previewPdfFiles/", pdfName);
            //    var truePdfPath = Server.MapPath(pdfPath);               
            //    ViewBag.selectedWordName = wordNameTwo;
            //    if (System.IO.File.Exists(truePdfPath))
            //    {                  
            //        ViewBag.previewFilePath = wordNameTwo + ".swf";
            //        return View(fileList);
            //    }
            //    else
            //    {
            //        ViewBag.selectedWordName = null;
            //        ViewBag.previewFilePath = null;
            //        return View(fileList);
            //    }
            //}
            //else
            //{
            //    ViewBag.selectedWordName = null;
            //    ViewBag.previewFilePath = null;
            //    return View(fileList);
            //}
            ViewBag.selectedWordName = null;
            ViewBag.previewFilePath = null;
            return View(fileList);
        }

        /// <summary>  
        /// 上传的word文件预览事件  
        /// </summary>  
        /// <param name="uploadWord"></param>  
        /// <returns></returns>  
        public ActionResult Comptrolleringone(string wordNameTwo)
        {
            var wordPath = Server.MapPath("~/Content/uploads");
            List<string> fileList = (Util.GetDirFiles(wordPath, "*.doc*").Union(Util.GetDirFiles(wordPath, "*.pdf"))).ToList();
            ViewBag.fileListSize = fileList.Count();
            if (wordNameTwo != "")
            {
                string pdfName = wordNameTwo + ".swf";
                string txtName = wordNameTwo + ".txt";
                string pdfPath = Path.Combine("~/Content/previewPdfFiles/", pdfName);
                string txtPath = Path.Combine("~/Content/uploads/", txtName);
                var truePdfPath = Server.MapPath(pdfPath);
                var trueTxtPath = Server.MapPath(txtPath);
                ViewBag.selectedWordName = wordNameTwo;               
                if (System.IO.File.Exists(truePdfPath))
                {
                    if (System.IO.File.Exists(trueTxtPath))
                    {
                        FileStream fs = System.IO.File.Open(trueTxtPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                        StreamReader sr = new StreamReader(fs, Encoding.UTF8);                        
                        String line = "";
                        String txtContent = "";
                        int a = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (a != 0)
                            {
                                txtContent = txtContent + "\\n" + line;
                            }
                            else {
                                txtContent = txtContent + line;
                                a++;
                            }
                        }
                        ViewBag.previewTxt = txtContent;
                        sr.Close();
                        fs.Close();
                    }
                    ViewBag.previewFilePath = wordNameTwo + ".swf";
                    return View(fileList);
                }
                else
                {
                    ViewBag.previewTxt = null;
                    ViewBag.selectedWordName = null;
                    ViewBag.previewFilePath = null;
                    return View(fileList);
                }
            }
            else
            {
                ViewBag.selectedWordName = null;
                ViewBag.previewFilePath = null;
                return View(fileList);
            }

        }  

        /// <summary>
        /// 根据指定的名称下载导入的能耗审计文件
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadImportWord(string wordNameOne)
        {           
            string wordName = wordNameOne+".doc";
            string wordPath = Path.Combine("~/Content/uploads/",wordName);
            var trueWordPath = Server.MapPath(wordPath);         
             if (!System.IO.File.Exists(trueWordPath)) 
            {
                wordName = wordNameOne + ".docx";
                wordPath = Path.Combine("~/Content/uploads/", wordName);
                trueWordPath = Server.MapPath(wordPath);
                if (!System.IO.File.Exists(trueWordPath))
                {
                    wordName = wordNameOne + ".pdf";
                    wordPath = Path.Combine("~/Content/uploads/", wordName);
                    trueWordPath = Server.MapPath(wordPath);

                };
            };
            return File(trueWordPath, "application/vnd.ms-word", wordName);
        }

        /// <summary>
        /// 根据指定的名称下载导入的能耗审计文件
        /// </summary>
        /// <returns></returns>
        public ActionResult ComptrollerDelete(string wordNameThree)
        {          
            var filePath = Server.MapPath("~/Content/uploads");           
            string pdfPath = Path.Combine("~/Content/previewPdfFiles/", wordNameThree+".pdf");
            string wordDocPath = Path.Combine("~/Content/uploads", wordNameThree + ".doc");
            string wordDocxPath = Path.Combine("~/Content/uploads", wordNameThree + ".docx");
            string uploadPdfPath = Path.Combine("~/Content/uploads", wordNameThree + ".pdf");
            string swfPath = Path.Combine("~/Content/previewPdfFiles/", wordNameThree + ".swf");
            var truePdfPath = Server.MapPath(pdfPath);
            var trueWordDocPath = Server.MapPath(wordDocPath);
            var trueWordDocxPath = Server.MapPath(wordDocxPath);
            var trueUploadPdfPath = Server.MapPath(uploadPdfPath);
            var trueSwfPath = Server.MapPath(swfPath);           
            if (System.IO.File.Exists(truePdfPath))
            {
                System.IO.File.Delete(truePdfPath);
                ViewBag.importedPointCount = 1;
            }
            else
            {
                Console.WriteLine( truePdfPath+"文件不存在");
            }
            if (System.IO.File.Exists(trueWordDocPath))
            {
                System.IO.File.Delete(trueWordDocPath);
                ViewBag.importedPointCount = 1;
            }
            else
            {
                Console.WriteLine(truePdfPath + "文件不存在");
            }
            if (System.IO.File.Exists(trueWordDocxPath))
            {
                System.IO.File.Delete(trueWordDocxPath);
                ViewBag.importedPointCount = 1;
            }
            else
            {
                Console.WriteLine(truePdfPath + "文件不存在");
            }
            if (System.IO.File.Exists(trueUploadPdfPath))
            {
                System.IO.File.Delete(trueUploadPdfPath);
                ViewBag.importedPointCount = 1;
            }
            else
            {
                Console.WriteLine(truePdfPath + "文件不存在");
            }
            if (System.IO.File.Exists(trueSwfPath)) 
            {
                System.IO.File.Delete(trueSwfPath);
                ViewBag.importedPointCount = 1;
             }
            else
            {
                Console.WriteLine(trueSwfPath + "文件不存在");
            }
            ViewBag.previewTxt = null;
            return View();
        }

        /// <summary>
        /// 上传能耗审计文件（.doc，.docx或.pdf后缀）
        /// </summary>
        /// <param name="uploadWord"></param>
        /// <returns></returns>
        public ActionResult Comptrollering(HttpFileCollectionBase uploadWord, string remark)
        {          
            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasFile())
                {
                    continue;
                }
                string uploadsPath = Server.MapPath("~/Content/uploads/");
                string tempFileName = Request.Files[upload].FileName;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(tempFileName);
                if (remark != null)
                {
                    string fullPath = Path.Combine(uploadsPath, fileNameWithoutExtension + ".txt");
                    if (!System.IO.File.Exists(fullPath))
                    {                        
                        FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine(remark);
                        sw.Close();
                        fs.Close();
                    }
                    else {
                        FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Write);
                        StreamWriter sr = new StreamWriter(fs);
                        sr.WriteLine(remark);
                        sr.Close();
                        fs.Close();
                    }                   
                }
               
                string fileName = Path.GetFileName(tempFileName);
                string fileExtension = Path.GetExtension(tempFileName);
                // 判断是否上传的后缀是否存在
                string uploadExcelExtens = Util.GetConfigValue("uploadWordExtens");
                string[] uploadExcelExtensions = uploadExcelExtens.Split(new char[] { ',' });
                string preExtension = "";
                if (fileExtension.Length > 0)
                {
                    preExtension = fileExtension.Substring(1);
                }
                if (!uploadExcelExtensions.Contains(preExtension))
                {
                    continue;
                }               
                string previewPdfFilesPath = Server.MapPath("~/Content/previewPdfFiles/");
                string fullTempFilePath = Path.Combine(uploadsPath, fileName);
                string pdfSavePath = Path.Combine(previewPdfFilesPath, fileNameWithoutExtension+".pdf");
                Request.Files[upload].SaveAs(fullTempFilePath);
                if (preExtension.Equals("pdf"))
                {
                    Request.Files[upload].SaveAs(pdfSavePath);
                }else{
                   Util.DOCConvertToPDF(fullTempFilePath, pdfSavePath);//转换为pdf为预览做准备                  
                }
                if (System.IO.File.Exists(pdfSavePath))
                {
                    string savePath = Server.MapPath("~/Content/previewPdfFiles");
                    string swfSavePath = Path.Combine(savePath, fileNameWithoutExtension + ".swf");
                    Util.PDFConvertToSWF(pdfSavePath, swfSavePath);
                    ViewBag.importedPointCount = 1;
                }
                else {
                    Console.WriteLine("没有生成pdf文件所以无法生成相应的swf文件");
                }
            }         
            return View();
        }
    }
}
