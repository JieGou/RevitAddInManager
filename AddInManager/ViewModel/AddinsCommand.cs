using RevitAddinManager.Model;

namespace RevitAddinManager.ViewModel;

/// <summary>
/// Addins命令类
/// </summary>
public class AddinsCommand : Addins
{
    /// <summary>
    /// 外部命令
    /// </summary>
    private static string ExternalName = "ExternalCommands";

    /// <summary>
    /// 外部命令名称
    /// </summary>
    private static string ExternalCName = "ECName";

    /// <summary>
    /// 外部命令数量
    /// </summary>
    private static string ExternalCount = "ECCount";

    /// <summary>
    /// 外部命令对应的类名称
    /// </summary>
    private static string ExternalClassName = "ECClassName";

    /// <summary>
    /// 外部命令对应的程序集名称
    /// </summary>
    private static string ExternalAssembly = "ECAssembly";

    /// <summary>
    /// 外部命令的帮助描述信息
    /// </summary>
    private static string ExternalDescription = "ECDescription";

    public void ReadItems(IniFile file)
    {
        var num = file.ReadInt(ExternalName, ExternalCount);
        var i = 1;
        while (i <= num)
        {
            ReadExternalCommand(file, i++);
        }
        SortAddin();
    }

    void ReadExternalCommand(IniFile file, int nodeNumber)
    {
        var name = file.ReadString(ExternalName, ExternalCName + nodeNumber);
        var text = file.ReadString(ExternalName, ExternalAssembly + nodeNumber);
        var text2 = file.ReadString(ExternalName, ExternalClassName + nodeNumber);
        var description = file.ReadString(ExternalName, ExternalDescription + nodeNumber);
        if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text))
        {
            return;
        }
        AddItem(new AddinItem(AddinType.Command)
        {
            Name = name,
            AssemblyPath = text,
            FullClassName = text2,
            Description = description
        });
    }

    /// <summary>
    /// 保存命令到ini文件
    /// </summary>
    /// <param name="file"></param>
    public void Save(IniFile file)
    {
        file.WriteSection(ExternalName);
        file.Write(ExternalName, ExternalCount, maxCount);
        var num = 0;
        foreach (var addin in addinDict.Values)
        {
            foreach (var addinItem in addin.ItemList)
            {
                if (num >= maxCount)
                {
                    break;
                }
                if (addinItem.Save)
                {
                    WriteExternalCommand(file, addinItem, ++num);
                }
            }
        }
        file.Write(ExternalName, ExternalCount, num);
    }

    private bool WriteExternalCommand(IniFile file, AddinItem item, int number)
    {
        file.Write(ExternalName, ExternalCName + number, item.Name);
        file.Write(ExternalName, ExternalClassName + number, item.FullClassName);
        file.Write(ExternalName, ExternalAssembly + number, item.AssemblyPath);
        file.Write(ExternalName, ExternalDescription + number, item.Description);
        return true;
    }
}