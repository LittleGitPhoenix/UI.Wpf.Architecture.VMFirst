#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion

using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Phoenix.UI.Wpf.Architecture.VMFirst.Classes;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

/// <summary>
/// Interface of view models utilizing an <see cref="IBusyIndicatorHandler"/>.
/// </summary>
public interface IBusyIndicatorViewModel
{
	/// <summary> <see cref="IBusyIndicatorHandler"/> used for displaying a workload indicator. </summary>
	IBusyIndicatorHandler BusyIndicatorHandler { get; }
}

/// <summary>
/// Helper to setup an <see cref="IBusyIndicatorViewModel"/> via the <c>SetupViewModel</c> method of the <c>Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.DefaultViewProvider</c>.
/// </summary>
public static class BusyIndicatorViewModelHelper
{
	/// <summary>
	/// Creates a callback for handling view models of type <see cref="IBusyIndicatorViewModel"/>.
	/// </summary>
	/// <param name="busyIndicatorHandlerFactory"> Factory method for obtaining an <see cref="IBusyIndicatorHandler"/> instance. </param>
	[Obsolete("Has been renamed to 'CreateCallback'.")]
	public static Action<object, FrameworkElement> CreateViewModelSetupCallback(Func<IBusyIndicatorHandler> busyIndicatorHandlerFactory)
		=> CreateCallback(busyIndicatorHandlerFactory);

	/// <summary>
	/// Creates a callback for handling view models of type <see cref="IBusyIndicatorViewModel"/>.
	/// </summary>
	/// <param name="busyIndicatorHandlerFactory"> Factory method for obtaining an <see cref="IBusyIndicatorHandler"/> instance. </param>
	public static Action<object, FrameworkElement> CreateCallback(Func<IBusyIndicatorHandler> busyIndicatorHandlerFactory)
	{
		return (viewModel, view) => SetupViewModel(viewModel, view, busyIndicatorHandlerFactory);
	}

	/// <summary>
	/// Callback that initializes the <see cref="IBusyIndicatorViewModel.BusyIndicatorHandler"/> property of an <see cref="IBusyIndicatorHandler"/>.
	/// </summary>
	/// <param name="viewModel"> The view model. </param>
	/// <param name="view"> The view as <see cref="FrameworkElement"/>. </param>
	/// <param name="busyIndicatorHandler"> An <see cref="IBusyIndicatorHandler"/> instance. This instance should be a different one per viewmodel. </param>
	public static void Callback(object viewModel, FrameworkElement view, IBusyIndicatorHandler busyIndicatorHandler)
	{
		SetupViewModel(viewModel, view, busyIndicatorHandler);
	}

	private static void SetupViewModel(object viewModel, FrameworkElement view, Func<IBusyIndicatorHandler> busyIndicatorHandlerFactory)
	{
		if (viewModel is not IBusyIndicatorViewModel) return;
		SetupViewModel(viewModel, view, busyIndicatorHandlerFactory.Invoke());
	}

	private static void SetupViewModel(object viewModel, FrameworkElement view, IBusyIndicatorHandler busyIndicatorHandler)
	{
		if (viewModel is not IBusyIndicatorViewModel busyIndicatorViewModel) return;

		var type = busyIndicatorViewModel.GetType();
		var propertyName = nameof(IBusyIndicatorViewModel.BusyIndicatorHandler);

		// Check if the 'BusyIndicatorHandler' property has an accessible setter.
		var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
		if (propertyInfo?.CanWrite ?? false)
		{
			propertyInfo.SetValue(viewModel, busyIndicatorHandler);
			return;
		}

		// Since the 'BusyIndicatorHandler' property has no setter, its backing field must be manipulated through reflection.
		var fieldInfo = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
		if (fieldInfo != null)
		{
			fieldInfo.SetValue(viewModel, busyIndicatorHandler);
			return;
		}
		
		Trace.WriteLine($"ERROR: Could not inject a '{nameof(IBusyIndicatorHandler)}' into the view model '{type.Name}' as its '{propertyName}' property either has no setter or its backing field could not be found.");
	}
}