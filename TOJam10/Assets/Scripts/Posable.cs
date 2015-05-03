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
    Sassy,
    Cute
}

public class Posable : Tossable
{
    protected new BoxCollider collider;

    public float rotationSpeed = 0.1f;

    public PosableState state { get; private set;}
    private PoseAnimation poseAnimation;

    private Vector3 wanderDirection;
    public float wanderSpeed;
    public float wanderTime;

    public float idleTime;
    public float poseMinTime; //How long this person who still posed for at a minimum
    public float poseMaxTime; //How long this person will stay posed for at a maximum.
    private Timer stateTimer;

    private Ray debugRay;

    public Target posingTarget { set; private get;}

    public Hat expectedHat;
    private Hat currentHat;

    public Player player;

    Animator animator;


    public override void Start()
    {
        base.Start();

        this.collider = this.GetComponent<BoxCollider>();
        this.animator = this.transform.FindChild("personModel").GetComponent<Animator>();

        this.Wander();
    }

    public void Update()
    {
        //Debug.Log("Current state: " + this.state.ToString().ToUpper());

        this.animator.SetBool("Walking", this.state == PosableState.Wandering);
        this.animator.SetBool("Sass", this.poseAnimation == PoseAnimation.Sassy);
        this.animator.SetBool("Cute", this.poseAnimation == PoseAnimation.Cute);

        switch (this.state)
        {
            case PosableState.Helpless:
                if (this.IsGrounded() && this.stateTimer == null)
                {
                    this.stateTimer = Timer.Register(1f, this.GoToNextState);
                }
                else if (!this.IsGrounded() && this.stateTimer != null)
                {
                    this.stateTimer.Cancel();
                    this.stateTimer = null;
                }

                break;

            case PosableState.Wandering:
            case PosableState.Bored:
                this.rigidbody.velocity = Vector3.zero;
                this.rigidbody.AddForceAtPosition(this.wanderDirection * this.wanderSpeed, this.transform.position);

                if (!this.IsGrounded())
                {
                    Debug.Log("I'm falling!");
                    this.BecomeHelpless(null);
                }

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

    public void GoToNextState()
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

                this.Wander();

                break;

            case PosableState.Posing:
                this.EnterState(PosableState.Bored, 5f);
                this.wanderDirection = (this.transform.position - this.posingTarget.transform.position).SetY(0).normalized * 2;
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

    public void BecomeHelpless(Vector3? mousePosition)
    {
        this.rigidbody.velocity = Vector3.zero;

        this.EnterState(PosableState.Helpless);
    }

    public void Wander()
    {
        Vector2 wanderDir2d = Random.insideUnitCircle.normalized;
        this.wanderDirection = new Vector3(wanderDir2d.x, 0, wanderDir2d.y);
        this.rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(wanderDirection, Vector3.up)));

        this.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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
        return !Input.GetMouseButton(0) && Physics.Raycast(transform.position, -Vector3.up, this.collider.bounds.extents.y + 0.01f);
    }

    public void equip(Hat hat)
    {
        if (this.currentHat != null)
        {

            this.currentHat.GetComponent<Rigidbody>().isKinematic = false;
            this.currentHat.GetComponent<Collider>().enabled = true;

            this.currentHat.transform.parent = this.transform.parent;

            this.currentHat.GetComponent<Tossable>().GetTossed(this.transform.forward * 4);

            this.currentHat.RestorParent();

            this.currentHat = null;
        }
        else
        {
            hat.transform.SetParent(this.transform);

            Vector3 hatPos = this.transform.position;
            hatPos.y = this.getYOfHead() + hat.GetComponent<Collider>().bounds.extents.y;

            hat.GetComponent<Rigidbody>().isKinematic = true;
            hat.GetComponent<Collider>().enabled = false;
            hat.transform.position = hatPos;

            this.currentHat = hat;

            //this.player.TossHeldObject();
        }
    }

    private float getNewPoseTime()
    {
        return Random.Range(this.poseMinTime, this.poseMaxTime);
    }

    private float getYOfHead()
    {
        return this.transform.position.y + this.collider.bounds.extents.y;
    }

    void OnCollisionEnter(Collision c)
    {
        Hat hat = c.gameObject.GetComponent<Hat>();
        if (hat != null && hat != this.currentHat)
        {
            Debug.Log("Equipping a hat.");
            this.equip(hat);
        }
    }

    public override bool isSatisfied()
    {
        Debug.Log("The expected hat is : " + this.expectedHat + " and the current hat is " + this.currentHat);
        return this.currentHat == this.expectedHat;
    }
}
