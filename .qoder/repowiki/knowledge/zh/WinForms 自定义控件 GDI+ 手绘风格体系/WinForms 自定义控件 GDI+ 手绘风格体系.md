---
kind: frontend_style
name: WinForms 自定义控件 GDI+ 手绘风格体系
category: frontend_style
scope:
    - '**'
source_files:
    - src/XyzController.Controls/PaintHelper.cs
    - src/XyzController.Controls/JoystickPad.cs
    - src/XyzController.Controls/AxisBar.cs
    - src/XyzController.Controls/ZBarView.cs
    - src/XyzController.Controls/XYView.cs
    - src/XyzController.Controls/JogButton.cs
    - src/XyzController.Controls/DroLabel.cs
    - src/XyzController/MainForm.cs
---

本仓库为 .NET Framework 4.6.1 + WinForms 桌面应用，**不存在 CSS/SCSS/Tailwind 等 Web 前端样式系统**。UI 视觉风格完全由 C# 代码通过 System.Drawing / GDI+ 在 `OnPaint` 中手绘实现，属于传统的 WinForms 自定义控件绘制风格。

### 采用的方式与工具
- **GDI+ 直接绘图**：所有控件（JoystickPad、AxisBar、XYView、ZBarView、JogButton、DroLabel）均继承自 `System.Windows.Forms.Control`，重写 `OnPaint` 使用 `Graphics`、`Brush`、`Pen`、`LinearGradientBrush` 等 API 绘制图形。
- **双缓冲抗锯齿**：控件构造函数中设置 `DoubleBuffered = true`、`ResizeRedraw = true`，并在 `OnPaint` 中启用 `SmoothingMode.AntiAlias` 和 `TextRenderingHint.ClearTypeGridFit`，保证动画流畅与文字清晰。
- **无第三方 UI 框架**：未引用 Material、Fluent、Avalonia、WPF 等任何现代 UI 库，纯原生 WinForms。
- **无外部样式文件**：仓库中不存在 `.css`、`.scss`、`.less`、`.xaml`、`.resx` 主题资源；颜色、字体、尺寸全部硬编码为 `Color.FromArgb(...)` 与 `Font("Segoe UI", ...)`。

### 关键文件与职责
- `src/XyzController.Controls/PaintHelper.cs`：集中封装重复的绘图逻辑（高质量 Graphics 初始化、背景填充、刻度步长选择、居中文本绘制、竖直条区域计算等），是全局“视觉规范”的唯一来源。
- `src/XyzController.Controls/JoystickPad.cs`：8 方向虚拟摇杆，展示渐变底盘、虚线十字辅助线、死区圆、带白色描边的蓝色手柄及方向箭头提示。
- `src/XyzController.Controls/AxisBar.cs`、`src/XyzController.Controls/ZBarView.cs`、`src/XyzController.Controls/XYView.cs`：三轴进度条与 XY 平面视图，统一采用竖条 + 右侧刻度线的布局。
- `src/XyzController.Controls/JogButton.cs`、`src/XyzController.Controls/DroLabel.cs`：JOG 按钮与 DRO 数值标签，遵循相同配色与字体约定。
- `src/XyzController/MainForm.cs` + `MainForm.Designer.cs`：主窗体仅负责布局组合，不直接参与像素级绘制。

### 架构与视觉约定
- **配色方案**：以冷灰蓝为主色调——背景 `Color.FromArgb(245,247,250)`、底盘渐变 `(230,235,245)→(210,218,230)`、指针蓝 `(120,170,240)→(40,100,210)`、边框 `Color.FromArgb(180,190,205)`、刻度线 `Color.FromArgb(160,170,185)`。所有颜色均以 ARGB 常量形式散落在各控件中，尚未抽取到集中设计令牌。
- **字体规范**：统一使用 Segoe UI，字号 7.5F～8F，文本颜色 `Color.FromArgb(120,130,145)` 或 `Color.FromArgb(150,160,175)`，呈现浅灰科技风。
- **响应式策略**：控件在 `OnResize` 中根据 `Width/Height` 动态计算半径、中心点、条形区域，保持正方形摇杆与居中布局，无需媒体查询。
- **可访问性**：控件默认不可聚焦（`ControlStyles.Selectable=false`，除 JoystickPad 外），无 ARIA 概念。

### 开发者应遵循的规则
1. **新增控件必须继承 Control 并重写 OnPaint**，不要依赖 Designer 生成的可视化布局来改变外观。
2. **复用 PaintHelper**：所有通用绘图（SetupGraphics、FillBackground、DrawCenteredTextInRect、ChooseStep、VerticalBarArea 等）优先调用该静态类，避免重复实现。
3. **颜色与字体集中管理**：当前分散在各文件中，建议后续将 ARGB 常量与 Font 定义提取到统一的 `Theme` 或 `DesignTokens` 类，便于一键换肤。
4. **保持抗锯齿与双缓冲**：新控件构造函数中务必设置 `DoubleBuffered = true`、`ResizeRedraw = true`，并在 `OnPaint` 开头调用 `PaintHelper.SetupGraphics(g)`。
5. **坐标与比例**：所有尺寸基于 `ClientRectangle` 相对计算，禁止硬编码绝对像素值，确保在不同 DPI 下缩放一致。
6. **交互反馈**：状态变化后调用 `Invalidate()` 触发重绘，不要在事件处理中直接 Sleep 阻塞 UI 线程。