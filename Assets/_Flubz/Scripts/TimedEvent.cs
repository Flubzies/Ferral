using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class TimedCallbackEvent
{
	[Tooltip ("Time (seconds) it takes for the event to be ready again.")]
	[SerializeField] float _coolDownTimeInSeconds;

	Action _OnEventTriggered = null;
	Action _OnEventComplete = null;
	MonoBehaviour _mono;

	public void Initialize (MonoBehaviour mono_)
	{
		_mono = mono_;
	}

	public void TriggerEvent ()
	{
		if (_OnEventTriggered != null) _OnEventTriggered.Invoke ();
		_mono.StartCoroutine (ResetEvent ());
	}

	IEnumerator ResetEvent ()
	{
		yield return new WaitForSeconds (_coolDownTimeInSeconds);

		if (_OnEventComplete != null)
		{
			_OnEventComplete.Invoke ();
			_OnEventComplete = null;
		}
	}

	public void OnEventTriggered (Action action_)
	{
		if (action_ != null) _OnEventTriggered = action_;
	}

	public void OnEventComplete (Action action_)
	{
		if (action_ != null) _OnEventComplete = action_;
	}
}

[System.Serializable]
public class TimedCooldown
{
	[SerializeField] float _coolDownTimeInSeconds = 1.0f;
	float _coolDownTimeStamp = -1.0f;
	public float GetPercent { get { return (Mathf.Clamp01 ((_coolDownTimeStamp - Time.time) / _coolDownTimeInSeconds)) * -1.0f + 1.0f; } }

	public void StartCoolDown ()
	{
		_coolDownTimeStamp = Time.time + _coolDownTimeInSeconds;
	}

	/// <summary>
	/// Checks if the timed event is ready to fire again.
	/// </summary>
	/// <param name="startCooldown_"> "If false, it will not automatically start the cool down when called." </param>
	/// <returns></returns>
	public bool EventReady (bool startCooldown_ = true)
	{
		if (_coolDownTimeStamp <= Time.time)
		{
			if (startCooldown_) StartCoolDown ();
			return true;
		}
		else return false;
	}

}