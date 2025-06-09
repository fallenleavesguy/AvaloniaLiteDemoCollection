using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace SelectableGrid.ViewModels;

public partial class PersonViewModel : ObservableObject, ISelectable
{
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private int _age;
    [ObservableProperty]
    private SexTypes _sex;
    [ObservableProperty]
    private bool _isAdult;    [ObservableProperty] 
    private bool _isSelected;

    public event Action? SelectionChanged;

    partial void OnIsSelectedChanged(bool value)
    {
        SelectionChanged?.Invoke();
    }
}

public enum SexTypes
{
    Unknown,
    Male,
    Female
}