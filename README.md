# XyzController —— XYZ 轴实时控制器（WinForms）

一个用 C# + WinForms 写的纯软件模拟界面，用于实时控制一个虚拟的 XYZ 坐标点。

- **目标框架**：.NET Framework 4.6.1
- **UI 框架**：WinForms（手动绘制，不依赖第三方库）
- **语言版本**：C# 5（兼容系统自带的 csc 编译器）

## 界面一览

```
┌─────────────────────────────────────┬──────┬──────────────────────┐
│                                     │      │  X 轴                │
│         XY 俯视图                   │      │  ◀ -1 ═══●═══ +1 ▶   │
│      （网格 / 轨迹 / 当前点）       │  Z   │                      │
│                                     │  条  │  Y 轴                │
│                                     │      │  ▼ -1 ═══●═══ +1 ▲   │
│                                     │      │                      │
│                                     │      │  Z 轴                │
│                                     │      │  ▽ -1 ═══●═══ +1 △   │
│                                     │      │                      │
│                                     │      │  通用                │
│                                     │      │  速度：中 ──●──      │
│                                     │      │  [回原点][居中]      │
│                                     │      │  [清除轨迹][随机]    │
├─────────────────────────────────────┴──────┴──────────────────────┤
│ 当前 X=12.34 Y=-5.67 Z=0.00  →  目标 X=… Y=… Z=…                 │
└───────────────────────────────────────────────────────────────────┘
```

## 功能

### 多种控制方式（全部互相同步）

| 方式 | 说明 |
|---|---|
| **滑块** | 拖动 X / Y / Z 滑块设定目标 |
| **数字框** | 直接输入精确数值（支持小数） |
| **± 按钮** | 每次步进 1 |
| **键盘** | 方向键 / WASD 控制 X、Y；Q/E 或 PageUp/Down 控制 Z |
| **鼠标** | 在 XY 俯视图上左键拖动，直接设定目标点 |
| **右键** | 在 XY 视图上右键 → 设为原点 (0, 0) |

### 键盘快捷键

| 按键 | 动作 |
|---|---|
| `←` `→` 或 `A` `D` | X 轴 ±1 |
| `↑` `↓` 或 `W` `S` | Y 轴 ±1 |
| `Q` `E` 或 `PgUp` `PgDn` | Z 轴 ±1 |
| `Shift` + 上述键 | 步进改为 ±10 |
| `Space` | 一键回原点 (0, 0, 0) |
| `Esc` | 清除运动轨迹 |

### 可视化

- **XY 俯视图**：自动绘制网格、坐标轴、十字光标、运动轨迹（最近 400 个点）和目标点（橙色空心圆）。
- **Z 条**：竖直进度条样式，带刻度、目标 Z 标记（橙色箭头）、当前 Z 指示条。
- **平滑动画**：当前值会以二次曲线插值逼近目标值，模拟真实电机的加减速过程。速度可在「通用」区调节。

## 文件结构

```
XyzController/
├── Program.cs                  # 入口（[STAThread]、EnableVisualStyles）
├── MainForm.cs                 # 主窗体逻辑（事件绑定、键盘、动画）
├── MainForm.Designer.cs        # 控件布局声明（InitializeComponent）
├── XYView.cs                   # 自定义控件：XY 俯视图
├── ZBarView.cs                 # 自定义控件：Z 轴竖条
├── Properties/
│   └── AssemblyInfo.cs         # 程序集信息
├── XyzController.csproj        # 项目文件（目标框架 v4.6.1）
├── XyzController.exe           # 已编译生成的可执行文件
└── README.md
```

## 如何编译运行

### 方式 1：直接用已编译好的 exe

仓库里已附带编译好的 `XyzController.exe`，双击即可运行（前提是系统装了 .NET Framework 4.6.1+，Windows 10/11 默认都有）。

### 方式 2：用系统自带的 csc 编译

Windows 自带的 .NET Framework 编译器位于：

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
```

在项目目录下打开 **CMD**（注意是 cmd，不是 PowerShell 或 Git Bash），执行：

```cmd
csc /target:winexe /out:XyzController.exe ^
    /r:System.Drawing.dll /r:System.Windows.Forms.dll ^
    Program.cs MainForm.cs MainForm.Designer.cs XYView.cs ZBarView.cs Properties\AssemblyInfo.cs
```

> **注意**：在 Git Bash 中调用 csc 时，`/target:winexe` 这类 `/` 开头的参数会被 Git Bash 当作 Unix 路径，需要写成 `//target:winexe`，或直接通过 `cmd.exe //C '...'` 调用。本项目的源码已确认在系统自带编译器下可成功编译。

### 方式 3：用 Visual Studio 打开

双击 `XyzController.csproj`，按 `F5` 调试运行，或 `Ctrl+F5` 直接运行。

### 方式 4：用 MSBuild 编译

```cmd
msbuild XyzController.csproj /p:Configuration=Release
```

输出在 `bin\Release\XyzController.exe`。

## 设计要点

1. **目标值 vs 当前值**：所有 UI 控件只修改 `TargetX/Y/Z`，由 `animTimer`（50 FPS）按二次曲线插值把 `CurrentX/Y/Z` 平滑过渡过去 —— 模拟真实运动。
2. **双向同步防递归**：滑块改了会同步数字框，数字框改了也会同步滑块。用 `_syncing` 标志位避免循环触发。
3. **双缓冲绘制**：`DoubleBuffered = true`，动画不闪烁。
4. **DPI 自适应**：`AutoScaleMode = Dpi`，在高分屏上不会糊成一团。

## 扩展建议

如果想接真实硬件，只要在 `MainForm` 里改两处即可：

- **发送指令**：在 `SetTarget` / `animTimer_Tick` 末尾，把 `TargetX/Y/Z` 通过串口发送出去（例如 `serialPort.WriteLine("G0 X" + x + " Y" + y + " Z" + z)`）。
- **读取反馈**：订阅串口的 `DataReceived` 事件，把回传的实际坐标更新到 `xyView.CurrentX/Y` 等属性上。

界面、动画、轨迹、状态栏等可以完全复用。
