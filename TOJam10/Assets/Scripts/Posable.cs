using System;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using Random = UnityEngine.Random;

public enum PosableState 
{
    Posing,
    Wandering,
    Idle,
    Bored,
    LayingDown,
    Helpless
}

public enum PoseAnimation
{
    Flattered
}

public class Posable : Tossable
{
    protected new BoxCollider collider;

    public float rotationSpeed = 0.1f;

    private PosableState state;
    private PoseAnimation poseAnimation;

    private Vector3 wanderDirection;
    public float wanderSpeed;
    public float wanderTime;

    public float idleTime;
    public float posMinTime; //How long this person who still posed for at a minimum
    public float posMaxTime; //How long this person will stay posed for at a maximum.
    private Timer stateTimer;

    public override void Start()
    {
        base.Start();

        this.collider = this.GetComponent<BoxCollider>();
        this.Wander();
    }

    public void Update()
    {
        //Debug.Log("Current state: " + this.state.ToString().ToUpper());

        switch (this.state)
        {
            case PosableState.Helpless:
                if (this.IsGrounded() && this.stateTimer == null)
                {
                    this.stateTimer = Timer.Register(1f, this.GoToNextState);
                    Debug.Log("I hit the ground");
                }
                else if (!this.IsGrounded() && this.stateTimer != null)
                {
                    this.stateTimer.Cancel();
                    this.stateTimer = null;
                    Debug.Log("I left the ground");
                }

                break;

            case PosableState.Wandering:
                this.rigidbody.velocity = Vector3.zero;
                this.rigidbody.AddForceAtPosition(this.wanderDirection*this.wanderSpeed, this.transform.position);
                break;
            case PosableState.Posing:
                this.transform.Rotate(Vector3.up, 10);
                break;
        }
    }

    private void EnterState(PosableState state, float duration = -1)
    {
        if (this.stateTimer != null)
        {
            this.stateTimer.Cancel();
            this.stateTimer = null;
        }

        this.state = state;

        if (!duration.ApproximatelyEquals(-1))
        {
            this.stateTimer = Timer.Register(duration, this.GoToNextState);
        }
    }

    private void GoToNextState()
    {
        switch (this.state)
        {
            case PosableState.Wandering:
                this.Idle();
                break;

            case PosableState.Idle:
                this.Wander();
                break;

            case PosableState.Helpless:
                this.rigidbody.rotation = Quaternion.Euler(Vector3.zero);
                this.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                this.Wander();

                break;

            default:
                this.Wander();
                break;
        }
    }

    public void Pose(PoseAnimation anim)
    {
        Debug.Log("Starting to pose.");
        this.poseAnimation = anim;

        this.EnterState(PosableState.Posing, this.getNewPoseTime());
    }

    public void BecomeHelpless(Vector3 mousePosition)
    {
        this.rigidbody.velocity = Vector3.zero;

        this.EnterState(PosableState.Helpless);
    }

    public void Wander()
    {
        Vector2 wanderDir2d = Random.insideUnitCircle.normalized;
        this.wanderDirection = new Vector3(wanderDir2d.x, 0, wanderDir2d.y);

        this.EnterState(PosableState.Wandering, duration: this.wanderTime);
    }

    public void Idle()
    {
        this.EnterState(PosableState.Idle, duration: this.idleTime);
    }

    public override void GetPickedUp(Vector3 mousePos)
    {
        this.BecomeHelpless(mousePos);
    }

    public override void GetTossed(Vector3 dirAndSpeed)
    {
        base.GetTossed(dirAndSpeed);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, this.collider.bounds.extents.y * 2 + 0.1f);
    }

    private float getNewPoseTime()
    {
        return Random.Range(this.posMinTime, this.posMaxTime);
    }
}
