#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion

using System.Reflection;
using System.Windows;
using Autofac;
using Stylet;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Stylet;

/// <summary>
/// <c>Stylet</c> bootstrapper utilizing <see cref="Autofac"/> and the custom <see cref="StyletViewManager"/>.
/// </summary>
/// <typeparam name="TRootViewModel"> The type of the root view model. </typeparam>
public class StyletBootstrapper<TRootViewModel> : StyletBootstrapper<StyletViewManager, TRootViewModel>
	where TRootViewModel : class
{ }

/// <summary>
/// <c>Stylet</c> bootstrapper utilizing <see cref="Autofac"/>.
/// </summary>
/// <typeparam name="TViewManager"> The <see cref="IViewManager"/> type. </typeparam>
/// <typeparam name="TRootViewModel"> The type of the root view model. </typeparam>
public class StyletBootstrapper<TViewManager, TRootViewModel> : BootstrapperBase
	where TViewManager : IViewManager
	where TRootViewModel : class
{
	#region Delegates / Events
	#endregion

	#region Constants
	#endregion

	#region Fields

	/// <summary> Exception that should be thrown if <see cref="Container"/> is null when accessing it after it should have been created in <see cref="ConfigureBootstrapper"/>. </summary>
	protected static BootstrapperException MissingIocContainerException = new BootstrapperException("The internal ioc container is null.");

	#endregion

	#region Properties

	/// <summary> <see cref="Autofac"/>s <see cref="Autofac.IContainer"/>. </summary>
	private IContainer? Container { get; set; }

	/// <summary> The view model to display with application startup. </summary>
	protected virtual TRootViewModel RootViewModel => _rootViewModel ??= this.Container?.Resolve<TRootViewModel>() ?? throw MissingIocContainerException;
	private TRootViewModel? _rootViewModel;

	#endregion

	#region Enumerations
	#endregion

	#region (De)Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	public StyletBootstrapper()
	{
		// Initialize fields.
		this.Container = null;
		_rootViewModel = null;
	}

	#endregion

	#region Methods

	/// <inheritdoc cref="BootstrapperBase.Start"/>
	/// <remarks> This method "hides" <see cref="BootstrapperBase.Start"/>. </remarks>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public override void Start(string[] args) => base.Start(args);

	/// <inheritdoc cref="BootstrapperBase.ConfigureBootstrapper"/>
	/// <remarks> This method "hides" <see cref="BootstrapperBase.ConfigureBootstrapper"/> so it cannot be accidentally overridden. </remarks>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	protected override void ConfigureBootstrapper()
	{
		var builder = new ContainerBuilder();
		this.RegisterDefaultServices(builder);
#pragma warning disable CS0618
		this.ConfigureIoC(builder);
#pragma warning restore CS0618
		this.RegisterServices(builder);
		this.Container = builder.Build();
	}

	/// <summary>
	/// Carries out necessary default configuration of the IoC container.
	/// </summary>
	private void RegisterDefaultServices(ContainerBuilder builder)
	{
		builder.RegisterType(typeof(TViewManager)).As<IViewManager>().SingleInstance();
		builder.RegisterInstance<IWindowManagerConfig>(this).ExternallyOwned();
		builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
		builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
		builder.RegisterType<MessageBoxViewModel>().As<IMessageBoxViewModel>().ExternallyOwned(); // Not singleton!

		//! Do not register any types automatically anymore. This can be done in 'RegisterServices'.
		//// Register all public types of the assembly that implements this class. If this class is used directly, then do nothing. 
		//var assembly = this.GetType().Assembly;
		//if (assembly != System.Reflection.Assembly.GetExecutingAssembly())
		//{
		//	builder
		//		.RegisterAssemblyTypes(assembly)
		//		.PublicOnly()
		//		//.Where(type => !type.Name.Contains("ProcessedByFody"))
		//		.ExternallyOwned()
		//		;
		//}
	}

	/// <inheritdoc cref="RegisterServices"/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	[Obsolete($"Please use '{nameof(RegisterServices)}' instead.")]
	protected virtual void ConfigureIoC(ContainerBuilder builder) { }

	/// <summary>
	/// Override to register your own types with the IoC container.
	/// </summary>
	protected virtual void RegisterServices(ContainerBuilder builder) { }

	/// <inheritdoc />
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	protected override void Launch()
	{
		if (this.Container is null) throw MissingIocContainerException;

		var applicationName = GetApplicationName() ?? null;
		var assemblyVersion = GetAssemblyVersion() ?? new Version();
		var fileVersion = GetFileVersion() ?? assemblyVersion;
		var informationalVersion = GetInformationalVersion() ?? fileVersion.ToString();

		this.BeforeLaunch(this.Container, applicationName, assemblyVersion, fileVersion, informationalVersion);
		base.DisplayRootView(this.RootViewModel);
	}

	/// <inheritdoc cref="BootstrapperBase.Configure"/>
	/// <remarks> This method "hides" <see cref="BootstrapperBase.Configure"/>. This is necessary, as the ioc container no longer is accessible from outside this class. </remarks>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	[Obsolete($"Please use '{nameof(BeforeLaunch)}' instead.")]
	protected new void Configure() => base.Configure();
	
	/// <summary>
	/// Called before <typeparamref name="TRootViewModel"/> will be displayed.
	/// </summary>
	/// <param name="container"> The ioc container. </param>
	/// <param name="applicationName"> The name of the running application. </param>
	/// <param name="assemblyVersion"> The assembly version of the running executable. This is specified in the project file as 'AssemblyVersion'. </param>
	/// <param name="fileVersion"> The file version of the running executable. This is specified in the project file as 'FileVersion'. </param>
	/// <param name="informationalVersion"> The informational version of the running executable. This is specified in the project file as 'Version'. </param>
	protected virtual void BeforeLaunch(IContainer container, string? applicationName, Version assemblyVersion, Version fileVersion, string informationalVersion) { }
	
	/// <inheritdoc />
	[Obsolete($"Please use '{nameof(AfterLaunch)}' instead.")]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnLaunch()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		if (this.Container is null) throw MissingIocContainerException;
		this.AfterLaunch(this.Container);
		base.OnLaunch();
	}

	/// <summary>
	/// Called after <typeparamref name="TRootViewModel"/> has been displayed.
	/// </summary>
	/// <param name="container"> The ioc container. </param>
	/// <remarks> This is just a forwarded method of <see cref="OnLaunch"/> with a better name. </remarks>
	protected virtual void AfterLaunch(IContainer container) { }
	
	/// <inheritdoc />
	[Obsolete($"Please use '{nameof(OnClosing)}' instead.")]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnExit(ExitEventArgs args)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		if (this.Container is null) throw MissingIocContainerException;
		this.OnClosing(this.Container, args);
		base.OnExit(args);
	}

	/// <summary>
	/// Called before the application is closing.
	/// </summary>
	/// <param name="container"> The ioc container. </param>
	/// <param name="args"> The <see cref="ExitEventArgs"/>. </param>
	/// <remarks> This is just a forwarded method of <see cref="OnExit"/> with a better name. </remarks>
	protected virtual void OnClosing(IContainer container, ExitEventArgs args) { }

	#region Helper

	/// <inheritdoc />
	public override object GetInstance(Type type)
	{
		return this.Container?.Resolve(type) ?? throw MissingIocContainerException;
	}

	/// <summary>
	/// Gets the name of the running application, which is specified in the project file as 'AssemblyName'.
	/// </summary>
	/// <returns> The application name or null. </returns>
	private static string? GetApplicationName()
	{
		var entryAssembly = Assembly.GetEntryAssembly();
		var applicationName = entryAssembly?.GetName().Name;
		return applicationName;
	}

	/// <summary>
	/// Gets the assembly version of the running executable, which is specified in the project file as 'AssemblyVersion'.
	/// </summary>
	/// <returns> The assembly <see cref="Version"/> or null. </returns>
	private static Version? GetAssemblyVersion()
	{
		var entryAssembly = Assembly.GetEntryAssembly();
		var assemblyVersion = entryAssembly?.GetName().Version ?? new Version();
		return assemblyVersion;
	}

	/// <summary>
	/// Gets the file version of the running executable, which is specified in the project file as 'FileVersion'.
	/// </summary>
	/// <returns> The file <see cref="Version"/> or null. </returns>
	private static Version? GetFileVersion()
	{
		var entryAssembly = Assembly.GetEntryAssembly();
		var fileVersionString = entryAssembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
		if (String.IsNullOrWhiteSpace(fileVersionString)) return null;
		try
		{
			var fileVersion = new Version(fileVersionString);
			return fileVersion;
		}
		catch (Exception)
		{
			return null;
		}
	}

	/// <summary>
	/// Gets the informational version of the running executable, which is specified in the project file as 'InformationalVersion'.
	/// </summary>
	/// <returns> The informational <see cref="Version"/> or null. </returns>
	private static string? GetInformationalVersion()
	{
		var entryAssembly = Assembly.GetEntryAssembly();
		var informationalVersion = entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		return String.IsNullOrWhiteSpace(informationalVersion) ? null : informationalVersion;
	}

	#endregion

	/// <inheritdoc />
	public override void Dispose()
	{
		base.Dispose();
		ScreenExtensions.TryDispose(_rootViewModel);
		this.Container?.Dispose();
	}

	#endregion
}