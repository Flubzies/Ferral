using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SollaraGames.ObjectPooling
{
	public class PoolManager : MonoBehaviour
	{
		public static PoolManager _instance = null;
		void Awake ()
		{
			if (_instance == null) _instance = this;
			else if (_instance != this) Destroy (gameObject);
		}

		[InlineEditor][SerializeField] List<ObjectPool> _objectPools;

		private void Start ()
		{
			foreach (ObjectPool pool in _objectPools)
			{
				pool.Initialize ();
			}
		}

		public PoolObject GetObjectFromPool (PoolObjectType poolObjectType_, Vector3 position_, Quaternion rotation_)
		{
			PoolObject poolObject = poolObjectType_._objectPool.GetObjectFromPool (poolObjectType_, position_, rotation_);
			if (poolObject == null) Debug.LogError (poolObjectType_.name + " not found!");
			return poolObject;
		}

		public void ReturnObjectToPool (PoolObject poolObject_, bool reParent_ = false)
		{
			poolObject_._poolObjectType._objectPool.ReturnObjectToPool (poolObject_, reParent_);
		}
	}
}