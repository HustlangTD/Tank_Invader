using Fusion;
using FusionExamples.Tanknarok;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
	[SerializeField] private RectTransform _leftJoy;
	[SerializeField] private RectTransform _leftKnob;
	[SerializeField] private RectTransform _rightJoy;
	[SerializeField] private RectTransform _rightKnob;
	private Transform _canvas;
	//-- Unity Methods --//
	private void Awake()
	{
		_canvas = GetComponentInParent<Canvas>().transform;
	}

	public void OnToggleReady()
	{
		foreach (InputController ic in FindObjectsOfType<InputController>())
		{
			if(ic.Object.HasInputAuthority)
				ic.ToggleReady();
		}
	}
	///-- Public Methods --//
	public void OnDisconnect()
	{
		NetworkRunner runner = FindObjectOfType<NetworkRunner>();
		if (runner != null && !runner.IsShutdown)
		{
			// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
			runner.Shutdown(false);
		}
	}
	//-- Private Methods --//
	private void SetJoy(RectTransform joy, RectTransform knob, bool active, Vector2 center, Vector2 current)
	{
		center /= _canvas.localScale.x;
		current /= _canvas.localScale.x;

		joy.gameObject.SetActive(active);
		joy.anchoredPosition = center;

		current -= center;
		if (current.magnitude > knob.rect.width / 2)
			current = current.normalized * knob.rect.width / 2;
		
		knob.anchoredPosition = current;
	}
	//-- Public Methods --//
	public void SetLeft(bool active, Vector2 down, Vector2 current)
	{
		SetJoy(_leftJoy, _leftKnob, active, down, current);
	}

	public void SetRight(bool active, Vector2 down, Vector2 current)
	{
		SetJoy(_rightJoy, _rightKnob, active, down, current);
	}
}
