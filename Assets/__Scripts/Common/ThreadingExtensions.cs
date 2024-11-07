using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace KK.Common
{
	public static class ThreadingExtensions
	{

		/*
				/// <summary>
				/// Extending AsyncOperation class with GetAwaiter() method to use it with \"await\" keyword
				/// </summary>

				public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
				{
					var source = new TaskCompletionSource<AsyncOperation>();
					asyncOp.completed += _ => source.SetResult(null);

					return ((Task)source.Task).GetAwaiter();
				}
				/// <summary>
				/// Extending AsyncInstantiateOperation class with GetAwaiter() method to use it with \"await\" keyword
				/// </summary>
				public static TaskAwaiter GetAwaiter(this AsyncInstantiateOperation asyncOp)
				{
					var source = new TaskCompletionSource<AsyncOperation>();
					asyncOp.completed += operation => source.SetResult(operation);

					return ((Task)source.Task).GetAwaiter();
				}*/

		/// <summary>
		/// Extending AsyncOperation class with AsTask() method that transforms it into a Task
		/// </summary>
		public static Task AsTask(this AsyncOperation asyncOperation, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (asyncOperation == null)
				throw new ArgumentNullException(nameof(asyncOperation), "Provided async operation is null");

			if (asyncOperation.isDone)
				return Task.CompletedTask;

			var tcs = new TaskCompletionSource<object>();

			IDisposable registration = null;
			if (cancellationToken.CanBeCanceled)
				registration = cancellationToken.Register(OnCanceled);

			asyncOperation.completed += OnCompleted;

			return tcs.Task;

			void OnCompleted(AsyncOperation op)
			{
				registration?.Dispose();
				asyncOperation.completed -= OnCompleted;
				tcs.TrySetResult(null);
			}

			void OnCanceled()
			{
				registration.Dispose();
				asyncOperation.completed -= OnCompleted;
				tcs.TrySetCanceled(cancellationToken);
			}
		}
	}
}
