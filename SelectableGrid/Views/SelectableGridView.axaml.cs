using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SelectableGrid.ViewModels;

namespace SelectableGrid.Views;

public partial class SelectableGridView : UserControl
{
    public SelectableGridView()
    {
        DataContext = new SelectableGridViewViewModel();
        InitializeComponent();
    }
}