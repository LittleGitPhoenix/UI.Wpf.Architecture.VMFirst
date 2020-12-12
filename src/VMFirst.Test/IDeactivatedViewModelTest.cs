using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test
{
	[TestFixture]
	public class IDeactivatedViewModelTest
	{
		[Test]
		[Apartment(ApartmentState.STA)]
		public void DeactivatedViewModelHelper_Callback_Is_Invoked_Until_Window_Is_Really_Closed()
		{
			// Arrange
			var view = new Window();

			var counter = 0;
			var viewModelMock = new Mock<IDeActivatedViewModel>();
			viewModelMock
				.Setup(model => model.OnClosing(It.IsAny<CancelEventArgs>()))
				.Callback<CancelEventArgs>
				(
					args =>
					{
						counter++;
						if (counter < 3) args.Cancel = true; //! Allow closing at the 3rd attempt.
					}
				)
				.Verifiable()
				;
			var viewModel = viewModelMock.Object;

			Action<object, FrameworkElement> setupCallback = DeactivatedViewModelHelper.Callback;
			
			// Act
			setupCallback.Invoke(viewModel, view);

			// Assert
			view.Close();
			viewModelMock.Verify(model => model.OnClosing(It.IsAny<CancelEventArgs>()), Times.Exactly(1));
			view.Close();
			viewModelMock.Verify(model => model.OnClosing(It.IsAny<CancelEventArgs>()), Times.Exactly(2));
			view.Close();
			viewModelMock.Verify(model => model.OnClosing(It.IsAny<CancelEventArgs>()), Times.Exactly(3));
			view.Close();
			viewModelMock.Verify(model => model.OnClosing(It.IsAny<CancelEventArgs>()), Times.Exactly(3));
		}
	}
}