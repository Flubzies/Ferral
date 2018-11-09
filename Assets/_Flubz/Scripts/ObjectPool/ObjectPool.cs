/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */

// Farjad Mohammad - Modified. 
// Now uses ScriptableObject enums for references instead of strings.
// Made variable names clearer.
// Made the ObjectPool a non Singleton. Now managed by PoolManager with multiple ObjectPool objects.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SollaraGames.ObjectPooling
{
    [System.Serializable]
    public class PoolInfo
    {
        public PoolObjectType _poolObjectType;
        public PoolObject _prefab;
        public int _poolSize = 400;
        public bool _fixedSize;
    }

    class Pool
    {
        private Stack<PoolObject> _availableObjStack = new Stack<PoolObject> ();

        private PoolObjectType _poolObjectType;
        private PoolObject _poolObjectPrefab;
        private bool _fixedSize;
        private int _poolSize;

        public Pool (PoolObjectType poolObjectType_, PoolObject poolObjectPrefab_, int initialCount_, bool fixedSize_, Transform parent_, ObjectPool objectPool_)
        {
            this._poolObjectType = poolObjectType_;
            this._poolObjectType._objectPool = objectPool_;
            this._poolObjectPrefab = poolObjectPrefab_;
            this._poolSize = initialCount_;
            this._fixedSize = fixedSize_;

            //populate the pool
            for (int index = 0; index < initialCount_; index++)
            {
                AddObjectToPool (NewObjectInstance (parent_));
            }
        }

        //o(1)
        private void AddObjectToPool (PoolObject poolObject_)
        {
            //add to pool
            poolObject_.gameObject.SetActive (false);
            _availableObjStack.Push (poolObject_);
            poolObject_._isPooled = true;
        }

        private PoolObject NewObjectInstance (Transform parent_)
        {
            PoolObject poolObject = (PoolObject) GameObject.Instantiate (_poolObjectPrefab, Vector3.zero, Quaternion.identity, parent_);
            poolObject._poolObjectType = _poolObjectType;
            return poolObject;
        }

        //o(1)
        public PoolObject NextAvailableObject (Vector3 position_, Quaternion rotation_, Transform parent_)
        {
            PoolObject poolObject = null;
            if (_availableObjStack.Count > 0)
            {
                poolObject = _availableObjStack.Pop ();
            }
            else if (_fixedSize == false)
            {
                //increment size var, this is for info purpose only
                _poolSize++;
                Debug.Log (string.Format ("Growing pool {0}. New size: {1}", _poolObjectType, _poolSize));
                //create new object
                poolObject = NewObjectInstance (parent_);
            }
            else
            {
                Debug.LogWarning ("No object available & cannot grow pool: " + _poolObjectType);
            }

            PoolObject result = null;
            if (poolObject != null)
            {
                poolObject._isPooled = false;
                result = poolObject;
                result.gameObject.SetActive (true);

                result.transform.position = position_;
                result.transform.rotation = rotation_;
            }

            return result;
        }

        //o(1)
        public void ReturnObjectToPool (PoolObject poolObject_)
        {

            if (_poolObjectType.Equals (poolObject_._poolObjectType))
            {
                if (poolObject_._isPooled)
                {
                    Debug.LogWarning (poolObject_.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");
                }
                else
                {
                    AddObjectToPool (poolObject_);
                }
            }
            else
            {
                Debug.LogError (string.Format ("Trying to add object to incorrect pool {0} {1}", poolObject_._poolObjectType, _poolObjectType));
            }
        }
    }

    [System.Serializable]
    [CreateAssetMenu (fileName = "ObjectPool", menuName = "NilocPrototype/ObjectPooling/ObjectPool", order = 0)]
    public class ObjectPool : ScriptableObject
    {

        [Tooltip ("Editing Pool Info value at runtime has no effect")]
        public PoolInfo[] _poolInfo;

        Dictionary<PoolObjectType, Pool> _poolDictionary = new Dictionary<PoolObjectType, Pool> ();

        public void Initialize ()
        {
            CheckForDuplicatePoolNames ();
            CreatePools ();
        }

        private void CheckForDuplicatePoolNames ()
        {
            for (int index = 0; index < _poolInfo.Length; index++)
            {
                PoolObjectType poolObjectType = _poolInfo[index]._poolObjectType;

                for (int internalIndex = index + 1; internalIndex < _poolInfo.Length; internalIndex++)
                {
                    if (poolObjectType.Equals (_poolInfo[internalIndex]._poolObjectType))
                    {
                        Debug.LogError (string.Format ("Pool {0} & {1} have the same name. Assign different names.", index, internalIndex));
                    }
                }
            }
        }

        private void CreatePools ()
        {
            foreach (PoolInfo currentPoolInfo in _poolInfo)
            {
                Pool pool = new Pool (currentPoolInfo._poolObjectType, currentPoolInfo._prefab,
                    currentPoolInfo._poolSize, currentPoolInfo._fixedSize, PoolManager._instance.transform, this);

                _poolDictionary[currentPoolInfo._poolObjectType] = pool;
            }
        }

        public PoolObject GetObjectFromPool (PoolObjectType poolObjectType_, Vector3 position_, Quaternion rotation_)
        {
            PoolObject result = null;

            if (_poolDictionary.ContainsKey (poolObjectType_))
            {
                Pool pool = _poolDictionary[poolObjectType_];
                result = pool.NextAvailableObject (position_, rotation_, PoolManager._instance.transform);
                if (result == null)
                {
                    Debug.LogWarning ("No object available in pool. Consider setting fixedSize to false.: " + poolObjectType_);
                }

            }
            else
            {
                Debug.LogError ("Invalid pool name specified: " + poolObjectType_);
            }

            return result;
        }

        public void ReturnObjectToPool (PoolObject poolObject_, bool reParent_ = false)
        {
            if (poolObject_ == null)
            {
                Debug.LogWarning ("Specified object is not a pooled _instance: " + poolObject_.name);
            }
            else
            {
                if (_poolDictionary.ContainsKey (poolObject_._poolObjectType))
                {
                    Pool pool = _poolDictionary[poolObject_._poolObjectType];
                    if (reParent_) poolObject_.transform.parent = PoolManager._instance.transform;
                    pool.ReturnObjectToPool (poolObject_);
                }
                else
                {
                    Debug.LogWarning ("No pool available with name: " + poolObject_._poolObjectType);
                }
            }
        }
    }
}