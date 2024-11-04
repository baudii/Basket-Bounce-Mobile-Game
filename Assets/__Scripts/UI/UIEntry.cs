using BasketBounce.Systems;
using BasketBounce.UI;
using UnityEngine;
using System.Threading.Tasks;
using KK.Common;

public class UIEntry : SceneEntryPoint
{
	[SerializeField] UI_Manager uiManagerPrefab;

	UI_Manager uiManager;
	UI_Overview uiOverview;
	UI_BounceCounter uiBounceCounter;
	UI_Button_Handler[] uiButtonHandlers;
	UI_LevelSelector uiLevelSelector;
	UI_LevelNameController uiLevelNameController;
	public override Task Setup()
	{
		Cts.Token.ThrowIfCancellationRequested();

		uiManager = Instantiate(uiManagerPrefab);
		uiOverview = uiManager.GetComponentInChildren<UI_Overview>(true);
		uiBounceCounter = uiManager.GetComponentInChildren<UI_BounceCounter>(true);
		uiButtonHandlers = uiManager.GetComponentsInChildren<UI_Button_Handler>(true);
		uiLevelSelector = uiManager.GetComponentInChildren<UI_LevelSelector>(true);
		uiLevelNameController = uiManager.GetComponentInChildren<UI_LevelNameController>(true);

		DIContainer.Register(uiManager);

		return Task.CompletedTask;
	}
	public override Task Activate()
	{
		Cts.Token.ThrowIfCancellationRequested();

		DIContainer.InjectIn(uiLevelSelector, uiBounceCounter, uiManager, uiOverview, uiLevelNameController);
		DIContainer.InjectIn(uiButtonHandlers);

		return Task.CompletedTask;
	}
}
