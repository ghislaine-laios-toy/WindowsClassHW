using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Lesson3DllRegEdit;

public class Registry
{
    [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegCreateKeyEx(IntPtr hKey, string lpSubKey, int reserved, string type, int dwOptions,
        int regSam, IntPtr lpSecurityAttributes, out IntPtr phkResult,
        out int lpdwDisposition);

    [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegSetValueEx(IntPtr hKey, string lpValueName, uint unReserved, uint unType,
        byte[] lpData, uint dataCount);

    [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegCloseKey(IntPtr hKey);

    [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegEnumKey(IntPtr hKey, int dwIndex, StringBuilder lpData, int ipcbName);

    [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegEnumValue(IntPtr hKey, int dwIndex, StringBuilder ipValueName, ref uint ipcbValueName,
        IntPtr ipReserved, ref RegistryValueKind lpType, StringBuilder ipData, ref uint lpcbData);

    [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegDeleteKeyEx(IntPtr hKey, string lpSubKey, int samDesired, int Reserved);

    private static void CheckRet(int ret, string functionName)
    {
        if (ret != 0) throw new RegistryException($"{functionName} fails", ret);
    }

    public static IntPtr CreateOrOpenKey(IntPtr key, string subKey, out int lpdwDisposition)
    {
        var ret = RegCreateKeyEx(key, subKey, 0, "", DwOptions.REG_OPTION_NON_VOLATILE,
            AccessRights.KEY_ALL_ACCESS | AccessRights.KEY_WOW64_64KEY, IntPtr.Zero, out var resultPtr,
            out lpdwDisposition);
        CheckRet(ret, nameof(RegCreateKeyEx));
        return resultPtr;
    }

    public static IntPtr CreateOrOpenKey(IntPtr key, string subKey)
    {
        return CreateOrOpenKey(key, subKey, out _);
    }

    //public static IntPtr OpenKey(IntPtr key, string subKey, out int lpdwDisposition) {}

    public static void SetValue(IntPtr key, string name, string value)
    {
        var data = Encoding.Unicode.GetBytes(value);
        var ret = RegSetValueEx(key, name, 0, 1, data, (uint)data.Length);
        CheckRet(ret, nameof(RegSetValueEx));
    }

    public static void CloseKey(IntPtr key)
    {
        var ret = RegCloseKey(key);
        CheckRet(ret, nameof(RegCloseKey));
    }

    public static IList<string> GetSubkeys(IntPtr key)
    {
        const int bufferLength = 200;
        var buffer = new StringBuilder(bufferLength);
        var result = new List<string>();
        for (int i = 0, ret = 0; ret == 0; i++)
        {
            ret = RegEnumKey(key, i, buffer, bufferLength);
            if (ret == 259) break;
            CheckRet(ret, nameof(RegEnumKey));
            result.Add(buffer.ToString());
        }

        return result;
    }

    public static IList<(string, string)> GetValues(IntPtr key)
    {
        uint bufferLength = 200;
        var nameBuffer = new StringBuilder((int)bufferLength);
        var valueBuffer = new StringBuilder((int)bufferLength);
        var result = new List<(string, string)>();
#pragma warning disable CA1416
        var lpType = RegistryValueKind.Unknown;
#pragma warning restore CA1416
        for (int i = 0, ret = 0; ret == 0; i++)
        {
            ret = RegEnumValue(key, i, nameBuffer, ref bufferLength, IntPtr.Zero, ref lpType, valueBuffer,
                ref bufferLength);
            if (ret == 259) break;
            CheckRet(ret, nameof(RegEnumValue));
            result.Add((nameBuffer.ToString(), valueBuffer.ToString()));
        }

        return result;
    }

    public static void DeleteKey(IntPtr key, string subKey)
    {
        var ret = RegDeleteKeyEx(key, subKey, 0, 0);
        CheckRet(ret, nameof(RegDeleteKeyEx));
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Handlers
    {
        public static readonly IntPtr HKEY_CLASSES_ROOT = new(unchecked((int)0x80000000));
        public static readonly IntPtr HKEY_CURRENT_USER = new(unchecked((int)0x80000001));
        public static readonly IntPtr HKEY_LOCAL_MACHINE = new(unchecked((int)0x80000002));
        public static readonly IntPtr HKEY_USERS = new(unchecked((int)0x80000003));
        public static readonly IntPtr HKEY_PERFORMANCE_DATA = new(unchecked((int)0x80000004));
        public static readonly IntPtr HKEY_CURRENT_CONFIG = new(unchecked((int)0x80000005));
        public static readonly IntPtr HKEY_DYN_DATA = new(unchecked((int)0x80000006));
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class DwOptions
    {
        public const int REG_OPTION_NON_VOLATILE = 0x00000000;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class AccessRights
    {
        public const int STANDARD_RIGHTS_ALL = 0x001F0000;
        public const int KEY_QUERY_VALUE = 0x0001;
        public const int KEY_SET_VALUE = 0x0002;
        public const int KEY_CREATE_SUB_KEY = 0x0004;
        public const int KEY_ENUMERATE_SUB_KEYS = 0x0008;
        public const int KEY_NOTIFY = 0x0010;
        public const int KEY_CREATE_LINK = 0x0020;
        public const int SYNCHRONIZE = 0x00100000;
        public const int KEY_WOW64_64KEY = 0x0100;

        public const int KEY_ALL_ACCESS = (STANDARD_RIGHTS_ALL | KEY_QUERY_VALUE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY |
                                           KEY_ENUMERATE_SUB_KEYS
                                           | KEY_NOTIFY | KEY_CREATE_LINK) & ~SYNCHRONIZE;
    }
}

public class RegistryException : Exception
{
    public RegistryException(string message, int code) : base($"[ERROR {code}] {message}")
    {
    }
}