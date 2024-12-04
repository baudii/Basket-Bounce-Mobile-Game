using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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


		/// <summary>
		/// Executes task safely. Which implies catching any exceptions. Even if called from
		/// void method.
		/// </summary>
		/// <param name="task">The <see cref="Task"></see> to execute</param>
		public static async void SafeExectute(this Task task)
		{
			var stackFrame = new StackFrame(3);
			var callerMethod = stackFrame.GetMethod();
			string context = $"{callerMethod.ReflectedType.Name}.{callerMethod.Name}()";
			string formattedStackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace();

			UnityEngine.Debug.Log(formattedStackTrace);
			UnityEngine.Debug.Log(context);
			try
			{
				await task;
			}
			catch (Exception ex)
			{
				HandleException(ex, context);
			}
		}

		/// <summary>
		/// Executes task safely. Which implies catching any exceptions. Even if called from
		/// void method.
		/// </summary>
		/// <param name="task">The <see cref="Task"></see> to execute</param>
		public static async void SafeExectuteFactory(Func<Task> taskFactory)
		{
			var stackFrame = new StackFrame(3);
			var callerMethod = stackFrame.GetMethod(); 
			string formattedStackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace();
			UnityEngine.Debug.Log(formattedStackTrace);
			string context = $"{callerMethod.ReflectedType.Name}.{callerMethod.Name}()";

			try
			{
				await taskFactory();
			}
			catch (Exception ex)
			{
				HandleException(ex, context);
			}
		}

		private static void HandleException(Exception ex, string context)
		{
			var log = Utils.GetLogWithContext(nameof(Utils), 
				separator: "\n-----------------------------------\n", 
				ex.Message, 
				$"Error occured during {nameof(SafeExectute)}() or {nameof(SafeExectuteFactory)}(). Caller context:{context}",
				ex.StackTrace);
			UnityEngine.Debug.LogError(log);

			throw ex;
		}
	}
}
