﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnergyMonitor.Controllers.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("smtp.126.com")]
        public string mailServerName {
            get {
                return ((string)(this["mailServerName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("energymonitor@126.com")]
        public string mailFrom {
            get {
                return ((string)(this["mailFrom"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("energymonitor@126.com")]
        public string mailUserName {
            get {
                return ((string)(this["mailUserName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("cis-51355517")]
        public string mailPassword {
            get {
                return ((string)(this["mailPassword"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("复旦大学绿色校园能耗监管平台邮箱激活")]
        public string activeSubject {
            get {
                return ((string)(this["activeSubject"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>激活</title>
</head>
<body>
<p>{0}，您好：</p>
&nbsp;&nbsp;请点击以下链接完成复旦大学校园节能监管平台邮箱修改的激活！<br /><br />
&nbsp;&nbsp;<a href=""{1}"" style=""font-size:25px"">{1}</a><br /><br />
&nbsp;&nbsp;如有问题请联系管理员，谢谢！<br />
&nbsp;&nbsp;复旦大学绿色校园监管平台<br />
</body>
</html>")]
        public string activeBody {
            get {
                return ((string)(this["activeBody"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://218.193.130.178/")]
        public string activeWebSite {
            get {
                return ((string)(this["activeWebSite"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int pageSize {
            get {
                return ((int)(this["pageSize"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("zh-cn:中文,en-us:English")]
        public string supportLanguage {
            get {
                return ((string)(this["supportLanguage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("f@!_2*-6")]
        public string desKey {
            get {
                return ((string)(this["desKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[5(%n>*?")]
        public string desVector {
            get {
                return ((string)(this["desVector"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public double loginExpired {
            get {
                return ((double)(this["loginExpired"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://tac.fudan.edu.cn/thirds/jnjp.act?t=")]
        public string uisLoginUrl {
            get {
                return ((string)(this["uisLoginUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://tac.fudan.edu.cn/thirds/logout.act")]
        public string uisLogoutUrl {
            get {
                return ((string)(this["uisLogoutUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public double rdsExpired {
            get {
                return ((double)(this["rdsExpired"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("xls,xlsx")]
        public string uploadExcelExtens {
            get {
                return ((string)(this["uploadExcelExtens"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6")]
        public double wsExpired {
            get {
                return ((double)(this["wsExpired"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=10.131.200.70;Initial Catalog=EnergyMonitor;Persist Security Info=Tru" +
            "e;User ID=sa;Password=LabTel55664465")]
        public string EnergyMonitor {
            get {
                return ((string)(this["EnergyMonitor"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Program Files\\SWFTools")]
        public string SWFToolsPath {
            get {
                return ((string)(this["SWFToolsPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("doc,docx,pdf")]
        public string uploadWordExtens {
            get {
                return ((string)(this["uploadWordExtens"]));
            }
        }
    }
}
