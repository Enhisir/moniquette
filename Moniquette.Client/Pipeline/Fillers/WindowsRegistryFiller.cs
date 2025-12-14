using Hardware.Info;
using Microsoft.Win32;
using Moniquette.Common;
using Moniquette.Common.Helpers;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

[System.Runtime.Versioning.SupportedOSPlatform(Literals.Windows)]
public class WindowsRegistryFiller(IHardwareInfo info) : IReportFiller
{
    public Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        // HardwareInfo should be already refreshed 
        if (!info.OperatingSystem.Name.Equals(
                Literals.Windows, 
                StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult(report);

        var registryArchive = new Dictionary<string, string>();
        foreach (var (hive, view, _) in Roots)
        {
            TryActionBase(() =>
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                GetRegistryEntriesDfs(registryArchive, baseKey);
            });
        }
        report.WindowsRegistry = registryArchive;
        
        return Task.FromResult(report);
    }

    private static readonly (RegistryHive Hive, RegistryView View, string BasePath)[] Roots =
    [
        (RegistryHive.LocalMachine, RegistryView.Registry64, "HKLM64"),
        (RegistryHive.LocalMachine, RegistryView.Registry32, "HKLM32"),
        (RegistryHive.CurrentUser, RegistryView.Registry64, "HKCU64"),
        (RegistryHive.CurrentUser, RegistryView.Registry32, "HKCU32"),
        (RegistryHive.Users, RegistryView.Registry64, "HKU64"),
        (RegistryHive.Users, RegistryView.Registry32, "HKU32"),
        (RegistryHive.ClassesRoot, RegistryView.Registry64, "HKCR64"),
        (RegistryHive.ClassesRoot, RegistryView.Registry32, "HKCR32"),
        (RegistryHive.CurrentConfig, RegistryView.Registry64, "HKCC64"),
        (RegistryHive.CurrentConfig, RegistryView.Registry32, "HKCC32")
    ];

    private void GetRegistryEntriesDfs(
        Dictionary<string, string> registryArchive,
        RegistryKey key,
        string basePath = "",
        int currentDepth = 0,
        int maxDepth = 5)
    {
        if (currentDepth > maxDepth) return;
        TryActionBase(() =>
        {
            foreach (var entry in key.GetValueNames())
            {
                TryActionBase(() =>
                {
                    var value = key.GetValue(entry)?.ToString() ?? "NULL";
                    registryArchive.Add(entry, value);
                });
            }
        });

        TryActionBase(() =>
        {
            foreach (var name in key.GetSubKeyNames())
            {
                TryActionBase(() =>
                {
                    using var sub = key.OpenSubKey(name)!;
                    GetRegistryEntriesDfs(
                        registryArchive,
                        sub,
                        $"{basePath}\\{name}".Trim('\\'),
                        currentDepth + 1,
                        maxDepth);
                });
            }
        });
    }

    private void TryAction<TException>(
        Action action,
        Action<TException>? exceptionAction = null)
        where TException : Exception
    {
        try
        {
            action.Invoke();
        }
        catch (TException e)
        {
            exceptionAction?.Invoke(e);
        }
    }

    private void TryActionBase(
        Action action,
        Action<Exception>? exceptionAction = null)
        => TryAction(action, exceptionAction);
}