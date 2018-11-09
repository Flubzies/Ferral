using System;
using System.Collections;
using Sirenix.OdinInspector;
using SollaraGames.Managers;
using SollaraGames.ObjectPooling;
using UnityEngine;

namespace SollaraGames.ObjectPooling.PoolObjects
{
    public class Projectile : PoolObject
    {
        [SerializeField] protected Rigidbody _rb;
        [SerializeField] protected bool _selfCollisionDestruct = true;
        [SerializeField] bool _usingTrail;
        [ShowIf ("_usingTrail")][SerializeField] TrailRenderer _projectileTrail;

        protected string _projectileTag;
        protected float _speed;
        protected int _damage;
        protected float _lifetime;
        protected bool _projectileReturned;

        protected bool _applyHitEffectToStructures;
        protected PoolObjectType _dudEffect;
        protected PoolObjectType _hitEffect;

        private void Awake ()
        {
            _projectileTag = gameObject.tag;
        }

        private void OnEnable ()
        {
            _projectileReturned = false;
            if (_projectileTrail) _projectileTrail.Clear ();
        }

        public virtual void SetProjectileProperties (int damage_, Vector3 fireDirection_, float speed_, float lifetime_)
        {
            SetDamage (damage_);
            SetVelocity (fireDirection_, speed_);
            SetLifetime (lifetime_);
        }

        public virtual void SetDamage (int damage_)
        {
            _damage = damage_;
        }

        public virtual void SetVelocity (Vector3 fireDirection_, float speed_)
        {
            _rb.velocity = fireDirection_ * speed_;
        }

        public virtual void SetLifetime (float lifetime_)
        {
            _lifetime = lifetime_;
            StartCoroutine (Lifetime ());
        }

        public virtual void SetHitEffects (PoolObjectType hitEffect_, PoolObjectType dudEffect_, bool applyHitEffectToStructures_)
        {
            _hitEffect = hitEffect_;
            _dudEffect = dudEffect_;
            _applyHitEffectToStructures = applyHitEffectToStructures_;
        }

        protected virtual void OnCollisionEnter (Collision other)
        {
            if (!_selfCollisionDestruct)
                if (other.gameObject.CompareTag (_projectileTag)) return;

            if (HealthDamager.AttemptToDamage (other.gameObject, _damage))
            {
                DestroyProjectile (Quaternion.FromToRotation (Vector3.forward, other.contacts[0].normal), true);
            }
            else
            {
                DestroyProjectile (Quaternion.FromToRotation (Vector3.forward, other.contacts[0].normal), _applyHitEffectToStructures);
            }
        }

        protected void DestroyProjectile (Quaternion rotation_, bool hitSuccessful_)
        {
            StopCoroutine (Lifetime ());
            if (hitSuccessful_ && _hitEffect != null) PoolManager._instance.GetObjectFromPool (_hitEffect, transform.position, rotation_);
            else if (_dudEffect != null) PoolManager._instance.GetObjectFromPool (_dudEffect, transform.position, rotation_);
            if (!_projectileReturned) PoolManager._instance.ReturnObjectToPool (this);
            _projectileReturned = true;
        }

        private void OnDisable ()
        {
            _projectileReturned = true;
        }

        protected IEnumerator Lifetime ()
        {
            yield return new WaitForSeconds (_lifetime);
            DestroyProjectile (Quaternion.identity, false);
        }
    }
}

[Serializable]
public class ProjectileProperties<T, U>
{
    public PoolObjectType _projectile;
    public int _damage;
    public T _speed;
    public U _lifetime;
    [FoldoutGroup ("Effects")] public bool _applyHitEffectToStructures;
    [FoldoutGroup ("Effects")] public PoolObjectType _hitEffect;
    [FoldoutGroup ("Effects")] public PoolObjectType _dudEffect;
}

namespace ProjectileType
{
    [Serializable] public class Uzi : ProjectileProperties<MinMax, float> { }

    [Serializable] public class ElectricStream : ProjectileProperties<float, float> { }

    [Serializable] public class DeathBlossom : ProjectileProperties<float, float> { }

    [Serializable] public class FireBreath : ProjectileProperties<MinMax, MinMax> { }
}

[Serializable]
public struct MinMax
{
    public float _min;
    public float _max;

    public MinMax (float min_, float max_)
    {
        _min = min_;
        _max = max_;
    }

    public float GetRandom
    {
        get { return UnityEngine.Random.Range (_min, _max); }
    }
}

public class HealthDamager
{
    public static bool AttemptToDamage (GameObject other, int damage_)
    {
        Health health = other.gameObject.GetComponent<Health> ();
        if (health)
        {
            health.Damage (damage_);
            return true;
        }
        else return false;
    }
}