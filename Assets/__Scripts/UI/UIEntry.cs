using BasketBounce.Systems;
using BasketBounce.Gameplay;
using BasketBounce.Gameplay.Levels;
using BasketBounce.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class UIEntry : BaseGameEntry
{
	[SerializeField] UI_Manager uiManagerPrefab;

	UI_Manager uiManager;
	UI_Overview uiOverview;
	UI_BounceCounter uiBounceCounter;
	UI_Button_Handler[] uiButtonHandlers;
	UI_LevelSelector uiLevelSelector;
	UI_LevelNameController uiLevelNameController;
	public override Task Setup(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		uiManager = Instantiate(uiManagerPrefab);
		uiOverview = uiManager.GetComponentInChildren<UI_Overview>(true);
		uiBounceCounter = uiManager.GetComponentInChildren<UI_BounceCounter>(true);
		uiButtonHandlers = uiManager.GetComponentsInChildren<UI_Button_Handler>(true);
		uiLevelSelector = uiManager.GetComponentInChildren<UI_LevelSelector>(true);
		uiLevelNameController = uiManager.GetComponentInChildren<UI_LevelNameController>(true);

		return Task.CompletedTask;
	}
	public override Task Activate(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		GetDependancy(out GameManager gameManager);
		GetDependancy(out LevelManager levelManager);
		GetDependancy(out Ball ball);
		GetDependancy(out DollyCameraController dolly);
		GetDependancy(out GestureDetector gestureDetector);

		uiLevelSelector.Init(levelManager);
		foreach (var buttonHandler in uiButtonHandlers)
			buttonHandler.Init(gameManager, levelManager, uiManager);
		uiBounceCounter.Init(ball);

		uiManager.Init(gameManager, levelManager, ball);
		uiOverview.Init(levelManager, dolly);
		uiLevelNameController.Init(gestureDetector);

		return Task.CompletedTask;
	}
}
