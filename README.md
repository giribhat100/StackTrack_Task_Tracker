# StackTrack

A modern, high-performance Task Management Desktop Application designed using WPF, C#, and .NET. The application breaks away from standard, monotonous operating system window chrome by introducing a **Glassmorphism (Glassy UI)** aesthetic combined with strict adherence to the **MVVM (Model-View-ViewModel)** architectural pattern.

This document outlines the core architectural and implementation decisions made during the design and development phases of the system.

## Design Decisions

### 1. Architectural Pattern (Pure MVVM - Model-View-ViewModel)
*   **Decision:** Strict implementation of the MVVM pattern with a generic, zero-code-behind Command helper infrastructure (`RelayCommand`). Inter-layer communications rely on Data Binding and data structures utilizing change notifications (`INotifyPropertyChanged`).
*   **Reason:** Coupling UI design layout tightly with domain logic (code-behind callbacks) causes maintenance degradation over time. MVVM isolates application state and actions from view-rendering implementations.
*   **Impact:** **Maintainability & Scalability.** Allows developers to alter design controls, button states, or list layouts without touching database tracking layers or asynchronous logic. It also lays the absolute foundation for executing isolated unit testing on task behaviors without initializing any UI instances.

### 2. Window Lifecycle Interactivity (Pure MVVM Control Execution)
*   **Decision:** Injecting the `Window` instance object directly as an actionable element template (`CommandParameter="{Binding ElementName=MainWindowShell}"`) to be consumed down-level by a centralized generic infrastructure (`CloseWindowCommand`).
*   **Reason:** Custom chrome setups require a mechanism to shut down application processes since the operating system close box is stripped. Traditional implementations fallback to view-layer handlers (`CloseButton_Click` in `MainWindow.xaml.cs`), breaking pure MVVM architectures.
*   **Impact:** **Simplicity & Cleanliness.** Completely flattens and purges the View's code-behind file to nothing but its construction blueprint (`InitializeComponent()`), honoring strict separation of concerns.

### 3. Collection Management & Filtering Strategy (ICollectionView over Linq Remapping)
*   **Decision:** Leveraging WPF's native infrastructure tracking system `ICollectionView` (`CollectionViewSource.GetDefaultView`) coupled with dynamic predicate handlers (`FilteredTasks.Filter = FilterTaskPredicate`) to process "All", "Pending", and "Completed" filters.
*   **Reason:** Alternative approaches create separate arrays or clear/re-populate standard `ObservableCollection` lists on every toggle event, which destroys reference points, tracking integrity, and causes UI stuttering.
*   **Impact:** **Performance & Responsiveness.** Re-filtering happens live within memory pipelines without resetting layout elements or rebuilding underlying item bindings. The performance profile remains flat even under higher data-density tracking conditions.

## Technology 

*   **Framework:** .NET Core / WPF (Windows Presentation Foundation)
*   **Language:** C# 
*   **Data Layout:** XAML with custom `ControlTemplates`
*   **Async Operations:** Task-based Asynchronous Pattern (TAP)


## Execution & Running Instructions

1. Clone or download the source directories into a designated repository workspace.
2. Open the solution layout folder within **Visual Studio 2022** (or JetBrains Rider).
3. Validate that your targeting compiler profile matches `.NET 4.8` or higher SDK dependencies.
4. Build the workspace configuration (`Ctrl + Shift + B`).
5. Execute or Debug the runtime bundle (`F5`).
