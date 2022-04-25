using System.ComponentModel;
using System.Windows;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test;

[TestFixture]
public class IDeactivatedViewModelTest
{
	#region Setup

#pragma warning disable 8618 // → Always initialized in the 'Setup' method before a test is run.
	private IFixture _fixture;
#pragma warning restore 8618

	[OneTimeSetUp]
	public void BeforeAllTests() { }

	[SetUp]
	public void BeforeEachTest()
	{
		_fixture = new Fixture().Customize(new AutoMoqCustomization());
	}

	[TearDown]
	public void AfterEachTest() { }

	[OneTimeTearDown]
	public void AfterAllTest() { }

	#endregion

	#region Tests

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

	#endregion
}