using Lesson3DllRegEdit;

var key = Registry.CreateOrOpenKey(Registry.Handlers.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion",
    out var lpdwDisposition);
var subkeys = Registry.GetSubkeys(key);
Registry.CloseKey(key);
key = Registry.CreateOrOpenKey(Registry.Handlers.HKEY_CURRENT_USER, @"SoftWare\DemoSoftware", out lpdwDisposition);
Registry.SetValue(key, "demo1", "hello");
Registry.SetValue(key, "demo2", "world");
Registry.CloseKey(key);

var demoKey1 = Registry.CreateOrOpenKey(Registry.Handlers.HKEY_LOCAL_MACHINE,
    @"SOFTWARE\Microsoft\Windows\CurrentVersion", out var _1LpdwDisposition);
var subkeys1 = Registry.GetSubkeys(demoKey1);
Registry.CloseKey(demoKey1);
var demoKey2 = Registry.CreateOrOpenKey(Registry.Handlers.HKEY_CURRENT_USER, @"SoftWare\DemoSoftware",
    out var _2LpdwDisposition);
Registry.SetValue(demoKey2, "demo1", "hello");
Registry.SetValue(demoKey2, "demo2", "world");
var values2 = Registry.GetValues(demoKey2);
Registry.CloseKey(key);
Registry.DeleteKey(Registry.Handlers.HKEY_CURRENT_USER, @"SoftWare\DemoSoftware");
// Add a breakpoint here to see variables.