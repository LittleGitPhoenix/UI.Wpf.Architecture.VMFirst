#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Classes;

/// <inheritdoc cref="IBusyIndicatorHandler"/>
public class BusyIndicatorHandler : IBusyIndicatorHandler
{
	#region Delegates / Events

	/// <inheritdoc />
	public event PropertyChangedEventHandler? PropertyChanged;
	
	/// <summary> Called to raise <see cref="PropertyChanged"/>. </summary>
	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	#endregion

	#region Constants
	#endregion

	#region Fields

#if NET45
	private static readonly Task CompletedTask = Task.FromResult(false);
#endif

	/// <summary> Stack of all busy messages. </summary>
	private readonly ConcurrentStack<string?> _busyMessages;

	/// <summary> Cancellation token for canceling upcoming busy indicator (used for a timely delayed indicator). </summary>
	private CancellationTokenSource? _tokenSource;

	#endregion

	#region Properties

	/// <inheritdoc />
	public bool IsBusy
	{
		get => _isBusy;
		private set
		{
			if (value != _isBusy)
			{
				_isBusy = value;
				this.OnPropertyChanged();
			}
		}
	}
	private bool _isBusy;

	/// <inheritdoc />
	public string? BusyMessage
	{
		get => _busyMessage;
		private set
		{
			if (value != _busyMessage)
			{
				_busyMessage = value;
				this.OnPropertyChanged();
			}
		}
	}
	private string? _busyMessage;

	/// <inheritdoc />
	/// <value> Default: 200ms </value>
	public TimeSpan InitialDelay { get; set; }

	#endregion

	#region Enumerations
	#endregion

	#region (De)Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	public BusyIndicatorHandler()
	{
		_busyMessages = new ();
		this.InitialDelay = TimeSpan.FromMilliseconds(200);
	}

	#endregion

	#region Methods

	#region Synchronous

	/// <inheritdoc />
	public void Execute(Action method, Action? doneCallback = null)
		=> this.Execute(null, method, doneCallback);

	/// <inheritdoc />
	public void Execute(string? message, Expression<Func<bool>> toggle, Action method)
		=> this.Execute(message, method, ToggleAccessorAndCreateDoneCallback(toggle));

	/// <inheritdoc />
	public void Execute(string? message, Action method, Action? doneCallback = null)
	{
		try
		{
			this.Show(message);
			method?.Invoke();
		}
		finally
		{
			doneCallback?.Invoke();
			this.Revoke();
		}
	}

	#endregion

	#region Asynchronous

	#region Methods

	/// <inheritdoc />
	public async Task ExecuteAsync(Func<Task> asyncMethod, Action? doneCallback = null)
		=> await this.ExecuteAsync(null, asyncMethod, doneCallback);

	/// <inheritdoc />
	public async Task ExecuteAsync(string? message, Expression<Func<bool>> toggle, Func<Task> asyncMethod)
		=> await this.ExecuteAsync(message, asyncMethod, ToggleAccessorAndCreateDoneCallback(toggle));

	/// <inheritdoc />
	public async Task ExecuteAsync(string? message, Func<Task> asyncMethod, Action? doneCallback = null)
	{
		try
		{
			this.Show(message);
#if NET45
			await (asyncMethod?.Invoke() ?? BusyIndicatorHandler.CompletedTask);
#else       
			await (asyncMethod?.Invoke() ?? Task.CompletedTask);
#endif
		}
		finally
		{
			doneCallback?.Invoke();
			this.Revoke();
		}
	}

	#endregion

	#region Functions

	/// <inheritdoc />
	public async Task<T> ExecuteAsync<T>(Func<Task<T>> asyncFunction, Action? doneCallback = null)
		=> await this.ExecuteAsync(null, asyncFunction, doneCallback);

	/// <inheritdoc />
	public async Task<T> ExecuteAsync<T>(string? message, Expression<Func<bool>> toggle, Func<Task<T>> asyncFunction)
		=> await this.ExecuteAsync(message, asyncFunction, ToggleAccessorAndCreateDoneCallback(toggle));

	/// <inheritdoc />
	public async Task<T> ExecuteAsync<T>(string? message, Func<Task<T>> asyncFunction, Action? doneCallback = null)
	{
		try
		{
			this.Show(message);
			if (asyncFunction is null) return default;
			return await asyncFunction.Invoke();
		}
		finally
		{
			doneCallback?.Invoke();
			this.Revoke();
		}
	}

	#endregion

	#endregion

	#region Asynchronous - Cancelable

	/// <inheritdoc />
	public async Task ExecuteAsync(Func<CancellationToken, Task> asyncMethod, Action? doneCallback = null, CancellationToken cancellationToken = default)
		=> await this.ExecuteAsync(null, asyncMethod, doneCallback, cancellationToken);

	/// <inheritdoc />
	public async Task ExecuteAsync(string? message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default)
		=> await this.ExecuteAsync(message, asyncMethod, ToggleAccessorAndCreateDoneCallback(toggle), cancellationToken);

	/// <inheritdoc />
	public async Task ExecuteAsync(string? message, Func<CancellationToken, Task> asyncMethod, Action? doneCallback = null, CancellationToken cancellationToken = default)
	{
		try
		{
			this.Show(message);
			if (!cancellationToken.IsCancellationRequested)
			{
#if NET45
				await (asyncMethod?.Invoke(cancellationToken) ?? BusyIndicatorHandler.CompletedTask);
#else           
				await (asyncMethod?.Invoke(cancellationToken) ?? Task.CompletedTask);
#endif
			}
		}
		finally
		{
			doneCallback?.Invoke();
			this.Revoke();
		}
	}

	#endregion

	#region Asynchronous - Wrapped Task

	#region Method

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(Action method, Action? doneCallback = null, CancellationToken cancellationToken = default)
		=> await this.ExecuteTaskAsync(null, method, doneCallback, cancellationToken);

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(string? message, Expression<Func<bool>> toggle, Action method, CancellationToken cancellationToken = default)
		=> await this.ExecuteTaskAsync(message, method, ToggleAccessorAndCreateDoneCallback(toggle), cancellationToken);

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(string? message, Action method, Action? doneCallback = null, CancellationToken cancellationToken = default)
		=> await this.ExecuteAsync(message, async () => { await Task.Run(method, cancellationToken); }, doneCallback);

	#endregion

	#region Function

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(Func<Task> asyncMethod, Action? doneCallback = null)
		=> await this.ExecuteTaskAsync(null, asyncMethod, doneCallback);

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(string? message, Expression<Func<bool>> toggle, Func<Task> asyncMethod)
		=> await this.ExecuteTaskAsync(message, asyncMethod, ToggleAccessorAndCreateDoneCallback(toggle));

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(string? message, Func<Task> asyncMethod, Action? doneCallback = null)
		=> await this.ExecuteAsync(message, async () => { await Task.Run(async () => { await asyncMethod.Invoke(); }); }, doneCallback);

	#endregion

	#region Function - Cancelable

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(Func<CancellationToken, Task> asyncMethod, Action? doneCallback = null, CancellationToken cancellationToken = default)
		=> await this.ExecuteTaskAsync(null, asyncMethod, doneCallback, cancellationToken);

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(string? message, Expression<Func<bool>> toggle, Func<CancellationToken, Task> asyncMethod, CancellationToken cancellationToken = default)
		=> await this.ExecuteTaskAsync(message, asyncMethod, ToggleAccessorAndCreateDoneCallback(toggle), cancellationToken);

	/// <inheritdoc />
	public async Task ExecuteTaskAsync(string? message, Func<CancellationToken, Task> asyncMethod, Action? doneCallback = null, CancellationToken cancellationToken = default)
		=> await this.ExecuteAsync(message, async (token) => { await Task.Run(async () => { await asyncMethod.Invoke(token); }, token); }, doneCallback, cancellationToken);

	#endregion

	#endregion

	#region Helper

	/// <summary>
	/// Builds an <see cref="Accessor{Boolean}"/> from the <paramref name="toggle"/> expression, toggles it and returns a callback that will toggle the <see cref="Accessor{Boolean}"/> back.
	/// </summary>
	/// <param name="toggle"> The <see cref="Expression"/> used to build the <see cref="Accessor{Boolean}"/>. </param>
	/// <returns> A callback that will toggle the <see cref="Accessor{Boolean}"/>. </returns>
	private static Action ToggleAccessorAndCreateDoneCallback(Expression<Func<bool>> toggle)
	{
		var accessor = new Accessor<bool>(toggle);
		accessor.Set(!accessor.Get());
		return () => accessor.Set(!accessor.Get());
	}

	#endregion

	#region Busy Indicator

	/// <inheritdoc />
	public void Show(string? message = null)
	{
		this.Show(message, overrideCurrent: false);
	}

	/// <inheritdoc />
	public void Override(string? message = null)
	{
		this.Show(message, overrideCurrent: true);
	}

	private void Show(string? message, bool overrideCurrent)
	{
		//string obsoleteMessage = null;
		if (overrideCurrent) _busyMessages.TryPop(out _);
		_busyMessages.Push(message);

		//System.Diagnostics.Debug.WriteLine
		//(
		//	overrideCurrent
		//		? $"Replaced: '{obsoleteMessage ?? "[NULL]"}' with '{message ?? "[NULL]"}' | Messages: {_busyMessages.Count}"
		//		: $"Showing: '{message ?? "[NULL]"}' | Messages: {_busyMessages.Count}"
		//);

		this.ShowBusyMessage();
	}

	private void ShowBusyMessage()
	{
		var showMessage = () => 
		{
			// Get the next message and display it.
			_busyMessages.TryPeek(out var nextMessage);
			this.BusyMessage = nextMessage;
			this.IsBusy = true;
		};

		// Check if other messages are already displayed or no delay should be applied.
		if (this.IsBusy || this.InitialDelay == TimeSpan.Zero)
		{
			// YES: Directly show the new one.
			showMessage.Invoke();
			return;
		}

		// Check if the cancellation has already been created and if not then create it.
		if (Interlocked.CompareExchange(ref _tokenSource, new CancellationTokenSource(), null) is null)
		{
			// NO: Create a task that will show the busy indicator after 200 ms. This is done so that the indicator won't be shown if the task finishes real fast.
			var token = _tokenSource.Token;
			var busyTask = Task.Run
			(
				async () =>
				{
					await Task.Delay(delay: this.InitialDelay, cancellationToken: token);
					showMessage.Invoke();
				},
				token
			);
		}
	}

	/// <inheritdoc />
	public void Revoke()
	{
		_busyMessages.TryPop(out _);
		if (_busyMessages.IsEmpty)
		{
			this.Close();
		}
		else
		{
			this.ShowBusyMessage();
		}
		//System.Diagnostics.Debug.WriteLine($"Removed: '{obsoleteMessage ?? "[NULL]"}' | Messages: {_busyMessages.Count}");
	}

	/// <inheritdoc />
	public void Close()
	{
		// Cancel any current running delayed indicator task.
		var currentToken = Interlocked.CompareExchange(ref _tokenSource, null, _tokenSource);
		currentToken?.Cancel();
		currentToken?.Dispose();

		this.IsBusy = false;
		this.BusyMessage = null;
		_busyMessages.Clear();
	}

	#endregion
	
	#endregion
}