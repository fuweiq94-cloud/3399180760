---
kind: frontend_style
name: WinForms/WPF 混合 UI 样式体系
category: frontend_style
scope:
    - '**'
source_files:
    - src/XyzController/Form1.cs
    - src/XyzController/MainForm.Designer.cs
    - src/XyzController.WpfHost/MainWindow.xaml
    - src/XyzController.Controls/XYView.cs
    - src/XyzController.Controls/ZBarView.cs
---

本仓库采用 WinForms + WPF 混合架构，前端样式体系由两套独立子系统组成：

**1. WinForms 层（XyzController 主程序）**
- 样式完全通过代码内联设置 `BackColor`、`ForeColor`、`Font` 等属性，未使用 `.resx` 资源文件或外部样式表。
- 颜色采用硬编码的 ARGB 值，形成一套固定配色方案：背景色 `#F5FAFF` / `#201C28`，前景色 `#78FFB4`（DRO 数字显示）、`#3C4658`（控件文本）。
- 字体统一使用 `Consolas`（等宽数字显示）和 `Segoe UI`（常规文本），字号与粗细在 Designer.cs 中直接指定。
- 布局依赖 `DockStyle.Fill`、`TableLayoutPanel` 百分比列宽等 WinForms 原生布局机制，无响应式适配逻辑。
- 自定义控件库（`XyzController.Controls`）通过重写 `OnPaint` 实现绘图，样式与绘制逻辑耦合在同一文件中。

**2. WPF 宿主层（XyzController.WpfHost）**
- 仅包含一个极简 XAML 窗口，使用内联十六进制颜色 `#2D2D30` 作为导航栏背景。
- 未定义任何 `ResourceDictionary`、`Style`、`Theme` 或 `Color` 资源文件，所有视觉样式均硬编码在 XAML 中。
- 通过 `WpfPage` 基类承载 WinForms 控件（Airspace 限制下嵌入），WPF 层本身不渲染业务界面。

**关键约定：**
- 无 CSS/SCSS/Less 等 Web 样式技术栈。
- 无设计令牌（Design Tokens）集中管理，颜色/字体散落在各 Form 的 Designer.cs 中。
- 无主题切换能力，不支持运行时换肤。
- 无第三方 UI 组件库引用，全部基于 .NET Framework 4.6.1 原生控件。