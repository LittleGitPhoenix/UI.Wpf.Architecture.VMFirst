#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Classes
{
	/// <summary>
	/// Workload handler with bindable properties for views to signal ongoing work.
	/// </summary>
	public interface IBusyIndicatorHandler : INotifyPropertyChanged
	{
		/// <summary> Flag if the view model is currently busy. </summary>
		bool IsBusy { get; }

		/// <summary> A message that will be displayed along with the busy indicator. </summary>
		string BusyMessage { get; }

		/// <summary>
		/// <para> Defines the <see cref="TimeSpan"/> that has to pass until the first busy message along with the indicator will be shown. This can be used to prevent a flickering busy message for very fast tasks. </para>
		/// <para> Set this to <see cref="TimeSpan.Zero"/> to completely disable delayed showing of the busy indicator. </para>
		/// </summary>
		TimeSpan InitialDelay { get; set; }

		/*
		 * #######################################################################################
		 * # Those methods will run in the calling thread and therefore block further execution. #
		 * #######################################################################################
		 */
		#region Synchronous

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="method"/>.
		/// </summary>
		/// <param name="method"> The <see cref="Action"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="method"/> has finished. </param>
		void Execute(Action method, Action doneCallback = null);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="method"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="method"/> is being executed. </param>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="method"/>s execution. </param>
		/// <param name="method"> The <see cref="Action"/> to execute. </param>
		void Execute(string message, Expression<Func<bool>> toggle, Action method);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="method"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="method"/> is being executed. </param>
		/// <param name="method"> The <see cref="Action"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="method"/> has finished. </param>
		void Execute(string message, Action method, Action doneCallback = null);

		#endregion

		/*
		 * ############################################################################################
		 * # Those methods will await the passed function and therefore not block the calling thread. #
		 * ############################################################################################
		 */
		#region Asynchronous

		#region Methods
		
		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/>.
		/// </summary>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		Task ExecuteAsync(Func<Task> asyncMethod, Action doneCallback = null);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="asyncMethod"/>s execution. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{Task}"/> to execute. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteAsync(string message, Expression<Func<bool>> toggle, Func<Task> asyncMethod);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteAsync(string message, Func<Task> asyncMethod, Action doneCallback = null);

		#endregion

		#region Functions

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncFunction"/>.
		/// </summary>
		/// <typeparam name="T"> The type that the <paramref name="asyncFunction"/> will return. </typeparam>
		/// <param name="asyncFunction"> An asynchronous function that will be executed and returns a <see cref="Task{T}"/>. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncFunction"/> has finished. </param>
		/// <returns> An awaitable <see cref="Task{T}"/>. </returns>
		Task<T> ExecuteAsync<T>(Func<Task<T>> asyncFunction, Action doneCallback = null);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncFunction"/>.
		/// </summary>
		/// <typeparam name="T"> The type that the <paramref name="asyncFunction"/> will return. </typeparam>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="asyncMethod"/>s execution. </param>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncFunction"/> is being executed. </param>
		/// <param name="asyncFunction"> An asynchronous function that will be executed and returns a <see cref="Task{T}"/>. </param>
		/// <returns> An awaitable <see cref="Task{T}"/>. </returns>
		Task<T> ExecuteAsync<T>(string message, Expression<Func<bool>> toggle, Func<Task<T>> asyncFunction);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncFunction"/>.
		/// </summary>
		/// <typeparam name="T"> The type that the <paramref name="asyncFunction"/> will return. </typeparam>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncFunction"/> is being executed. </param>
		/// <param name="asyncFunction"> An asynchronous function that will be executed and returns a <see cref="Task{T}"/>. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncFunction"/> has finished. </param>
		/// <returns> An awaitable <see cref="Task{T}"/>. </returns>
		Task<T> ExecuteAsync<T>(string message, Func<Task<T>> asyncFunction, Action doneCallback = null);

		#endregion

		#endregion

		/*
		 * ############################################################################################
		 * # Those methods will await the passed function and therefore not block the calling thread. #
		 * # They additionally provide cancellation support.                                          #
		 * ############################################################################################
		 */
		#region Asynchronous - Cancelable

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> with cancellation support.
		/// </summary>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{CancellationToken, Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the <paramref name="asyncMethod"/>. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteAsync(Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> with cancellation support.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="asyncMethod"/>s execution. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{CancellationToken, Task}"/> to execute. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the <paramref name="asyncMethod"/>. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteAsync(string message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> with cancellation support.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{CancellationToken, Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the <paramref name="asyncMethod"/>. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteAsync(string message, Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);

		#endregion

		/*
		 * ###########################################################################################
		 * # Those methods will wrap the passed method in a task that will be awaited.               #
		 * # Therefore not blocking the calling thread and guaranteeing execution in another thread. #
		 * ###########################################################################################
		 */
		#region Asynchronous - Wrapped Task
		
		/*
		 * #######################################################
		 * # Wraps simple methods within their own awaited task. #
		 * #######################################################
		 */
		#region Method

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="method"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="method"> The <see cref="Action"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="method"/> has finished. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the created <see cref="Task"/> before it starts. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(Action method, Action doneCallback = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="method"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="method"/> is being executed. </param>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="method"/>s execution. </param>
		/// <param name="method"> The <see cref="Action"/> to execute. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the created <see cref="Task"/> before it starts. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Action method, CancellationToken cancellationToken = default);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="method"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="method"/> is being executed. </param>
		/// <param name="method"> The <see cref="Action"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="method"/> has finished. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the created <see cref="Task"/> before it starts. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(string message, Action method, Action doneCallback = null, CancellationToken cancellationToken = default);

		#endregion

		/*
		 * #################################################################################################
		 * # Wraps asynchronous tasks within their own awaited task.                                       #
		 * # This is useful, if it is unknown, whether the underlying task really runs synchronous or not. #
		 * #################################################################################################
		 */
		#region Function

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(Func<Task> asyncMethod, Action doneCallback = null);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="asyncMethod"/>s execution. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{Task}"/> to execute. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Func<Task> asyncMethod);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(string message, Func<Task> asyncMethod, Action doneCallback = null);

		#endregion

		/*
		 * #################################################################################################
		 * # Wraps asynchronous tasks within their own awaited and cancelable task.                        #
		 * # This is useful, if it is unknown, whether the underlying task really runs synchronous or not. #
		 * #################################################################################################
		 */
		#region Function - Cancelable

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{CancellationToken, Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the created <see cref="Task"/> before it starts. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="toggle"> An <see cref="Expression"/> for a boolean value that will be toggled before and after the <paramref name="asyncMethod"/>s execution. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{CancellationToken, Task}"/> to execute. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the created <see cref="Task"/> before it starts. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(string message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default);

		/// <summary>
		/// Activates the busy indicator while executing the passed <paramref name="asyncMethod"/> that will be wrapped within its own <see cref="Task"/>.
		/// </summary>
		/// <param name="message"> The message that is displayed while the <paramref name="asyncMethod"/> is being executed. </param>
		/// <param name="asyncMethod"> The asynchronous <see cref="Func{CancellationToken, Task}"/> to execute. </param>
		/// <param name="doneCallback"> An optional callback that is invoked when the <paramref name="asyncMethod"/> has finished. </param>
		/// <param name="cancellationToken"> An optional <see cref="CancellationToken"/> to cancel the created <see cref="Task"/> before it starts. </param>
		/// <returns> An awaitable <see cref="Task"/>. </returns>
		Task ExecuteTaskAsync(string message, Func<CancellationToken, Task> asyncMethod, Action doneCallback = null, CancellationToken cancellationToken = default);

		#endregion


		#endregion

		#region Busy Indicator

		/// <summary>
		/// Activates the busy indicator by setting the <see cref="IsBusy"/> property to <c>True</c>.
		/// </summary>
		/// <param name="message"> An optional message to display alongside the busy indicator. </param>
		void Show(string message = null);

		/// <summary>
		/// Overrides the currently displayed busy message.
		/// </summary>
		/// <param name="message"> An optional message to display alongside the busy indicator. </param>
		void Override(string message = null);

		/// <summary>
		/// Removes the topmost busy message in the stack of all currently displayed messages. If this is the last one, then the busy indicator will be hidden.
		/// </summary>
		void Revoke();

		/// <summary>
		/// Completely disables the busy indicator by setting the <see cref="IsBusy"/> property to <c>False</c>.
		/// </summary>
		void Close();

		#endregion
	}
}