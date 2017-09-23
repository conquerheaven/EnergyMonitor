using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Controllers.Utils
{
    public static class HtmlHelperExtensions
    {

        /// <summary>
        /// 生成所有菜单
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parentIndex">0表示用户模块，1表示管理员模块</param>
        /// <returns></returns>
        public static IHtmlString Nav(this HtmlHelper helper, int parentIndex)
        {
            IList<Function> navFunctionList = null;
            IList<Function> subFunctionList = null;
            if (parentIndex != 0)
            {
                navFunctionList = helper.ViewContext.HttpContext.Session["userNavFunctions"] as IList<Function>;
                subFunctionList = helper.ViewContext.HttpContext.Session["userSubFunctions"] as IList<Function>;
            }
            
            var sb = new StringBuilder();
            var currentNode = SiteMap.CurrentNode;
            bool currentSelectedFlag = true;
            if (currentNode != null)
            {
                if (currentNode.Title.Trim() == "")
                {
                    currentNode = currentNode.ParentNode;
                }
            }
            else
            {
                currentSelectedFlag = false;
            }
            var userNavNodes = SiteMap.RootNode.ChildNodes[parentIndex].ChildNodes;
            if (parentIndex == 0) //学生平台
            {
                foreach (SiteMapNode navNode in userNavNodes)
                {
                    sb.Append("<li>");
                    if (currentSelectedFlag && currentNode.ParentNode != null && navNode == currentNode.ParentNode)
                    {
                        sb.AppendFormat("<a class='expanded heading' href='{0}'>{1}</a>", navNode.Url, navNode.Title);
                        sb.Append("<ul class='navigation'>");
                        foreach (SiteMapNode node in navNode.ChildNodes)
                        {
                            if (currentSelectedFlag && node == currentNode)
                            {
                                sb.AppendFormat("<li class='heading selected'>{0}</li>", node.Title);
                                currentSelectedFlag = false;
                            }
                            else
                            {
                                sb.AppendFormat("<li><a href='{0}'>{1}</a></li>", node.Url, node.Title);
                            }
                        }
                        sb.Append("</ul>");
                    }
                    else
                    {
                        sb.AppendFormat("<a class='collapsed heading' href='{0}'>{1}</a>", navNode.Url, navNode.Title);
                    }
                    sb.Append("</li>");
                }
            }
            else //管理员平台
            {
                var urlPrefix = SiteMap.RootNode.ChildNodes[parentIndex].Url;
                foreach (SiteMapNode navNode in userNavNodes)
                {
                    var currentNavNode = navFunctionList.Where(x => x.FN_Name == navNode.Title).FirstOrDefault();
                    if (currentNavNode != null)
                    {
                        sb.Append("<li>");
                        if (currentSelectedFlag && currentNode.ParentNode != null && navNode == currentNode.ParentNode)
                        {
                            sb.AppendFormat("<a class='expanded heading'>{0}</a>", navNode.Title);
                            sb.Append("<ul class='navigation'>");
                            foreach (SiteMapNode node in navNode.ChildNodes)
                            {
                                var currentSubNode = subFunctionList.Where(x => x.FN_Name == node.Title).FirstOrDefault();
                                if (currentSubNode != null)
                                {
                                    if (currentSelectedFlag && node == currentNode)
                                    {
                                        sb.AppendFormat("<li class='heading selected'>{0}</li>", node.Title);
                                        currentSelectedFlag = false;
                                    }
                                    else
                                    {
                                        sb.AppendFormat("<li><a href='{0}'>{1}</a></li>", node.Url, node.Title);
                                    }
                                }
                            }
                            sb.Append("</ul>");
                        }
                        else
                        {
                            var navUrl = "#";
                            var firstSubNode = subFunctionList.Where(x => x.FN_ID.StartsWith(currentNavNode.FN_ID)).FirstOrDefault();
                            if(firstSubNode!=null)
                            {
                                navUrl = urlPrefix + firstSubNode.FN_LinkLocation;
                            }
                            // 地图，即全局模块新窗口打开
                            if (navUrl != "/Admin/Global/Map")
                            {
                                sb.AppendFormat("<a class='collapsed heading' href='{0}'> {1}</a>", navUrl, navNode.Title);
                            }
                            else
                            {
                                sb.AppendFormat("<a class='collapsed heading' href='{0}' target='_blank'>    {1} </a>", navUrl, navNode.Title);
                            }
                            
                        }
                        sb.Append("</li>");
                    }
                }
            }
            return helper.Raw(sb.ToString());
        }

        /// <summary>
        /// 生成Location区域
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlString Location(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            var currentNode = SiteMap.CurrentNode;
            if (currentNode != null)
            {
                if (currentNode.Title.Trim() == "")
                {
                    currentNode = currentNode.ParentNode;
                }
                sb.Append("<li><strong>Location:</strong></li>");
                if (currentNode.ParentNode != null)
                {
                    sb.AppendFormat("<li>{0}</li><li>/</li>", currentNode.ParentNode.Title);
                }
                sb.AppendFormat("<li class='current'>{0}</li>", currentNode.Title);
            }
            return helper.Raw(sb.ToString());
        }
    }
}
