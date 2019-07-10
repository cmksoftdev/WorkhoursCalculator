using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkhoursCalculator
{
    public class WorkHoursRepository
    {
        private readonly Config config;
        public List<Day> Days { get; private set; }

        public WorkHoursRepository(Config config)
        {
            this.config = config;
            Load();
        }

        public void CreateCsv()
        {
            try
            {
                using (var file = File.CreateText(config.CsvFilePath))
                {
                    TimeSpan fulltime = TimeSpan.Zero;
                    file.WriteLine($"Datum,Gekommen,Gegangen,Arbeit,Pausenzeit,Arbeitszeit,Gesamt");
                    foreach (var day in Days)
                    {
                        fulltime += day.WorkHours;
                        file.WriteLine($"{day.DateString},{day.StartString},{day.EndeString},{day.Work},{day.Pause},{day.WorkHours},{fulltime}");
                    }
                }
            }
            catch
            {

            }
        }

        public void ImportCsv()
        {
            try
            {

                if (!File.Exists(config.CsvFilePath))
                    return;
                var csv = File.ReadAllLines(config.CsvFilePath).Skip(1);
                Days = new List<Day>();
                foreach (var line in csv)
                {
                    var values = line.Split(',');
                    Days.Add(new Day
                    {
                        Date = DateTime.Parse(values[0]),
                        Start = DateTime.Parse(values[1]),
                        End = values[2] == "" ? (DateTime?)null : DateTime.Parse(values[2]),
                        Work = values[3],
                        Pause = TimeSpan.Parse(values[4]),
                    });
                }
                Save();
            }
            catch
            {

            }
        }

        public void Save()
        {
            File.WriteAllText(config.TimeDataFilePath, JsonConvert.SerializeObject(Days));
        }

        public void Load()
        {
            if (!File.Exists(config.TimeDataFilePath))
            {
                Days = new List<Day>();
                Save();
            }
            Days = JsonConvert.DeserializeObject<List<Day>>(File.ReadAllText(config.TimeDataFilePath));
        }
    }
}
