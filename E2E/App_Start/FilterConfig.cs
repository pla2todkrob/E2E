using E2E.Models;
using E2E.Models.Filter;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace E2E
{
    public class ClsAssembly
    {
        public ClsAssembly()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version = assemblyName.Version.ToString();
            Product = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            Trademark = assembly.GetCustomAttribute<AssemblyTrademarkAttribute>().Trademark;

            string projectPath = AppDomain.CurrentDomain.BaseDirectory;
            string iconPath = Path.Combine(projectPath, "favicon.ico");
            using (Stream iconStream = new FileStream(iconPath, FileMode.Open))
            {
                if (iconStream != null)
                {
                    using (Icon icon = new Icon(iconStream))
                    {
                        // Convert the icon to a base64 string to store in the Logo property
                        using (MemoryStream ms = new MemoryStream())
                        {
                            icon.Save(ms);
                            byte[] iconBytes = ms.ToArray();
                            Logo = "data:image/png;base64," + Convert.ToBase64String(iconBytes);
                        }
                    }
                }
            }
        }

        public string Company { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Product { get; set; }
        public string Trademark { get; set; }
        public string Version { get; set; }
    }

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new TimingFilterAttribute());
            filters.Add(new ExceptionFilterAttribute());
            filters.Add(new ClearCacheFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
