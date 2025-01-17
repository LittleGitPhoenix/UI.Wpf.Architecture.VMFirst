﻿#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewModels;

/// <summary>
/// <para> Base class for view-aware view models that - due to this knowledge - have special handlers like <see cref="OnInitialActivate"/>. </para>
/// <para> In order for this view model to properly function, its <see cref="BindView"/> method must be invoked passing in the linked view as soon as possible. </para>
/// <para> If the view resolver of <b>Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider</b> is used to create instances of this class, then <see cref="BindView"/> mustn't be called. The implemented interfaces <see cref="IViewAwareViewModel"/> and <see cref="IActivatedViewModel"/> suffice for setting everything up. </para>
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged, IViewAwareViewModel, IActivatedViewModel
{
	#region Delegates / Events

	/// <inheritdoc />
	public event PropertyChangedEventHandler? PropertyChanged;
	
	/// <summary> Called to raise <see cref="PropertyChanged"/>. </summary>
	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	#endregion

	#region Constants
	#endregion

	#region Fields

	/// <summary> Flag that signals if the current <see cref="View"/> has already been loaded once. </summary>
	private int _loaded;

	#endregion

	#region Properties
	
	/// <summary>
	/// The View attached to this ViewModel.
	/// </summary>
	/// <remarks> ATTENTION: Using this defies common MVVM principles and should be an absolute last resort. </remarks>
	public System.Windows.FrameworkElement? View
	{
		get => _view;
		set
		{
			if (object.ReferenceEquals(value, _view)) return;
			_view = value;
			this.OnPropertyChanged();
		}
	}
	private System.Windows.FrameworkElement? _view;

	/// <summary>
	/// The name associated with this ViewModel. Shown e.g. in a window's title bar, or as a tab's displayName
	/// </summary>
	/// <remarks>
	/// <para> Will be applied to the following view properties (if possible): </para>
	/// <para> - <see cref="System.Windows.Window.Title"/> </para>
	/// <para> - <see cref="System.Windows.Controls.HeaderedContentControl.Header"/> </para>
	/// </remarks>
	public string? DisplayName
	{
		get => _displayName;
		set
		{
			value ??= String.Empty;
			if (value == _displayName) return;
			_displayName = value;
			this.ApplyDisplayName(value);
			this.OnPropertyChanged();
		}
	}
	private string? _displayName;

	#endregion

	#region Enumerations
	#endregion

	#region (De)Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	protected ViewModelBase() { }

	#endregion

	#region Methods
	
	/// <summary>
	/// Callback that must be hooked up to the <see cref="System.Windows.FrameworkElement.Loaded"/> event of the <see cref="System.Windows.FrameworkElement"/> that is linked to this view model in order to make use of the <see cref="OnInitialActivate"/> method.
	/// </summary>
	public virtual void BindView(System.Windows.FrameworkElement frameworkElement)
	{
		// Save this as view.
		this.View = frameworkElement;

		// Add bindings.
		this.AddViewBindings(frameworkElement);
	}
	
	private void AddViewBindings(System.Windows.FrameworkElement frameworkElement)
	{
		if (frameworkElement is null) return;

		// Check if the view has been loaded.
		if (frameworkElement.IsLoaded)
		{
			// YES: Directly execute the loaded callback.
			this.OnLoaded();
		}
		else
		{
			// NO: Hook up to the loaded event.
			frameworkElement.Loaded += this.OnLoaded;
		}

		// Apply the display name.
		this.MergeDisplayName(frameworkElement);

		//? What is the purpose of this?
		//* I cannot remember...but since no actual code is executed, I commented it out.
		//// Traverse the inheritance chain of the new view from most generic to most specific for additional bindings.
		//if (frameworkElement is System.Windows.Controls.Control control)
		//{
		//	// ...

		//	if (control is System.Windows.Controls.ContentControl contentControl)
		//	{
		//		// ...
		//	}
		//}
	}

	private void MergeDisplayName(System.Windows.FrameworkElement frameworkElement)
	{
		if (!String.IsNullOrEmpty(this.DisplayName))
		{
			this.ApplyDisplayName(this.DisplayName);
			return;
		}
		else
		{
			this.DisplayName = frameworkElement switch
			{
				System.Windows.Window window => window.Title,
				System.Windows.Controls.HeaderedContentControl headeredContentControl => headeredContentControl.Header.ToString(),
				_ => this.DisplayName
			};
		}
	}

	private void ApplyDisplayName(string newDisplayName)
	{
		switch (this.View)
		{
			case System.Windows.Window window:
			{
				window.Title = newDisplayName;
				break;
			}
			case System.Windows.Controls.TabItem tabItem:
			{
				tabItem.Header = newDisplayName;
				break;
			}
		}
	}

	private void OnLoaded(object sender, System.Windows.RoutedEventArgs args)
		=> this.OnLoaded();

	/// <summary>
	/// Callback that must be hooked up to the <see cref="System.Windows.FrameworkElement.Loaded"/> event of the <see cref="System.Windows.FrameworkElement"/> that is linked to this view model in order to make use of the <see cref="OnInitialActivate"/> method.
	/// </summary>
	private void OnLoaded()
	{
		// This must be executed only once and never again.
		if (Interlocked.CompareExchange(ref _loaded, 1, 0) == 1) return;
		this.OnInitialActivate();
	}

	/// <summary>
	/// Called only once after the linked view has been loaded.
	/// </summary>
	public virtual void OnInitialActivate() { }
	
	#endregion
}