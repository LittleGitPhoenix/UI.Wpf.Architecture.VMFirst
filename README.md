# Phoenix.UI.Wpf.Architecture.VMFirst

| .NET Framework | .NET Core | .NET |
| :-: | :-: | :-: |
| :heavy_check_mark: 4.8 | :heavy_check_mark: 3.1 | :heavy_check_mark: 5.0 :heavy_check_mark: 6.0 |

Collection of base projects for the **View Model First** architecture approach of **WPF** applications.
___

# Table of content

[toc]
___

# ViewModel Interfaces

**ViewModel** interfaces enhance simple view models to be able to handle certain commonly needed tasks by making them implement callback functions or properties. Standalone those interfaces don't do that much, but together with an `IViewProvider` like the `DefaultViewProvider` from the separate **NuGet** package ***Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider*** they can be very helpful. 

When using an `IViewProvider` those viewmodel interfaces can automatically be hooked up via the `IViewProvider.ViewLoaded` event. This event provides the newly loaded viewmodel and its bound view via its `ViewLoadedEventArgs`. When the event is raised, callback methods can be executed that initialize all viewmodel interfaces the newly loaded viewmodel implements. Those callback methods can typically be found in a static helper class with a similar name to their viewmodel interface.

For example the [`IActivatedViewModel`](#IActivatedViewModel) interface has a static helper class `ActivatedViewModelHelper` with the callback method simply named `Callback`. This method takes in a viewmodel and its bound view as parameters.

````csharp
static void Callback(object viewModel, FrameworkElement view)
````

Since the event does provide those two parameters, all that needs to be done is attaching all callbacks methods of the viewmodel interfaces to the `IViewProvider.ViewLoaded` event. This should be done early on in an application.

## IActivatedViewModel

This interface is for view models that need to know when their linked view has been loaded. This can then be used to perform some kind of initialization task.

It provides the following method:

```csharp
void OnInitialActivate();
```

### Usage with **`IViewProvider`**

```csharp
IViewProvider provider = new DefaultViewProvider();
provider.ViewLoaded += (sender, args) => ActivatedViewModelHelper.Callback(args.ViewModel, args.View);
```

## IDeactivatedViewModel

This interface is for view models that need to know when their linked view is about to close. This will only apply to view models that are bound to a **Window** as only those provide the necessary **Closing** event.

It provides the following method:

```csharp
void OnClosing();
```

### Usage with **`IViewProvider`**

```csharp
IViewProvider provider = new DefaultViewProvider();
provider.ViewLoaded += (sender, args) => DeactivatedViewModelHelper.Callback(args.ViewModel, args.View);
```

## IViewAwareViewModel

This interface is for view models that need to know about their view.

<div style='padding:0.1em; border-style: solid; border-width: 0px; border-left-width: 10px; border-color: #ff0000; background-color: #ff000020' >
	<span style='margin-left:1em; text-align:left'>
    	<b>Warning</b>
    </span>
    <br>
	<div style='margin-left:1em; margin-right:1em;'>
		Please note, that directly accessing and interacting with the view from a view model defies common <b>MVVM</b> principles and should be an absolute last resort.
    </div>
</div>

It provides the following property:

```csharp
FrameworkElement View { get; }
```

### Usage with **`IViewProvider`**

```csharp
IViewProvider provider = new DefaultViewProvider();
provider.ViewLoaded += (sender, args) => ViewAwareViewModelHelper.Callback(args.ViewModel, args.View);
```

## IBusyIndicatorViewModel

This interface is for view models that utilize an [`IBusyIndicatorHandler`](#IBusyIndicatorHandler).

It provides the following property:

```csharp
IBusyIndicatorHandler BusyIndicatorHandler { get; }
```

### Usage with **`IViewProvider`**

The setup of an `IBusyIndicatorViewModel` differs a little bit from the other ones, as it additionally needs an [`IBusyIndicatorHandler`](#IBusyIndicatorHandler) besides the viewmodel and its bound view. There are two ways of setting this viewmodel interface up:

- `BusyIndicatorViewModelHelper.Callback`

```csharp
IViewProvider provider = new DefaultViewProvider();
provider.ViewLoaded += (sender, args) =>
{
	IBusyIndicatorHandler busyIndicator = new IBusyIndicatorHandler(); // This must be an implementing class.
	BusyIndicatorViewModelHelper.Callback(args.ViewModel, args.View, busyIndicator);
};
```

- `BusyIndicatorViewModelHelper.CreateCallback` (this is the better option for IOC)

```csharp
Func<IBusyIndicatorHandler> busyIndicatorFactory = () => new IBusyIndicatorHandler(); // This must be an implementing class.
var callback = BusyIndicatorViewModelHelper.CreateCallback(busyIndicatorFactory);
IViewProvider provider = new DefaultViewProvider();
provider.ViewLoaded += (sender, args) => callback.Invoke(args.ViewModel, args.View);
```
___

# IBusyIndicatorHandler

The `IBusyIndicatorHandler` interface and its implementing class `BusyIndicatorHandler` can be used by view models that want to inform their bound view, that currently some work is being executed. It provides the bindable *signal properties* `IsBusy` and `BusyMessage` that the view can use and show some kind of waiting animation (**BusyIndicator**) or lock certain parts of the UI. 

The *signal properties* can be changed by directly calling the below methods of the `BusyIndicatorHandler`.

- Activates the busy indicator by setting the `IsBusy` property to **True** and sets the `BusyMessage` to the defined **message**. Internally this pushes the new message onto a stack of messages and displays the topmost one.

```csharp
void Show(string message = null);
```

<div style='padding:0.1em; border-style: solid; border-width: 0px; border-left-width: 10px; border-color: #ff0000; background-color: #ff000020' >
	<span style='margin-left:1em; text-align:left'>
    	<b>Warning</b>
    </span>
    <br>
	<div style='margin-left:1em; margin-right:1em;'>
		Please note, that every message that is pushed with the <i>Show</i> method must be removed. Otherwise the <i>IsBusy</i> property won't be set to <i>false</i>. Either use one <i>Revoke</i> call for every message, or call <i>Close</i> if the workload has finished.
    </div>
</div>

- Overrides the currently displayed `BusyMessage`.

```csharp
void Override(string message = null);
```

- Removes the topmost `BusyMessage` in the stack of all currently displayed messages. If this is the last one, then the busy indicator will be hidden.

```csharp
void Revoke();
```

- Completely disables the busy indicator by setting the `IsBusy` property to **False**.

```csharp
void Close();
```

Normally the usage of the following methods is better than manually calling above methods, as below ones are made to encapsulate workload and implicitly handle changing the *signal properties*.

**Those methods will run in the calling thread and therefore block further execution.**

- Activates the busy indicator while executing the passed method.

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

- Activates the busy indicator while executing the passed asynchronous method.

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

- Activates the busy indicator while executing the passed asynchronous method with cancellation support.

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

- Activates the busy indicator while executing the passed method that will be wrapped within its own Task.

⇒ Wraps simple methods within their own awaited task.

```csharp
Task ExecuteTaskAsync(Action method, Action doneCallback = null, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Action method, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Action method, Action doneCallback = null, CancellationToken cancellationToken = default);
```

⇒ Wraps asynchronous tasks within their own awaited task. This is useful, if it is unknown, whether the underlying task really runs synchronous or not.

```csharp
Task ExecuteTaskAsync(Func<Task> asyncMethod, Action doneCallback = null);
```
```csharp
Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Func<Task> asyncMethod);
```
```csharp
Task ExecuteTaskAsync(string message, Func<Task> asyncMethod, Action doneCallback = null);
```

⇒ Wraps asynchronous tasks within their own awaited and cancelable task. This is useful, if it is unknown, whether the underlying task really runs synchronous or not.

```csharp
Task ExecuteTaskAsync(Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default);
```
```csharp
Task ExecuteTaskAsync(string message, Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);
```

If a view model wants to use the `IBusyIndicatorHandler` it should implement the [`IBusyIndicatorViewModel`](#IBusyIndicatorViewModel) interface.

___

# Stylet

The **Phoenix.UI.Wpf.Architecture.VMFirst.Stylet** package provides some assets to be used together with [**Stylet**](https://github.com/canton7/Stylet).

## StyletViewManager

This is an implementation of **Stylet.IViewManager** where view model to view resolving is handled by a **Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.IViewProvider**.

By default **Autofac** is responsible to resolve the `StyletViewManager` and all its requirements. If not configured otherwise, the IOC container will create it with `Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.DefaultViewProvider` as its default `IViewProvider`. This `IViewProvider` is not accessible and setting up any [viewmodel interfaces](#ViewModel-Interfaces) won't work. Therefore it is recommended to manually register this or any other `IViewProvider` with the IOC container. Following is an example how to do this:

- Register the services:

```csharp
private static void RegisterViewModelFirst(ContainerBuilder builder)
{
	// Register the view loaded callbacks that will be used by the DefaultViewProvider.
	builder
		.Register<Action<object, FrameworkElement>>(_ => ActivatedViewModelHelper.Callback)
		.SingleInstance()
		;
	builder
		.Register<Action<object, FrameworkElement>>(_ => DeactivatedViewModelHelper.Callback)
		.SingleInstance()
		;
	builder
		.Register<Action<object, FrameworkElement>>(_  => ViewAwareViewModelHelper.Callback)
		.SingleInstance()
		;
	builder
		.Register<Action<object, FrameworkElement>>(context => BusyIndicatorViewModelHelper.CreateCallback(context.Resolve<Func<IBusyIndicatorHandler>>()))
		.SingleInstance()
		;

	// Register the view provider. 
	builder.RegisterType<DefaultViewProvider>().As<IViewProvider>().SingleInstance();

	// Register the special view provider needed by the DialogProvider to display (metro) dialogs.
	builder.RegisterType<MetroDialogAssemblyViewProvider>().As<DialogAssemblyViewProvider>().SingleInstance();

	// Register the DefaultDialogManager that uses the applications main window to display dialogs.
	builder.RegisterType<DefaultDialogManager>().As<IDefaultDialogManager>().SingleInstance();

	// Register the busy handler.
	builder.RegisterType<BusyIndicatorHandler>().As<IBusyIndicatorHandler>();
}
```

- Override `StyletBootstrapper.BeforeLaunch` in your custom bootstrapper implementation to setup handling of [viewmodel interfaces](#ViewModel-Interfaces).
```csharp
/// <inheritdoc />
protected override void BeforeLaunch
(
	IContainer container,
	string applicationName,
	Version assemblyVersion,
	Version fileVersion,
	string informationalVersion
)
{
	// Get the callbacks and ALL view providers.
	var callbacks = container.Resolve<ICollection<Action<object, FrameworkElement>>>();
	var viewProviders = container.Resolve<ICollection<IViewProvider>>();
	
	// Hook the callbacks up to the ViewLoaded event of the view providers.
	foreach (var viewProvider in viewProviders)
	{
		viewProvider.ViewLoaded += (sender, args) =>
		{
			foreach (var callback in callbacks)
			{
				callback.Invoke(args.ViewModel, args.View);
			}
		};
	}
}
```

## StyletBootstrapper

A custom bootstrapper inheriting from  **Stylet.BootstrapperBase** that uses **Autofac** as IOC container and the [`StyletViewManager`](#StyletViewManager) as view manager. This should be the base class of the bootstrapper for each project.

Below is template for a custom bootstrapper.

```c#
internal class MyBootstrapper : StyletBootstrapper<MyWindow>
{
	/// <inheritdoc />
	protected override void OnStart() { }

	/// <inheritdoc />
	protected override void RegisterServices(ContainerBuilder builder) { }

	/// <inheritdoc />
	protected override void BeforeLaunch
	(
		IContainer container,
		string applicationName,
		Version assemblyVersion,
		Version fileVersion,
		string informationalVersion
	) { }

	/// <inheritdoc />
	protected override void AfterLaunch(IContainer container) { }

	/// <inheritdoc />
	protected override void OnClosing(IContainer container, ExitEventArgs args) { }
}
```



___

# Authors

* **Felix Leistner**: _v1.x_ - _v3.x_