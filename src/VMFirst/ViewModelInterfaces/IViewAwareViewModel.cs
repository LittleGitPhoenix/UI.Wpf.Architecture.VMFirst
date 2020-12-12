#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces
{
	/// <summary>
	/// Interface for view models that know about their view.
	/// </summary>
	public interface IViewAwareViewModel
	{
		/// <summary>
		/// The View attached to this ViewModel.
		/// </summary>
		/// <remarks> ATTENTION: Using this defies common MVVM principles and should be an absolute last resort. </remarks>
		FrameworkElement View { get; }
	}

	/// <summary>
	/// Helper to setup an <see cref="IViewAwareViewModel"/> via the <c>SetupViewModel</c> method of the <c>Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.DefaultViewProvider</c>.
	/// </summary>
	public static class ViewAwareViewModelHelper
	{
		/// <summary>
		/// Creates a callback for handling view models of type <see cref="IViewAwareViewModel"/>.
		/// </summary>
		[Obsolete("Directly use the 'Callback' method instead of this factory.")]
		public static Action<object, FrameworkElement> CreateViewModelSetupCallback()
			=> Callback;

		/// <summary>
		/// Callback that binds the <paramref name="view"/> to a <paramref name="viewModel"/> of type <see cref="IViewAwareViewModel"/>.
		/// </summary>
		/// <param name="viewModel"> The view model. </param>
		/// <param name="view"> The view as <see cref="FrameworkElement"/>. </param>
		public static void Callback(object viewModel, FrameworkElement view)
		{
			if (!(viewModel is IViewAwareViewModel viewAwareViewModel)) return;

			var type = viewAwareViewModel.GetType();
			var propertyName = nameof(IViewAwareViewModel.View);

			// Check if the 'View' property has an accessible setter.
			var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (propertyInfo?.CanWrite ?? false)
			{
				propertyInfo.SetValue(viewModel, view);
				return;
			}

			// Since the 'View' property has no setter, its backing field must be manipulated through reflection.
			var fieldInfo = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(viewModel, view);
				return;
			}

			Trace.WriteLine($"ERROR: Could not inject the view '{view?.GetType().Name ?? "[NULL]"}' into the view model '{type.Name}' as its '{propertyName}' property either has no setter or its backing field could not be found.");
		}
	}
}