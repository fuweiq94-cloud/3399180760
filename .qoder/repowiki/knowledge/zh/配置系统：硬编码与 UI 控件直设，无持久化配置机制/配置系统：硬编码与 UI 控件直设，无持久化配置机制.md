---
kind: configuration_system
name: 配置系统：硬编码与 UI 控件直设，无持久化配置机制
category: configuration_system
scope:
    - '**'
source_files:
    - src/XyzController/Program.cs
    - src/XyzController/MainForm.cs
    - src/XyzController/Logic/AxisController.cs
    - src/XyzController/Logic/AxisJogService.cs
---

本仓库未实现任何运行时配置系统。所有可调参数均以**硬编码或 UI 控件直接赋值**的方式注入到业务层，不存在配置文件、环境变量、注册表或用户设置文件的加载逻辑。

具体表现：
- **轴范围（Min/Max）**：由 MainForm 在构造时从 TrackBar/NumericUpDown 的 Minimum/Maximum 属性读取并传入 XyzControllerHub，再传递给 AxisController 构造函数，属于 UI 设计期值，不持久化。
- **速度设置**：通过 MainForm 中 trbSpeed 滑块的值直接赋给 _hub.SpeedSetting，随窗体运行即时生效，退出即丢失。
- **JOG 步长**：由 nudJogStep 控件 ValueChanged 事件实时写入 AxisJogService.StepDistance，同样仅内存态。
- **程序启动模式**：Program.Main 通过命令行参数 `main` / `jump` / `trajectory` / `nav` 选择启动页面，这是唯一的“外部可配置”入口，但也不持久化。
- **WPF 宿主页面注册**：RunMultiPage 中 WpfPage 列表是代码内联注册的，新增页面需改源码。

未发现以下配置相关代码：`ConfigurationManager`、`Properties.Settings`、`app.config`、`*.json`、`*.xml`、`*.ini`、`*.yaml`、`*.toml`、`*.env`、`File.ReadAllText` 等文件 I/O 操作。

结论：该仓库当前处于“演示/原型”阶段，配置全部内联于 UI 层，不具备跨进程/跨会话的配置管理能力。若后续需要支持部署期调参（如轴限位、默认速度、JOG 步长），建议引入 `appsettings.json` + `IOptions<T>` 或 .NET Framework 下的 `ConfigurationManager` + `app.config` 方案，并将配置加载逻辑下沉至 Hub 或独立 ConfigProvider 类。