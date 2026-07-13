using StackTrack.Helpers;
using StackTrack.Model;
using StackTrack.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace StackTrack.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FileIOHandler _taskService;
        private ObservableCollection<TaskDto> _tasks;
        private string _newTaskTitle;
        private string _searchText = string.Empty;
        private string _selectedPriority = StringResources.medium;
        private string _selectedFilter = StringResources.all;
        private TaskDto _selectedTask;

        #region Peoperties
        public ObservableCollection<TaskDto> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }
        public ICollectionView FilteredTasks { get; set; }
        public List<string> Priorities { get; } = new List<string> (StringResources._priorities);
        public List<string> Filters { get; } = new List<string>(StringResources._Filters);
        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set { _newTaskTitle = value; OnPropertyChanged(); }
        }

        public string SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                FilteredTasks.Refresh(); 
            }
        }

        public TaskDto SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilteredTasks.Refresh(); // Re-evaluates list live as you type
            }
        }
        #endregion

        #region Commands
        public ICommand AddTaskCommand { get; }
        public ICommand ToggleCompleteCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        
        #endregion

        public MainViewModel()
        {
            _taskService = new FileIOHandler();
            LoadTasks();
            FilteredTasks = CollectionViewSource.GetDefaultView(Tasks);
            FilteredTasks.Filter = FilterTaskPredicate;
            AddTaskCommand = new CommandHandler(AddTask, CanAddTask);
            ToggleCompleteCommand = new CommandHandler(ToggleComplete, CanModifyTask);
            DeleteTaskCommand = new CommandHandler(DeleteTask, CanModifyTask);
            CloseWindowCommand = new CommandHandler(CloseWindow);
            MinimizeWindowCommand= new CommandHandler(MinimizeWindow);
        }

        

        #region Methods
        private bool FilterTaskPredicate(object obj)
        {
            if (obj is TaskDto task)
            {
                // 1. Check Category/Status Filter View state
                bool matchesFilter = true;
                if (SelectedFilter == StringResources.completed) matchesFilter = task.IsCompleted;
                else if (SelectedFilter == StringResources.pending) matchesFilter = !task.IsCompleted;

                // 2. Check Search Bar text state
                bool matchesSearch = true;
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    matchesSearch = task.Title.Contains(SearchText);
                }
                return matchesFilter && matchesSearch;
            }
            return false;
        }

        private bool CanAddTask(object obj) => !string.IsNullOrWhiteSpace(NewTaskTitle);
        private void AddTask(object obj)
        {
            Tasks.Add(new TaskDto
            {
                Title = NewTaskTitle.Trim(),
                Priority = SelectedPriority,
                IsCompleted = false
            });

            NewTaskTitle = string.Empty;
            SaveTasks();
        }

        private bool CanModifyTask(object obj) => SelectedTask != null;

        private void ToggleComplete(object obj)
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = !SelectedTask.IsCompleted;
                FilteredTasks.Refresh(); 
                SaveTasks();
            }
        }

        private void DeleteTask(object obj)
        {
            if (SelectedTask != null)
            {
                Tasks.Remove(SelectedTask);
                SaveTasks();
            }
        }

        private async void LoadTasks()
        {
            try
            {
                Tasks = new ObservableCollection<TaskDto>();
                var tasks = await _taskService.LoadTasksAsync();
                Tasks.Clear();
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
            catch
            {
                Tasks = new ObservableCollection<TaskDto>();
            }
        }

        private async void SaveTasks()
        {
            try
            {

                await _taskService.SaveTasksAsync(Tasks.ToList());
            }
            catch
            {
            }
        }
        private void CloseWindow(object parameter)
        {
            if (parameter is System.Windows.Window window)
            {
                window.Close();
            }
        }

        private void MinimizeWindow(object obj)
        {
            if (obj is System.Windows.Window window)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}