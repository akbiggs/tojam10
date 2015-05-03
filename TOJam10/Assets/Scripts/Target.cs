using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Target : Satisfiable
{
    private Timer poseTimer;

    //Who I expect to stand on top of me.
    public Posable expectedPosable;
    Posable currentPosable;

    public float timeUntilPoseState = 0.3f;

    void OnTriggerStay(Collider c)
    {
        Posable posable = c.gameObject.GetComponent<Posable>();

        if (posable != null && posable != this.currentPosable &&
            posable.state != PosableState.Helpless && posable.state != PosableState.Bored && 
            this.poseTimer == null)
        {
            this.currentPosable = posable;

            this.poseTimer = Timer.Register(timeUntilPoseState, () =>
            {
                int pose = Random.Range(1, Enum.GetValues(typeof (PoseAnimation)).Length);

                posable.Pose((PoseAnimation) pose);
                posable.posingTarget = this;

                this.poseTimer = null;
            });

            posable.GoToNextState();
        }
    }

    void OnTriggerExit(Collider c)
    {

        if (c.gameObject.GetComponent<Posable>() == this.currentPosable)
        {
            this.currentPosable = null;

            if (this.poseTimer != null)
            {
                this.poseTimer.Cancel();
                this.poseTimer = null;
            }
        }
    }

    public override bool isSatisfied()
    {
        return (this.currentPosable == this.expectedPosable) && (this.expectedPosable.state == PosableState.Posing);
    }

    public override bool IsActive()
    {
        return true;
    }
}
