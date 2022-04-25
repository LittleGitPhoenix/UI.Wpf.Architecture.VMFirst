using System.Windows;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test;

[TestFixture]
public class IActivatedViewModelTest
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
	[Ignore("The 'IsLoaded' property of a 'FrameworkElement' cannot be manipulated and it is also not set to True after manually raising the 'Loaded' event. Since this is a requirement of this test, it is currently disabled.")]
	public void ActivatedViewModelHelper_Callback_Is_Invoked_Even_When_View_Is_Already_Loaded()
	{
		// Arrange
		var viewMock = new Mock<FrameworkElement>();
		viewMock.SetupGet(element => element.IsLoaded).Returns(true); //! Doesn't work.
		var view = viewMock.Object;
		view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent)); //! Doesn't work either.
		
		var viewModelMock = new Mock<IActivatedViewModel>();
		viewModelMock.Setup(model => model.OnInitialActivate()).Verifiable();
		var viewModel = viewModelMock.Object;
		Action<object, FrameworkElement> setupCallback = ActivatedViewModelHelper.Callback;

		// Act
		setupCallback.Invoke(viewModel, view);

		// Assert
		viewModelMock.Verify(model => model.OnInitialActivate(), Times.Once);
	}

	[Test]
	[Apartment(ApartmentState.STA)]
	public void ActivatedViewModelHelper_Callback_Is_Invoked_Only_Once()
	{
		// Arrange
		var view = new Window();
		var viewModelMock = new Mock<IActivatedViewModel>();
		viewModelMock.Setup(model => model.OnInitialActivate()).Verifiable();
		var viewModel = viewModelMock.Object;
		Action<object, FrameworkElement> setupCallback = ActivatedViewModelHelper.Callback;
		
		// Act
		setupCallback.Invoke(viewModel, view);
		view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
		view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

		// Assert
		viewModelMock.Verify(model => model.OnInitialActivate(), Times.Once);
	}

	#endregion
}