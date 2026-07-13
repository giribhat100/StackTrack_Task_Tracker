using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StackTrack.Model
{
    public class TaskDto : INotifyPropertyChanged
    {
        private string _title;
        private string _priority;
        private bool _isCompleted;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set { _isCompleted = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Title}|{Priority}|{IsCompleted}";
        }

        public static TaskDto Parse(string line)
        {
            var parts = line.Split('|');
            if (parts.Length != 3) return null;

            return new TaskDto
            {
                _title = parts[0],
                _priority = parts[1],
                IsCompleted = Convert.ToBoolean(parts[2])
            };
        }
    }

}
