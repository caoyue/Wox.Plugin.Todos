using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wox.Infrastructure.Storage;

namespace Wox.Plugin.Todos
{
    public class Main : IPlugin, ISettingProvider, ISavable
    {
        private static Todos _todos;

        private readonly PluginJsonStorage<Settings> _storage;
        private readonly Settings _setting;

        public Main()
        {
            _storage = new PluginJsonStorage<Settings>();
            _setting = _storage.Load();
        }

        #region Query
        public List<Result> Query(Query query)
        {
            _todos.Reload();

            _todos.ActionKeyword = query.ActionKeyword;
            var help = new Help(_todos.Context, query);

            if (query.FirstSearch.Equals("-"))
            {
                return help.Show;
            }

            if (!query.FirstSearch.StartsWith("-"))
            {
                return Search(query.Search, t => !t.Completed);
            }

            TodoCommand op;
            if (!Enum.TryParse(query.FirstSearch.TrimStart('-'), true, out op))
            {
                return Search(query.Search, t => !t.Completed);
            }

            switch (op)
            {
                case TodoCommand.H:
                    return help.Show;
                case TodoCommand.C:
                    if (query.SecondSearch.Equals("--all", StringComparison.OrdinalIgnoreCase))
                    {
                        return new List<Result> {
                            new Result {
                                Title = "Mark all todos as done?",
                                SubTitle = "click to make all todos as done",
                                IcoPath = _todos.GetFilePath(),
                                Action = c => {
                                    _todos.CompleteAll();
                                    return true;
                                }
                            }
                        };
                    }
                    var cResults = _todos.Find(
                        t => t.Content.IndexOf(query.SecondToEndSearch, StringComparison.OrdinalIgnoreCase) >= 0 && !t.Completed,
                        t2 => "click to mark todo as done",
                        (c, t3) =>
                        {
                            _todos.Complete(t3);
                            //requery to refresh results
                            _todos.Context.API.ChangeQuery($"{query.ActionKeyword} -l ", true);
                            return false;
                        });
                    return cResults;
                case TodoCommand.R:
                    if (query.SecondSearch.Equals("--all", StringComparison.OrdinalIgnoreCase))
                    {
                        return new List<Result> {
                            new Result {
                                Title = "Remove all todos?",
                                SubTitle = "click to remove all todos",
                                IcoPath = _todos.GetFilePath(),
                                Action = c => {
                                    _todos.RemoveAll();
                                    return true;
                                }
                            }
                        };
                    }
                    else if (query.SecondSearch.Equals("--done", StringComparison.OrdinalIgnoreCase))
                    {
                        return new List<Result> {
                            new Result {
                                Title = "Remove all completed todos?",
                                SubTitle = "click to remove all todos",
                                IcoPath = _todos.GetFilePath(),
                                Action = c => {
                                    _todos.RemoveAllCompletedTodos();
                                    return true;
                                }
                            }
                        };
                    }
                    var results = _todos.Find(
                        t => t.Content.IndexOf(query.SecondToEndSearch, StringComparison.OrdinalIgnoreCase) >= 0,
                        t2 => "click to remove todo",
                        (c, t3) =>
                        {
                            _todos.Remove(t3);
                            return true;
                        });
                    return results;
                case TodoCommand.A:
                    return new List<Result> {
                        AddResult(query.SecondToEndSearch)
                    };
                case TodoCommand.L:
                    return Search(query.SecondToEndSearch);
                case TodoCommand.Rl:
                    return new List<Result> {
                        new Result {
                            Title = "Reload todos from data file?",
                            SubTitle = "click to reload",
                            IcoPath = _todos.GetFilePath(),
                            Action = c => {
                                _todos.Reload();
                                _todos.Context.API.ChangeQuery($"{query.ActionKeyword} ", true);
                                return false;
                            }
                        }
                    };
                default:
                    return Search(query.Search, t => !t.Completed);
            }
        }

        #endregion

        public void Init(PluginInitContext context)
        {
            _todos = new Todos(context, _setting);
        }

        public Control CreateSettingPanel()
        {
            return new FilePathSetting(_setting);
        }

        #region Utils

        private static List<Result> Search(string search, Func<Todo, bool> conditions = null)
        {
            var s = search;
            var results = _todos.Find(t =>
                t.Content.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0
                && (conditions?.Invoke(t) ?? true));
            if (!string.IsNullOrEmpty(s) && !results.Any())
            {
                results.Insert(0, AddResult(s));
            }
            return results;
        }

        private static Result AddResult(string content)
        {
            return new Result
            {
                Title = $"add new item \"{content}\"",
                SubTitle = "",
                IcoPath = _todos.GetFilePath(),
                Action = c =>
                {
                    _todos.Add(new Todo
                    {
                        Content = content,
                        Completed = false,
                        CreatedTime = DateTime.Now
                    });
                    return false;
                }
            };
        }

        public void Save()
        {
            _storage.Save();
        }

        #endregion
    }
}
