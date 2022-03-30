using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using RevitAddinManager.Model;
using RevitAddinManager.ViewModel;

//Done 1、搜索高亮
//Done 2、右键切换当前选择项SelectedItem
//Done 3、右键菜单图标以及快捷键
//TODO 4、TreeView 鼠标掠过效果

namespace RevitAddinManager.View;

/// <summary>
/// Interaction logic for FrmAddInManager.xaml
/// </summary>
public partial class FrmAddInManager : Window
{
    private readonly AddInManagerViewModel viewModel;
    public FrmAddInManager(AddInManagerViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        viewModel = vm;
        vm.FrmAddInManager = this;
    }

    private void TbxDescription_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (viewModel.MAddinManagerBase.ActiveCmdItem != null && TabControl.SelectedIndex == 0)
        {
            viewModel.MAddinManagerBase.ActiveCmdItem.Description = TbxDescription.Text;
        }
        if (viewModel.MAddinManagerBase.ActiveAppItem != null && TabControl.SelectedIndex == 1)
        {
            viewModel.MAddinManagerBase.ActiveAppItem.Description = TbxDescription.Text;
        }
        viewModel.MAddinManagerBase.AddinManager.SaveToAimIni();
    }


    /// <summary>
    /// TreeViewCommand树节点的右键选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeViewCommand_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        #region mvvm模式下右键选中方案
        var currentSelectedItem = SelectItemByRightClickOfMvvm(TreeViewCommand);
        if (currentSelectedItem != null && currentSelectedItem.DataContext is AddinModel treeNode)
        {
            viewModel.SelectedCommandItem = treeNode;
        }
        #endregion
    }
    /// <summary>
    /// TreeViewApp树节点右键选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeViewApp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var currentSelectedItem = SelectItemByRightClickOfMvvm(TreeViewApp);
        if (currentSelectedItem != null && currentSelectedItem.DataContext is AddinModel treeNode)
        {
            viewModel.SelectedAppItem = treeNode;
        }
    }
    public TreeViewItem SelectItemByRightClickOfMvvm(ItemsControl source)
    {
        if (!(source is TreeView) && !(source is TreeViewItem))
        {
            throw new ArgumentException("只支持参数为TreeView或者TreeViewItem", "source");
        }
        foreach (object item in source.Items)
        {
            TreeViewItem currentItem = source.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
            Point mousePosition = Mouse.GetPosition(currentItem);
            Rect itemRect = VisualTreeHelper.GetDescendantBounds(currentItem);
            // 可能是选中的项，也可能是选中项的父节点
            if (itemRect.Contains(mousePosition))
            {
                // 看看是不是它的孩子被选中了，否则就是它自己被选中了              
                if (currentItem.IsExpanded)
                {
                    // 只判断展开的项
                    TreeViewItem selectedItem = SelectItemByRightClickOfMvvm(currentItem);
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = true;
                        return selectedItem;
                    }
                }
                currentItem.IsSelected = true;

                return currentItem;
            }
        }
        return null;
    }
}