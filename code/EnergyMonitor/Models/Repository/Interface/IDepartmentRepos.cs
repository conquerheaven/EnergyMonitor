using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IDepartmentRepos 
    {
        /// <summary>
        /// 得到所有部门
        /// </summary>
        /// <returns></returns>
        IList<DepartmentInfo> GetAllDepartments();

        /// <summary>
        /// 根据部门名称模糊查询
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        IQueryable<DepartmentInfo> GetDepartmentsByName(string departmentName);

        /// <summary>
        /// 根据部门名称查询
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        IQueryable<DepartmentInfo> GetDepartmentsWithName(string departmentName);

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        bool AddDepartment(DepartmentInfo department);

        /// <summary>
        /// 根据部门ID获取部门
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        DepartmentInfo GetDepartmentByID(int departmentID);

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        bool ModifyDepartment(DepartmentInfo department);

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        bool DeleteDepartment(int departmentID);

        /// <summary>
        /// 查询部门的用户数
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        int QueryUserCountByDepart(int departmentID);
    }
}
