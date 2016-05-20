using System.ComponentModel;

namespace Wox.Plugin.Todos
{
    public enum TodoCommand
    {
        [Description("List")]
        L,
        [Description("Add")]
        A,
        [Description("Complete")]
        C,
        [Description("Remove")]
        R,
        [Description("Help")]
        H
    }
}
