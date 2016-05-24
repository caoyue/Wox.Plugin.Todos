Wox.Plugin.Todos
--------------------------

A simple todo app for [Wox](https://github.com/Wox-launcher/Wox)

![demo.gif](https://raw.githubusercontent.com/caoyue/Wox.Plugin.Todos/master/todos.gif)

### usage
- type `td -h` to view supported commands

### sync your todo list
- open your plugin directory(open settings -> Plugin -> Wox.Plguin.Todos -> click 'Plugin Directory')
- put `config.txt` in plugin directory with a file path which could be synced, e.g.

    ```
    C:\Users\me\OneDrive\todos.data.json
    ```
- use `td -rl` or `restart Wox` to take effect
- tips: cause data file may be synced after the initialization of Wox plugin, if todo list not updated, use `td -rl` to reload todos from the data file
