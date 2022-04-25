using System.Windows;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test;

[TestFixture]
public class IViewAwareViewModelTest
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

	class ViewAwareViewModelWithSetableProperty : IViewAwareViewModel
	{
		/// <inheritdoc />
		public FrameworkElement? View { get; set; }
	}

	class ViewAwareViewModelWithAutoProperty : IViewAwareViewModel
	{
		/// <inheritdoc />
		public FrameworkElement? View { get; }
	}

	[Test]
	[Apartment(ApartmentState.STA)]
	public void ViewAwareViewModelHelper_Direct_Property_Injection_Succeeds()
	{
		// Arrange
		var view = new Window();
		var viewModel = new ViewAwareViewModelWithSetableProperty();
		var setupCallback = ViewAwareViewModelHelper.Callback;

		// Act
		setupCallback.Invoke(viewModel, view);

		// Assert
		Assert.That(viewModel.View, Is.EqualTo(view));
	}

	[Test]
	[Apartment(ApartmentState.STA)]
	public void ViewAwareViewModelHelper_Indirect_Property_Injection_Succeeds()
	{
		// Arrange
		var view = new Window();
		var viewModel = new ViewAwareViewModelWithAutoProperty();
		var setupCallback = ViewAwareViewModelHelper.Callback;

		// Act
		setupCallback.Invoke(viewModel, view);

		// Assert
		Assert.That(viewModel.View, Is.EqualTo(view));
	}

	[Test]
	public void ViewAwareViewModelHelper_Property_Injection_Fails()
	{
		// Arrange
		var viewModel = new Mock<IViewAwareViewModel>().Object; // Mocked object does not have a setter or a backing field.
		var setupCallback = ViewAwareViewModelHelper.Callback;

		// Act
		setupCallback.Invoke(viewModel, null);

		// Assert
		Assert.That(viewModel.View, Is.Null);
	}

	#endregion
}