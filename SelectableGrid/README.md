# 可复用全选功能使用指南

## 概述

本项目实现了一个可复用的全选功能，通过抽象基类 `SelectableCollectionViewModelBase<T>` 来提供通用的全选/取消全选逻辑。任何需要全选功能的 ViewModel 都可以继承这个基类。

## 核心组件

### 1. ISelectable 接口

所有可选择的项目必须实现此接口：

```csharp
public interface ISelectable
{
    bool IsSelected { get; set; }
    event Action? SelectionChanged;
}
```

### 2. SelectableCollectionViewModelBase<T> 抽象基类

提供全选功能的基类，包含以下核心功能：
- 自动管理选择状态
- 支持三态复选框（全选/全不选/部分选择）
- 自动处理集合变化事件
- 提供便捷的批量操作方法

### 3. ICanSelectAll 接口

定义全选功能的契约：

```csharp
public interface ICanSelectAll 
{
    bool? IsAllSelected { get; set; }
}
```

## 使用步骤

### 步骤1：让数据模型实现 ISelectable 接口

```csharp
public partial class PersonViewModel : ObservableObject, ISelectable
{
    [ObservableProperty] private bool _isSelected;
    public event Action? SelectionChanged;

    partial void OnIsSelectedChanged(bool value)
    {
        SelectionChanged?.Invoke();
    }
    
    // 其他属性...
}
```

### 步骤2：让 ViewModel 继承 SelectableCollectionViewModelBase

```csharp
public partial class MyViewModel : SelectableCollectionViewModelBase<PersonViewModel>
{
    [ObservableProperty] private ObservableCollection<PersonViewModel> _people;

    // 实现抽象属性
    protected override ObservableCollection<PersonViewModel> SelectableItems => People;

    public MyViewModel()
    {
        People = new ObservableCollection<PersonViewModel>();
        
        // 重要：调用基类初始化方法
        InitializeSelectAllFeature();
    }
}
```

### 步骤3：在 AXAML 中绑定全选功能

```xaml
<DataGridTemplateColumn Header="选择">
    <DataGridTemplateColumn.HeaderTemplate>
        <DataTemplate>
            <CheckBox IsChecked="{Binding $parent[UserControl].DataContext.IsAllSelected, Mode=TwoWay}" 
                      Content="全选" />
        </DataTemplate>
    </DataGridTemplateColumn.HeaderTemplate>
    <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <CheckBox IsChecked="{Binding IsSelected}" />
        </DataTemplate>
    </DataGridTemplateColumn.CellTemplate>
</DataGridTemplateColumn>
```

## 提供的便捷方法

基类提供了以下便捷方法：

```csharp
// 获取所有选中的项目
protected IEnumerable<T> GetSelectedItems()

// 删除所有选中的项目
protected void RemoveSelectedItems()

// 初始化全选功能（构造函数中调用）
protected void InitializeSelectAllFeature()

// 清理资源（可选，在析构时调用）
protected void CleanupSelectAllFeature()
```

## 示例用法

### 人员管理示例

```csharp
[RelayCommand]
private void DeleteSelectedPersons()
{
    RemoveSelectedItems(); // 使用基类提供的方法
}

[RelayCommand]
private void ProcessSelectedPersons()
{
    var selectedPersons = GetSelectedItems().ToList();
    foreach (var person in selectedPersons)
    {
        // 处理选中的人员
    }
}
```

### 任务管理示例

```csharp
[RelayCommand]
private void MarkSelectedAsCompleted()
{
    var selectedTasks = GetSelectedItems().ToList();
    foreach (var task in selectedTasks)
    {
        task.IsCompleted = true;
    }
}
```

## 特性

1. **自动状态管理**：自动处理全选状态的三态变化
2. **事件自动订阅**：集合项目的添加/删除时自动处理事件订阅
3. **防止递归调用**：内置机制防止选择状态更新时的递归调用
4. **类型安全**：使用泛型确保类型安全
5. **MVVM 友好**：完全兼容 MVVM 模式和数据绑定

## 注意事项

1. 子类构造函数中必须调用 `InitializeSelectAllFeature()`
2. 数据模型必须实现 `ISelectable` 接口
3. 在 AXAML 中正确绑定 `IsAllSelected` 属性
4. 使用 `$parent[UserControl].DataContext` 来访问 ViewModel 的全选属性

## 扩展性

这个设计可以轻松扩展到其他场景：
- 文件管理器的文件选择
- 邮件客户端的邮件选择
- 商品管理的商品选择
- 等等...

只需要让对应的数据模型实现 `ISelectable` 接口，ViewModel 继承 `SelectableCollectionViewModelBase<T>` 即可。
