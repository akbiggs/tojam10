using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
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
    None = 0,
    Sassy = 1,
    Cute = 2,
    Smack = 3,
    Cool = 4
}

public class Posable : Tossable
{
    public List<string> texts = new List<string>()
    {
        "This is a nice home."
    };

    protected new BoxCollider collider;

    public float rotationSpeed = 0.1f;

    public PosableState state { get; private set;}
    private PoseAnimation poseAnimation;

    private Vector3 wanderDirection;
    public float wanderSpeed;
    public float wanderMinTime;
    public float wanderMaxTime;

    public float rotateSpeed = 200;

    public float idleTime;
    public float poseMinTime; //How long this person who still posed for at a minimum
    public float poseMaxTime; //How long this person will stay posed for at a maximum.

    public float quipMinTime;
    public float quipMaxTime;
    
    private Timer stateTimer;

    private Ray debugRay;

    public Target posingTarget;

    public Hat expectedHat;
    private Hat currentHat;
    private Hat tossedHat;

    public GameObject headEnd;

    public Player player;

    public Animator animator;

    public bool startsNaked = false;
    public bool needsToBePainted = false;

    private bool currentlyNaked;
    private bool currentlyPainted;

    public Renderer skinRenderer;
    public Material suitMaterial;
    public Material paintMaterial;
    private Material originalMaterial;

    // Rotation we should blend towards.
    private Quaternion _targetRotation = Quaternion.identity;

    public override void Start()
    {
        base.Start();

        this.collider = this.GetComponent<BoxCollider>();
        this.animator = this.transform.FindChild("personModel").GetComponent<Animator>();

        this.poseAnimation = PoseAnimation.None;

        this.currentlyNaked = this.startsNaked;
        this.currentlyPainted = !this.needsToBePainted;

        this.Wander();

        this.PlayRandomQuipAfterTime();

        this.originalMaterial = this.skinRenderer.material; 
    }

    public void PlayRandomQuipAfterTime()
    {
        Timer.Register(Random.Range(this.quipMinTime, this.quipMaxTime), () =>
        {
            if (this.gameObject.name.ToLower().Contains("child"))
            {
                SoundManager.PlayRandomSound(SoundManager.instance.childSounds, this.transform.position);
            }
            else if (this.gameObject.name.ToLower().Contains("alien"))
            {
                SoundManager.PlayRandomSound(SoundManager.instance.lonelySounds, this.transform.position);
            }
            else
            {
                SoundManager.PlayRandomSound(SoundManager.instance.niceHomeSounds, this.transform.position);
            }

            this.PlayRandomQuipAfterTime();
        });
    }
    public void Update()
    {
        Debug.Log("Current state: " + this.state.ToString().ToUpper());

        this.animator.SetBool("Walking", this.state == PosableState.Wandering || this.state == PosableState.Bored);
        this.animator.SetBool("Idle", this.state == PosableState.Idle);
        this.animator.SetBool("PickedUp", this.state == PosableState.Helpless);

        this.animator.SetBool("Sass", this.poseAnimation == PoseAnimation.Sassy);
        this.animator.SetBool("Cute", this.poseAnimation == PoseAnimation.Cute);
        this.animator.SetBool("Smack", this.poseAnimation == PoseAnimation.Smack);
        this.animator.SetBool("Cool", this.poseAnimation == PoseAnimation.Cool);

        switch (this.state)
        {
            case PosableState.Helpless:
                if (this.IsGrounded() && this.stateTimer == null)
                {
                    this.GoToNextState();
                    //this.stateTimer = Timer.Register(0.5f, this.GoToNextState);
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

                // Turn towards our target rotation.
               this. transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, this.rotateSpeed * Time.deltaTime);

                //if (!this.IsGrounded())
                //{
                //    Debug.Log("I'm falling!");
                //    this.BecomeHelpless(null);
                //}

                break;

            case PosableState.Posing:
                //this.transform.Rotate(Vector3.up, 10);
                this.transform.rotation = Quaternion.Euler(Vector3.forward);
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
        this.poseAnimation = PoseAnimation.None;
        
        switch (this.state)
        {
            case PosableState.Wandering:
                this.Idle();
                break;

            case PosableState.Idle:
                this.Wander();
                break;

            case PosableState.Helpless:
                //this.rigidbody.rotation = Quaternion.Euler(Vector3.zero);

                this.Idle();

                break;

            case PosableState.Posing:
                this.EnterState(PosableState.Bored, 2f);
                this.wanderDirection = (this.transform.position - this.posingTarget.transform.position).SetY(0).normalized * 2;

                this.SetBlendedEulerAngles(Quaternion.LookRotation(Vector3.Cross(wanderDirection, Vector3.up)).eulerAngles);
                this.rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(this.wanderDirection, Vector3.up)));
                break;

            default:
                this.Wander();
                break;
        }
    }

    // Call this when you want to turn the object smoothly.
    public void SetBlendedEulerAngles(Vector3 angles)
    {
        _targetRotation = Quaternion.Euler(angles);
    }

    public void Pose(PoseAnimation anim)
    {
        Debug.Log("Starting to pose.");
        this.poseAnimation = anim;

        this.EnterState(PosableState.Posing, this.GetNewPoseTime());

        SoundManager.PlayRandomSound(SoundManager.instance.poseSounds, this.transform.position);
    }

    public void BecomeHelpless(Vector3? mousePosition)
    {
        this.rigidbody.velocity = Vector3.zero;
        this.poseAnimation = PoseAnimation.None;

        this.EnterState(PosableState.Helpless);
    }

    public void Wander()
    {
        Vector2 wanderDir2d = Random.insideUnitCircle.normalized;
        this.wanderDirection = new Vector3(wanderDir2d.x, 0, wanderDir2d.y);
        this.SetBlendedEulerAngles(Quaternion.LookRotation(Vector3.Cross(wanderDirection, Vector3.up)).eulerAngles);

        this.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        this.EnterState(PosableState.Wandering, duration: Random.Range(this.wanderMinTime, this.wanderMaxTime));
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
        return !Input.GetMouseButton(0) && Physics.Raycast(transform.position, -Vector3.up, this.collider.bounds.extents.y + 0.15f);
    }

    public void Equip(Hat hat)
    {
        if (hat != this.tossedHat)
        {
            if (this.currentHat != null)
            {
                this.DropEquippedHat();
            }

            Debug.Log("Equipping new hat: " + hat);
            hat.transform.SetParent(this.headEnd.transform);

            hat.GetComponent<Rigidbody>().isKinematic = true;
            //hat.GetComponent<Collider>().enabled = false;
            hat.transform.localPosition = Vector3.zero.SetY(hat.offsetFromHead);
            hat.transform.localRotation = Quaternion.Euler(new Vector3(270, 90, 0));

            this.currentHat = hat;
            this.currentHat.owner = this.gameObject;

            // play quips about hat
            if (this.currentHat.name == "Pinapple")
            {
                SoundManager.MaybePlayRandomSound(SoundManager.instance.pineappleSounds, this.transform.position, 0.2f);
            }
        }
    }

    public void DropEquippedHat()
    {
        Debug.Log("Current hat is " + this.currentHat + "which should be getting thrown.");
        this.currentHat.GetComponent<Rigidbody>().isKinematic = false;
        //this.currentHat.GetComponent<Collider>().enabled = true;

        this.currentHat.transform.parent = this.transform.parent;

        this.currentHat.GetComponent<Tossable>().GetTossed(this.transform.forward * 4);

        this.currentHat.RestoreParent();

        this.tossedHat = this.currentHat;
        this.tossedHat.owner = null;
        Timer.Register(0.5f, () => { this.tossedHat = null; });
        this.currentHat = null;
    }

    private float GetNewPoseTime()
    {
        return Random.Range(this.poseMinTime, this.poseMaxTime);
    }

    private float GetYOfHead()
    {
        return this.transform.position.y + this.collider.bounds.extents.y;
    }

    void OnCollisionEnter(Collision c)
    {
        Hat hat = c.gameObject.GetComponent<Hat>();
        if (hat != null && hat != this.currentHat)
        {
            Debug.Log("Equipping a hat.");
            this.Equip(hat);

            return;
        }

        if (this.startsNaked)
        {
            ClothingRack clothingRack = c.gameObject.GetComponent<ClothingRack>();
            if (clothingRack != null)
            {
                this.skinRenderer.material = this.suitMaterial;
                this.currentlyNaked = false;

                return;
            }
        }

        PaintCan paintCan = c.gameObject.GetComponent<PaintCan>();
        if (paintCan != null)
        {
            this.skinRenderer.material = this.paintMaterial;
            this.currentlyPainted = true;

            return;
        }

        WaterBucket waterBucket = c.gameObject.GetComponent<WaterBucket>();
        if (waterBucket != null && this.currentlyPainted)
        {
            this.skinRenderer.material = this.originalMaterial;
            this.currentlyPainted = false;
        }
    }

    override public int getNumSatisfy()
    {
        Debug.Log("Checking satisfaction for " + this);

        int count = 0;
        if (this.currentHat == this.expectedHat)
        {
            Debug.Log("  The hats match");
            count++;
        }

        if (this.startsNaked && !this.currentlyNaked)
        {
            Debug.Log("  The nakedness matches");
            count++;
        }

        if (this.needsToBePainted && this.currentlyPainted)
        {
            Debug.Log("  The paint matches.");
            count++;
        }

        Debug.Log("  expected " + this.getTotalToSatisfy() + " matches and got " + count);
        return count;
    }

    override public int getTotalToSatisfy()
    {
        int count = 1;

        if (this.startsNaked)
            count++;

        if (this.needsToBePainted)
            count++;

        return count;
    }
}
