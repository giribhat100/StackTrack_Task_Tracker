using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackTrack.Helpers
{
    public static class StringResources
    {
        public const string low = "Low";
        public const string medium = "Medium";
        public const string high = "High";
        public const string all = "All";
        public const string pending = "Pending";
        public const string completed = "Completed";
        public static List<string> _priorities = new List<string>() { low, medium, high };
        public static List<string> _Filters = new List<string>() { all, pending, completed };
        public const string _taskFilepath = @"C:\StackTrack\tasks.txt";

    }
}
