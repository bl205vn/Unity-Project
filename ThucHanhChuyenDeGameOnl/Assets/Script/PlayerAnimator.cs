using Fusion;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private Animator _animator;
    private PlayerMovement _playerMovement;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();

        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public override void Render()
    {
        if (_animator == null || _playerMovement == null) return;

        // Đẩy giá trị từ Networked Properties sang Animator cục bộ mỗi khung hình màn hình để mượt mà
        
        // Với tốc độ (Speed), nội suy mềm mại giữa giá trị cũ và mới để khớp với thời gian chuyển động
        float currentSpeed = _animator.GetFloat(_animIDSpeed);
        float targetSpeed = _playerMovement.AnimSpeed;
        _animator.SetFloat(_animIDSpeed, Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f));

        _animator.SetFloat(_animIDMotionSpeed, _playerMovement.AnimMotionSpeed);
        
        _animator.SetBool(_animIDGrounded, _playerMovement.AnimGrounded);
        _animator.SetBool(_animIDJump, _playerMovement.AnimJump);
        _animator.SetBool(_animIDFreeFall, _playerMovement.AnimFreeFall);
    }
}
