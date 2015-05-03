﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    None,
    Sassy,
    Cute
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
    public float wanderTime;

    public float idleTime;
    public float poseMinTime; //How long this person who still posed for at a minimum
    public float poseMaxTime; //How long this person will stay posed for at a maximum.
    private Timer stateTimer;

    private Ray debugRay;

    public Target posingTarget;

    public Hat expectedHat;
    private Hat currentHat;
    private Hat tossedHat;

    public Player player;

    public Animator animator;

    public bool isNaked = false;

    public Renderer skinRenderer;
    public Material suitMaterial;

    public override void Start()
    {
        base.Start();

        this.collider = this.GetComponent<BoxCollider>();
        this.animator = this.transform.FindChild("personModel").GetComponent<Animator>();

        this.poseAnimation = PoseAnimation.None;

        this.Wander();
    }

    public void Update()
    {
        Debug.Log("Current state: " + this.state.ToString().ToUpper());

        this.animator.SetBool("Walking", this.state == PosableState.Wandering || this.state == PosableState.Bored);
        this.animator.SetBool("Idle", this.state == PosableState.Idle);
        this.animator.SetBool("PickedUp", this.state == PosableState.Helpless);

        this.animator.SetBool("Sass", this.poseAnimation == PoseAnimation.Sassy);
        this.animator.SetBool("Cute", this.poseAnimation == PoseAnimation.Cute);

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

        this.EnterState(PosableState.Posing, this.GetNewPoseTime());
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
        return !Input.GetMouseButton(0) && Physics.Raycast(transform.position, -Vector3.up, this.collider.bounds.extents.y + 0.15f);
    }

    public void Equip(Hat hat)
    {
        if (hat != this.tossedHat)
        {
            if (this.currentHat != null)
            {
                Debug.Log("Current hat is " + this.currentHat + "which should be getting thrown.");
                this.currentHat.GetComponent<Rigidbody>().isKinematic = false;
                this.currentHat.GetComponent<Collider>().enabled = true;

                this.currentHat.transform.parent = this.transform.parent;

                this.currentHat.GetComponent<Tossable>().GetTossed(this.transform.forward*4);

                this.currentHat.RestoreParent();

                this.tossedHat = this.currentHat;
                Timer.Register(0.5f, () => { this.tossedHat = null; });
                this.currentHat = null;
            }

            Debug.Log("Equipping new hat: " + hat);
            hat.transform.SetParent(this.transform);

            Vector3 hatPos = this.transform.position;
            hat.transform.localRotation = Quaternion.Euler(new Vector3(270, 90, 0));
            hatPos.y = this.GetYOfHead() + hat.GetComponent<Collider>().bounds.extents.y;

            hat.GetComponent<Rigidbody>().isKinematic = true;
            hat.GetComponent<Collider>().enabled = false;
            hat.transform.position = hatPos;

            this.currentHat = hat;
        }
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
        }

        if (this.isNaked)
        {
            ClothingRack clothingRack = c.gameObject.GetComponent<ClothingRack>();
            if (clothingRack != null)
            {
                this.skinRenderer.material = this.suitMaterial;
            }
        }
    }

    public override bool isSatisfied()
    {
        Debug.Log("The expected hat is : " + this.expectedHat + " and the current hat is " + this.currentHat);
        return this.currentHat == this.expectedHat;
    }
}
