﻿using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Management.Core;
using Windows.Management.Deployment;

namespace McPatchForMultiInstance;
public static class Util
{

    // ----- The following methods are from https://github.com/MCMrARM/mc-w10-version-launcher/blob/master/MCLauncher/MainWindow.xaml.cs
    public static string GetBackupMinecraftDataDir()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string tmpDir = Path.Combine(localAppData, "TmpMinecraftLocalState");
        return tmpDir;
    }

    public static void BackupMinecraftDataForRemoval(string packageFamily)
    {
        var data = ApplicationDataManager.CreateForPackageFamily(packageFamily);
        string tmpDir = GetBackupMinecraftDataDir();
        if (Directory.Exists(tmpDir))
        {
            Console.WriteLine("BackupMinecraftDataForRemoval error: " + tmpDir + " already exists");
            Process.Start("explorer.exe", tmpDir);
            throw new Exception("Temporary dir exists");
        }
        Console.WriteLine("Moving Minecraft data to: " + tmpDir);
        Directory.Move(data.LocalFolder.Path, tmpDir);
    }

    public static void RestoreMove(string from, string to)
    {
        foreach (var f in Directory.EnumerateFiles(from))
        {
            string ft = Path.Combine(to, Path.GetFileName(f));
            if (File.Exists(ft))
                File.Delete(ft);

            File.Move(f, ft);
        }
        foreach (var f in Directory.EnumerateDirectories(from))
        {
            string tp = Path.Combine(to, Path.GetFileName(f));
            if (!Directory.Exists(tp))
            {
                if (File.Exists(tp))
                    continue;
                Directory.CreateDirectory(tp);
            }
            RestoreMove(f, tp);
        }
    }

    public static void RestoreMinecraftDataFromReinstall(string packageFamily)
    {
        string tmpDir = GetBackupMinecraftDataDir();
        if (!Directory.Exists(tmpDir))
            return;
        var data = ApplicationDataManager.CreateForPackageFamily(packageFamily);
        Console.WriteLine("Moving backup Minecraft data to: " + data.LocalFolder.Path);
        RestoreMove(tmpDir, data.LocalFolder.Path);
        Directory.Delete(tmpDir, true);
    }

    public static async Task RemovePackage(Package pkg, string packageFamily)
    {
        Console.WriteLine("Removing package: " + pkg.Id.FullName);
        if (!pkg.IsDevelopmentMode)
        {
            BackupMinecraftDataForRemoval(packageFamily);
            new PackageManager().RemovePackageAsync(pkg.Id.FullName, 0).AsTask().Wait();
        }
        else
        {
            Console.WriteLine("Package is in development mode");
            new PackageManager().RemovePackageAsync(pkg.Id.FullName, RemovalOptions.PreserveApplicationData).AsTask().Wait();
        }
        Console.WriteLine("Removal of package done: " + pkg.Id.FullName);
    }

    public static string GetPackagePath(Package pkg)
    {
        try
        {
            return pkg.InstalledLocation.Path;
        }
        catch (FileNotFoundException)
        {
            return "";
        }
    }

    public static async Task UnregisterPackage(string packageFamily, string gameDir)
    {
        foreach (var pkg in new PackageManager().FindPackages(packageFamily))
        {
            string location = GetPackagePath(pkg);
            if (location == "" || location == gameDir)
            {
                await RemovePackage(pkg, packageFamily);
            }
        }
    }

    public static async Task ReRegisterPackage(string packageFamily, string gameDir)
    {
        foreach (var pkg in new PackageManager().FindPackages(packageFamily))
        {
            string location = GetPackagePath(pkg);
            if (location == gameDir)
            {
                Console.WriteLine("Skipping package removal - same path: " + pkg.Id.FullName + " " + location);
                return;
            }
            await RemovePackage(pkg, packageFamily);
        }
        Console.WriteLine("Registering package");
        string manifestPath = Path.Combine(gameDir, "AppxManifest.xml");
        new PackageManager().RegisterPackageAsync(new Uri(manifestPath), null, DeploymentOptions.DevelopmentMode).AsTask().Wait();
        Console.WriteLine("App re-regiRegisterPackageAsyncster done!");
        RestoreMinecraftDataFromReinstall(packageFamily);
    }
    // ----------

    // Check if developer mode is enabled with the key name "AllowDevelopmentWithoutDevLicense"
    public static bool IsDeveloperModeEnabled()
    {
        try
        {
            // Check if dev mode is enabled using the error code returned from cmd /c "reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock /v AllowDevelopmentWithoutDevLicense | findstr /I /C:"0x1""
            // Create a process to run the command
            Process p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\cmd.exe");
            p.StartInfo.Arguments = "/c \"reg query HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AppModelUnlock /v AllowDevelopmentWithoutDevLicense | findstr /I /C:\"0x1\"\"";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            // Get the output
            string output = p.StandardOutput.ReadToEnd();
            // Check if the output contains "0x1"
            if (output.Contains("0x1"))
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while checking if developer mode is enabled: " + ex);
            Console.ReadKey(true);
            return true;
        }
    }

    // Field to get the minecraft.windows process
    public static Process? McProc
    {
        get
        {
            // Get all processes
            Process[] processes = Process.GetProcesses();
            // Loop through all processes
            foreach (Process process in processes)
            {
                // Check if the process name is Minecraft.Windows
                if (process.ProcessName == "Minecraft.Windows")
                {
                    // Return the process
                    return process;
                }
            }
            // Return null
            return null;
        }
    }

}

