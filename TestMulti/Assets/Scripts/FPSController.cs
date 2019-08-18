using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private float _jumpDistance = 5f;
    [SerializeField] private float _forwardBackwardSpeed;
    [SerializeField] private float _strafeSpeed;

    [SerializeField] private float _crowlSpeedMalusMultiplier = 0.4f;
    private float _defaultCrowlSpeedMultiplier = 1f;
    private float _crowlSpeed;

    [SerializeField] private float _climbingSpeed = 1f;
    [SerializeField] private float _climbingStrafeSpeed = 0.3f;

    [SerializeField] private float _cameraSensitivity = 2f;
    private float _rotX;
    private float _rotY;
    private float _verticalVelocity;

    [SerializeField] private PhotonTransformView _photonTransformView;
    [SerializeField] private PhotonView _photonView;

    [SerializeField] private CharacterController _characterController;

    private Vector3 m_LastPosition;
    [SerializeField] private Animator _animator;
    private float _animatorSpeed;

    private Vector3 _currentMovement;
    private Vector3 _sideMovement;
    private Vector3 _frontBackMovement;
    private float _currentTurnSpeed;
    private float _moveForwardBackward;
    private float _moveSideWay;

    [SerializeField] private bool isGrounded;

    [SerializeField] private CharacterMovingStates CharacterState;

    void Start()
    {
        _crowlSpeed = 1;
    }

    void Update()
    {
        if (_photonView.isMine == true)
        {
            switch (CharacterState)
            {
                case CharacterMovingStates.Idle:

                    ResetSpeedValues();

                    if (Input.GetAxis("Vertical") > 0.05f || Input.GetAxis("Vertical") < -0.05f || 
                        Input.GetAxis("Horizontal") > 0.05f || Input.GetAxis("Horizontal") < -0.05f)
                    {
                        CharacterState = CharacterMovingStates.Running;
                    }

                    if (Input.GetKeyDown("c"))
                    {
                        CharacterState = CharacterMovingStates.Crowl;
                    }

                    UpdateRotationAndMovement();
                    UpdateJumpMovement();
                    MoveCharacterController();

                    break;

                case CharacterMovingStates.Running:

                    ResetSpeedValues();

                    if (Input.GetAxis("Vertical") < 0.05f && Input.GetAxis("Vertical") > -0.05f &&
                        Input.GetAxis("Horizontal") < 0.05f && Input.GetAxis("Horizontal") > -0.05f )
                    {
                        CharacterState = CharacterMovingStates.Idle;
                    }

                    if (Input.GetKeyDown("c"))
                    {
                        CharacterState = CharacterMovingStates.Crowl;
                    }

                    UpdateRotationAndMovement();
                    UpdateJumpMovement();
                    MoveCharacterController();

                    break;

                case CharacterMovingStates.Climbing:

                    ResetSpeedValues();

                    UpdateClimbingMovement();
                    MoveCharacterController();

                    break;

                case CharacterMovingStates.Crowl:

                    ResetSpeedValues();

                    ApplyCrowlModifier();

                    UpdateRotationAndMovement();
                    MoveCharacterController();

                    if (Input.GetKeyDown("c"))
                    {
                        GetBackToNormalAfterCrowl();
                        CharacterState = CharacterMovingStates.Idle;
                    }

                    break;
            }

            ApplySynchronizedValues();
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!_characterController.isGrounded)
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        isGrounded = _characterController.isGrounded;
    }

    void ResetSpeedValues()
    {
        _currentMovement = new Vector3(Vector3.zero.x, _verticalVelocity, Vector3.zero.z);
        _currentTurnSpeed = 0;
    }

    void ApplySynchronizedValues()
    {
        _photonTransformView.SetSynchronizedValues(_currentMovement, _currentTurnSpeed);
    }

    void ApplyGravityToCharacterController()
    {
        _characterController.Move(transform.up * Time.deltaTime * -9.81f);
    }

    void MoveCharacterController()
    {
        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    void UpdateClimbingMovement()
    {
        _moveForwardBackward = Input.GetAxis("Vertical") * _climbingSpeed;
        _moveSideWay = Input.GetAxis("Horizontal") * _climbingStrafeSpeed;

        _rotX = Input.GetAxis("Mouse X") * _cameraSensitivity;
        _rotY -= Input.GetAxis("Mouse Y") * _cameraSensitivity;

        _rotY = Mathf.Clamp(_rotY, -50f, 70f);

        Vector3 movement = new Vector3(_moveSideWay, _moveForwardBackward, _moveForwardBackward / 2);
        transform.Rotate(0, _rotX, 0);
        _camera.transform.localRotation = Quaternion.Euler(_rotY, 0, 0);

        _currentMovement = transform.rotation * movement;
    }

    void ApplyCrowlModifier()
    {
        _characterController.height = 1;
        _crowlSpeed = _crowlSpeedMalusMultiplier;
    }

    void GetBackToNormalAfterCrowl()
    {
        _characterController.height = 2;
        _crowlSpeed = _defaultCrowlSpeedMultiplier;
    }

    void UpdateRotationAndMovement()
    {
        _moveForwardBackward = Input.GetAxis("Vertical") * _forwardBackwardSpeed * _crowlSpeed;
        _moveSideWay = Input.GetAxis("Horizontal") * _strafeSpeed * _crowlSpeed;

        _rotX = Input.GetAxis("Mouse X") * _cameraSensitivity;
        _rotY -= Input.GetAxis("Mouse Y") * _cameraSensitivity;

        _rotY = Mathf.Clamp(_rotY, -50f, 70f);

        Vector3 movement = new Vector3(_moveSideWay, _verticalVelocity, _moveForwardBackward);
        transform.Rotate(0, _rotX, 0);
        _camera.transform.localRotation = Quaternion.Euler(_rotY, 0, 0);

        _currentMovement = transform.rotation * movement;
    }

    void UpdateJumpMovement()
    {
        if (_characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _verticalVelocity = _jumpDistance;
            }
        }
    }

     void UpdateAnimation()
    {
        Vector3 movementVector = transform.position - m_LastPosition;

        float speed = Vector3.Dot(movementVector.normalized, transform.forward);
        float direction = Vector3.Dot(movementVector.normalized, transform.right);

        if (Mathf.Abs(speed) < 0.2f)
        {
            speed = 0f;
        }

        if (speed > 0.6f)
        {
            speed = 1f;
            direction = 0f;
        }

        if (speed >= 0f)
        {
            if (Mathf.Abs(direction) > 0.7f)
            {
                speed = 1f;
            }
        }

        _animatorSpeed = Mathf.MoveTowards(_animatorSpeed, speed, Time.deltaTime * 5f);

        _animator.SetFloat("Speed", _animatorSpeed);
        _animator.SetFloat("Direction", direction);

        m_LastPosition = transform.position;
    }

    [PunRPC]
    void SwitchState(CharacterMovingStates state)
    {
        CharacterState = state;
    }
}
