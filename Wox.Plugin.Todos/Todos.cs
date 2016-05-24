using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Newtonsoft.Json;

namespace Wox.Plugin.Todos
{
    public class Todo
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool Completed { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class Todos
    {
        private const string ConfigFile = @"config.txt";
        private const string DataFile = @"todos.data.json";

        private string _dataFilePath;

        private List<Todo> _todoList;
        public PluginInitContext Context { get; }

        public string ActionKeyword { get; set; }

        public Todos(PluginInitContext context)
        {
            Context = context;

            if (context.CurrentPluginMetadata.ActionKeywords != null
                && context.CurrentPluginMetadata.ActionKeywords.Any()) {
                ActionKeyword = context.CurrentPluginMetadata.ActionKeywords[0];
            }

            Load();
        }

        public List<Result> Results => ToResults(_todoList);

        public int MaxId
        {
            get { return _todoList != null && _todoList.Any() ? _todoList.Max(t => t.Id) : 0; }
        }

        public void Reload()
        {
            Load();
        }

        public List<Result> Find(
            Func<Todo, bool> func,
            Func<Todo, string> subTitleFormatter = null,
            Func<ActionContext, Todo, bool> itemAction = null)
        {
            return ToResults(_todoList.Where(func), subTitleFormatter, itemAction);
        }

        public Todos Add(Todo todo, Action callback = null)
        {
            if (string.IsNullOrEmpty(todo.Content)) {
                return this;
            }

            todo.Id = MaxId + 1;
            _todoList.Add(todo);
            Save();
            if (callback == null) {
                Context.API.ChangeQuery($"{ActionKeyword} ");
            }
            else {
                callback();
            }
            return this;
        }

        public Todos Remove(Todo todo, Action callback = null)
        {
            var item = _todoList.FirstOrDefault(t => t.Id == todo.Id);
            if (item != null) {
                _todoList.Remove(item);
            }
            Save();
            if (callback == null) {
                Context.API.ChangeQuery($"{ActionKeyword} ");
                Alert("Success", "todo removed!");
            }
            else {
                callback();
            }
            return this;
        }

        public Todos RemoveAll(Action callback = null)
        {
            _todoList.RemoveAll(t => true);
            Save();
            if (callback == null) {
                Context.API.ChangeQuery($"{ActionKeyword} ");
                Alert("Success", "all todos removed!");
            }
            else {
                callback();
            }
            return this;
        }

        public Todos Complete(Todo todo, Action callback = null)
        {
            var item = _todoList.FirstOrDefault(t => t.Id == todo.Id);
            if (item != null) {
                item.Completed = true;
            }
            Save();
            callback?.Invoke();
            return this;
        }

        public Todos CompleteAll(Action callback = null)
        {
            _todoList.ForEach(t => {
                t.Completed = true;
            });
            if (callback == null) {
                Context.API.ChangeQuery($"{ActionKeyword} ");
                Alert("Success", "all todos done!");
            }
            else {
                callback();
            }
            return this;
        }

        public void Alert(string title, string content)
        {
            Context.API.ShowMsg(title, content, GetFilePath());
        }

        public string GetFilePath(string icon = "")
        {
            return Path.Combine(Context.CurrentPluginMetadata.PluginDirectory,
                string.IsNullOrEmpty(icon) ? @"ico\app.png" : icon);
        }

        private void Load()
        {
            _dataFilePath = File.Exists(GetFilePath(ConfigFile))
                ? File.ReadAllText(GetFilePath(ConfigFile))
                : GetFilePath(DataFile);

            try {
                var text = File.ReadAllText(_dataFilePath);
                _todoList = JsonConvert.DeserializeObject<List<Todo>>(text);
            }
            catch (FileNotFoundException) {
                _todoList = new List<Todo>();
                Save();
            }
            catch (Exception e) {
                throw new Exception($"data file broken: {e.Message}!");
            }
        }

        private void Save()
        {
            try {
                var json = JsonConvert.SerializeObject(_todoList);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception e) {
                throw new Exception($"write data failed: {e.Message}!");
            }
        }

        private List<Result> ToResults(
            IEnumerable<Todo> todos,
            Func<Todo, string> subTitleFormatter = null,
            Func<ActionContext, Todo, bool> itemAction = null)
        {
            var results = todos.OrderByDescending(t => t.CreatedTime)
                .Select(t => new Result {
                    Title = $"{t.Content}",
                    SubTitle = subTitleFormatter == null
                        ? $"{ToRelativeTime(t.CreatedTime)}"
                        : subTitleFormatter(t),
                    IcoPath = GetFilePath(t.Completed ? @"ico\done.png" : @"ico\todo.png"),
                    Action = c => {
                        if (itemAction != null) {
                            return itemAction(c, t);
                        }
                        try {
                            Clipboard.SetText(t.Content);
                        }
                        catch (ExternalException) {
                            Alert("Failed", "Copy failed, please try again later");
                        }
                        return true;
                    }
                }).ToList();

            if (!results.Any()) {
                results.Add(new Result {
                    Title = "No results",
                    SubTitle = "click to view help",
                    IcoPath = GetFilePath(),
                    Action = c => {
                        Context.API.ChangeQuery("td -h");
                        return false;
                    }
                });
            }
            return results;
        }

        private static string ToRelativeTime(DateTime value)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = DateTime.Now.Subtract(value);
            var seconds = ts.TotalSeconds;

            if (seconds < 1 * minute)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (seconds < 60 * minute)
                return ts.Minutes + " minutes ago";

            if (seconds < 120 * minute)
                return "an hour ago";

            if (seconds < 24 * hour)
                return ts.Hours + " hours ago";

            if (seconds < 48 * hour)
                return "yesterday";

            if (seconds < 30 * day)
                return ts.Days + " days ago";

            if (seconds < 12 * month) {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}
