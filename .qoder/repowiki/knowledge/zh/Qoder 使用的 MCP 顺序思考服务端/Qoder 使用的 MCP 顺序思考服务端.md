---
kind: external_dependency
name: Qoder 使用的 MCP 顺序思考服务端
slug: qoder-mcp-server-sequential-thinking
category: external_dependency
category_hints:
    - vendor_identity
    - client_constraint
scope:
    - '**'
---

### 身份与角色
- 由 Qoder 作为 MCP（Model Context Protocol）客户端加载的 Node.js 包 `@modelcontextprotocol/server-sequential-thinking`，在对话中通过 `npx -y @modelcontextprotocol/server-sequential-thinking` 启动。
- 本项目源码不包含该包的引用；它仅出现在本机调试/排障过程中，属于开发工具链依赖而非运行时依赖。

### 集成方式与约束
- 启动路径：MCP 进程以 stdio 传输调用 `npx.cmd`（Windows 下解析为 `.cmd`，不是 PowerShell 的 `.ps1`），因此 PowerShell 执行策略对 `.ps1` 的限制不影响 MCP。
- 运行环境要求：Node.js 安装目录必须加入用户级 PATH（如 `C:\Program Files\nodejs`），否则 MCP 子进程找不到 `npx`。
- 若 PATH 不可控，可在 MCP 配置中将 `command` 直接写为绝对路径（例如 `C:\Program Files\nodejs\npx.cmd`）绕过 PATH 查找。

### 注意
- 本卡记录的是本机开发环境的 MCP 使用方式，不属于 XyzController 项目的构建或运行依赖。