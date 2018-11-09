using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SollaraGames.ObjectPooling
{
	// enum that contains the type of the object.
	// also has a reference to it's object pool owner.
	[CreateAssetMenu (fileName = "PoolObjectType", menuName = "NilocPrototype/ObjectPooling/PoolObjectType", order = 0)]
	public class PoolObjectType : ScriptableObject
	{
		[HideInInspector] public ObjectPool _objectPool;
	}
}