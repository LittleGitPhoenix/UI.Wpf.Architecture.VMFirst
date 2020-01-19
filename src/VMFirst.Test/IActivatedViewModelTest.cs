using System.Threading;
using System.Windows;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test
{
	[TestFixture]
	public class IActivatedViewModelTest
	{
		[Test]
		[Apartment(ApartmentState.STA)]
		[Ignore("The 'IsLoaded' property of a 'FrameworkElement' cannot be manipulated. Since this is a requirement of this test, it is currently disabled.")]
		public void ActivatedViewModelHelper_Callback_Is_Invoked_Even_When_View_Is_Already_Loaded()
		{
			var viewMock = new Mock<FrameworkElement>();
			viewMock.SetupGet(element => element.IsLoaded).Returns(true);
			var view = viewMock.Object;

			var counter = 0;
			var viewModelMock = new Mock<IActivatedViewModel>();
			viewModelMock.Setup(model => model.OnInitialActivate()).Callback(() => counter++).Verifiable();
			var viewModel = viewModelMock.Object;

			var setupCallback = ActivatedViewModelHelper.CreateViewModelSetupCallback();

			setupCallback.Invoke(viewModel, view);
			Assert.That(counter, Is.EqualTo(1));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void ActivatedViewModelHelper_Callback_Is_Invoked_Only_Once()
		{
			var view = new Window();

			var counter = 0;
			var viewModelMock = new Mock<IActivatedViewModel>();
			viewModelMock.Setup(model => model.OnInitialActivate()).Callback(() => counter++).Verifiable();
			var viewModel = viewModelMock.Object;

			var setupCallback = ActivatedViewModelHelper.CreateViewModelSetupCallback();
			
			setupCallback.Invoke(viewModel, view);

			view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
			Assert.That(counter, Is.EqualTo(1));
			view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
			Assert.That(counter, Is.EqualTo(1));
		}
	}
}