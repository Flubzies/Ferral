using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, ICharacterPlayer
{
    [Title ("Player Movement")]
    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _movementDeadZone = 0.01f;
    [SerializeField] Rigidbody _playerRB;
    [SerializeField] string _cameraOffsetTag = "CameraOffset";
    [SerializeField] float _sprintSpeedModifier;

    [Title ("Player Rotation")]
    [SerializeField] float _rotSpeed = 1.0f;

    [Title ("Player UI")]
    [SerializeField] RectTransform _playerUI;

    [Title ("Player Health")]
    [SerializeField] Health _health;
    [SerializeField] Image _healthBar;
    [SerializeField] Color _healthFlashColor;

    [Title ("Player Properties")]
    // [SerializeField] RectTransform _targetDirRect;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Transform _firePoint;
    [SerializeField] PlayerInput _playerInput;

    public Transform GetTransform { get { return transform; } }
    public Transform GetFirePoint { get { return _firePoint; } }
    public Vector3 GetMovementAxis { get { return _MovementAxis; } }
    public float GetMoveSpeedModifer { get; set; }
    public float GetRotSpeedModifer { get; set; }
    public int _GamePlayerID { get; set; }

    Transform _cameraOffset;
    Color _defaultHealthColor;

    Vector3 _MovementAxis { get; set; }
    Rewired.Player _Input { get { return PlayerManager._instance.GetRewiredPlayer (_GamePlayerID); } }

    [Title ("Game Properties")]
    [SerializeField] float _wolfAttackRange = 2.0f;
    [SerializeField] float _playerAttackRange = 12.0f;
    [SerializeField] float _healthReductionRate;
    [SerializeField] int _wereWolfKillHealAmount;
    [SerializeField] LayerMask _characters;
    [SerializeField] LayerMask _obstacles;
    [SerializeField] TMP_Text _factionName;
    [SerializeField] LineRenderer _shootPrefab;
    [SerializeField] float _shootFadeTime = 1.0f;
    [SerializeField] AutoDestructParticle _hitEffect;
    [SerializeField] AutoDestructParticle _werewolfAttackEffect;
    [SerializeField] float _timeBeforeDestroy;
    [SerializeField] RagDoll _ragdoll;

    public bool _IsWereWolf { get; private set; }

    public void Initialize (int gamePlayerID_, bool isWereWolf_ = false)
    {
        if (!_playerRB) _playerRB = GetComponent<Rigidbody> ();
        _cameraOffset = GameObject.FindGameObjectWithTag (_cameraOffsetTag).transform;

        _health.OnDeath += OnDeath;
        _health.OnDamaged += OnDamaged;

        GetMoveSpeedModifer = 1.0f;
        GetRotSpeedModifer = 1.0f;
        _GamePlayerID = gamePlayerID_;
        _IsWereWolf = isWereWolf_;
        if (_IsWereWolf)
        {
            SetupWereWolf ();
        }

        _defaultHealthColor = _healthBar.color;
    }

    private void Update ()
    {
        if (!ReInput.isReady) return;
        if (_Input == null) return;

        PlayerMovement ();
        FlashHealthBar ();
        if (_Input.GetButtonDown (_playerInput._actionA))
            Attack ();
    }

    void SetupWereWolf ()
    {
        Debug.Log ("Setup WereWolf " + _GamePlayerID + " " + _IsWereWolf);
        _factionName.text = "Werewolf";
        StartCoroutine (HealthReduction ());
    }

    IEnumerator HealthReduction ()
    {
        _health.Damage (2);
        yield return new WaitForSeconds (_healthReductionRate);
        StartCoroutine (HealthReduction ());
    }

    void FlashHealthBar ()
    {
        if (_Input.GetButtonDown (_playerInput._actionY))
        {
            _healthBar.DOBlendableColor (_healthFlashColor, 0.2f).OnComplete (UnFlashHealthBar);
        }
    }

    void UnFlashHealthBar ()
    {
        _healthBar.DOBlendableColor (_defaultHealthColor, 0.2f);
    }

    void OnDeath ()
    {
        _health.OnDeath -= OnDeath;
        _ragdoll._ragdollGFX.transform.DOScale (Vector3.zero, 1.0f).OnComplete (DestroyGO);
    }

    void OnDamaged ()
    {
        _healthBar.fillAmount = _health._GetHealthPercent;
    }

    private void OnDestroy ()
    {
        _health.OnDeath -= OnDeath;
        _health.OnDamaged -= OnDamaged;
    }

    private void PlayerMovement ()
    {
        Vector3 movement = new Vector3 (_Input.GetAxis (_playerInput._movementX), 0, _Input.GetAxis (_playerInput._movementY));
        if (_Input.GetButton (_playerInput._actionB) && _IsWereWolf)
        {
            GetMoveSpeedModifer = _sprintSpeedModifier;
        }
        else GetMoveSpeedModifer = 1.0f;

        if (movement.magnitude > _movementDeadZone)
        {
            _MovementAxis = _cameraOffset.forward * movement.normalized.z + _cameraOffset.right * movement.normalized.x;

            _playerRB.MovePosition (Vector3.Lerp (transform.position,
                transform.position + (_MovementAxis),
                _moveSpeed * Time.deltaTime * GetMoveSpeedModifer));

            RotatePlayer (_MovementAxis);
        }
        else
        {
            _MovementAxis = Vector3.zero;
        }
    }

    private void RotatePlayer (Vector3 rotationAngle_)
    {
        transform.rotation = Quaternion.Lerp (transform.rotation,
            Quaternion.LookRotation (rotationAngle_, Vector3.up),
            Time.deltaTime * _rotSpeed * GetRotSpeedModifer);
    }

    public void SetupUIPosition (bool xPositionOffset_, bool bottomRightAnchor_)
    {
        if (bottomRightAnchor_)
        {
            _playerUI.pivot = Vector2.zero;
            _playerUI.anchorMin = Vector2.zero;
            _playerUI.anchorMax = Vector2.zero;
            _playerUI.position = Vector2.zero;
        }
        if (xPositionOffset_)
        {
            _playerUI.position += (Vector3.right * (Screen.width - _playerUI.rect.width));
        }
    }

    void WolfAttack (RaycastHit hit_)
    {
        _health.Heal (_wereWolfKillHealAmount);
        ICharacterPlayer icharp = hit_.collider.gameObject.GetComponent<ICharacterPlayer> ();
        if (icharp != null)
        {
            icharp.HasBeenKilled (1.0f);
            GameObject.Instantiate (_werewolfAttackEffect, hit_.point, Quaternion.identity);
        }
    }

    void CharacterAttack (GameObject other_, RaycastHit hit_)
    {
        ShootEffect (hit_, false);
        ICharacterPlayer icharp = hit_.collider.gameObject.GetComponent<ICharacterPlayer> ();
        if (icharp != null)
        {
            icharp.HasBeenKilled (0.1f);
            GameObject.Instantiate (_werewolfAttackEffect, hit_.point, Quaternion.identity);
        }
    }

    void Attack ()
    {
        RaycastHit hit;
        float _attackRange;
        if (_IsWereWolf) _attackRange = _wolfAttackRange;
        else _attackRange = _playerAttackRange;

        Physics.Raycast (_firePoint.position, _firePoint.forward * _attackRange, out hit, _attackRange, _characters);
        if (hit.collider == null) Physics.Raycast (_firePoint.position, _firePoint.forward * _attackRange, out hit, _attackRange, _obstacles);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag ("Character"))
            {
                Debug.Log ("Hit character");
                if (_IsWereWolf) WolfAttack (hit);
                else CharacterAttack (hit.collider.gameObject, hit);
            }
            else if (hit.collider.gameObject.CompareTag ("Player"))
            {
                Debug.Log ("Hit player");
                if (_IsWereWolf) WolfAttack (hit);
                else CharacterAttack (hit.collider.gameObject, hit);
            }
            else if (!_IsWereWolf)
            {
                ShootEffect (hit, false);
            }
        }
        else
        {
            if (!_IsWereWolf) ShootEffect (hit, true, _attackRange);
            Debug.Log ("Hit nothing.");
        }
    }

    void ShootEffect (RaycastHit hit_, bool missed_, float attackRange_ = 0.0f)
    {
        LineRenderer lr = null;

        if (!missed_)
        {
            Vector3[] pos = new Vector3[]
            {
                GetFirePoint.position,
                GetFirePoint.position + (GetFirePoint.forward * (hit_.distance / 2.0f)),
                hit_.point
            };

            lr = Instantiate (_shootPrefab, GetFirePoint.forward + GetFirePoint.position, Quaternion.identity);
            lr.SetPositions (pos);
            GameObject.Instantiate (_hitEffect, hit_.point, Quaternion.identity);
        }
        else
        {
            Vector3 endPos = GetFirePoint.position + GetFirePoint.forward * attackRange_;
            Vector3[] pos = new Vector3[]
            {
                GetFirePoint.position,
                GetFirePoint.position + GetFirePoint.forward * (attackRange_ / 2.0f),
                endPos
            };

            lr = Instantiate (_shootPrefab, GetFirePoint.forward + GetFirePoint.position, Quaternion.identity);
            lr.SetPositions (pos);
            GameObject.Instantiate (_hitEffect, endPos, Quaternion.identity);
        }

        Color2 cola = new Color2 (Color.white, Color.white);
        Color2 colb = new Color2 (Color.clear, Color.clear);
        lr.DOColor (cola, colb, _shootFadeTime);
        Destroy (lr.gameObject, _shootFadeTime + 0.5f);

    }

    public void HasBeenKilled (float timeBeforeKill_)
    {
        StartCoroutine (KillWait (timeBeforeKill_));
    }

    IEnumerator KillWait (float timeBeforeKill_)
    {
        yield return new WaitForSeconds (timeBeforeKill_);
        _ragdoll.ActivateRagDoll ();
        yield return new WaitForSeconds (_timeBeforeDestroy);
        _health.Damage (1000);
    }

    void DestroyGO ()
    {
        Destroy (gameObject);
        if (_IsWereWolf)
        {
            PlayerManager._instance.GameOver (true);
        }
    }

    [System.Serializable]
    class PlayerInput
    {
        [Title ("Player")]
        public string _movementX = "LAxisH";
        public string _movementY = "LAxisV";
        public string _targetRotationX = "RAxisH";
        public string _targetRotationY = "RAxisV";
        public string _actionA = "R1A";
        public string _actionB = "L1B";
        public string _actionX = "R2X";
        public string _actionY = "L2Y";
        [Title ("UI")]
        public string _InGameUIUp = "InGameUIDpadU";
        public string _InGameUIDown = "InGameUIDpadD";
        public string _InGameUIRight = "InGameUIDpadR";
        public string _InGameUILeft = "InGameUIDpadL";
        public string _InGameUISubmit = "InGameUISubmit";
        public string _InGameUINextCategory = "InGameUICancel";
    }

}
public interface ICharacterPlayer
{
    void HasBeenKilled (float timeBeforeKill_);
}