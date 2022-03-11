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
//Done 3、图标以及快捷键
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
    DispatcherTimer _typingTimer; // WPF
    private void tbxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (_typingTimer == null)
        {
            _typingTimer = new DispatcherTimer();
            _typingTimer.Interval = TimeSpan.FromMilliseconds(200);

            _typingTimer.Tick += new EventHandler(this.handleTypingTimerTimeout);
        }
        _typingTimer.Stop(); // Resets the timer
        _typingTimer.Tag = (sender as TextBox).Text; // This should be done with EventArgs
        _typingTimer.Start();

    }
    private void handleTypingTimerTimeout(object sender, EventArgs e)
    {
        //var timer = sender as Timer; // WinForms
        var timer = sender as DispatcherTimer; // WPF
        if (timer == null)
        {
            return;
        }

        // Testing - updates window title
        var isbn = timer.Tag.ToString();
        //windowFrame.Text = isbn; // WinForms
        //windowFrame.Title = isbn; // WPF
        FindControlItem(this.TreeViewCommand, isbn);
        FindControlItem(this.TreeViewApp, isbn);
        FindControlItem(this.DataGridStartup, isbn);
        // The timer must be stopped! We want to act only once per keystroke.
        timer.Stop();
    }

    public void FindControlItem(DependencyObject obj, string searchStr)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            ListViewItem lv = obj as ListViewItem;
            DataGridCell dg = obj as DataGridCell;
            TreeViewItem tv = obj as TreeViewItem;
            if (lv != null)
            {
                HighlightText(lv, tbxSearch.Text, isBold: true);
            }
            if (dg != null)
            {
                HighlightText(dg, tbxSearch.Text, isBold: true);
            }
            if (tv != null)
            {
                HighlightText(tv, tbxSearch.Text, isBold: true);
            }
            FindControlItem(VisualTreeHelper.GetChild(obj as DependencyObject, i), searchStr);
        }
    }
    //From http://u6.gg/ksn8p 短地址
    /// <summary>
    /// 高亮文本
    /// </summary>
    /// <param name="itx">包含文本的控件(容器)</param>
    /// <param name="searchKeywords">搜索关键字</param>
    /// <param name="background">背景色 默认黄色</param>
    /// <param name="foreground">前景色 默认黑色</param>
    /// <param name="isItalic">是否高亮部分斜体 默认true</param>
    /// <param name="isUnderline">是否高亮部分下划线 默认true</param>
    /// <param name="isBold">是否高亮部分加粗 默认false</param>
    private void HighlightText(Object itx, string searchKeywords,
                               Brush background = default(Brush),
                               Brush foreground = default(Brush),
                               bool isItalic = true,
                               bool isUnderline = true,
                               bool isBold = false)
    {
        if (background == default(Brush)) { background = Brushes.Yellow; }
        if (foreground == default(Brush)) { foreground = Brushes.Black; }

        if (itx == null) return;
        if (itx is TextBlock)
        {
            var regex = new Regex("(" + searchKeywords + ")", RegexOptions.IgnoreCase);
            TextBlock tb = itx as TextBlock;
            if (searchKeywords.Length == 0)
            {
                string str = tb.Text;
                tb.Inlines.Clear();
                tb.Inlines.Add(str);
                return;
            }
            string[] substrings = regex.Split(tb.Text);
            tb.Inlines.Clear();
            foreach (var item in substrings)
            {
                if (regex.Match(item).Success)
                {
                    Run runx = new Run(item);
                    runx.Background = background;
                    runx.Foreground = foreground;
                    if (isItalic) runx.FontStyle = FontStyles.Italic;
                    if (isBold) runx.FontWeight = FontWeights.Bold;
                    if (isUnderline) runx.TextDecorations = new TextDecorationCollection()
                        {
                            new TextDecoration() { Pen= new Pen(Brushes.Blue,1), PenThicknessUnit=TextDecorationUnit.FontRecommended, },
                        };
                    tb.Inlines.Add(runx);
                }
                else
                {
                    tb.Inlines.Add(item);
                }
            }
            return;
        }
        else
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
            {
                HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i), searchKeywords, background, foreground, isItalic, isUnderline, isBold);
            }
        }
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
        var currentSelectedItem = SelectItemByRightClickOfMvvm(TreeViewCommand);
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