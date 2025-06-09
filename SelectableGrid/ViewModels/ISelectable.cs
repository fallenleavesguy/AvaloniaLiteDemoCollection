using System;

namespace SelectableGrid.ViewModels;

/// <summary>
/// 定义可选择项目的接口
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// 项目是否被选中
    /// </summary>
    bool IsSelected { get; set; }
    
    /// <summary>
    /// 选择状态变化事件
    /// </summary>
    event Action? SelectionChanged;
}
