using CommunityToolkit.Mvvm.ComponentModel;

namespace SelectableGrid.ViewModels;

public interface ICanSelectAll 
{
    public bool? IsAllSelected { get; set; }
}