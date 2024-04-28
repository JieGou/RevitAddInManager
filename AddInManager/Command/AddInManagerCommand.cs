using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAddinManager.Model;

//Note 使用启动外部程序的方式调试 Revit崩溃 未找到解决方案
// 使用另一种替代方法进行调试:设置addin加载调试路径生成的dll，启动Revit，vs附加到进程。启动命令即可进行相应的调试

namespace RevitAddinManager.Command;

[Transaction(TransactionMode.Manual)]
public class AddInManagerManual : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Trace.Listeners.Clear();
        CodeListener codeListener = new CodeListener();
        Trace.Listeners.Add(codeListener);
        StaticUtil.RegenOption = RegenerationOption.Manual;
        StaticUtil.TransactMode = TransactionMode.Manual;
        Result result = AddinManagerBase.Instance.ExecuteCommand(commandData, ref message, elements, false);
        return result;
    }
}

[Transaction(TransactionMode.Manual)]
public class AddInManagerFaceless : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        return AddinManagerBase.Instance.ExecuteCommand(commandData, ref message, elements, true);
    }
}

[Transaction(TransactionMode.Manual)]
public class AddInManagerReadOnly : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        StaticUtil.RegenOption = RegenerationOption.Manual;
        StaticUtil.TransactMode = TransactionMode.ReadOnly;
        return AddinManagerBase.Instance.ExecuteCommand(commandData, ref message, elements, false);
    }
}

public class AddinManagerCommandAvail : IExternalCommandAvailability
{
    public AddinManagerCommandAvail()
    {
    }

    public bool IsCommandAvailable(UIApplication uiApp, CategorySet selectedCategories)
    {
        return true;
    }
}