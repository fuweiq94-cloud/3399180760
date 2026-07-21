---
name: pyqt
description: "PyQt/PySide6 overview hub - installation, comparison, project structure. See sub-skills for detailed topics."
metadata:
  author: mte90
  version: 2.0.0
  tags:
    - python
    - qt
    - pyqt
    - pyside
    - gui
    - desktop
    - hub
---

# PyQt/PySide Development

PyQt and PySide are Python bindings for the Qt application framework for building cross-platform desktop applications.

## Sub-Skills

For detailed information, see the specialized sub-skills:

| Skill | Description | Path |
|-------|-------------|------|
| **pyqt-core** | Signals, slots, timers, settings, file I/O | [core/SKILL.md](core/SKILL.md) |
| **pyqt-widgets** | All widgets and layouts | [widgets/SKILL.md](widgets/SKILL.md) |
| **pyqt-threading** | QThread, thread pools, concurrency | [threading/SKILL.md](threading/SKILL.md) |
| **pyqt-dialogs** | Standard and custom dialogs | [dialogs/SKILL.md](dialogs/SKILL.md) |
| **pyqt-testing** | pytest-qt testing patterns | [testing/SKILL.md](testing/SKILL.md) |
| **pyqt-styling** | QSS styling and themes | [styling/SKILL.md](styling/SKILL.md) |
| **pyqt-multimedia** | Audio, video, camera, recording | [multimedia/SKILL.md](multimedia/SKILL.md) |

## PyQt vs PySide Comparison

| Feature | PyQt5 | PyQt6 | PySide6 |
|---------|-------|-------|---------|
| License | GPL | GPL | LGPL |
| Qt Version | Qt 5 | Qt 6 | Qt 6 |
| Maintained | Security only | Active | Active |
| Signal Syntax | `pyqtSignal` | `pyqtSignal` | `Signal` |
| Slot Syntax | `pyqtSlot` | `pyqtSlot` | `Slot` |
| Property Syntax | `pyqtProperty` | `pyqtProperty` | `Property` |
| Commercial Use | Requires license | Requires license | Free |
| QML Registration | `qmlRegisterType()` | `qmlRegisterType()` | `@QmlElement` |

### When to Use Each

- **PySide6**: Recommended for most projects (LGPL, official Qt Company support)
- **PyQt6**: If you need GPL compatibility or existing PyQt codebase
- **PyQt5**: Legacy projects only (security fixes only)

## Installation

### PySide6 (Recommended)

```bash
pip install PySide6
```

### PyQt6

```bash
pip install PyQt6
```

### PyQt5 (Legacy)

```bash
pip install PyQt5
```

## Basic Application

```python
#!/usr/bin/env python3
import sys
from PySide6.QtWidgets import QApplication, QMainWindow, QLabel
from PySide6.QtCore import Qt

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("My Application")
        self.setGeometry(100, 100, 800, 600)
        
        label = QLabel("Hello, Qt!")
        label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.setCentralWidget(label)

def main():
    app = QApplication(sys.argv)
    window = MainWindow()
    window.show()
    sys.exit(app.exec())

if __name__ == "__main__":
    main()
```

## Recommended Project Structure

```
my_app/
├── src/
│   ├── __init__.py
│   ├── main.py
│   ├── main_window.py
│   ├── widgets/
│   │   ├── __init__.py
│   │   └── custom_widget.py
│   ├── models/
│   │   └── data_model.py
│   ├── resources/
│   │   ├── icons/
│   │   └── styles/
│   │       └── style.qss
│   └── utils/
│       └── helpers.py
├── tests/
│   └── test_main.py
├── requirements.txt
└── pyproject.toml
```

## Quick Reference

### Core Imports

```python
# QtWidgets - UI Components
from PySide6.QtWidgets import (
    QApplication, QMainWindow, QWidget,
    QLabel, QPushButton, QLineEdit, QTextEdit,
    QComboBox, QSpinBox, QCheckBox, QRadioButton,
    QSlider, QProgressBar, QGroupBox,
    QTabWidget, QStackedWidget, QSplitter,
    QListWidget, QTreeWidget, QTableWidget,
    QScrollArea, QToolBar, QStatusBar,
    QMenuBar, QMenu
)

# QtCore - Core Non-GUI
from PySide6.QtCore import (
    Qt, QObject, QTimer, QThread,
    Signal, Slot, Property,
    QSize, QPoint, QRect,
    QSettings, QFile, QDir,
    QUrl, QMimeData,
    QDateTime, QDate, QTime
)

# QtGui - Graphics
from PySide6.QtGui import (
    QIcon, QPixmap, QImage,
    QPainter, QPen, QBrush, QColor,
    QFont, QCursor,
    QKeySequence, QShortcut
)
```

### Signal/Slot Basics

```python
from PySide6.QtCore import QObject, Signal, Slot

class MyObject(QObject):
    valueChanged = Signal(int)
    
    @Slot(int)
    def setValue(self, value):
        self._value = value
        self.valueChanged.emit(value)

# Connect
button.clicked.connect(self.onButtonClick)

# Emit
self.valueChanged.emit(42)
```

### Layout Basics

```python
from PySide6.QtWidgets import QVBoxLayout, QHBoxLayout, QGridLayout, QFormLayout

# Vertical
layout = QVBoxLayout()
layout.addWidget(label)
layout.addWidget(button)

# Horizontal
h_layout = QHBoxLayout()
h_layout.addWidget(left)
h_layout.addWidget(right)

# Grid
grid = QGridLayout()
grid.addWidget(label, 0, 0)
grid.addWidget(input, 0, 1)

# Form
form = QFormLayout()
form.addRow("Name:", nameEdit)
```

## Signals and Slots

### Signal Declaration

```python
from PySide6.QtCore import QObject, Signal

class MyObject(QObject):
    valueChanged = Signal(int)
    nameChanged = Signal(str)
    dataReady = Signal(dict)
    errorOccurred = Signal(str)
    positionChanged = Signal(int, int)
```

### Connecting Signals to Slots

```python
# Connect signal to slot
button.clicked.connect(self.onButtonClick)
valueChanged.connect(self.updateValue)

# Connect with lambda
button.clicked.connect(lambda: print("Clicked!"))

# Connect with partial
from functools import partial
button.clicked.connect(partial(self.processItem, item_id))

# Disconnect
button.clicked.disconnect(self.onButtonClick)

# Block signals temporarily
button.blockSignals(True)
button.setChecked(True)
button.blockSignals(False)
```

### PyQt6 Syntax (Different)

```python
from PyQt6.QtCore import QObject, pyqtSignal, pyqtSlot

class MyObject(QObject):
    valueChanged = pyqtSignal(int)
    
    @pyqtSlot(int)
    def setValue(self, value):
        pass
```

## Layout Management

### Box Layouts

```python
from PySide6.QtWidgets import QVBoxLayout, QHBoxLayout, QGroupBox

layout = QVBoxLayout()
layout.addWidget(label)
layout.addWidget(button)
layout.addStretch()

h_layout = QHBoxLayout()
h_layout.addWidget(left_button)
h_layout.addStretch()
h_layout.addWidget(right_button)

# Nest layouts
main_layout = QVBoxLayout()
main_layout.addLayout(h_layout)
```

### Grid Layout

```python
from PySide6.QtWidgets import QGridLayout

layout = QGridLayout()
layout.addWidget(label1, 0, 0)
layout.addWidget(lineEdit, 0, 1)
layout.addWidget(bigWidget, 2, 0, 1, 2)  # span
```

### Layout Properties

```python
layout.setContentsMargins(10, 10, 10, 10)
layout.setSpacing(5)
layout.addWidget(label, alignment=Qt.AlignCenter)
layout.addWidget(widget1, stretch=1)
layout.addWidget(widget2, stretch=2)
```

## Common Widgets

### Input Widgets

```python
# Line edit
lineEdit = QLineEdit()
lineEdit.setPlaceholderText("Enter text...")
lineEdit.textChanged.connect(self.onTextChanged)

# Spin box
spinBox = QSpinBox()
spinBox.setRange(0, 100)
spinBox.valueChanged.connect(self.onValueChanged)

# Combo box
comboBox = QComboBox()
comboBox.addItems(["Option 1", "Option 2"])
comboBox.currentTextChanged.connect(self.onSelectionChanged)

# Slider
slider = QSlider(Qt.Horizontal)
slider.setRange(0, 100)
slider.valueChanged.connect(self.onSliderChanged)
```

### Container Widgets

```python
# Tab widget
tabWidget = QTabWidget()
tabWidget.addTab(page1, "Tab 1")
tabWidget.addTab(page2, "Tab 2")

# Scroll area
scrollArea = QScrollArea()
scrollArea.setWidget(contentWidget)
scrollArea.setWidgetResizable(True)

# Splitter
splitter = QSplitter(Qt.Horizontal)
splitter.addWidget(leftWidget)
splitter.addWidget(rightWidget)
```

## Event Handling

```python
class MyWidget(QWidget):
    def mousePressEvent(self, event):
        if event.button() == Qt.LeftButton:
            print("Left click at", event.pos())
        event.accept()
    
    def keyPressEvent(self, event):
        if event.key() == Qt.Key_Escape:
            self.close()
        event.accept()
    
    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setPen(QPen(Qt.blue, 2))
        painter.drawRect(10, 10, 100, 100)
    
    def closeEvent(self, event):
        reply = QMessageBox.question(self, 'Exit', 'Are you sure?',
            QMessageBox.Yes | QMessageBox.No)
        if reply == QMessageBox.Yes:
            event.accept()
        else:
            event.ignore()
```

## Styling with QSS

```css
QLabel { color: #333; font-size: 14px; }
QPushButton:hover { background-color: #e0e0e0; }
QPushButton:pressed { background-color: #c0c0c0; }
QPushButton[primary="true"] { background-color: #0078d4; color: white; }
```

```python
# Application-wide
app.setStyleSheet("QLabel { color: #333; }")
# From file
with open("style.qss", "r") as f:
    app.setStyleSheet(f.read())
```

## Threading

### Thread Safety Rules

1. **Never access widgets from worker threads**
2. **Use signals for cross-thread communication**
3. **Never block the main thread**

### QThread with Worker Object (Recommended)

```python
from PySide6.QtCore import QThread, Signal, QObject, Slot

class Worker(QObject):
    finished = Signal(object)
    progress = Signal(int)
    error = Signal(str)
    
    @Slot()
    def process(self):
        try:
            for i, item in enumerate(self.data):
                if self._is_cancelled:
                    return
                self.progress.emit(int((i + 1) / len(self.data) * 100))
            self.finished.emit({"status": "success"})
        except Exception as e:
            self.error.emit(str(e))

class ThreadController(QObject):
    def start_work(self, data):
        self.thread = QThread()
        self.worker = Worker(data)
        self.worker.moveToThread(self.thread)
        self.worker.finished.connect(self.on_finished)
        self.thread.started.connect(self.worker.process)
        self.thread.finished.connect(self.thread.deleteLater)
        self.thread.start()
```

### QThreadPool with QRunnable

```python
from PySide6.QtCore import QThreadPool, QRunnable, Signal, QObject

class TaskSignals(QObject):
    finished = Signal(object)
    error = Signal(str)

class ParallelTask(QRunnable):
    def __init__(self, task_id, data):
        super().__init__()
        self.task_id = task_id
        self.data = data
        self.signals = TaskSignals()
    
    def run(self):
        result = {"id": self.task_id, "processed": self.data.upper()}
        self.signals.finished.emit(result)
```

## Dialogs

```python
# File dialog
filename, _ = QFileDialog.getOpenFileName(self, "Open File", "/home/user",
    "Images (*.png *.jpg);;All Files (*)")

# Message box
reply = QMessageBox.question(self, "Confirm", "Are you sure?",
    QMessageBox.Yes | QMessageBox.No, QMessageBox.No)

# Input dialog
text, ok = QInputDialog.getText(self, "Input", "Enter name:")

# Custom Dialog
class CustomDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        layout = QVBoxLayout(self)
        self.nameEdit = QLineEdit()
        layout.addWidget(self.nameEdit)
        buttons = QDialogButtonBox(QDialogButtonBox.Ok | QDialogButtonBox.Cancel)
        buttons.accepted.connect(self.accept)
        buttons.rejected.connect(self.reject)
        layout.addWidget(buttons)
```

## Testing with pytest-qt

```bash
pip install pytest-qt
```

```python
def test_button_click(qtbot):
    button = QPushButton("Click Me")
    label = QLabel("Before")
    qtbot.addWidget(button)
    qtbot.addWidget(label)
    button.clicked.connect(lambda: label.setText("After"))
    qtbot.mouseClick(button, Qt.LeftButton)
    assert label.text() == "After"

def test_async_operation(qtbot):
    worker = Worker()
    with qtbot.waitSignal(worker.finished, timeout=1000) as blocker:
        worker.start()
    assert blocker.args == ["Done"]
```

## Packaging & Distribution

### PyInstaller

```bash
pip install pyinstaller
pyinstaller --onefile --windowed --name "MyApp" main.py
pyinstaller --onefile --windowed --icon=icon.ico --name "MyApp" main.py
```

### Nuitka

```bash
python -m nuitka --standalone --windows-console-mode=disable --output-dir=build main.py
```

## Best Practices

### Application Setup

```python
def main():
    QApplication.setAttribute(Qt.AA_EnableHighDpiScaling, True)
    QApplication.setAttribute(Qt.AA_UseHighDpiPixmaps, True)
    app = QApplication(sys.argv)
    app.setApplicationName("MyApp")
    app.setOrganizationName("MyCompany")
    window = MainWindow()
    window.show()
    sys.exit(app.exec())
```

### Settings Management

```python
from PySide6.QtCore import QSettings

class Settings:
    def __init__(self):
        self.settings = QSettings("MyCompany", "MyApp")
    
    def save_window_state(self, window):
        self.settings.setValue("window/geometry", window.saveGeometry())
        self.settings.setValue("window/state", window.saveState())
    
    def restore_window_state(self, window):
        geometry = self.settings.value("window/geometry")
        if geometry:
            window.restoreGeometry(geometry)
```

### Error Handling

```python
import traceback
from PySide6.QtWidgets import QMessageBox

def excepthook(exc_type, exc_value, exc_tb):
    tb = "".join(traceback.format_exception(exc_type, exc_value, exc_tb))
    QMessageBox.critical(None, "Error", f"An error occurred:\n\n{tb}")

sys.excepthook = excepthook
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "module not found" | `pip install PySide6` |
| High DPI blur | Enable AA_UseHighDpiPixmaps |
| Signals not working | Check Signal/Slot signatures match |
| UI freezing | Use QThread for long operations |
| Memory leak | Delete widgets with `.deleteLater()` |

## References

- [Qt Documentation](https://doc.qt.io/qt-6/)
- [PySide6 Documentation](https://doc.qt.io/qtforpython/)
- [PyQt6 Documentation](https://www.riverbankcomputing.com/static/Docs/PyQt6/)
- [pytest-qt](https://pytest-qt.readthedocs.io/)
