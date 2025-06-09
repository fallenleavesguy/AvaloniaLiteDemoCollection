using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SelectableGrid.ViewModels;

public interface ICanSelectAll
{
    /// <summary>
    /// 是否全选
    /// </summary>
    bool? IsAllSelected { get; set; }
}

/// <summary>
/// 可复用的全选功能基类，适用于任何包含可选择项目集合的ViewModel
/// </summary>
/// <typeparam name="T">集合中项目的类型，必须实现ISelectable接口</typeparam>
public abstract partial class SelectableCollectionViewModelBase<T> : ViewModelBase, ICanSelectAll
    where T : ISelectable
{
    [ObservableProperty] private bool? _isAllSelected;

    private bool _isUpdatingSelectAllState = false;

    /// <summary>
    /// 子类需要实现此属性来提供可选择的项目集合
    /// </summary>
    protected abstract ObservableCollection<T> SelectableItems { get; }

    /// <summary>
    /// 初始化全选功能，子类构造函数中需要调用此方法
    /// </summary>
    protected void InitializeSelectAllFeature()
    {
        // 为集合变化事件订阅处理器
        SelectableItems.CollectionChanged += OnSelectableItemsCollectionChanged;

        // 为现有项目订阅事件
        SubscribeToAllItemEvents();

        // 更新全选状态
        UpdateSelectAllState();
    }

    /// <summary>
    /// 清理资源，子类析构时应调用此方法
    /// </summary>
    protected void CleanupSelectAllFeature()
    {
        SelectableItems.CollectionChanged -= OnSelectableItemsCollectionChanged;
        UnsubscribeFromAllItemEvents();
    }

    /// <summary>
    /// 处理集合变化事件，自动订阅/取消订阅SelectionChanged事件
    /// </summary>
    private void OnSelectableItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 处理移除的项目
        if (e.OldItems != null)
        {
            foreach (T item in e.OldItems)
            {
                item.SelectionChanged -= OnItemSelectionChanged;
            }
        }

        // 处理添加的项目
        if (e.NewItems != null)
        {
            foreach (T item in e.NewItems)
            {
                item.SelectionChanged += OnItemSelectionChanged;
            }
        }

        UpdateSelectAllState();
    }

    /// <summary>
    /// 为所有现有项目订阅SelectionChanged事件
    /// </summary>
    private void SubscribeToAllItemEvents()
    {
        foreach (var item in SelectableItems)
        {
            item.SelectionChanged += OnItemSelectionChanged;
        }
    }

    /// <summary>
    /// 取消所有项目的SelectionChanged事件订阅
    /// </summary>
    private void UnsubscribeFromAllItemEvents()
    {
        foreach (var item in SelectableItems)
        {
            item.SelectionChanged -= OnItemSelectionChanged;
        }
    }

    /// <summary>
    /// 处理IsAllSelected属性变化
    /// </summary>
    partial void OnIsAllSelectedChanged(bool? value)
    {
        if (!_isUpdatingSelectAllState && value.HasValue)
        {
            // 使用临时标志避免递归调用
            _isUpdatingSelectAllState = true;

            foreach (var item in SelectableItems)
            {
                item.IsSelected = value.Value;
            }

            _isUpdatingSelectAllState = false;
        }
    }

    /// <summary>
    /// 单个项目选择状态变化时的处理
    /// </summary>
    private void OnItemSelectionChanged()
    {
        UpdateSelectAllState();
    }

    /// <summary>
    /// 更新全选状态
    /// </summary>
    private void UpdateSelectAllState()
    {
        _isUpdatingSelectAllState = true;

        if (SelectableItems.Count == 0)
        {
            IsAllSelected = false;
        }
        else if (SelectableItems.All(p => p.IsSelected))
        {
            IsAllSelected = true;
        }
        else if (SelectableItems.All(p => !p.IsSelected))
        {
            IsAllSelected = false;
        }
        else
        {
            IsAllSelected = null; // Some selected, some not (indeterminate state)
        }

        _isUpdatingSelectAllState = false;
    }

    /// <summary>
    /// 获取所有被选中的项目
    /// </summary>
    public IEnumerable<T> GetSelectedItems()
    {
        return SelectableItems.Where(item => item.IsSelected);
    }

    /// <summary>
    /// 删除所有被选中的项目
    /// </summary>
    protected void RemoveSelectedItems()
    {
        var itemsToRemove = GetSelectedItems().ToList();
        foreach (var item in itemsToRemove)
        {
            SelectableItems.Remove(item);
        }
    }
}