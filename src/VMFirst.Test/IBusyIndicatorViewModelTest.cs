using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.Classes;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewModelInterfaces;

namespace VMFirst.Test
{
	[TestFixture]
	public class IBusyIndicatorViewModelTest
	{
		class BusyIndicatorViewModelWithSetableProperty : IBusyIndicatorViewModel
		{
			/// <inheritdoc />
			public IBusyIndicatorHandler BusyIndicatorHandler { get; set; }
		}

		class BusyIndicatorViewModelWithAutoProperty : IBusyIndicatorViewModel
		{
			/// <inheritdoc />
			public IBusyIndicatorHandler BusyIndicatorHandler { get; }
		}

		[Test]
		public void BusyIndicatorViewModelHelper_Direct_Property_Injection_Succeeds()
		{
			var busyIndicatorHandlerMock = new Mock<IBusyIndicatorHandler>();
			var viewModel = new BusyIndicatorViewModelWithSetableProperty();
			var setupCallback = BusyIndicatorViewModelHelper.CreateViewModelSetupCallback(() => busyIndicatorHandlerMock.Object);
			
			setupCallback.Invoke(viewModel, null);

			Assert.That(viewModel.BusyIndicatorHandler, Is.Not.Null);
			Assert.That(viewModel.BusyIndicatorHandler, Is.AssignableTo<IBusyIndicatorHandler>());
		}

		[Test]
		public void BusyIndicatorViewModelHelper_Indirect_Property_Injection_Succeeds()
		{
			var busyIndicatorHandlerMock = new Mock<IBusyIndicatorHandler>();
			var viewModel = new BusyIndicatorViewModelWithAutoProperty();
			var setupCallback = BusyIndicatorViewModelHelper.CreateViewModelSetupCallback(() => busyIndicatorHandlerMock.Object);

			setupCallback.Invoke(viewModel, null);

			Assert.That(viewModel.BusyIndicatorHandler, Is.Not.Null);
			Assert.That(viewModel.BusyIndicatorHandler, Is.AssignableTo<IBusyIndicatorHandler>());
		}

		[Test]
		public void BusyIndicatorViewModelHelper_Property_Injection_Fails()
		{
			var busyIndicatorHandlerMock = new Mock<IBusyIndicatorHandler>();
			var viewModel = new Mock<IBusyIndicatorViewModel>().Object; //! Mocked object does not have a setter or a backing field and this is why the test fails.
			var setupCallback = BusyIndicatorViewModelHelper.CreateViewModelSetupCallback(() => busyIndicatorHandlerMock.Object);

			setupCallback.Invoke(viewModel, null);

			Assert.That(viewModel.BusyIndicatorHandler, Is.Null);
		}
	}
}