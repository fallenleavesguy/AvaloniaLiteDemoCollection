using CommunityToolkit.Mvvm.ComponentModel;

namespace SelectableGrid.ViewModels;

public partial class PersonViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private int _age;
    [ObservableProperty]
    private SexTypes _sex;
    [ObservableProperty]
    private bool _isAdult;
    [ObservableProperty] private bool _isSelected;

}

public enum SexTypes
{
    Unknown,
    Male,
    Female
}