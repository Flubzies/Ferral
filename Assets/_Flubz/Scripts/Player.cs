using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Title ("Player Movement")]
    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _movementDeadZone = 0.01f;
    [SerializeField] Rigidbody _playerRB;
    [SerializeField] string _cameraOffsetTag = "CameraOffset";

    [Title ("Player Rotation")]
    [SerializeField] float _rotSpeed = 1.0f;

    [Title ("Player UI")]
    [SerializeField] RectTransform _playerUI;
    [SerializeField] int _UIXPositionOffset = 620;

    [Title ("Player Health")]
    [SerializeField] Health _health;
    [SerializeField] Image _healthBar;
    [SerializeField] TMP_Text _playerID;
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
    [SerializeField][ReadOnly] Color _defaultHealthColor;

    Vector3 _MovementAxis { get; set; }
    Rewired.Player _Input { get { return PlayerManager._instance.GetRewiredPlayer (_GamePlayerID); } }

    public void Initialize (int gamePlayerID_)
    {
        if (!_playerRB) _playerRB = GetComponent<Rigidbody> ();
        _cameraOffset = GameObject.FindGameObjectWithTag (_cameraOffsetTag).transform;

        _health.OnDeath += OnDeath;
        _health.OnDamaged += OnDamaged;

        GetMoveSpeedModifer = 1.0f;
        GetRotSpeedModifer = 1.0f;
        _GamePlayerID = gamePlayerID_;
        _playerID.text = (gamePlayerID_ + 1).ToString ();

        _defaultHealthColor = _healthBar.color;
    }

    private void Update ()
    {
        if (!ReInput.isReady) return;
        if (_Input == null) return;

        PlayerMovement ();
        FlashHealthBar ();
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
        gameObject.SetActive (false);
        // PlayerManager._instance.RemovePlayer (this);
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
            _playerUI.position += (Vector3.right * _UIXPositionOffset);
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