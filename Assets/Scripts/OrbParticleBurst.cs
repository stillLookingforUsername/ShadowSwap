using UnityEngine;
public class OrbParticleBurst : MonoBehaviour
{
    public ParticleSystem burstEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerMovement2D player))
        {
            if (burstEffect != null)
            {
                burstEffect.Play();
            }
        }
    }
}