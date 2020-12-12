#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Windows;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider;
using Stylet;
using Stylet.Xaml;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Stylet
{
	/// <summary>
	/// Custom <see cref="IViewManager"/>.
	/// </summary>
	public class StyletViewManager : IViewManager
	{
		#region Delegates / Events
		#endregion

		#region Constants
		#endregion

		#region Fields

		private readonly IViewProvider _viewProvider;

		#endregion

		#region Properties
		#endregion

		#region Enumerations
		#endregion

		#region (De)Constructors

		/// <summary>
		/// Constructor that uses a new <see cref="DefaultViewProvider"/> instance.
		/// </summary>
		public StyletViewManager() : this(new DefaultViewProvider()) { }
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewProvider"> An <see cref="IViewProvider"/> used for resolving the views for given view models. </param>
		public StyletViewManager(IViewProvider viewProvider)
		{
			// Save parameters.

			// Initialize fields.
			_viewProvider = viewProvider ?? new DefaultViewProvider();
			_viewProvider.ViewLoaded += this.NewViewLoaded;
		}

		#endregion

		#region Methods

		/// <inheritdoc />
		public void OnModelChanged(DependencyObject targetLocation, object oldValue, object newValue)
		{
			if (oldValue == newValue) return;

			var view = this.CreateAndBindViewForModelIfNecessary(newValue);
			if (view is Window)
			{
				var ex = new StyletInvalidViewTypeException($"s:View.Model=\"...\" tried to show a View of type '{view.GetType().Name}', but that View derives from the Window class. " + "Make sure any Views you display using s:View.Model=\"...\" do not derive from Window (use UserControl or similar)");
				throw ex;
			}
			
			View.SetContentProperty(targetLocation, view);
		}

		/// <inheritdoc />
		public UIElement CreateAndBindViewForModelIfNecessary(object model)
		{
			if (model is null) return null;

			// Check if the viewmodel has already a valid bound view.
			UIElement view;
			if (model is IViewAwareViewModel viewAwareModel && viewAwareModel.View != null)
			{
				view = viewAwareModel.View;
			}
			else if (model is IViewAware styletViewAwareModel && styletViewAwareModel.View != null)
			{
				view = styletViewAwareModel.View;
			}
			else
			{
				// If the view couldn't be obtained create a new one.
				view = this.CreateViewForModel(model);
			}
			return view;
		}

		/// <inheritdoc />
		public UIElement CreateViewForModel(object model)
		{
			return _viewProvider.GetViewInstance(model);
		}
		
		/// <summary>
		/// This is called by <see cref="_viewProvider"/> every time a new view has been loaded.
		/// </summary>
		internal virtual void NewViewLoaded(object sender, ViewLoadedEventArgs args)
		{
			this.BindViewToModel(args.View, args.ViewModel);
		}

		/// <inheritdoc />
		public void BindViewToModel(UIElement view, object model)
		{
			View.SetActionTarget(view, model);
		}

		#endregion
	}
}