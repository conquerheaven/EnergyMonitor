using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;

 
namespace EnergyMonitor.Models.Repository.Implement
{
    public class DepartmentRepos : IDepartmentRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public DepartmentRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IDepartmentRepos Members

        /// <summary>
        /// 得到所有部门
        /// </summary>
        /// <returns></returns>
        public IList<DepartmentInfo> GetAllDepartments()
        {
            var list = from d in _dataContext.DepartmentInfos
                       select d;
            return list.ToList<DepartmentInfo>();
        }

        /// <summary>
        /// 根据部门名称模糊查询
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        public IQueryable<DepartmentInfo> GetDepartmentsByName(string departmentName)
        {
            return _dataContext.DepartmentInfos.Where(x => x.DI_Name.Contains(departmentName));
        }

        /// <summary>
        /// 根据部门名称查询
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        public IQueryable<DepartmentInfo> GetDepartmentsWithName(string departmentName)
        {
            return _dataContext.DepartmentInfos.Where(x => x.DI_Name == departmentName);
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public bool AddDepartment(DepartmentInfo department)
        {
            try
            {
                _dataContext.DepartmentInfos.InsertOnSubmit(department);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据部门ID获取部门
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public DepartmentInfo GetDepartmentByID(int departmentID)
        {
            return _dataContext.DepartmentInfos.Where(x => x.DI_ID == departmentID).FirstOrDefault();
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public bool ModifyDepartment(DepartmentInfo department)
        {
            try
            {
                DepartmentInfo oldDepartment = _dataContext.DepartmentInfos.Single(x => x.DI_ID == department.DI_ID);
                oldDepartment.DI_Name = department.DI_Name;
                oldDepartment.DI_LinkMan = department.DI_LinkMan;
                oldDepartment.DI_Remark = department.DI_Remark;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public bool DeleteDepartment(int departmentID)
        {
            try
            {
                _dataContext.DepartmentInfos.DeleteOnSubmit(_dataContext.DepartmentInfos.Single(x => x.DI_ID == departmentID));
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询部门的用户数
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public int QueryUserCountByDepart(int departmentID)
        {
            return _dataContext.Users.Where(x => x.USR_DepartID == departmentID).Count();
        }

        #endregion
    }
}
