# Phoenix.UI.Wpf.Architecture.VMFirst

| .NET Framework | .NET Standard | .NET Core |
| :-: | :-: | :-: |
| :heavy_check_mark: 4.5 | :heavy_minus_sign: | :heavy_check_mark: 3.1 |

Collection of base projects for the **View Model First** architecture approach of **WPF** applications.

___

# ViewModel Interfaces

**ViewModel** interfaces enhance simple view models to be able to handle certain commonly needed tasks by making them implement callback functions or properties. Standalone those interfaces don't do that much, but together with ***Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.DefaultViewProvider*** (a separate **NuGet** package) they can be very helpful. 

Typically each of the interfaces has a static helper class with a similar name that provides a ***CreateViewModelSetupCallback*** method, that can be used with the ***DefaultViewProvider*** to setup the interface functionality for implementing view models. Basically the ***DefaultViewProvider*** invokes the callback provided by the ***CreateViewModelSetupCallback*** method once it has resolved view and view model and the callback handles hooking up the interface.

For example the [***IActivatedViewModel***](#IActivatedViewModel) interface has a static helper class ***ActivatedViewModelHelper*** that returns a setup *callback* via its ***CreateViewModelSetupCallback*** method. This *callback* can be passed to a new instance of the ***DefaultViewProvider*** class. If the ***DefaultViewProvider*** has sucessfully resolved a view for a given view model, it invokes all the setup *callbacks* it has internally stored. The callback then checks if the view model and the view fullfill some requirements (e.g. if the view model implements the interface the callback is responsible for) and hooks the view model up. In case of the ***IActivatedViewModel*** this means, that the view models ***OnInitialActivate*** method will be hooked up to the views ***Loaded*** event.

## IActivatedViewModel

This interface is for view models that need to know when their linked view has been loaded to e.g. perform some kind of initialization task.

It provides the following callback:

```csharp
void OnInitialActivate();
```

### Usage with *DefaultViewProvider*

```csharp
var setupCallback = ActivatedViewModelHelper.CreateViewModelSetupCallback();
var viewProvider = new DefaultViewProvider(setupCallback);
```

## IDeactivatedViewModel

This interface is for view models that need to know when their linked view is about to close. This will only apply to view models that are bound to a **Window** as only those provide the necessary **Closing** event.

It provides the following callback:

```csharp
void OnClosing();
```

### Usage with *DefaultViewProvider*

```csharp
var setupCallback = DeactivatedViewModelHelper.CreateViewModelSetupCallback();
var viewProvider = new DefaultViewProvider(setupCallback);
```

## IViewAwareViewModel

This interface is for view models that need to know about their view. Please note, that directly accessing and interacting the view from a view model defies common **MVVM** principles and should be an absolute last resort.

It provides the following property:

```csharp
FrameworkElement View { get; }
```

### Usage with *DefaultViewProvider*

```csharp
var setupCallback = ViewAwareViewModelHelper.CreateViewModelSetupCallback();
var viewProvider = new DefaultViewProvider(setupCallback);
```

## IBusyIndicatorViewModel

This interface is for view models that utilize an [***IBusyIndicatorHandler***](#IBusyIndicatorHandler).

It provides the following property:

```csharp
IBusyIndicatorHandler BusyIndicatorHandler { get; }
```

### Usage with *DefaultViewProvider*

```csharp
Func<IBusyIndicatorHandler> busyIndicatorFactory = () => new IBusyIndicatorHandler(); // This must be an implementing class.
var setupCallback = BusyIndicatorViewModelHelper.CreateViewModelSetupCallback(busyIndicatorFactory);
var viewProvider = new DefaultViewProvider(setupCallback);
```

# IBusyIndicatorHandler

The ***IBusyIndicatorHandler*** interface and its implementing class ***BusyIndicatorHandler*** can be used by view models that want to inform their bound view, that currently some work is being done. It provides bindable *signal properties* **IsBusy** and **BusyMessage** that the view can use and e.g. show some kind of waiting animation (**BusyIndicator**) or lock certain parts of the gui. 

The *signal properties* can be changed by directly calling the below methods of the ***IBusyIndicatorHandler***.

```csharp
/// <summary>
/// Activates the busy indicator by setting the <see cref="IsBusy"/> property to <c>True</c>.
/// </summary>
/// <param name="message"> An optional message to display alongside the busy indicator. </param>
void Show(string message = null);

/// <summary>
/// Overrides the currently displayed busy message.
/// </summary>
/// <param name="message"> An optional message to display alongside the busy indicator. </param>
void Override(string message = null);

/// <summary>
/// Removes the topmost busy message in the stack of all currently displayed messages. If this is the last one, then the busy indicator will be hidden.
/// </summary>
void Revoke();

/// <summary>
/// Completely disables the busy indicator by setting the <see cref="IsBusy"/> property to <c>False</c>.
/// </summary>
void Close();
```

Normally the usage of the following methods is better that manually calling above methods, as below ones are made to encapsulate workload and implicitly handle changing the *signal properties*.

- **Those methods will run in the calling thread and therefore block further execution.**

*Activates the busy indicator while executing the passed method.*
```csharp
void Execute(Action method, Action doneCallback = null);
```
```csharp
void Execute(string message, Expression<Func<bool>> toggle, Action method);
```
```csharp
void Execute(string message, Action method, Action doneCallback = null);
```

**Those methods will await the passed function and therefore not block the calling thread.**

*Activates the busy indicator while executing the passed asynchronous method.*
```csharp
Task ExecuteAsync(Func<Task> asyncMethod, Action doneCallback = null);
```
```csharp
Task ExecuteAsync(string message, Expression<Func<bool>> toggle, Func<Task> asyncMethod);
```
```csharp
Task ExecuteAsync(string message, Func<Task> asyncMethod, Action doneCallback = null);
```
```csharp
Task<T> ExecuteAsync<T>(Func<Task<T>> asyncFunction, Action doneCallback = null);
```
```csharp
Task<T> ExecuteAsync<T>(string message, Expression<Func<bool>> toggle, Func<Task<T>> asyncFunction);
```
```csharp
Task<T> ExecuteAsync<T>(string message, Func<Task<T>> asyncFunction, Action doneCallback = null);
```

**Those methods will await the passed function and therefore not block the calling thread. They additionally provide cancellation support.**

*Activates the busy indicator while executing the passed asynchronous method with cancellation support.*
```csharp
Task ExecuteAsync(Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteAsync(string message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteAsync(string message, Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);
```

**Those methods will wrap the passed method in a task that will be awaited, therefore not blocking the calling thread and guaranteeing execution in another thread.**

*Activates the busy indicator while executing the passed method that will be wrapped within its own Task.*

*Wraps simple methods within their own awaited task.*
```csharp
Task ExecuteTaskAsync(Action method, Action doneCallback = null, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Action method, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Action method, Action doneCallback = null, CancellationToken cancellationToken = default);
```

*Wraps asynchronous tasks within their own awaited task. This is useful, if it is unknown, whether the underlying task really runs synchronous or not.*
```csharp
Task ExecuteTaskAsync(Func<Task> asyncMethod, Action doneCallback = null);
```
```csharp
Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Func<Task> asyncMethod);
```
```csharp
Task ExecuteTaskAsync(string message, Func<Task> asyncMethod, Action doneCallback = null);
```

*Wraps asynchronous tasks within their own awaited and cancelable task. This is useful, if it is unknown, whether the underlying task really runs synchronous or not.*
```csharp
Task ExecuteTaskAsync(Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);
```

If a view model wants to use the ***IBusyIndicatorHandler*** it should implement the [IBusyIndicatorViewModel](#IBusyIndicatorViewModel) interface.

# Authors

* **Felix Leistner** - _Initial release_