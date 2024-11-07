using BasketBounce.Systems;
using BasketBounce.UI;
using UnityEngine;
using System.Threading.Tasks;
using KK.Common;

public class UIEntry : SceneEntryPoint
{
	[SerializeField] UI_Manager uiManagerPrefab;
	UI_Manager uiManager;


	public override Task Setup()
	{
		Cts.Token.ThrowIfCancellationRequested();

		uiManager = Instantiate(uiManagerPrefab);
		DIContainer.Register(uiManager);

		uiManager.Init();

		return Task.CompletedTask;
	}
	public override Task Activate()
	{
		Cts.Token.ThrowIfCancellationRequested();

		return Task.CompletedTask;
	}
}
