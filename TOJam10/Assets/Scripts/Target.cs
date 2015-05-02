using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    private Timer poseTimer;
    private Posable firstEnteredObject;

    void OnTriggerStay(Collider c)
    {
        Posable posable = c.gameObject.GetComponent<Posable>();

        if (posable != null && posable != this.firstEnteredObject &&
            posable.state != PosableState.Helpless && posable.state != PosableState.Bored && 
            this.poseTimer == null)
        {
            this.firstEnteredObject = posable;
            posable.posingTarget = this;

            this.poseTimer = Timer.Register(0.5f, () =>
            {
                posable.Pose(PoseAnimation.Flattered);
                this.poseTimer = null;
            });
        }
    }

    void OnTriggerExit(Collider c)
    {
        
        if (c.gameObject.GetComponent<Posable>() == this.firstEnteredObject)
        {
            this.firstEnteredObject = null;

            if (this.poseTimer != null)
            {
                this.poseTimer.Cancel();
                this.poseTimer = null;
            }
        }
    }
}
