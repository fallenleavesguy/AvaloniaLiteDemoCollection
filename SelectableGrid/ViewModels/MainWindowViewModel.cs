using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SelectableGrid.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<PersonViewModel> _people;

    [ObservableProperty] private ObservableCollection<SexTypes> _sexTypeList;

    [ObservableProperty] private string _newPersonName = "";

    [ObservableProperty] private SexTypes _selectedSexType = SexTypes.Unknown;


    [ObservableProperty] private string _newPersonAge = "0";

    public MainWindowViewModel()
    {
        SexTypeList = new ObservableCollection<SexTypes>(Enum.GetValues(typeof(SexTypes)).Cast<SexTypes>());
        SelectedSexType = SexTypes.Unknown;

        People =
        [
            new PersonViewModel { Name = "Çisil", Age = 21, Sex = SexTypes.Female },
            new PersonViewModel { Name = "Faruk", Age = 31, Sex = SexTypes.Male },
            new PersonViewModel { Name = "Cem", Age = 16, Sex = SexTypes.Male },
            new PersonViewModel { Name = "Arda", Age = 22, Sex = SexTypes.Unknown }
        ];
    }

    [RelayCommand]
    private void AddPerson()
    {
        if (!int.TryParse(NewPersonAge.ToString(), out int age) || age < 0)
        {
            return;
        }

        var newPerson = new PersonViewModel
        {
            Name = NewPersonName ?? "New Person",
            Age = Convert.ToInt32(NewPersonAge),
            Sex = SelectedSexType
        };
        People.Add(newPerson);

        // Clear input fields after adding
        NewPersonName = string.Empty;
        NewPersonAge = "0";
        SelectedSexType = SexTypes.Unknown;
    }

    [RelayCommand]
    private void DeleteSelectedPerson()
    {
        var personsToRemove = People.Where(p => p.IsSelected);
        foreach (var person in personsToRemove.ToList())
        {
            People.Remove(person);
        }
    }
}