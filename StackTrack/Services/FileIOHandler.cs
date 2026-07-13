using StackTrack.Helpers;
using StackTrack.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StackTrack.Services
{
    internal class FileIOHandler
    {
        private readonly string _filePath;
        private readonly object _lockObject = new object();

        public FileIOHandler(string filePath = StringResources._taskFilepath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Dispose();
            }
        }

        /// <summary>
        /// Asynchronously load all tasks from file
        /// </summary>
        public async Task<List<TaskDto>> LoadTasksAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    lock (_lockObject)
                    {
                        if (!File.Exists(_filePath))
                            return new List<TaskDto>();

                        var lines = File.ReadAllLines(_filePath);
                        return lines
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .Select(TaskDto.Parse)
                            .Where(t => t != null)
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
                    return new List<TaskDto>();
                }
            });
        }

        /// <summary>
        /// Asynchronously save all tasks to file
        /// </summary>
        public async Task SaveTasksAsync(List<TaskDto> tasks)
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_lockObject)
                    {
                        var lines = tasks.Select(t => t.ToString()).ToArray();
                        File.WriteAllLines(_filePath, lines);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving tasks: {ex.Message}");
                }
            });
        }

    }
}



