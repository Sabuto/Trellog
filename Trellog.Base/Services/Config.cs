using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Trellog.Base.Services
{
    public class Config
    {
        public Models.Config LoadConfig(string file)
        {
            try
            {
                return JsonConvert.DeserializeObject<Models.Config>(File.ReadAllText(file));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public void SaveConfig(Models.Config data, string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(data));
        }
    }
}