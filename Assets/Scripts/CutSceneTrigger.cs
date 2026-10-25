using System;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneTrigger : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private bool _hasPlayed = false;
    private PlayerMovement2D controlledPlayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerMovement2D player))
            {
                if (_hasPlayed) return;
                if (playableDirector != null)
                {
                    _hasPlayed = true;  //mark as done
                    controlledPlayer = player;
                    player.AcquireControlLock(this);
                    player.OnPlayerRespawn += OnPlayerRespawn;

                    //Unity’s PlayableDirector has a stopped callback that fires when the timeline finishes.
                    //when timeline stops, re-enable playermovement
                    playableDirector.stopped += OnCutSceneFinished;
                    playableDirector.Play();
                }
            }
        }

    private void OnCutSceneFinished(PlayableDirector director)
    {
        ReleaseControl();

        //optional destroy the trigger
        //Destroy(gameObject);
    }

    private void OnPlayerRespawn(object sender, EventArgs args)
    {
        if (playableDirector) playableDirector.Stop();
        ReleaseControl();
    }

    private void ReleaseControl()
    {
        if (playableDirector) playableDirector.stopped -= OnCutSceneFinished;
        if (controlledPlayer)
        {
            controlledPlayer.ReleaseControlLock(this);
            controlledPlayer.OnPlayerRespawn -= OnPlayerRespawn;
        }
        controlledPlayer = null;
    }

    private void OnDisable() => ReleaseControl();
}
