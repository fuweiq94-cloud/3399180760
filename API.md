# XyzController · API 接口文档

> 目标框架：.NET Framework 4.6.1
> 工控机兼容：所有源码不使用 Lambda `=>` 表达式
> 设计器兼容：所有 `*.Designer.cs` 仅使用字面常量（VS2017+ 可直接打开）

---

## 📚 目录

- [项目结构](#项目结构)
- [一、自定义控件层（XyzController.Controls）](#一自定义控件层xyzcontrollercontrols)
  - [1.1 XYView — XY 平面俯视图](#11-xyview--xy-平面俯视图)
  - [1.2 ZBarView — Z 轴竖直条](#12-zbarview--z-轴竖直条)
  - [1.3 JogButton — 点动按钮](#13-jogbutton--点动按钮)
  - [1.4 JoystickPad — 8 方向摇杆](#14-joystickpad--8-方向摇杆)
  - [1.5 DroLabel — 数字读数 DRO](#15-drolabel--数字读数-dro)
  - [1.6 AxisBar — 双向轴位置条](#16-axisbar--双向轴位置条)
  - [1.7 MathHelper — 数学工具](#17-mathhelper--数学工具)
  - [1.8 PaintHelper — 绘图工具（internal）](#18-painthelper--绘图工具internal)
- [二、业务逻辑层（XyzController.Logic）](#二业务逻辑层xyzcontrollerlogic)
  - [2.1 AxisController — 单轴控制器](#21-axiscontroller--单轴控制器)
  - [2.2 XyzControllerHub — 三轴统一控制器](#22-xyzcontrollerhub--三轴统一控制器)
  - [2.3 AxisJogService — JOG 手动进给服务](#23-axisjogservice--jog-手动进给服务)
  - [2.4 JogMode — JOG 模式枚举](#24-jogmode--jog-模式枚举)
- [三、WPF 宿主层（XyzController.WpfHost）](#三wpf-宿主层xyzcontrollerwpfhost)
  - [3.1 WpfHostLauncher — 启动入口](#31-wpfhostlauncher--启动入口)
  - [3.2 WpfPage — 页面模型](#32-wpfpage--页面模型)
- [四、典型用法示例](#四典型用法示例)
- [五、后期接入真 EtherCAT SDK 的步骤](#五后期接入真-ethercat-sdk-的步骤)
- [六、工具箱与设计器使用（VS2017+）](#六工具箱与设计器使用vs2017)

---

## 项目结构

```
XyzController.sln
├── src/
│   ├── XyzController/                  ← 主程序（业务项目）
│   │   ├── Program.cs                  ← 程序入口
│   │   ├── MainForm.cs                 ← 主控制器界面
│   │   ├── PointJumpForm.cs            ← 点位跳转
│   │   ├── TrajectoryViewForm.cs       ← 运动轨迹
│   │   ├── HardwareForm.cs             ← 硬件调试（XYZU 四轴）
│   │   └── Logic/                      ← 业务逻辑层
│   │       ├── AxisController.cs
│   │       ├── XyzControllerHub.cs
│   │       ├── AxisJogService.cs
│   │       └── JogMode.cs
│   │
│   ├── XyzController.Controls/         ← 自定义控件库（DLL）
│   │   ├── XYView.cs                   ← XY 平面视图
│   │   ├── ZBarView.cs                 ← Z 轴竖直条
│   │   ├── JogButton.cs                ← 点动按钮
│   │   ├── JoystickPad.cs              ← 8 方向摇杆
│   │   ├── DroLabel.cs                 ← 数字读数 DRO
│   │   ├── AxisBar.cs                  ← 双向位置条
│   │   ├── MathHelper.cs               ← 数学工具（public static）
│   │   └── PaintHelper.cs              ← 绘图工具（internal static）
│   │
│   ├── XyzController.WpfHost/          ← WPF 宿主框架（DLL + 启动 EXE）
│   │   ├── WpfHostLauncher.cs          ← 对外入口
│   │   ├── WpfPage.cs                  ← 页面模型
│   │   ├── MainWindow.xaml(.cs)        ← WPF 主窗口
│   │   └── Bootstrapper.cs             ← 通用启动器（编译成 Launcher.exe）
│   │
│   └── XyzController.Tests/            ← 单元测试项目
```

---

## 一、自定义控件层（XyzController.Controls）

命名空间：`XyzController.Controls`

### 1.1 XYView — XY 平面俯视图

XY 平面俯视图控件。显示坐标网格、原点、十字光标、目标点、运动轨迹，支持鼠标点击拾取。

```csharp
public class XYView : Control
```

#### 属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `RangeMin` | `float` | -100 | 机械坐标系最小值 |
| `RangeMax` | `float` | 100 | 机械坐标系最大值 |
| `CurrentX` | `float` | 0 | **只读**，当前显示 X 坐标（动画逼近 TargetX） |
| `CurrentY` | `float` | 0 | **只读**，当前显示 Y 坐标（动画逼近 TargetY） |
| `TargetX` | `float` | 0 | 目标 X 坐标 |
| `TargetY` | `float` | 0 | 目标 Y 坐标 |

#### 方法

| 方法 | 返回 | 说明 |
|---|---|---|
| `Advance(float speedFraction)` | `void` | 每帧由 Timer 调用，让 Current 向 Target 平滑过渡（lerp 经 ClampLerp 钳制到 [0.02,1]），并追加轨迹点 |
| `ClearTrail()` | `void` | 清空运动轨迹并重绘 |

#### 事件

| 事件 | 委托 | 触发时机 |
|---|---|---|
| `TargetSetByMouse` | `EventHandler<PointF>` | 鼠标左键点击/拖拽设定新目标，或右键设为原点时触发，参数为机械坐标 `PointF` |

#### 用法示例

```csharp
// 在窗体里：
xyView.RangeMin = -200f;
xyView.RangeMax = 200f;
xyView.TargetSetByMouse += new EventHandler<PointF>(XyView_TargetSetByMouse);

// Timer 每 30ms 调用一次：
xyView.TargetX = hub.X.Target;
xyView.TargetY = hub.Y.Target;
xyView.Advance(0.3f);   // 让 CurrentX/Y 平滑追过去

// 事件处理：
private void XyView_TargetSetByMouse(object sender, PointF e)
{
    hub.X.SetTarget(e.X);
    hub.Y.SetTarget(e.Y);
}
```

---

### 1.2 ZBarView — Z 轴竖直条

竖直进度条样式的 Z 轴指示控件。显示刻度、当前值、目标值标记。

```csharp
public class ZBarView : Control
```

#### 属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `RangeMin` | `float` | -50 | Z 轴最小值 |
| `RangeMax` | `float` | 100 | Z 轴最大值 |
| `CurrentZ` | `float` | 0 | **只读**，当前 Z 值（由 Advance 平滑逼近 TargetZ） |
| `TargetZ` | `float` | 0 | 目标 Z 值 |

#### 方法

| 方法 | 返回 | 说明 |
|---|---|---|
| `Advance(float lerp)` | `void` | 每帧调用，CurrentZ 向 TargetZ 平滑过渡（lerp 经 ClampLerp 钳制） |

#### 用法示例

```csharp
zBar.RangeMin = -50f;
zBar.RangeMax = 100f;

// Timer Tick 里：
zBar.TargetZ = hub.Z.Target;
zBar.Advance(0.3f);
```

> **复用提示**：U 轴或其他旋转/直线轴也可用 ZBarView，只是把 `TargetZ` 当成"任意单轴目标值"使用。HardwareForm 里的 `uBar` 就是这么用的。

---

### 1.3 JogButton — 点动按钮

点动按钮（机床 JOG 模式）。按住不放每隔一个间隔重复触发 `Jog` 事件，松开立即触发 `Stop` 事件。

```csharp
public class JogButton : Control
```

#### 属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Direction` | `int` | +1 | 持续触发时的方向标识（+1 / -1 或任意整数） |
| `InitialDelay` | `int` | 400 | 首次按下后多久开始连续触发（毫秒） |
| `RepeatInterval` | `int` | 80 | 连续触发的间隔（毫秒），越小移动越快 |

#### 事件

| 事件 | 委托 | 触发时机 |
|---|---|---|
| `Jog` | `EventHandler<int>` | 每次点动触发（首次按下立即触发一次，之后按 RepeatInterval 重复）。参数为 `Direction` |
| `Stop` | `EventHandler<int>` | 松开按钮时触发。参数为 `Direction` |

#### 用法示例

```csharp
// "+" 按钮持续 +1
jogXPlus.Direction = +1;
jogXPlus.Jog += new EventHandler<int>(JogButton_Jog);
jogXPlus.Stop += new EventHandler<int>(JogButton_Stop);

private void JogButton_Jog(object sender, int direction)
{
    _jogService.OnJogStart(direction);
}

private void JogButton_Stop(object sender, int direction)
{
    _jogService.OnJogStop();
}
```

---

### 1.4 JoystickPad — 8 方向摇杆

8 方向虚拟摇杆。按住中心圆拖动（或鼠标按住任意位置）输出方向矢量 `(dx, dy)`，每个分量 ∈ {-1, 0, +1}；松开自动回中；可同时按 2 个方向（如右上 = (+1, +1)）。

```csharp
public class JoystickPad : Control
```

#### 属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DeadZone` | `float` | 0.25 | 方向死区（归一化位移），拖动距离超过此值才算有方向 |

#### 事件

| 事件 | 委托 | 触发时机 |
|---|---|---|
| `DirectionChanged` | `EventHandler<System.Drawing.Point>` | 方向或强度变化时触发。参数为 `Point(dx, dy)`，dx/dy ∈ {-1, 0, +1}；**Y 轴已翻转**，向上输出 +1 |

#### 用法示例

```csharp
joystick.DirectionChanged += new EventHandler<Point>(Joystick_DirectionChanged);

private void Joystick_DirectionChanged(object sender, Point e)
{
    // e.X = 水平方向（-1=左, 0=中, +1=右）
    // e.Y = 垂直方向（-1=下, 0=中, +1=上）
    if (e.X != 0) hub.X.Step(e.X);
    if (e.Y != 0) hub.Y.Step(e.Y);
}
```

---

### 1.5 DroLabel — 数字读数 DRO

DRO（Digital Read Out）数字读数控件。机床/CNC 大字号坐标显示风格：轴名 + 高对比等宽大数字 + 数值变化时短暂高亮 + 可设报警阈值超限变红。

```csharp
public class DroLabel : Control
```

#### 属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `AxisName` | `string` | "X" | 显示的轴名（如 X / Y / Z / U） |
| `Value` | `float` | 0 | 当前数值（变化 >0.0001f 时刷新高亮并重绘） |
| `Decimals` | `int` | 3 | 小数位数 |
| `Unit` | `string` | "mm" | 单位字符串（如 mm / inch / °） |
| `AlarmHigh` | `float?` | null | 报警上限（可空），超过则数字变红；null 不检查 |
| `AlarmLow` | `float?` | null | 报警下限（可空），低于则数字变红；null 不检查 |
| `FlashDuration` | `int` | 200 | 数值变化后高亮持续多久（毫秒） |

#### 方法

| 方法 | 返回 | 说明 |
|---|---|---|
| `SetValue(float v)` | `void` | 设置数值（走 Value 属性触发高亮），并启动内部 flash 定时器 |

#### 用法示例

```csharp
droX.AxisName = "X";
droX.Unit = "mm";
droX.Decimals = 3;
droX.AlarmHigh = 90f;    // 超过 90mm 变红
droX.AlarmLow = -90f;

// Timer Tick 里：
droX.SetValue(hub.X.Current);
```

---

### 1.6 AxisBar — 双向轴位置条

双向轴位置条（带限位报警）。比 ZBarView 更工业化：水平/竖直方向可选；正值蓝色填充，负值橙色填充（区分方向）；可设置软限位超限变红报警；当前值数字直接画在指针旁。

```csharp
public class AxisBar : Control
{
    public enum Orientations { Horizontal, Vertical }
}
```

#### 属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientations` | Vertical | 条的方向（水平或竖直） |
| `RangeMin` | `float` | -100 | 数值范围最小值 |
| `RangeMax` | `float` | 100 | 数值范围最大值 |
| `CurrentValue` | `float` | 0 | 当前值 |
| `TargetValue` | `float` | 0 | 目标值（仅作为属性存在） |
| `SoftLimitPositive` | `float?` | null | 软限位（正方向），超过变红；null = 不检查 |
| `SoftLimitNegative` | `float?` | null | 软限位（负方向），低于变红；null = 不检查 |
| `AxisLabel` | `string` | "X" | 轴标签文字 |

#### 方法

| 方法 | 返回 | 说明 |
|---|---|---|
| `SetValue(float v)` | `void` | 设置 CurrentValue 并重绘 |

#### 用法示例

```csharp
axisBarX.Orientation = AxisBar.Orientations.Horizontal;
axisBarX.RangeMin = -100f;
axisBarX.RangeMax = 100f;
axisBarX.SoftLimitPositive = 90f;
axisBarX.SoftLimitNegative = -90f;
axisBarX.AxisLabel = "X 轴";

axisBarX.SetValue(hub.X.Current);
```

---

### 1.7 MathHelper — 数学工具

提取重复的数学运算。位于 `XyzController.Controls` 命名空间，**public static**，供 Logic 层和控件层共用。

```csharp
public static class MathHelper
```

#### 方法

| 方法 | 返回 | 说明 |
|---|---|---|
| `Clamp(float value, float min, float max)` | `float` | 限制到 [min, max]，不抛异常 |
| `Clamp01(float value)` | `float` | 限制到 [0, 1]，调用 Clamp(value, 0f, 1f) |
| `ClampLerp(float value)` | `float` | 限制到 [0.02, 1]，动画插值系数有效范围 |
| `Clamp(int value, int min, int max)` | `int` | 整数版本 |
| `Clamp(decimal value, decimal min, decimal max)` | `decimal` | decimal 版本 |

#### 用法示例

```csharp
float v = MathHelper.Clamp(input, -100f, 100f);
float k = MathHelper.Clamp01(lerpInput);
float animK = MathHelper.ClampLerp(speed / 100f);

int n = MathHelper.Clamp(trb.Value, 0, 100);
decimal d = MathHelper.Clamp(nud.Value, nud.Minimum, nud.Maximum);
```

---

### 1.8 PaintHelper — 绘图工具（internal）

绘图工具类，提取各自定义控件中重复的绘图逻辑。**internal static**（仅 XyzController.Controls 程序集内可见，Logic 层跨程序集无法访问）。

```csharp
internal static class PaintHelper
```

#### 方法

| 方法 | 返回 | 说明 |
|---|---|---|
| `SetupGraphics(Graphics g)` | `void` | 设置高质量绘图：Antialias + ClearType |
| `FillBackground(Graphics g, Control control)` | `void` | 用 `control.BackColor` 填充 ClientRectangle |
| `ChooseStep(float span, int targetSegments)` | `int` | 从 `{1,2,5,10,20,50,100,200,500,1000}` 选最接近 `span/targetSegments` 的值 |
| `ValueToY(RectangleF bar, float rangeMin, float rangeMax, float value)` | `float` | 数值映射到竖直条 Y 像素（值大靠上） |
| `VerticalBarArea(RectangleF clientRect, float barWidth, float topMargin, float bottomMargin)` | `RectangleF` | 计算居中竖直条区域 |
| `DrawBarBorder(Graphics g, RectangleF bar, Color color)` | `void` | 绘制条形边框 |
| `DrawCenteredText(Graphics g, string text, Font, Brush, float x, float y)` | `void` | 水平居中文本 |
| `DrawCenteredTextInRect(Graphics g, string text, Font, Brush, RectangleF rect)` | `void` | 矩形内水平+垂直都居中文本 |
| `DrawVerticalTicks(Graphics g, RectangleF bar, float rangeMin, float rangeMax, Func<float,float> valueToY)` | `void` | 绘制竖直条右侧刻度线和数值标签 |

> 内部使用，外部项目不应依赖。

---

## 二、业务逻辑层（XyzController.Logic）

命名空间：`XyzController.Logic`。**不依赖任何 UI 类型**（不引用 System.Windows.Forms / System.Drawing），可单独单元测试、可被控制台/Web/串口复用。

### 2.1 AxisController — 单轴控制器

单个轴的业务控制器。管理当前值、目标值、范围限位、动画推进。

```csharp
public class AxisController
```

#### 属性

| 属性 | 类型 | 可写 | 说明 |
|---|---|---|---|
| `Min` | `float` | private set | 范围下限，构造后只读 |
| `Max` | `float` | private set | 范围上限，构造后只读 |
| `Name` | `string` | private set | 轴名称，构造后只读 |
| `Current` | `float` | private set | 当前动画值（外部只读，仅由 Advance 修改） |
| `Target` | `float` | private set | 目标值（外部只读，仅由 SetTarget 系列修改） |
| `AtTarget` | `bool` | 只读计算 | `|Current - Target| < 0.001f` 时为 true |

#### 构造函数

```csharp
public AxisController(string name, float min, float max, float initial = 0f)
```

- `max <= min` 时抛 `ArgumentException("max 必须大于 min")`
- `initial` 经 Clamp 到 [Min, Max] 后同时赋给 Current 和 Target

#### 方法

| 方法 | 说明 |
|---|---|
| `SetTarget(float value)` | 设置目标值并自动 Clamp 到 [Min, Max]；若变化幅度 < 0.0001f 则不触发事件 |
| `Step(int delta)` | 在当前 Target 上加减一个整数步长 |
| `SetToCenter()` | 目标设为 (Min+Max)*0.5 |
| `SetToMin()` | 目标设为 Min |
| `SetToMax()` | 目标设为 Max |
| `ResetToOrigin()` | 若 0 在 [Min,Max] 内则回到 0，否则回退到 SetToCenter |
| `Advance(float lerpFraction)` | 推进一帧动画。lerp 经 Clamp01；**k <= 0 时完全不动**；\|Target-Current\| < 0.0005f 时吸附到 Target |

#### 事件

| 事件 | 委托 | 触发时机 |
|---|---|---|
| `Changed` | `EventHandler` | Current 或 Target 变化时 |

#### 用法示例

```csharp
var axis = new AxisController("X", -100f, 100f);
axis.Changed += delegate { Console.WriteLine("X = " + axis.Current); };

axis.SetTarget(50f);
Console.WriteLine(axis.Target);   // 50
Console.WriteLine(axis.Current);  // 0（未推进）

axis.Advance(0.3f);
Console.WriteLine(axis.Current);  // 15（0 + (50-0)*0.3）

while (!axis.AtTarget) axis.Advance(0.3f);
```

---

### 2.2 XyzControllerHub — 三轴统一控制器

XYZ 三轴的统一业务控制器。组合 3 个 AxisController，封装"速度档位 → 插值系数"的换算。

```csharp
public class XyzControllerHub
```

#### 属性

| 属性 | 类型 | 可写 | 说明 |
|---|---|---|---|
| `X` | `AxisController` | private set | X 轴，构造后只读 |
| `Y` | `AxisController` | private set | Y 轴，构造后只读 |
| `Z` | `AxisController` | private set | Z 轴，构造后只读 |
| `SpeedSetting` | `int` | public set | 速度档位 [0,100]，默认 20。0=最慢，100=瞬时到位 |
| `CurrentLerpFraction` | `float` | 只读计算 | 将 SpeedSetting 换算为 [0.02, 1.0] 的插值系数，公式 `0.02 + (s/100)² × 0.98`（二次曲线） |

#### 构造函数

```csharp
public XyzControllerHub(float xMin, float xMax,
                        float yMin, float yMax,
                        float zMin, float zMax)
```

为每个轴 new 一个 `AxisController`，订阅三轴 `Changed` 转发；`SpeedSetting` 初始化为 20。

#### 方法

| 方法 | 说明 |
|---|---|
| `SetTarget(float x, float y, float z)` | 同时设置三轴目标 |
| `ResetToOrigin()` | 三轴都回原点 |
| `SetToCenter()` | 三轴都移到各自中点 |
| `SetRandomTarget(Random rng)` | 三轴各取一个随机整数目标（范围 `[Min, Max+1)`） |
| `Advance()` | 用 `CurrentLerpFraction` 推进三轴动画一帧 |

#### 事件

| 事件 | 委托 | 触发时机 |
|---|---|---|
| `Changed` | `EventHandler` | 任一子轴的 Current/Target 变化时（通过订阅子轴 Changed 转发） |

#### 用法示例

```csharp
var hub = new XyzControllerHub(-100, 100, -100, 100, -50, 50);
hub.SpeedSetting = 50;
hub.Changed += delegate { RefreshUi(); };

hub.SetTarget(30f, 40f, 10f);
Console.WriteLine(hub.CurrentLerpFraction);  // 0.02 + 0.5² × 0.98 = 0.265

// Timer Tick：
hub.Advance();
```

---

### 2.3 AxisJogService — JOG 手动进给服务

把按钮按下/松开翻译为 AxisController 的目标设置，支持**寸动**与**连续**两种模式。

```csharp
public class AxisJogService
```

#### 属性

| 属性 | 类型 | 可写 | 说明 |
|---|---|---|---|
| `Axis` | `AxisController` | 只读 | 被控制的轴 |
| `Mode` | `JogMode` | 只读 | 当前模式，默认 `Incremental`（通过 SetMode 修改） |
| `StepDistance` | `float` | 只读 | 寸动步长，默认 1.0f（通过 SetStepDistance 修改） |
| `IsJogging` | `bool` | 只读 | 当前是否正在 JOG 中（仅连续模式持续为 true） |

#### 构造函数

```csharp
public AxisJogService(AxisController axis)
```

- `axis == null` 时抛 `ArgumentNullException("axis")`

#### 方法

| 方法 | 说明 |
|---|---|
| `SetMode(JogMode mode)` | 切换模式；相同则直接返回；切换时若 IsJogging=true 会先 OnJogStop |
| `SetStepDistance(float step)` | 设置寸动步长；`step <= 0` 抛 `ArgumentOutOfRangeException("step", "步长必须大于 0")` |
| `OnJogStart(int direction)` | 用户按下 JOG。direction 必须是 +1 或 -1，否则抛 `ArgumentOutOfRangeException`。**寸动**：Target += direction × StepDistance；**连续**：Target 设为该方向限位（Max 或 Min） |
| `OnJogStop()` | 用户松开 JOG。若 !IsJogging 直接返回；连续模式下把 Target 冻结到 Current 使动画立即停止；寸动模式无需调用 |
| `EmergencyStop()` | 紧急停止：清零 IsJogging/direction，把 Target 固定到 Current |

#### 行为对比

| 操作 | 寸动模式（Incremental） | 连续模式（Continuous） |
|---|---|---|
| 按下按钮 | 目标 += 步长 × 方向（每次一格） | 目标 = 方向的限位（Max 或 Min） |
| 松开按钮 | 无动作 | 目标冻结到当前位置（立即停止） |
| 持续按住 | Jog 事件重复触发，每次走一步 | Jog 只触发一次，持续运动 |
| 急停 | Target = Current（冻结） | Target = Current（冻结） |

#### 用法示例

```csharp
var jogX = new AxisJogService(hub.X);
jogX.SetMode(JogMode.Continuous);
jogX.SetStepDistance(0.5f);

// JogButton "+" 按钮：
jogXPlus.Direction = +1;
jogXPlus.Jog += delegate { jogX.OnJogStart(+1); };
jogXPlus.Stop += delegate { jogX.OnJogStop(); };

// JogButton "-" 按钮：
jogXMinus.Direction = -1;
jogXMinus.Jog += delegate { jogX.OnJogStart(-1); };
jogXMinus.Stop += delegate { jogX.OnJogStop(); };
```

---

### 2.4 JogMode — JOG 模式枚举

```csharp
public enum JogMode
{
    Incremental = 0,   // 寸动：每按一次按钮移动一个固定步长，到位置自动停
    Continuous = 1     // 连续：按住按钮持续运动，松开立即停止
}
```

---

## 三、WPF 宿主层（XyzController.WpfHost）

命名空间：`XyzController.WpfHost`。把 WinForms 窗体嵌入 WPF 窗口，提供顶部多页面导航栏。

### 3.1 WpfHostLauncher — 启动入口

对外启动入口。将任意 WinForms Form 嵌入 WPF 窗口运行，调用方无需接触宿主内部实现。

```csharp
public static class WpfHostLauncher
```

#### 方法（3 个 Run 重载）

| 方法 | 说明 |
|---|---|
| `Run(Form form) : int` | **单页面模式**（无顶部导航栏），传入一个已构造的 Form。`form == null` 抛 `ArgumentNullException("form")` |
| `Run<TForm>() : int where TForm : Form, new()` | 泛型便捷重载，等价于 `Run(new TForm())`。要求 TForm 有无参构造 |
| `Run(IList<WpfPage> pages) : int` | **多页面导航模式**（有顶部导航栏）。`pages == null` 抛 `ArgumentNullException`；`pages.Count == 0` 抛 `ArgumentException("页面列表不能为空。")` |

#### 调用要求（所有重载通用）

- ✅ 必须在 `[STAThread]` 线程中调用（通常为 `Program.Main`）
- ✅ 当前线程非 STA 时抛 `InvalidOperationException`
- ✅ 内部自动：DPI 感知、`EnableVisualStyles`、`SetCompatibleTextRenderingDefault(false)`、按需创建 WPF `Application`
- ✅ 阻塞至窗口关闭，返回 WPF 应用程序退出码

#### 用法示例

```csharp
// 1) 单页面（最简单）：
WpfHostLauncher.Run(new MainForm());

// 2) 泛型便捷：
WpfHostLauncher.Run<MainForm>();

// 3) 多页面导航（推荐）：
List<WpfPage> pages = new List<WpfPage>();
pages.Add(new WpfPage("主控制器", new MainForm()));
pages.Add(new WpfPage("点位跳转", new PointJumpForm()));
pages.Add(new WpfPage("硬件调试", new HardwareForm()));
WpfHostLauncher.Run(pages);
```

---

### 3.2 WpfPage — 页面模型

封装一个 WinForms Form + 选项卡标题，用于多页面导航模式。

```csharp
public class WpfPage
```

#### 属性

| 属性 | 类型 | 可写 | 说明 |
|---|---|---|---|
| `Title` | `string` | private set | 导航栏上显示的选项卡标题，构造后不可变 |
| `Content` | `Form` | private set | 要嵌入的 WinForms 窗体实例，构造后不可变 |

#### 构造函数

```csharp
public WpfPage(string title, Form content)
```

- `title`：选项卡标题（如"主控制器"）
- `content`：WinForms 窗体实例（如 `new MainForm()`）

#### 用法示例

```csharp
WpfPage page = new WpfPage("硬件调试", new HardwareForm());
Console.WriteLine(page.Title);    // "硬件调试"
Console.WriteLine(page.Content);  // HardwareForm 实例
```

---

## 四、典型用法示例

### 4.1 最小可运行窗体（单轴 + XYView）

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;
using XyzController.Controls;
using XyzController.Logic;

public class MinimalForm : Form
{
    private readonly AxisController _axisX;
    private readonly AxisController _axisY;
    private readonly XYView _view;
    private readonly Timer _timer;

    public MinimalForm()
    {
        _axisX = new AxisController("X", -100f, 100f);
        _axisY = new AxisController("Y", -100f, 100f);

        _view = new XYView();
        _view.Dock = DockStyle.Fill;
        _view.RangeMin = -100f;
        _view.RangeMax = 100f;
        _view.TargetSetByMouse += new EventHandler<PointF>(View_TargetSetByMouse);
        Controls.Add(_view);

        _timer = new Timer();
        _timer.Interval = 30;
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Start();
    }

    private void View_TargetSetByMouse(object sender, PointF e)
    {
        _axisX.SetTarget(e.X);
        _axisY.SetTarget(e.Y);
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _axisX.Advance(0.3f);
        _axisY.Advance(0.3f);

        _view.TargetX = _axisX.Target;
        _view.TargetY = _axisY.Target;
        _view.Advance(0.3f);
    }
}
```

### 4.2 XYZ 三轴 + JOG 服务完整示例

```csharp
var hub = new XyzControllerHub(-100, 100, -100, 100, -50, 100);
hub.SpeedSetting = 50;

// 每轴一个 JOG 服务
var jogX = new AxisJogService(hub.X);
jogX.SetMode(JogMode.Continuous);
jogX.SetStepDistance(1.0f);

// 绑定 JogButton 控件
jogXPlus.Direction = +1;
jogXMinus.Direction = -1;
jogXPlus.Jog  += delegate { jogX.OnJogStart(+1); };
jogXPlus.Stop += delegate { jogX.OnJogStop(); };
jogXMinus.Jog += delegate { jogX.OnJogStart(-1); };
jogXMinus.Stop += delegate { jogX.OnJogStop(); };

// 急停按钮
btnEStop.Click += delegate { jogX.EmergencyStop(); };

// Timer 推进：
hub.Advance();
xyView.TargetX = hub.X.Target;
xyView.Advance(hub.CurrentLerpFraction);
```

### 4.3 在 WpfHost 多页面注册窗体

```csharp
// Program.cs
[STAThread]
private static void Main(string[] args)
{
    List<WpfPage> pages = new List<WpfPage>();
    pages.Add(new WpfPage("主控制器", new MainForm()));
    pages.Add(new WpfPage("硬件调试", new HardwareForm()));
    WpfHostLauncher.Run(pages);
}
```

### 4.4 在 Designer 文件里设置控件属性（VS 设计器兼容写法）

```csharp
// HardwareForm.Designer.cs（节选）
this.xyView.RangeMin = -100F;        // ✅ 字面常量
this.xyView.RangeMax = 100F;         // ✅ 字面常量
this.xyView.TargetX = 0F;            // ✅ 字面常量
this.zBar.RangeMin = -50F;           // ✅ 字面常量

// ❌ 禁止写法（设计器会红屏）：
// this.xyView.RangeMax = 100 - 50;
// this.splitMain.SplitterDistance = this.Width - 80 - 4;
```

---

## 五、后期接入真 EtherCAT SDK 的步骤

当前 HardwareForm 用本地字段模拟 4 轴（开发/演示用）。接入真硬件时按以下步骤：

### 步骤 1：定义硬件抽象接口

新建 `src/XyzController/Logic/Hardware/IAxisHardware.cs`：

```csharp
public interface IAxisHardware
{
    string Name { get; }
    float MinLimit { get; }
    float MaxLimit { get; }
    float CurrentPosition { get; }    // 读编码器（mm 或 °）
    float TargetPosition { get; }     // 最近一次 MoveTo 设置的目标
    bool IsMoving { get; }            // 是否在运动中
    bool IsConnected { get; }         // 硬件是否在线
    void MoveTo(float position);      // 下发运动指令（非阻塞）
    void EmergencyStop();             // 立即急停
    void Home();                      // 回机械原点
    event EventHandler StateChanged;  // 状态变化通知（UI 订阅）
}
```

### 步骤 2：写假实现（开发调试用）

新建 `MockAxisHardware.cs`，用 Timer 模拟硬件周期推进，每 20ms 让 CurrentPosition 逼近 TargetPosition。

### 步骤 3：拿到 C# SDK 后写真实现

新建 `EtherCatAxisHardware.cs`：

```csharp
public class EtherCatAxisHardware : IAxisHardware
{
    private readonly int _slaveId;
    private readonly int _channel;
    private readonly IEtherCatSdk _sdk;  // 你供应商提供的 SDK 接口

    public EtherCatAxisHardware(string name, int slaveId, int channel, IEtherCatSdk sdk) {...}

    public float CurrentPosition
    {
        get { return _sdk.ReadEncoder(_slaveId, _channel); }
    }

    public void MoveTo(float position)
    {
        _sdk.MoveAbsolute(_slaveId, _channel, position, velocity: 50f);
    }

    public void EmergencyStop()
    {
        _sdk.EmergencyStop(_slaveId);
    }
    // ... 其他成员类似
}
```

### 步骤 4：HardwareForm 改动极小

```csharp
// 改前（模拟）：
private float _curX, _tgtX;

// 改后（真硬件）：
private readonly IAxisHardware _axisX;

// 构造函数：
//   _axisX = new EtherCatAxisHardware("X", slaveId: 1001, channel: 0, sdk);
//   _axisX.StateChanged += delegate { RefreshSingleAxis(); };

// RefreshTimer_Tick：
//   xyView.TargetX = _axisX.TargetPosition;
//   xyView.Advance(0.3f);
//   // 不再需要 _curX += (_tgtX - _curX) * lerp
//   // 因为真硬件自己规划速度曲线
```

### 步骤 5：用工厂切换开发/生产环境

```csharp
public static class HardwareFactory
{
    public static IAxisHardware CreateX()
    {
#if DEBUG
        return new MockAxisHardware("X", -100, 100);
#else
        return new EtherCatAxisHardware("X", slaveId: 1001, channel: 0, RealSdk.Instance);
#endif
    }
}
```

### 关键收益

| 收益 | 说明 |
|---|---|
| **UI 代码（HardwareForm.cs）几乎不变** | 只换字段类型 |
| **可单元测试** | 针对 MockAxisHardware 写，不依赖真硬件 |
| **切换开发/生产零成本** | 只改工厂，其他代码不动 |
| **多硬件平台支持** | 不同 SDK 各写一个 IAxisHardware 实现即可 |

---

## 六、工具箱与设计器使用（VS2017+）

### 6.1 把自定义控件加入 VS 工具箱

#### 方法 A：自动出现（推荐）

主项目已通过 `ProjectReference` 引用了 `XyzController.Controls.csproj`，重新生成解决方案后：

1. 打开任意 Form 的设计器视图（如 `MainForm.cs` 双击）
2. 看左侧 **工具箱**（Ctrl+Alt+X）
3. 顶部会出现 **"XyzController.Controls 组件"** 分类，自动列出全部 6 个控件
4. 直接拖拽到窗体即可

#### 方法 B：手动添加（DLL 引用场景）

如果只拿到了 `XyzController.Controls.dll`（不依赖源码项目）：

1. 工具箱空白处 **右键** → **选择项...**
2. 弹窗点 **浏览** → 选中 `XyzController.Controls.dll`
3. 确定后，6 个控件会出现在工具箱顶部的 **"所有 Windows 窗体"** 或自定义分类下

#### 6 个控件工具箱清单

| 控件 | 图标 | 说明 |
|---|---|---|
| `XYView` | 🔵 XY | XY 平面俯视图（带网格、目标点、轨迹） |
| `ZBarView` | 🟢 Z | Z 轴竖直条（也用于 U 轴） |
| `JogButton` | 🟠 J | 点动按钮（按住重复触发） |
| `JoystickPad` | 🟣 J | 8 方向虚拟摇杆 |
| `DroLabel` | ⚫ D | DRO 数字读数（带高亮/报警） |
| `AxisBar` | 🟢 A | 双向轴位置条（带软限位报警） |

### 6.2 拖拽到窗体后的属性面板

每个控件都通过设计时特性（`[Category]`/`[Description]`/`[DefaultValue]`）暴露了清晰的属性面板：

#### XYView 属性面板示例

```
┌─ 属性 ──────────────────────────────────────┐
│ Behavior                                   │
│   RangeMin        -100    机械坐标系最小值    │
│   RangeMax         100    机械坐标系最大值    │
│ Data                                        │
│   TargetX            0    目标 X 坐标         │
│   TargetY            0    目标 Y 坐标         │
│ Appearance                                  │
│   BackColor     245,247,250                  │
│   ForeColor      60,70,90                    │
└─────────────────────────────────────────────┘
```

#### 各控件属性分类一览

| 控件 | 分类 | 关键属性 |
|---|---|---|
| **XYView** | Behavior | `RangeMin`, `RangeMax` |
|  | Data | `TargetX`, `TargetY` |
|  | Action（事件） | `TargetSetByMouse` |
| **ZBarView** | Behavior | `RangeMin`, `RangeMax` |
|  | Data | `TargetZ` |
| **JogButton** | Behavior | `Direction`, `InitialDelay`, `RepeatInterval` |
|  | Action | `Jog`, `Stop` |
| **JoystickPad** | Behavior | `DeadZone` |
|  | Action | `DirectionChanged` |
| **DroLabel** | Appearance | `AxisName`, `Unit` |
|  | Data | `Value`, `Decimals` |
|  | Behavior | `AlarmHigh`, `AlarmLow`, `FlashDuration` |
| **AxisBar** | Appearance | `Orientation`, `AxisLabel` |
|  | Behavior | `RangeMin`, `RangeMax`, `SoftLimitPositive`, `SoftLimitNegative` |
|  | Data | `CurrentValue`, `TargetValue` |

> 💡 **CurrentValue/CurrentX/CurrentY/CurrentZ** 默认设了 `[Browsable(false)]`，属性面板看不到——因为它们是只读的（由 `Advance()` 内部修改）。

### 6.3 双击控件生成默认事件

通过 `[DefaultEvent]` 特性，**双击控件**会自动生成最常用的事件处理函数：

| 控件 | 双击生成的事件 | 默认签名 |
|---|---|---|
| **XYView** | `TargetSetByMouse` | `xyView_TargetSetByMouse(object, PointF e)` |
| **JogButton** | `Jog` | `jogButton1_Jog(object, int direction)` |
| **JoystickPad** | `DirectionChanged` | `joystickPad1_DirectionChanged(object, Point e)` |
| ZBarView / DroLabel / AxisBar | 无默认事件（双击进入 Load 事件） | — |

### 6.4 完整设计器使用流程示例

**目标**：从 0 开始做一个"单轴控制"窗体。

1. **新建窗体** `SingleAxisForm.cs`（VS 里右键项目 → 添加 → 窗体）
2. 双击打开设计器 → 工具箱拖入：
   - 1 个 `AxisBar`（垂直方向）
   - 1 个 `TrackBar`（系统控件）
   - 1 个 `DroLabel`
   - 1 个 `JogButton`（设 `Direction=+1`、`Text="+"`）
3. 在属性面板配置：
   - `axisBar1`：`RangeMin=-100`, `RangeMax=100`, `AxisLabel="X"`, `SoftLimitPositive=90`, `SoftLimitNegative=-90`
   - `droLabel1`：`AxisName="X"`, `Decimals=2`, `Unit="mm"`
   - `trackBar1`：`Minimum=-100`, `Maximum=100`
4. **双击 `JogButton`** → 自动生成 `jogButton1_Jog` 事件
5. 双击 `TrackBar` → 生成 `Scroll` 事件
6. 在 `.cs` 里写业务逻辑（不用关心 Designer 文件）

```csharp
public partial class SingleAxisForm : Form
{
    private float _current;
    private float _target;
    private readonly Timer _timer = new Timer();

    public SingleAxisForm()
    {
        InitializeComponent();
        _timer.Interval = 30;
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // 当前值向目标平滑过渡
        _current += (_target - _current) * 0.25f;
        axisBar1.SetValue(_current);
        droLabel1.SetValue(_current);
    }

    private void trackBar1_Scroll(object sender, EventArgs e)
    {
        _target = trackBar1.Value;
    }

    private void jogButton1_Jog(object sender, int direction)
    {
        _target += direction * 5f;  // 每按一次走 5mm
        trackBar1.Value = (int)_target;
    }
}
```

### 6.5 设计器约束（必读）

为兼容 VS2017 设计器，**`*.Designer.cs` 文件里必须只用字面常量**：

```csharp
// ✅ 允许（设计器和编译器都接受）：
this.xyView.RangeMin = -100F;
this.xyView.TargetX = 0F;
this.zBar.RangeMax = 100F;
this.Controls.Add(this.xyView);

// ❌ 禁止（设计器红屏，编译能过）：
this.xyView.RangeMax = 100 - 50;                // 运算表达式
this.xyView.TargetX = ComputeDefault();         // 方法调用
this.splitMain.SplitterDistance = Width - 80 - 4;  // 运算
for (int i = 0; i < 10; i++) { ... }            // 循环
if (cond) { this.btn.X = 1; }                   // 条件分支
```

如果需要根据运行时数据动态布局，把代码写到 `OnLoad` / `OnResize` / 构造函数里（设计器不执行这些方法）：

```csharp
// 在 Form.cs 里（不是 Designer.cs）：
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);
    splitMain.SplitterDistance = splitMain.Width - 80 - 4;  // ✅ 运行时执行
}
```

详见全局记忆里关于 WinForms Designer 文件约束的规则。

---

## 附录：版本兼容性

| 项 | 要求 |
|---|---|
| .NET Framework | 4.6.1+ |
| Visual Studio | 2017+（设计器要求 Designer 文件只用字面常量）|
| C# 语言版本 | 兼容 C# 2.0+ 的工控机编译器（**全项目 0 处 Lambda `=>`**）|
| 操作系统 | Windows 7+（WPF + WinForms 互操作）|

---

**文档生成日期**：2026-07-21
**项目版本**：包含 53 个单元测试，全部通过
