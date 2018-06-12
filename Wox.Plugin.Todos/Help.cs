using System.Collections.Generic;


namespace Wox.Plugin.Todos
{
    public class Help
    {
        public Help(Todos todos, Query query)
        {
            Todos = todos;
            Query = query;
        }

        public Todos Todos { get; }
        public Query Query { get; }

        public List<Result> Show
        {
            get
            {
                return new List<Result> {
                    new Result {
                        Title = $"{Query.ActionKeyword} -a [text]",
                        SubTitle = "add todos",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -a ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -rl",
                        SubTitle = "reload todos from data file",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} [keyword]",
                        SubTitle = "list todos",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -l ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -l [keyword]",
                        SubTitle = "list all todos, inclued completed todos",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -l ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -r [keyword]",
                        SubTitle = "remove todos",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -r ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -r --all",
                        SubTitle = "remove all todos",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -r --all");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -r --done",
                        SubTitle = "Remove all commpleted todos",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -r --done");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -c [keyword]",
                        SubTitle = "mark todo as done",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -c ");
                            return false;
                        }
                    },
                    new Result {
                        Title = $"{Query.ActionKeyword} -c --all",
                        SubTitle = "mark all todos as done",
                        IcoPath = Todos.GetFilePath(),
                        Action = c => {
                            Todos.Context.API.ChangeQuery($"{Query.ActionKeyword} -c --all");
                            return false;
                        }
                    }
                };
            }
        }
    }
}
