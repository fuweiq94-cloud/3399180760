---
name: desktop-apps
description: Desktop application development skill - triggered when building desktop apps for Windows, macOS, or Linux. Guides framework selection (Electron, Tauri, .NET MAUI, Qt, Flutter Desktop, WPF), native features, packaging, distribution, and auto-updates.
allowed-tools: Read, Write, Edit, Glob, Grep, Bash, WebFetch, WebSearch, Agent
---

# Desktop Application Development Skill

## When This Skill Activates
- User mentions: desktop app, Windows app, Mac app, Linux app, native app, Electron, Tauri
- During `/implement` when building for desktop
- When discussing packaging, installers, auto-updates, system tray

## Framework Selection Guide

### Ask First
1. Target platforms? (Windows only, Mac only, all three, or specific combo)
2. Team expertise? (Web tech, C#/.NET, C++, Dart, Rust)
3. App type? (Utility, data-heavy, media, creative tool, system tool)
4. Native look & feel important? (Yes -> native toolkit, No -> web-based)
5. App size constraints? (Electron = 100MB+, Tauri = 5-10MB)
6. Need system-level access? (File system, USB, hardware, registry)

### Framework Decision Tree
```
Web team + cross-platform?     -> Tauri (Rust + web frontend, tiny bundle)
Web team + maximum ecosystem?  -> Electron (Node.js + Chromium, large bundle)
C#/.NET team + Windows focus?  -> WPF or WinUI 3
C#/.NET team + cross-platform? -> .NET MAUI or Avalonia UI
C++ team + cross-platform?     -> Qt (mature, enterprise)
Dart/Flutter team?             -> Flutter Desktop (Win/Mac/Linux)
Python team?                   -> PyQt6 or Tkinter (simple)
Rust team?                     -> Tauri or Iced
```

### Framework Comparison
| Framework | Language | Platforms | Bundle Size | Performance | Native Feel |
|-----------|---------|-----------|-------------|-------------|-------------|
| **Tauri** | Rust + Web | Win/Mac/Linux | 5-10 MB | Excellent | Good (webview) |
| **Electron** | JS/TS | Win/Mac/Linux | 100-200 MB | Good | Web-like |
| **Flutter Desktop** | Dart | Win/Mac/Linux | 15-30 MB | Good | Custom (Material/Cupertino) |
| **.NET MAUI** | C# | Win/Mac | 30-50 MB | Good | Native |
| **WPF** | C# | Windows only | 10-20 MB | Good | Native Windows |
| **WinUI 3** | C# | Windows only | 10-20 MB | Excellent | Modern Windows |
| **Qt** | C++ | All + embedded | 20-50 MB | Excellent | Native |
| **Avalonia** | C# | Win/Mac/Linux | 20-40 MB | Good | Custom |
| **PyQt6** | Python | Win/Mac/Linux | 50-100 MB | Fair | Native |

### Desktop-Specific Architecture
```
Main Process (backend logic, system access, IPC)
  |
  |-- IPC Bridge (secure messaging)
  |
Renderer Process (UI, web technologies or native)
  |
  |-- Local Storage (SQLite, JSON, files)
  |-- System Integration (tray, notifications, shortcuts)
  |-- Auto-Updater
```

### Desktop-Specific Features
- **Auto-Update**: Electron (electron-updater), Tauri (built-in), Sparkle (Mac)
- **System Tray**: Minimize to tray, background operation
- **File System**: Read/write local files, drag & drop, file associations
- **Notifications**: Native OS notifications
- **Keyboard Shortcuts**: Global hotkeys, custom shortcuts
- **Deep Links**: Custom URL protocols (myapp://action)
- **Single Instance**: Prevent multiple app instances
- **Crash Reporting**: Sentry, Electron crashReporter

### Packaging & Distribution
| Platform | Installer | Store | Auto-Update |
|----------|-----------|-------|-------------|
| **Windows** | NSIS, MSI, MSIX | Microsoft Store | electron-updater, Squirrel |
| **macOS** | DMG, PKG | Mac App Store | Sparkle, Tauri updater |
| **Linux** | AppImage, Snap, Flatpak, deb, rpm | Snap Store, Flathub | AppImage built-in |

### Code Signing
- **Windows**: EV code signing certificate (required for SmartScreen trust)
- **macOS**: Apple Developer certificate + notarization (required for Gatekeeper)
- **Linux**: GPG signing for packages (optional but recommended)

### Security
- IPC validation: sanitize all messages between processes
- No Node.js in renderer (Electron): use contextBridge + preload
- Content Security Policy for webview-based apps
- Sandboxing: enable by default
- Auto-update over HTTPS with signature verification
