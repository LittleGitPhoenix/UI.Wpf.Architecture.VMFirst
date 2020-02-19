#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Phoenix.UI.Wpf.Architecture.VMFirst.Classes;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces
{
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
		/// Creates a custom setup method for view models of type <see cref="IBusyIndicatorViewModel"/>.
		/// </summary>
		/// <param name="busyIndicatorHandlerFactory"> Factory method for obtaining an <see cref="IBusyIndicatorHandler"/> instance. </param>
		public static Action<object, FrameworkElement> CreateViewModelSetupCallback(Func<IBusyIndicatorHandler> busyIndicatorHandlerFactory)
		{
			return (viewModel, view) => SetupViewModel(viewModel, view, busyIndicatorHandlerFactory);
		}

		private static void SetupViewModel(object viewModel, FrameworkElement view, Func<IBusyIndicatorHandler> busyIndicatorHandlerFactory)
		{
			if (!(viewModel is IBusyIndicatorViewModel busyIndicatorViewModel)) return;

			var type = busyIndicatorViewModel.GetType();
			var propertyName = nameof(IBusyIndicatorViewModel.BusyIndicatorHandler);

			// Check if the 'BusyIndicatorHandler' property has an accessible setter.
			var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (propertyInfo?.CanWrite ?? false)
			{
				propertyInfo.SetValue(viewModel, busyIndicatorHandlerFactory.Invoke());
				return;
			}

			// Since the 'BusyIndicatorHandler' property has no setter, its backing field must be manipulated through reflection.
			var fieldInfo = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(viewModel, busyIndicatorHandlerFactory.Invoke());
				return;
			}
			
			Trace.WriteLine($"ERROR: Could not inject a '{nameof(IBusyIndicatorHandler)}' into the view model '{type.Name}' as its '{propertyName}' property either has no setter or its backing field could not be found.");
		}
	}
}