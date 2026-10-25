using UnityEngine;
public class OrbTransport : MonoBehaviour
{
    private Transform _player;
    private Rigidbody2D _playerRb;
    private bool _carryingPlayer = false;
    private PlayerMovement2D movement;
    private bool wasSimulated;

    public bool IsCarryingPlayer()
    {
        return _carryingPlayer;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.timeScale > 0f && collision.TryGetComponent(out PlayerMovement2D player) && !_carryingPlayer && player.CanAcceptInput)
        {
            _player = collision.transform;
            _playerRb = _player.GetComponent<Rigidbody2D>();
            movement = player;
            wasSimulated = _playerRb && _playerRb.simulated;
            var rope = player.GetComponent<PlayerRope>();
            if (rope) rope.DetachFromRope();
            player.SetCarrier(this);

            // Parent to orb
            _player.SetParent(transform);
            _carryingPlayer = true;

            // SNAP player to orb center (optionally offset if needed)
            _player.localPosition = Vector3.zero;

            // Disable movement script

            // Freeze physics
            if (_playerRb)
            {
                _playerRb.simulated = false;
                _playerRb.linearVelocity = Vector2.zero;
                _playerRb.angularVelocity = 0f;
            }
        }
    }

    public void ReleasePlayer()
    {
        if (_player)
        {
            _player.SetParent(null);

            // Re-enable player movement
            if (movement) movement.SetCarrier(null);

            // Re-enable physics
            if (_playerRb)
            {
                _playerRb.simulated = wasSimulated;
            }

        }
        _player = null;
        _playerRb = null;
        movement = null;
        _carryingPlayer = false;
    }

    private void OnDisable() => ReleasePlayer();
}
