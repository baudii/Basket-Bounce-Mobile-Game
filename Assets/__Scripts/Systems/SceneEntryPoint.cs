using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BasketBounce.Systems
{
	public abstract class SceneEntryPoint : MonoBehaviour
	{
		public static CancellationTokenSource Cts;
		public abstract Task Setup();
		public abstract Task Activate();
	}
}
