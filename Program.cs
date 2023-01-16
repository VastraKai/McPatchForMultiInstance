using System.Diagnostics;
namespace McPatchForMultiInstance;
public class Program
{
    public static void Main(string[] args)
    {

        // Check if development mode is enabled (broken lol)
        if (!Util.IsDeveloperModeEnabled())
        {
            // Show a message box
            Console.WriteLine("Please enable developer mode in the settings app.");
            Console.ReadKey(true);
            // Exit the program
            return;
        }
        while (Util.McProc == null)
        {
            Console.Write("Please open Minecraft...\r");
        }
        Console.Write("\r                        \r");
        while (Util.McProc.MainModule == null) { Thread.Sleep(100); }
        string McPath = Path.GetFullPath(Util.McProc.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        Console.WriteLine("Modifying AppxManifest");
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        McManContent =
        McManContent.Replace("<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\">",
                             "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\" desktop4:SupportsMultipleInstances=\"true\">");
        McManContent =
        McManContent.Replace("<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/2014/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\">",
                             "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/201/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\" xmlns:desktop4=\"http://schemas.microsoft.com/appx/manifest/desktop/windows10/4\">");
        File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
        Console.WriteLine("Re-registering Appx...");
        Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", McPath).Wait();
        Console.WriteLine("Finished.");
        // Launch minecraft with the Minecraft: protocol
        Process.Start("explorer.exe", "minecraft:");
    }
}
