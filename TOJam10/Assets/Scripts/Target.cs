using UnityEngine;
using System.Collections;

public class Target : Satisfiable
{
    private Timer poseTimer;

    //Who I expect to stand on top of me.
    public Posable expectedPosable;
    Posable currentPosable;

    void OnTriggerStay(Collider c)
    {
        Posable posable = c.gameObject.GetComponent<Posable>();

        if (posable != null && posable != this.currentPosable &&
            posable.state != PosableState.Helpless && posable.state != PosableState.Bored && 
            this.poseTimer == null)
        {
            this.currentPosable = posable;

            this.poseTimer = Timer.Register(0.5f, () =>
            {
                posable.Pose(PoseAnimation.Sassy);
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
}
