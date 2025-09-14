using UnityEngine;
public class OrbTransport : MonoBehaviour
{
    private Transform _player;
    private Rigidbody2D _playerRb;
    private bool _carryingPlayer = false;

    public bool IsCarryingPlayer()
    {
        return _carryingPlayer;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMovement2D player) && !_carryingPlayer)
        {
            _player = collision.transform;
            _playerRb = _player.GetComponent<Rigidbody2D>();

            // Parent to orb
            _player.SetParent(transform);
            _carryingPlayer = true;

            // SNAP player to orb center (optionally offset if needed)
            _player.localPosition = Vector3.zero;

            // Disable movement script
            var movement = _player.GetComponent<PlayerMovement2D>();
            if (movement) movement.enabled = false;

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
            _player.rotation = Quaternion.identity; //reset rotation

            // Re-enable player movement
            var movement = _player.GetComponent<PlayerMovement2D>();
            if (movement) movement.enabled = true;

            // Re-enable physics
            if (_playerRb)
            {
                _playerRb.simulated = true;
            }

            _player = null;
            _playerRb = null;
            _carryingPlayer = false;
        }
    }
}
