using System;
using System.Web.Mvc;
using System.Data.Linq;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;

namespace EnergyMonitor.Controllers.Utils
{
    /// <summary>
    /// 控制类生成Excel扩展方法
    /// </summary>
    public static class ExcelControllerExtensions
    {
        /// <summary>
        /// 根据数据源中的数据生成表头导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IQueryable格式数据源</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IQueryable rows,
            string fileName
        )
        {
            return new ExcelResult(rows, fileName);
        }

        /// <summary>
        /// 根据数据源中的数据生成表头导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IList格式数据源</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IList rows,
            string fileName
        )
        {
            return new ExcelResult(rows.AsQueryable(), fileName);
        }

        /// <summary>
        /// 根据表头导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IQueryable格式数据源</param>
        /// <param name="fileName">文件名</param>
        /// <param name="headers">表头</param>
        /// <remarks>表头长度必须同数据源中的属性个数一致</remarks>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers
        )
        {
            return new ExcelResult(rows, fileName, headers);
        }

        /// <summary>
        /// 根据表头导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IList格式数据源</param>
        /// <param name="fileName">文件名</param>
        /// <param name="headers">表头</param>
        /// <remarks>表头长度必须同数据源中的属性个数一致</remarks>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IList rows,
            string fileName,
            string[] headers
        )
        {
            return new ExcelResult(rows.AsQueryable(), fileName, headers);
        }

        /// <summary>
        /// 由数据表头属性导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IQueryable格式数据源</param>
        /// <param name="fileName">生成文件名</param>
        /// <param name="headers">表头</param>
        /// <param name="properties">数据源中字段属性，必须匹配真实属性</param>
        /// <remarks>表头个数必须与属性个数相同</remarks>
        /// <returns>生成的Excel文件流</returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers,
            string[] properties
        )
        {
            return new ExcelResult(rows, fileName, headers, properties);
        }

        /// <summary>
        /// 由数据表头属性导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IList格式数据源</param>
        /// <param name="fileName">生成文件名</param>
        /// <param name="headers">表头</param>
        /// <param name="properties">数据源中字段属性，必须匹配真实属性</param>
        /// <remarks>表头个数必须与属性个数相同</remarks>
        /// <returns>生成的Excel文件流</returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IList rows,
            string fileName,
            string[] headers,
            string[] properties
        )
        {
            return new ExcelResult(rows.AsQueryable(), fileName, headers, properties);
        }

        /// <summary>
        /// 由数据表头属性导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows"></param>
        /// <param name="fileName"></param>
        /// <param name="title">标题</param>
        /// <param name="headers"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IList rows,
            string fileName,
            string title,
            string[] headers,
            string[] properties
        )
        {
            return new ExcelResult(rows.AsQueryable(), fileName, headers, properties, title);
        }

        /// <summary>
        /// 根据制定属性导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IQueryable格式数据源</param>
        /// <param name="fileName">文件名</param>
        /// <param name="headers">表头</param>
        /// <param name="properties">属性</param>
        /// <param name="tableStyle">表格样式</param>
        /// <param name="headerStyle">表头样式</param>
        /// <param name="itemStyle">每列样式</param>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers,
            string[] properties,
            TableStyle tableStyle,
            TableItemStyle headerStyle,
            TableItemStyle itemStyle
        )
        {
            return new ExcelResult(rows, fileName, headers, properties, null, tableStyle, headerStyle, itemStyle);
        }

        /// <summary>
        /// 根据制定属性导出Excel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="rows">IList格式数据源</param>
        /// <param name="fileName">文件名</param>
        /// <param name="headers">表头</param>
        /// <param name="properties">属性</param>
        /// <param name="tableStyle">表格样式</param>
        /// <param name="headerStyle">表头样式</param>
        /// <param name="itemStyle">每列样式</param>
        /// <returns></returns>
        public static ActionResult Excel
        (
            this Controller controller,
            IList rows,
            string fileName,
            string[] headers,
            string[] properties,
            TableStyle tableStyle,
            TableItemStyle headerStyle,
            TableItemStyle itemStyle
        )
        {
            return new ExcelResult(rows.AsQueryable(), fileName, headers, properties, null, tableStyle, headerStyle, itemStyle);
        }

    }
}
