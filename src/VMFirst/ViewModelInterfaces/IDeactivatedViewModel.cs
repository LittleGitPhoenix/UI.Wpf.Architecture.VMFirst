#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces
{
	/// <summary>
	/// Interface for view models that have a callback that is invoked when their <see cref="Window"/> is about to close.
	/// </summary>
	/// <remarks> This will only work for views that are <see cref="Window"/>s as only those provide a <see cref="Window.Closing"/> event. </remarks>
	public interface IDeActivatedViewModel
	{
		/// <summary>
		/// Called only once when the linked view is closing.
		/// </summary>
		/// <param name="args"> <see cref="CancelEventArgs"/> that can be used to stop closing. </param>
		void OnClosing(CancelEventArgs args);
	}

	/// <summary>
	/// Helper to setup an <see cref="IDeActivatedViewModel"/> via the <c>SetupViewModel</c> method of the <c>Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider.DefaultViewProvider</c>.
	/// </summary>
	public static class DeactivatedViewModelHelper
	{
		/// <summary>
		/// Creates a callback for handling view models of type <see cref="IDeActivatedViewModel"/>.
		/// </summary>
		[Obsolete("Directly use the 'Callback' method instead of this factory.")]
		public static Action<object, FrameworkElement> CreateViewModelSetupCallback()
			=> Callback;

		/// <summary>
		/// Callback that hooks up <see cref="IDeActivatedViewModel.OnClosing"/> to the <paramref name="view"/>s <see cref="Window.Closing"/> event.
		/// </summary>
		/// <param name="viewModel"> The view model. </param>
		/// <param name="view"> The view as <see cref="FrameworkElement"/>. </param>
		public static void Callback(object viewModel, FrameworkElement view)
		{
			if (!(viewModel is IDeActivatedViewModel deactivatedViewModel)) return;
			if (!(view is Window window)) return;
			HandleLoading(window, deactivatedViewModel);
		}

		private static void HandleLoading(Window window, IDeActivatedViewModel viewModel)
		{
			if (window is null) return;
			if (viewModel is null) return;
			

			void ClosingHandler(object sender, CancelEventArgs args)
			{
				viewModel?.OnClosing(args);
			}

			var closed = 0;

			void ClosedHandler(object sender, EventArgs args)
			{
				if (window is null) return;
				if (viewModel is null) return;

				// This must be executed only once and never again.
				if (Interlocked.CompareExchange(ref closed, 1, 0) == 0)
				{
					window.Closing -= ClosingHandler;
					window.Closed -= ClosedHandler;

					window = null;
					viewModel = null;
				}
			}

			// NO: Hook up to the closing event.
			window.Closing += ClosingHandler;
			window.Closed += ClosedHandler;
		}
	}
}