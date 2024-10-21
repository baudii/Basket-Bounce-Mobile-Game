using UnityEngine;
using UnityEngine.Events;
using KK.Common;
using BasketBounce.Gameplay;
using BasketBounce.Systems.Interfaces;

namespace BasketBounce.Systems
{
	public class LevelData : MonoBehaviour
	{
		[SerializeField] string levelHeader;
		[SerializeField] string russianLevelHeader;
		[SerializeField] Transform finTransform;
		[SerializeField] ResetableManager resetableManager;

		[SerializeField] int bounces3star;
		[SerializeField] int bounces2star;
		[SerializeField] bool useThreshHold;
		[SerializeField] bool canNotStuck;
		[SerializeField] float overrideStuckTime;
		[SerializeField] UnityEvent OnBallReleasedEvent;
		[SerializeField] UnityEvent OnFirstTimeLoadEvemt;
		[SerializeField] UnityEvent OnClickAnywhereEvent;
		/*	[SerializeField] float time3star;
			[SerializeField] float time2star;*/

		public bool UseThreshold => useThreshHold;
		public bool CanNotStuck => canNotStuck;
		public float OverrideStuckTime => overrideStuckTime;
		public string LevelHeader => levelHeader;

		private static float? _screenHeight;
		private static float ScreenHeight
		{
			get
			{
				if (_screenHeight == null)
					_screenHeight = Screen.height;
				return (float)_screenHeight;
			}
		}

		private static Camera _cam;
		private static Camera Cam
		{
			get
			{
				if (_cam == null)
					_cam = Camera.main;
				return _cam;
			}
		}

#if UNITY_EDITOR
		[Header("Editor Only")]
		[SerializeField] bool validateLevel;
		private void OnValidate()
		{
			if (validateLevel)
			{
				if (resetableManager == null)
					resetableManager = GetComponent<ResetableManager>();
				transform.ForEachDescendant(child =>
				{
					if (child.TryGetComponent(out ILevelValidator levelValidator))
					{
						levelValidator.Validate();
					}
					if (child.name.Contains("Finish"))
					{
						finTransform = child;
					}
				});
			}
		}
#endif
		public bool IsFinishInScreen()
		{
			Vector3 screenPos = Cam.WorldToScreenPoint(finTransform.position);
			if (screenPos.y <= ScreenHeight)
			{
				return true;
			}
			return false;
		}

		public void Init()
		{
			resetableManager.Fill();
			resetableManager.ResetAll();
		}

		public void OnFirstTimeLoad()
		{
			OnFirstTimeLoadEvemt?.Invoke();
		}

		public void OnBallReleased()
		{
			OnBallReleasedEvent?.Invoke();
		}

		public void OnClickAnywhere()
		{
			OnClickAnywhereEvent?.Invoke();
		}

		public void OnLevelUnload()
		{
			ResetLevel();
		}

		public void ResetLevel()
		{
			resetableManager.ResetAll();
		}

		public Vector3 GetFinPos()
		{
			return finTransform.position;
		}

		public ScoreData ConvertToStars(int bounces)
		{
			if (bounces2star == 0)
				bounces2star = bounces3star + 3;
			if (bounces <= bounces3star)
				return new ScoreData(3, 0);
			if (bounces <= bounces2star)
				return new ScoreData(2, bounces3star);
			return new ScoreData(1, bounces2star);
		}
	}
}