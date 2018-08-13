using System.Collections.Generic;
using System.IO;

namespace Wox.Plugin.Todos
{
    public class Help
    {
        private readonly PluginInitContext _context;
        private readonly Query _query;
        private readonly string _iconPath;

        public Help(PluginInitContext context, Query query)
        {
            _context = context;
            _query = query;
            _iconPath = Path.Combine(_context.CurrentPluginMetadata.PluginDirectory, @"ico\app.png");
        }


        public List<Result> Show
        {
            get
            {
                return new List<Result> {
                    new Result {
                        Title = $"{_query.ActionKeyword} -a [text]",
                        SubTitle = "add todos",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -a ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -rl",
                        SubTitle = "reload todos from data file",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} [keyword]",
                        SubTitle = "list todos",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -l ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -l [keyword]",
                        SubTitle = "list all todos, inclued completed todos",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -l ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -r [keyword]",
                        SubTitle = "remove todos",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -r ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -r --all",
                        SubTitle = "remove all todos",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -r --all");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -r --done",
                        SubTitle = "Remove all commpleted todos",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -r --done");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -c [keyword]",
                        SubTitle = "mark todo as done",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -c ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{_query.ActionKeyword} -c --all",
                        SubTitle = "mark all todos as done",
                        IcoPath = _iconPath,
                        Action = c => {
                            _context.API.ChangeQuery($"{_query.ActionKeyword} -c --all");
                            return false;
                        }
                    }
                };
            }
        }
    }
}
