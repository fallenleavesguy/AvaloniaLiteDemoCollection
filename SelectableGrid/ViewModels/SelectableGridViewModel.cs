using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SelectableGrid.ViewModels;

public partial class SelectableGridViewViewModel : SelectableCollectionViewModelBase<PersonViewModel>
{
    [ObservableProperty] private ObservableCollection<PersonViewModel> _people;

    [ObservableProperty] private ObservableCollection<SexTypes> _sexTypeList;

    [ObservableProperty] private string _newPersonName = "";

    [ObservableProperty] private SexTypes _selectedSexType = SexTypes.Unknown;

    [ObservableProperty] private string _newPersonAge = "0";

    /// <summary>
    /// 实现基类的抽象属性，返回可选择的项目集合
    /// </summary>
    protected override ObservableCollection<PersonViewModel> SelectableItems => People;

    public SelectableGridViewViewModel()
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

        // 调用基类的初始化方法来设置全选功能
        InitializeSelectAllFeature();
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

        People.Add(newPerson); // 基类会自动处理订阅

        // Clear input fields after adding
        NewPersonName = string.Empty;
        NewPersonAge = "0";
        SelectedSexType = SexTypes.Unknown;
    }

    [RelayCommand]
    private void DeleteSelectedPerson()
    {
        // 使用基类提供的方法删除选中的项目
        RemoveSelectedItems();
    }
}