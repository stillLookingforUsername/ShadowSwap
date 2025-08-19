using UnityEngine;

public class PlayerWalkingDust : MonoBehaviour
{
    public ParticleSystem walkingDust;
    private PlayerMovement2D _playerMovement2D;

    private void Awake()
    {
        _playerMovement2D = GetComponent<PlayerMovement2D>();
    }

    private void Update()
    {
        if (walkingDust == null) return;
        bool ground = _playerMovement2D.IsGrounded();
        bool moving = Mathf.Abs(_playerMovement2D.GetMoveInput()) > 0.1f;

        //emit dust only while walking on ground
        if (ground && moving && Mathf.Abs(_playerMovement2D.GetLinearVelocity().y) < 0.01f)
        {
            if (!walkingDust.isPlaying)
            {
                walkingDust.Play();
            }
        }
        else
        {
            if (walkingDust.isPlaying)
            {
                walkingDust.Stop();
            }
        }
    }
}