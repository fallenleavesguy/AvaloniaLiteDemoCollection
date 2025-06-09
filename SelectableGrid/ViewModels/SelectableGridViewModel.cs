using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SelectableGrid.ViewModels;

public partial class SelectableGridViewViewModel : ViewModelBase, ICanSelectAll
{
    [ObservableProperty] private ObservableCollection<PersonViewModel> _people;

    [ObservableProperty] private ObservableCollection<SexTypes> _sexTypeList;

    [ObservableProperty] private string _newPersonName = "";

    [ObservableProperty] private SexTypes _selectedSexType = SexTypes.Unknown;

    [ObservableProperty] private string _newPersonAge = "0";

    [ObservableProperty] private bool? _isAllSelected;

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

        // 使用CollectionChanged事件自动处理添加/删除时的事件订阅
        People.CollectionChanged += OnPeopleCollectionChanged;

        // 为现有项目订阅事件
        SubscribeToAllPersonEvents();

        UpdateSelectAllState();
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

        People.Add(newPerson); // CollectionChanged事件会自动处理订阅

        // Clear input fields after adding
        NewPersonName = string.Empty;
        NewPersonAge = "0";
        SelectedSexType = SexTypes.Unknown;
    }

    [RelayCommand]
    private void DeleteSelectedPerson()
    {
        var personsToRemove = People.Where(p => p.IsSelected).ToList();
        foreach (var person in personsToRemove)
        {
            People.Remove(person); // CollectionChanged事件会自动处理取消订阅
        }
    }

    private bool _isUpdatingSelectAllState = false;

    /// <summary>
    /// 处理People集合变化事件，自动订阅/取消订阅SelectionChanged事件
    /// </summary>
    private void OnPeopleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 处理移除的项目
        if (e.OldItems != null)
        {
            foreach (PersonViewModel person in e.OldItems)
            {
                person.SelectionChanged -= OnPersonSelectionChanged;
            }
        }

        // 处理添加的项目
        if (e.NewItems != null)
        {
            foreach (PersonViewModel person in e.NewItems)
            {
                person.SelectionChanged += OnPersonSelectionChanged;
            }
        }

        UpdateSelectAllState();
    }

    /// <summary>
    /// 为所有现有Person订阅SelectionChanged事件
    /// </summary>
    private void SubscribeToAllPersonEvents()
    {
        foreach (var person in People)
        {
            person.SelectionChanged += OnPersonSelectionChanged;
        }
    }

    partial void OnIsAllSelectedChanged(bool? value)
    {
        if (!_isUpdatingSelectAllState && value.HasValue)
        {
            // 使用临时标志避免递归调用，不需要取消订阅事件
            _isUpdatingSelectAllState = true;

            foreach (var person in People)
            {
                person.IsSelected = value.Value;
            }

            _isUpdatingSelectAllState = false;
        }
    }

    private void OnPersonSelectionChanged()
    {
        UpdateSelectAllState();
    }

    private void UpdateSelectAllState()
    {
        _isUpdatingSelectAllState = true;

        if (People.Count == 0)
        {
            IsAllSelected = false;
        }
        else if (People.All(p => p.IsSelected))
        {
            IsAllSelected = true;
        }
        else if (People.All(p => !p.IsSelected))
        {
            IsAllSelected = false;
        }
        else
        {
            IsAllSelected = null; // Some selected, some not (indeterminate state)
        }

        _isUpdatingSelectAllState = false;
    }
}