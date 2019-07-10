using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WorkhoursCalculator
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Config config;
        private readonly WorkHoursRepository repository;
        private bool save = true;

        public string Title => "Workhours Calculator " + (save ? " saved" : " UNSAVED DATA!");

        public MainViewModel(WorkHoursRepository repository, Config config)
        {
            this.repository = repository;
            this.config = config;
            Days = new ObservableCollection<Day>(repository.Days);
            command = new Command();
            command.CanExecuteFunc = x => true;
            command.ExecuteFunc = x => Com((string)x);
            Task.Run(() =>
            {
                while (true)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Refresh();
                    });
                    Thread.Sleep(1000);
                }
            });
        }

        public ObservableCollection<Day> days;
        public ObservableCollection<Day> Days
        {
            get => days;
            set
            {
                if (value != null && value != days)
                {
                    days = value;
                    OnPropertyChanged(nameof(Days));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public string TimeRemaining
        {
            get
            {
                var time = TimeSpan.FromHours(config.HoursPerWeek);
                repository.Days.ForEach(x => time-=x.WorkHours);
                return time.ToString(@"hh\:mm\:ss");
            }
        }

        public string Today
        {
            get
            {
                var time = TimeSpan.Zero;
                repository.Days.Where(x => x.Date.Date == DateTime.Now.Date)
                    .ToList()
                    .ForEach(x => time += x.WorkHours);
                return (time < TimeSpan.Zero ? "-" : "") + time.ToString(@"hh\:mm\:ss");
            }
        }

        public void Save()
        {
            save = false;
            OnPropertyChanged(nameof(Title));
            repository.Save();
        }

        public void GoHome()
        {
            //repository.Load();
            repository.Days.ForEach(x =>
            {
                if (x.End == null)
                    x.End = DateTime.Now;
            });
            save = false;
            Days = new ObservableCollection<Day>(repository.Days);
        }
        public void Refresh()
        {
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(Today));
        }
        public void Load()
        {
            repository.Load();
            save = true;
            Days = new ObservableCollection<Day>(repository.Days);
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(Today));
        }

        public void Com(string str)
        {
            if (str == "csv")
                CreateCsv();
            else if (str == "refresh")
                GoHome();
            else if (str == "save")
                Save();
            else if (str == "add")
                Add();
            else if (str == "load")
                ImportCsv();
            else if (str == "lo")
                Load();
        }

        public void Add()
        {
            repository.Days.Add(new Day { Date = DateTime.Now, Start=DateTime.Now });
            save = false;
            Days = new ObservableCollection<Day>(repository.Days);
        }

        public void CreateCsv()
        {
            repository.CreateCsv();
        }

        public void ImportCsv()
        {
            repository.ImportCsv();
            Days = new ObservableCollection<Day>(repository.Days);
        }

        Command command;
        public Command Command => command;
    }

    public class Command : ICommand
    {
        public Predicate<object> CanExecuteFunc
        {
            get;
            set;
        }

        public Action<object> ExecuteFunc
        {
            get;
            set;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ExecuteFunc(parameter);
        }
    }
}
