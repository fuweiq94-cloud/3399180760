---
kind: logging_system
name: 无结构化日志系统（仅使用 Console.WriteLine）
category: logging_system
scope:
    - '**'
source_files:
    - XyzController/src/XyzController/Program.cs
    - XyzController/src/XyzController/MainForm.cs
    - XyzController/src/XyzController/Logic/AxisController.cs
    - XyzController/src/XyzController/Logic/XyzControllerHub.cs
    - XyzController/src/XyzController.Tests/Testing/TestRunner.cs
---

本仓库未引入任何第三方日志框架或结构化日志基础设施。整个应用（WinForms 主程序、Logic 业务层、Controls 控件库）均未使用 NLog、Serilog、log4net、Microsoft.Extensions.Logging 等常见 .NET 日志方案，也未出现 System.Diagnostics.Trace/Debug 调用。

唯一与“输出”相关的代码集中在单元测试项目 `XyzController.Tests` 中，通过 `Console.WriteLine` 打印测试标题、进度条、PASS/FAIL 结果以及异常信息，属于控制台测试运行器的调试输出，并非生产级日志。

主程序和业务逻辑层没有任何日志记录点：
- `Program.Main` 仅初始化 WinForms 并启动 `MainForm`；
- `MainForm` 将状态展示在 UI 的 `lblStatus` 文本框中，而非写入日志文件；
- `AxisController`、`XyzControllerHub`、`AxisJogService` 等业务类不产生任何日志输出。

因此，本项目不存在可识别的日志系统架构、级别策略、结构化字段或输出通道配置。若需引入日志能力，建议在 `Program.Main` 早期统一初始化一个集中式 logger（如 Serilog），并在 `XyzControllerHub` 和 `AxisJogService` 等关键路径注入使用。