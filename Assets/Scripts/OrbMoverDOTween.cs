using UnityEngine;
using DG.Tweening;

public class OrbMoverDOTween : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private Transform[] wayPoints;
    public float moveDuration = 3f;
    public PathType pathType = PathType.CatmullRom;
    public PathMode pathMode = PathMode.TopDown2D;

    public bool loop = true;
    public LoopType loopType = LoopType.Yoyo;

    private Tween _pathTween;
    private OrbTransport _orbTransport;
    private bool _hasmoved = false; //to ensure it only moves once

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasmoved) return;
        if (collision.TryGetComponent(out PlayerMovement2D player))
        {
            _hasmoved = true;
            StartMoving(); //there is one bug here after dropping the player, the orb moves again from B->A->B again
        }
    }

    private void StartMoving()
    {
        if (wayPoints.Length < 2) return;

        _orbTransport = GetComponent<OrbTransport>();

        Vector3[] pathPositions = new Vector3[wayPoints.Length];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            pathPositions[i] = wayPoints[i].position;
        }

        // Tween setup
        _pathTween = transform.DOPath(pathPositions, moveDuration, pathType, pathMode, 10, Color.green)
            .SetOptions(false)
            .SetLookAt(0.01f)
            .SetEase(Ease.InOutSine);

        if (loop)
            _pathTween.SetLoops(-1, loopType);
        else
            _pathTween.OnComplete(ReleasePlayerAtEnd);
    }

    private void Update()
    {
        if (!_orbTransport) return;

        // Release player when orb is close to final waypoint
        if (_orbTransport.IsCarryingPlayer())
        {
            Transform lastPoint = wayPoints[wayPoints.Length - 1];
            if (Vector3.Distance(transform.position, lastPoint.position) < 0.2f)
            {
                _orbTransport.ReleasePlayer();
            }
        }
    }

    private void ReleasePlayerAtEnd()
    {
        if (_orbTransport) _orbTransport.ReleasePlayer();
    }

    private void OnDestroy()
    {
        if (_pathTween != null && _pathTween.IsActive())
            _pathTween.Kill();
    }
}
