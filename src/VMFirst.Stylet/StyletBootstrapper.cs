#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using Autofac;
using Stylet;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Stylet
{

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
		#endregion

		#region Properties

		/// <summary> <see cref="Autofac"/>s <see cref="IContainer"/>. </summary>
		protected IContainer Container { get; private set; }

		/// <summary> The view model to display with application startup. </summary>
		protected virtual TRootViewModel RootViewModel => _rootViewModel ?? (_rootViewModel = this.Container.Resolve<TRootViewModel>());
		private TRootViewModel _rootViewModel;

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

		/// <inheritdoc />
		protected override void ConfigureBootstrapper()
		{
			var builder = new ContainerBuilder();
			this.DefaultConfigureIoC(builder);
			this.ConfigureIoC(builder);
			this.Container = builder.Build();
		}

		/// <summary>
		/// Carries out default configuration of the IoC container. Override if you don't want to do this.
		/// </summary>
		/// <remarks> Normally this should not be overridden. For custom configuration of the IOC use <see cref="ConfigureIoC"/> instead. </remarks>
		protected virtual void DefaultConfigureIoC(ContainerBuilder builder)
		{
			builder.RegisterType(typeof(TViewManager)).As<IViewManager>().SingleInstance();
			builder.RegisterInstance<IWindowManagerConfig>(this).ExternallyOwned();
			builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
			builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
			builder.RegisterType<MessageBoxViewModel>().As<IMessageBoxViewModel>().ExternallyOwned(); // Not singleton!

			//! Do not register any types automatically anymore. This can be done in 'ConfigureIoC'.
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

		/// <summary>
		/// Override to add your own types to the IoC container.
		/// </summary>
		protected virtual void ConfigureIoC(ContainerBuilder builder) { }

		/// <inheritdoc />
		public override object GetInstance(Type type)
		{
			return this.Container.Resolve(type);
		}

		/// <inheritdoc />
		protected override void Launch()
		{
			base.DisplayRootView(this.RootViewModel);
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			base.Dispose();
			ScreenExtensions.TryDispose(_rootViewModel);
			this.Container?.Dispose();
		}

		#endregion
	}
}