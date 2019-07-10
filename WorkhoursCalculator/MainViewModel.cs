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

        public MainViewModel(WorkHoursRepository repository, Config config)
        {
            this.repository = repository;
            this.config = config;
            Days = new ObservableCollection<Day>();
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

        public ObservableCollection<Day> Days { get; set; }

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
                return time.ToString();
            }
        }

        public void Save()
        {
            repository.Save();
        }

        public void GoHome()
        {
            repository.Load();
            repository.Days.ForEach(x =>
            {
                if (x.End == null)
                    x.End = DateTime.Now;
            });
        }
        public void Refresh()
        {
            Days = new ObservableCollection<Day>(repository.Days);
            OnPropertyChanged(nameof(Days));
            OnPropertyChanged(nameof(TimeRemaining));
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
        }

        public void Add()
        {
            repository.Days.Add(new Day { Date = DateTime.Now, Start=DateTime.Now });
        }

        public void CreateCsv()
        {
            repository.CreateCsv();
        }

        public void ImportCsv()
        {
            repository.ImportCsv();
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
