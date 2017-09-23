using System;
using System.Web.Mvc;
using System.Data.Linq;
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Drawing;


namespace EnergyMonitor.Controllers.Utils
{
    /// <summary>
    /// 自定义返回Excel类
    /// </summary>
    public class ExcelResult : ActionResult
    {
        private string _fileName;
        private IQueryable _rows;
        private string[] _headers = null;
        private string[] _properties = null;
        private string _title = null;

        private TableStyle _tableStyle;
        private TableItemStyle _headerStyle;
        private TableItemStyle _itemStyle;

        public string FileName
        {
            get { return _fileName; }
        }

        public IQueryable Rows
        {
            get { return _rows; }
        }


        public ExcelResult(IQueryable rows, string fileName)
            : this(rows, fileName, null, null, null, null, null, null)
        {

        }

        public ExcelResult(IQueryable rows, string fileName, string[] headers)
            : this(rows, fileName, null, null, null, null, null, null)
        {

        }

        public ExcelResult(IQueryable rows, string fileName, string[] headers, string[] properties)
            : this(rows, fileName, headers, properties, null, null, null, null)
        {

        }

        public ExcelResult(IQueryable rows, string fileName, string[] headers, string[] properties, string title)
            : this(rows, fileName, headers, properties, title, null, null, null)
        {

        }

        public ExcelResult(IQueryable rows, string fileName, string[] headers, string[] properties, string title, TableStyle tableStyle, TableItemStyle headerStyle, TableItemStyle itemStyle)
        {
            _rows = rows;
            _fileName = fileName;
            _headers = headers;
            _properties = properties;
            _title = title;

            _tableStyle = tableStyle;
            _headerStyle = headerStyle;
            _itemStyle = itemStyle;

            // provide defaults
            if (_tableStyle == null)
            {
                _tableStyle = new TableStyle();
                _tableStyle.BorderStyle = BorderStyle.Solid;
                _tableStyle.BorderColor = Color.Black;
                _tableStyle.BorderWidth = Unit.Parse("2px");
            }
            if (_headerStyle == null)
            {
                _headerStyle = new TableItemStyle();
                _headerStyle.BackColor = Color.LightGray;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            // Create HtmlTextWriter
            StringWriter sw = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(sw);

            // Create html tag
            tw.RenderBeginTag(HtmlTextWriterTag.Html);
            // Write head tag
            tw.Write("<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /></head>");
            // Create body tag
            tw.RenderBeginTag(HtmlTextWriterTag.Body);

            // Build HTML Table from Items
            if (_tableStyle != null)
                _tableStyle.AddAttributesToRender(tw);

            // Create table tag
            tw.RenderBeginTag(HtmlTextWriterTag.Table);

            // Generate headers from table
            if (_headers == null)
            {
                foreach (Object row in _rows)
                {
                    _headers = row.GetType().GetProperties().Select(m => m.Name).ToArray<string>();
                    break;
                }
            }

            // Create caption tag
            if (_title != null)
            {
                tw.RenderBeginTag(HtmlTextWriterTag.Caption);
                tw.Write(_title);
                tw.RenderEndTag();
            }
            
            // Create Header Row
            tw.RenderBeginTag(HtmlTextWriterTag.Thead);
            foreach (String header in _headers)
            {
                if (_headerStyle != null)
                    _headerStyle.AddAttributesToRender(tw);
                tw.RenderBeginTag(HtmlTextWriterTag.Th);
                //string encodeHeader = System.Web.HttpUtility.UrlEncode(header);
                tw.Write(header);
                tw.RenderEndTag();
            }
            tw.RenderEndTag();

            // Create Data Rows
            tw.RenderBeginTag(HtmlTextWriterTag.Tbody);
            foreach (Object row in _rows)
            {
                tw.RenderBeginTag(HtmlTextWriterTag.Tr);
                if (_properties != null)
                {
                    if (_properties.Length == _headers.Length)
                    {
                        foreach (string prop in _properties)
                        {
                            var obj = row.GetType().GetProperty(prop).GetValue(row, null);
                            string strValue = obj != null ? obj.ToString() : null;
                            //strValue = ReplaceSpecialCharacters(strValue);
                            if (_itemStyle != null)
                                _itemStyle.AddAttributesToRender(tw);
                            tw.RenderBeginTag(HtmlTextWriterTag.Td);
                            tw.Write(HttpUtility.HtmlEncode(strValue) + "&nbsp;");
                            tw.RenderEndTag();
                        }
                    }
                }
                else
                {
                    if (row.GetType().GetProperties().Count() == _headers.Length)
                    {
                        foreach (var prop in row.GetType().GetProperties())
                        {
                            string strValue = prop.GetValue(row, null).ToString();
                            //strValue = ReplaceSpecialCharacters(strValue);
                            if (_itemStyle != null)
                                _itemStyle.AddAttributesToRender(tw);
                            tw.RenderBeginTag(HtmlTextWriterTag.Td);
                            tw.Write(HttpUtility.HtmlEncode(strValue) + "&nbsp;");
                            tw.RenderEndTag();
                        }
                    }
                }
                tw.RenderEndTag();
            }
            tw.RenderEndTag(); // tbody

            tw.RenderEndTag(); // table

            tw.RenderEndTag(); // body
            tw.RenderEndTag(); // html
            WriteFile(_fileName, "application/ms-excel", sw.ToString());       
        }

        //private static string ReplaceSpecialCharacters(string value)
        //{
        //    value = value.Replace("’", "'");
        //    value = value.Replace("“", "\"");
        //    value = value.Replace("”", "\"");
        //    value = value.Replace("–", "-");
        //    value = value.Replace("…", "...");
        //    return value;
        //}

        private static void WriteFile(string fileName, string contentType, string content)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.HtmlEncode(fileName));
            context.Response.Charset = "utf-16";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = contentType;
            context.Response.Write(content);
            context.Response.End();
        }
    }
}
