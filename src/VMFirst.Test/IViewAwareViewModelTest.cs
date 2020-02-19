using System.Threading;
using System.Windows;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test
{
	[TestFixture]
	public class IViewAwareViewModelTest
	{
		class ViewAwareViewModelWithSetableProperty : IViewAwareViewModel
		{
			/// <inheritdoc />
			public FrameworkElement View { get; set; }
		}

		class ViewAwareViewModelWithAutoProperty : IViewAwareViewModel
		{
			/// <inheritdoc />
			public FrameworkElement View { get; }
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void ViewAwareViewModelHelper_Direct_Property_Injection_Succeeds()
		{
			var view = new Window();
			var viewModel = new ViewAwareViewModelWithSetableProperty();
			var setupCallback = ViewAwareViewModelHelper.CreateViewModelSetupCallback();
			
			setupCallback.Invoke(viewModel, view);

			Assert.That(viewModel.View, Is.EqualTo(view));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void ViewAwareViewModelHelper_Indirect_Property_Injection_Succeeds()
		{
			var view = new Window();
			var viewModel = new ViewAwareViewModelWithAutoProperty();
			var setupCallback = ViewAwareViewModelHelper.CreateViewModelSetupCallback();

			setupCallback.Invoke(viewModel, view);

			Assert.That(viewModel.View, Is.EqualTo(view));
		}
		[Test]
		public void ViewAwareViewModelHelper_Property_Injection_Fails()
		{
			var viewModel = new Mock<IViewAwareViewModel>().Object; // Mocked object does not have a setter or a backing field.
			var setupCallback = ViewAwareViewModelHelper.CreateViewModelSetupCallback();

			setupCallback.Invoke(viewModel, null);

			Assert.That(viewModel.View, Is.Null);
		}
	}
}