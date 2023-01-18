using System.Diagnostics;

namespace McPatchForMultiInstance;
public class Program
{
    public static void PatchMain(string[] args)
    {
        bool? enable = null;
        if (args.Length == 1)
        {
            if (args[0] == "-e")
                enable = true;
            else if (args[0] == "-d")
                enable = false;
            else
            {
                // output usage and exit
                Console.WriteLine($"Usage: {Process.GetCurrentProcess().ProcessName} [-e|-d]");
                return;
            }
        }
        // Check if development mode is enabled
        if (!Util.IsDeveloperModeEnabled())
        {
            Console.WriteLine("Please enable developer mode in the settings app...");
            Process.Start("explorer.exe", "ms-settings:developers");
            while (!Util.IsDeveloperModeEnabled())
            {
                Thread.Sleep(100);
            }
        }
        if (Util.McProc == null) Process.Start("explorer.exe", "minecraft:");
        while (Util.McProc == null) Thread.Sleep(100);
        while (Util.McProc.MainModule == null) { Thread.Sleep(100); }

        string McPath = Path.GetFullPath(Util.McProc.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        Util.GrantAccess(McPath);

        bool? result = null;
        if (enable == null) result = McUtil.ToggleMultiInstance();
        else if ((bool)enable) McUtil.EnableMultiInstance();
        else McUtil.DisableMultiInstance();

        result ??= enable;
        Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", McPath).Wait();

        if (result != null && (bool)result) Console.WriteLine("Multi-instance is now enabled!");
        else Console.WriteLine("Multi-instance is now disabled!");
        if (enable == null) Thread.Sleep(3000);
        // Launch minecraft with the Minecraft: protocol
        Process.Start("explorer.exe", "minecraft:");
    }
    public static void Main(string[] args)
    {
        try
        {
            PatchMain(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
            Console.ReadKey(true);
        }
    }
}
