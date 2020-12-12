#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Threading;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces
{
	/// <summary>
	/// Interface for view models that have a callback that is invoked when their view has been loaded completely.
	/// </summary>
	public interface IActivatedViewModel
	{
		/// <summary>
		/// Called only once after the linked view has been loaded and is ready to use.
		/// </summary>
		void OnInitialActivate();
	}

	/// <summary>
	/// Helper to setup an <see cref="IActivatedViewModel"/> via the <c>SetupViewModel</c> method of the <c>Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.DefaultViewProvider</c>.
	/// </summary>
	public static class ActivatedViewModelHelper
	{
		/// <summary>
		/// Creates a callback for handling view models of type <see cref="IActivatedViewModel"/>.
		/// </summary>
		[Obsolete("Directly use the 'Callback' method instead of this factory.")]
		public static Action<object, FrameworkElement> CreateViewModelSetupCallback()
			=> Callback;

		/// <summary>
		/// Callback that hooks up <see cref="IActivatedViewModel.OnInitialActivate"/> to the <paramref name="view"/>s <see cref="FrameworkElement.Loaded"/> event.
		/// </summary>
		/// <param name="viewModel"> The view model. </param>
		/// <param name="view"> The view as <see cref="FrameworkElement"/>. </param>
		public static void Callback(object viewModel, FrameworkElement view)
		{
			if (!(viewModel is IActivatedViewModel activatedViewModel)) return;
			HandleLoading(view, activatedViewModel);
		}

		private static void HandleLoading(FrameworkElement view, IActivatedViewModel viewModel)
		{
			if (view is null) return;
			if (viewModel is null) return;

			void OnLoaded()
			{
				// Execute the initial activated method in the view model.
				viewModel.OnInitialActivate();
			}

			// Check if the view has been loaded already.
			if (view.IsLoaded)
			{
				// YES: Directly execute the loaded callback.
				OnLoaded();
			}
			else
			{
				var loaded = 0;

				void LoadedHandler(object sender, RoutedEventArgs args)
				{
					if (view is null) return;
					if (viewModel is null) return;

					view.Loaded -= LoadedHandler;

					// This must be executed only once and never again.
					if (Interlocked.CompareExchange(ref loaded, 1, 0) == 0)
					{
						OnLoaded();
					}

					view = null;
					viewModel = null;
				}

				// NO: Hook up to the loaded event.
				view.Loaded += LoadedHandler;
			}
		}
	}
}