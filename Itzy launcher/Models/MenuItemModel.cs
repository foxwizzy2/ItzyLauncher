using System.Windows.Input;

namespace ItzyLauncher.Models;

public sealed class MenuItemModel
{
    public string Id { get; set; } = "";

    public string Text { get; set; } = "";

    public string Icon { get; set; } = "";

    public string Action { get; set; } = "";

    public string Value { get; set; } = "";

    public ICommand? Command { get; set; }
}