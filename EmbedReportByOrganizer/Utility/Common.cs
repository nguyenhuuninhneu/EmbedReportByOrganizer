using EmbedReportByOrganizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace EmbedReportByOrganizer
{
    public static class Common
    {
        public static T ReadJson<T>(string name)
        {
            try
            {
                //name = HelperEncryptor.Md5Hash(name);
                // gets the directory where the program is launched from and adds the foldername
                string path = HttpContext.Current.Server.MapPath("~/App_Data");

                //Creates a directory(folder) if it doesen't exist

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = path + "/" + name + ".json";

                if (!System.IO.File.Exists(filePath))
                {
                    var json = JsonConvert.SerializeObject(new List<SettingModel>());
                    using (var stream = new StreamWriter(filePath))
                    {
                        stream.Write(json);
                        stream.Dispose();
                    };
                    System.IO.File.Create(filePath).Dispose();
                }
                var str = "";
                using (var stream = new StreamReader(filePath))
                {
                    str = stream.ReadToEnd();
                    stream.Dispose();
                };
                return JsonConvert.DeserializeObject<T>(str);

            }
            catch (Exception)
            {
                return default(T);
            }
        }
        public static void WriteJson(object obj, string name)
        {
            try
            {
                //gets the directory where the program is launched from and adds the foldername
                string path = HttpContext.Current.Server.MapPath("~/App_Data");

                //Creates a directory(folder) if it doesen't exist

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = path + "/" + name + ".json";
                var json = JsonConvert.SerializeObject(obj);
                using (var stream = new StreamWriter(filePath))
                {
                    stream.Write(json);
                    stream.Dispose();
                };
            }
            catch (Exception ex)
            {
            }
        }
    }
}
