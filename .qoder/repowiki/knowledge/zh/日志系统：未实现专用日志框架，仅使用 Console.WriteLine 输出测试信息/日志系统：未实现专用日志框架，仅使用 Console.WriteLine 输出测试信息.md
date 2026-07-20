---
kind: logging_system
name: 日志系统：未实现专用日志框架，仅使用 Console.WriteLine 输出测试信息
category: logging_system
scope:
    - '**'
---

经全仓库扫描，该代码库**未实现任何专用的日志系统**。所有业务层（XyzController、Controls、WpfHost）均未引入 Serilog、NLog、log4net、Microsoft.Extensions.Logging 等第三方日志框架，也未使用 System.Diagnostics.Trace/Debug 进行结构化记录。唯一与“日志”相关的输出集中在单元测试项目 `src/XyzController.Tests` 中，通过 `Console.WriteLine` 打印测试结果、颜色标记和进度条，属于调试/演示用途，并非生产级日志方案。

- 无 `log/`、`logging/` 目录或配置项
- 无全局 Logger 初始化、日志级别管理、文件/控制台 Sink 路由
- 业务类（AxisController、AxisJogService、XyzControllerHub 等）内部无任何日志调用
- csproj 依赖中不包含任何日志相关 NuGet 包

因此，本仓库不存在可归纳的 logging_system 架构或约定。