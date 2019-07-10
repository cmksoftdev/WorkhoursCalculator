using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkhoursCalculator
{
    public class Config
    {
        const string CONFIG_FILE = "config.json";
        public string TimeDataFilePath { get; set; }
        public string CsvFilePath { get; set; }
        public int HoursPerWeek { get; set; }

        public static Config Load()
        {
            if (!File.Exists(CONFIG_FILE))
                File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(new Config
                {
                    TimeDataFilePath = "times.json",
                    CsvFilePath = "out.csv",
                    HoursPerWeek = 40
                }));
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(CONFIG_FILE));
        }
    }
}
