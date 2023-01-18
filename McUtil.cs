namespace McPatchForMultiInstance;
public static class McUtil
{
    private static string orig1 = "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\">";
    private static string rep1 = "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\" desktop4:SupportsMultipleInstances=\"true\">";
    private static string orig2 = "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/2014/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\">";
    private static string rep2 = "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/201/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\" xmlns:desktop4=\"http://schemas.microsoft.com/appx/manifest/desktop/windows10/4\">";
    public static bool ToggleMultiInstance()
    {
        string McPath = Path.GetFullPath(Util.McProc.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        
        if (McManContent.Contains(orig1) && McManContent.Contains(orig2))
        {

            McManContent = McManContent.Replace(orig1, rep1);
            McManContent = McManContent.Replace(orig2, rep2);
            File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
            return true;
        }
        else if (McManContent.Contains(rep1) && McManContent.Contains(rep2))
        {
            McManContent = McManContent.Replace(rep1, orig1);
            McManContent = McManContent.Replace(rep2, orig2);
            File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
            return false;
        }
        return false;
        
    }
    public static void EnableMultiInstance()
    {
        string McPath = Path.GetFullPath(Util.McProc.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        McManContent = McManContent.Replace(orig1, rep1);
        McManContent = McManContent.Replace(orig2, rep2);
        File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
    }
    public static void DisableMultiInstance()
    {
        string McPath = Path.GetFullPath(Util.McProc.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        McManContent = McManContent.Replace(rep1, orig1);
        McManContent = McManContent.Replace(rep2, orig2);
        File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
    }
}
