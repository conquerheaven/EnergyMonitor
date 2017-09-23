using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EnergyMonitor.Controllers.Utils
{
    public class Pager
    {
        private int totalRows; // 总行数
        private int pageSize = 10; // 每页显示的行数
        private int totalPages; // 总页数
        private int currentPage; // 当前页数
        private int startRow; // 当前页在数据库中的起始行

        public Pager(int _currentPage, int _totalRowsOrTotalPages, bool isTotalRows)
        {
            pageSize = Util.PageSize;
            if (isTotalRows)
            {
                
                totalRows = _totalRowsOrTotalPages; // 获得总行数

                totalPages = totalRows / pageSize; // 计算总页数
                int mod = totalRows % pageSize;
                if (mod > 0)
                {
                    totalPages++;
                }
                currentPage = _currentPage;
                startRow = 0;
            }
            else
            {
                currentPage = _currentPage;
                startRow = (currentPage - 1) * pageSize;
                totalPages = _totalRowsOrTotalPages;
            }
        }

        public Pager(int _currentPage, int _totalRowsOrTotalPages)
        {
            pageSize = Util.PageSize;

            totalRows = _totalRowsOrTotalPages; // 获得总行数

            totalPages = totalRows / pageSize; // 计算总页数
            int mod = totalRows % pageSize;
            if (mod > 0)
            {
                totalPages++;
            }
            currentPage = _currentPage;
            startRow = 0;
        }

        public int TotalPages 
        {
            get 
            {
                return totalPages;
            }
        }

        public int PageSize
        {
            get
            {
                return pageSize;
            }
        }

        public int StartRow
        {
            get
            {
                return startRow;
            }
        }
    }
}
