using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] CinemachineVirtualCamera _cvcamera;
	[SerializeField] CinemachineTargetGroup _cineTargetGroup;
	[SerializeField] float _defaultTargetWeight;
	[SerializeField] float _defaultTargetRadius;

	List<CinemachineTargetGroup.Target> _playersToFollow = new List<CinemachineTargetGroup.Target> ();

	private void Awake ()
	{
		PlayerManager._instance.OnPlayerAdded += AddPlayerToTargetGroup;
		PlayerManager._instance.OnPlayerRemoved += AddPlayerToTargetGroup;
	}

	private void OnDestroy ()
	{
		PlayerManager._instance.OnPlayerAdded -= AddPlayerToTargetGroup;
		PlayerManager._instance.OnPlayerRemoved -= AddPlayerToTargetGroup;
	}

	void AddPlayerToTargetGroup ()
	{
		List<Player> _players = PlayerManager._instance._Players;
		_playersToFollow.Clear ();

		foreach (Player player in _players)
		{
			CinemachineTargetGroup.Target tempTarget = new CinemachineTargetGroup.Target ();
			tempTarget.weight = _defaultTargetWeight;
			tempTarget.radius = _defaultTargetRadius;
			tempTarget.target = player.transform;

			_playersToFollow.Add (tempTarget);
		}

		_cineTargetGroup.m_Targets = _playersToFollow.ToArray ();
	}

}