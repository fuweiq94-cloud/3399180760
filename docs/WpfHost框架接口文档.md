# WpfHost 框架接口文档

## 1. 概述

XyzController.WpfHost 是一个可复用的 WPF 宿主框架，用于在 WPF 窗口中承载 WinForms 窗体，并提供顶部导航栏多页面切换能力。

框架以 DLL + Launcher EXE 的形式输出，业务项目无需接触任何 WPF 源码，只需在自己的 `Program.cs` 中注册页面即可。

### 1.1 输出产物

| 文件 | 说明 |
|------|------|
| `XyzController.WpfHost.dll` | 框架核心 DLL，提供公共 API |
| `XyzController.WpfHost.Launcher.exe` | 通用启动器，通过反射加载业务 DLL 的 `Program.Main` |

### 1.2 架构关系

```
Launcher.exe（Bootstrapper）
    │  反射扫描同目录 *.dll
    │  找到含 static Main(string[]) 的类型
    ↓
业务 DLL（如 XyzController.dll）
    │  Program.Main(args)
    │  构建 List<WpfPage>，调用 WpfHostLauncher.Run(pages)
    ↓
WpfHost.dll（框架）
    │  创建 WPF Application + MainWindow
    │  根据 pages 列表动态生成导航栏 + WindowsFormsHost
    ↓
WPF 窗口（顶部导航栏 + 内容区）
```

## 2. 公共 API

### 2.1 WpfPage — 页面模型

```csharp
namespace XyzController.WpfHost
{
    public class WpfPage
    {
        public string Title { get; }   // 导航栏显示的标题
        public Form Content { get; }   // WinForms 窗体实例

        public WpfPage(string title, Form content);
    }
}
```

### 2.2 WpfHostLauncher — 启动入口

```csharp
namespace XyzController.WpfHost
{
    public static class WpfHostLauncher
    {
        // 单页面模式（无导航栏，直接嵌入一个 Form）
        public static int Run(Form form);

        // 单页面泛型便捷重载
        public static int Run<TForm>() where TForm : Form, new();

        // 多页面导航模式（顶部导航栏 + 页面切换）
        public static int Run(IList<WpfPage> pages);
    }
}
```

调用要求：

- 必须在 `[STAThread]` 线程中调用（通常为 `Program.Main`）。
- `Run` 为阻塞式调用，窗口关闭后返回退出码。
- 多页面模式要求 `pages` 非空且 `Count > 0`。

## 3. 业务方接入指南

### 3.1 新项目接入步骤

1. 将以下文件复制到业务项目的输出目录：
   - `XyzController.WpfHost.dll`
   - `XyzController.WpfHost.Launcher.exe`

2. 业务项目添加对 `XyzController.WpfHost.dll` 的引用。

3. 编写 `Program.cs`：

```csharp
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XyzController.WpfHost;

namespace MyProject
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            List<WpfPage> pages = new List<WpfPage>();
            pages.Add(new WpfPage("页面一", new Form1()));
            pages.Add(new WpfPage("页面二", new Form2()));
            pages.Add(new WpfPage("页面三", new Form3()));
            WpfHostLauncher.Run(pages);
        }
    }
}
```

4. 生成项目，确保业务 DLL 与 Launcher.exe 在同一目录。

5. 运行 `XyzController.WpfHost.Launcher.exe`（不带参数即为多页导航模式）。

### 3.2 关键约定

- 业务 `Program.Main` 必须为 `static`，签名为 `Main(string[])`，可以是 `private`。
- 业务项目输出类型可以是 Library（DLL），由 Launcher 反射调用。
- 导航栏按钮数量 = `pages` 列表长度，无上限，完全由业务方决定。
- 每个 `WpfPage` 的 `Form` 会被自动设置为 `TopLevel=false`、无边框、`Dock=Fill`，业务方无需手动处理。

### 3.3 命令行参数

Launcher 会将命令行参数原样透传给业务 `Program.Main(string[] args)`，业务方可自行解析：

```csharp
string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "nav";
switch (mode)
{
    case "main":   WpfHostLauncher.Run(new MainForm()); break;  // 单页面
    case "nav":
    default:       RunMultiPage(); break;                        // 多页面导航
}
```

## 4. 框架内部机制（仅供了解，无需修改）

### 4.1 Bootstrapper 反射加载

Launcher.exe 启动后：
1. 扫描 EXE 同目录下所有 `*.dll`（跳过 System.*、Microsoft.* 等框架 DLL）。
2. 对每个 DLL 调用 `Assembly.LoadFrom`，查找含 `static Main(string[])` 的类型。
3. 找到第一个匹配的后反射调用，透传命令行参数。
4. 若未找到，弹出提示框。

### 4.2 导航栏动态生成

MainWindow 的 XAML 中导航栏只是一个空的 `StackPanel`，按钮在 `Window_Loaded` 时由 `BuildNavigation()` 根据 `_pages` 列表循环生成。切换页面通过 `WindowsFormsHost.Visibility` 控制显隐。

### 4.3 Airspace 限制

WPF 与 WinForms 存在 Airspace 限制（不能在同一区域重叠渲染）。框架将内容区设计为纯 `WindowsFormsHost` 承载，不在其上方放置 WPF 元素。业务方的 Form 内部可自由使用任何 WinForms 控件。

### 4.4 程序集解析

Launcher.exe 的 AssemblyName 与 WpfHost.dll 不同（文件名不同），避免 CLR 将 EXE 误认为同名 DLL。同时注册了 `AssemblyResolve` 事件，确保业务 DLL 的间接依赖（如 Controls.dll）能从同目录正确加载。

## 5. 构建说明

WpfHost 项目使用后置构建目标 `BuildExe`，在编译 DLL 后自动将 `Bootstrapper.cs` 编译为 `XyzController.WpfHost.Launcher.exe`（`TargetType=winexe`）。

业务项目（XyzController）的 Debug 输出路径配置为 `..\XyzController.WpfHost\bin\Debug\`，确保生成后业务 DLL 与 Launcher 自动位于同一目录。

修改业务代码后必须重新生成解决方案，否则 Launcher 加载的仍是旧版 DLL。

## 6. 部署清单

将以下文件放在同一目录即可运行：

```
MyApp/
├── XyzController.WpfHost.Launcher.exe   ← 启动入口（双击运行）
├── XyzController.WpfHost.dll            ← 框架
├── MyProject.dll                        ← 业务 DLL（含 Program.Main）
└── MyProject.Controls.dll               ← 业务依赖（如有）
```
