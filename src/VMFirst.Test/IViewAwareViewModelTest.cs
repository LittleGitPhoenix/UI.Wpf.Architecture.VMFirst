using System;
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
			// Arrange
			var view = new Window();
			var viewModel = new ViewAwareViewModelWithSetableProperty();
			Action<object, FrameworkElement> setupCallback = ViewAwareViewModelHelper.Callback;

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
			Action<object, FrameworkElement> setupCallback = ViewAwareViewModelHelper.Callback;

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
			Action<object, FrameworkElement> setupCallback = ViewAwareViewModelHelper.Callback;

			// Act
			setupCallback.Invoke(viewModel, null);

			// Assert
			Assert.That(viewModel.View, Is.Null);
		}
	}
}