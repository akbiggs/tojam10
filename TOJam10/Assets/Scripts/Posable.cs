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

public class Posable : MonoBehaviour
{
    public float rotationSpeed = 0.1f;

    private PosableState state;
    private PoseAnimation poseAnimation;

    private new Rigidbody rigidbody;

    private Vector3 wanderDirection;
    public float wanderSpeed;
    public float wanderTime;

    public float idleTime;

    public void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();

        this.Wander();
    }

    public void Update()
    {
        Debug.Log("Current state: " + this.state.ToString().ToUpper());
        switch (this.state)
        {
            case PosableState.Helpless:
                this.transform.Rotate(Vector3.up, this.rotationSpeed);
                break;

            case PosableState.Wandering:
                this.rigidbody.velocity = Vector3.zero;
                this.rigidbody.AddForceAtPosition(this.wanderDirection*this.wanderSpeed, this.transform.position);
                break;
        }
    }

    public void EnterState(PosableState state, float duration = -1)
    {
        this.state = state;

        if (!duration.ApproximatelyEquals(-1))
        {
            Timer.Register(duration, this.GoToNextState);
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
        }
    }

    public void Pose(PoseAnimation anim)
    {
        this.poseAnimation = anim;

        this.EnterState(PosableState.Posing);
    }

    public void BecomeHelpless(Vector3 mousePosition)
    {
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
}
