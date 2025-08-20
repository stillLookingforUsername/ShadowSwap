using System;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneTrigger : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private bool _hasPlayed = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerMovement2D player))
            {
                if (_hasPlayed) return;
                if (playableDirector != null)
                {
                    _hasPlayed = true;  //mark as done
                    playableDirector.Play();
                    PlayerMovement2D.Instance.enabled = false; //disable movement

                    //Unity’s PlayableDirector has a stopped callback that fires when the timeline finishes.
                    //when timeline stops, re-enable playermovement
                    playableDirector.stopped += OnCutSceneFinished;
                }
            }
        }

    private void OnCutSceneFinished(PlayableDirector director)
    {
        PlayerMovement2D.Instance.enabled = true;
        playableDirector.stopped -= OnCutSceneFinished; //unscribe to avoid multiple calls

        //optional destroy the trigger
        //Destroy(gameObject);
    }
}
