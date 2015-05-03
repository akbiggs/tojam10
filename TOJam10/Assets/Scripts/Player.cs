using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityStandardAssets.ImageEffects;

public enum PlayerState
{
    Playing,
    TakingPhoto,
    ReviewingPhoto
}

public class Player : MonoBehaviour
{
    public static Player instance;

    private Vector3 previousHeldPosition;
    private Vector3 currentHeldPosition;

    private Vector3 previousMousePosition;
    private Vector3 currentMousePosition;

    private Tossable heldTossable;
    public float holdHeight;

    public bool showStart;
    public GameObject startCanvas;
    private BlurOptimized cameraBlur;

    public BoxCollider screenToWorldMap;

    public float lengthOfFlash; //How long the flash will go off for.

    public Lamp[] lamps;
    private Timer flashTimer;

    public Camera snapCamera;
    public Animator takePhotoAnimator;

    public Texture2D cursorCanGrab;
    public Texture2D cursorDoingGrab;
    public Texture2D cursorDefault;

    private PlayerState state;

    void Awake()
    {
        Player.instance = this;

        this.previousMousePosition = this.currentMousePosition = Input.mousePosition;

        this.state = PlayerState.Playing;

        LevelController.instance.interactionOnPause = this.showStart;
    }

    void Start()
    {
       this.cameraBlur =  Camera.main.GetComponent<BlurOptimized>();

       if (showStart)
       {
           this.cameraBlur.enabled = true;
           this.startCanvas.SetActive(true);
       }
    }

    void Update()
    {
	    this.previousMousePosition = this.currentMousePosition;
	    this.currentMousePosition = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("When space was pressed, state was: " + this.state);

            if (LevelController.instance.wonThisLevel)
            {
                LevelController.instance.FadeToNextLevel();
                Timer.Register(1, LevelController.instance.NextLevel, false);
                LevelController.instance.fadeoutCanvas.gameObject.SetActive(true);
            }
            else if (this.state == PlayerState.Playing)
            {
                if (!LevelController.instance.interactionOnPause)
                {
                    Debug.Log("SNAP PHOTO");
                    this.SnapPhoto();
                }
            }
            else
            {
                this.state = PlayerState.Playing;

                this.takePhotoAnimator.gameObject.SetActive(false);
                this.snapCamera.enabled = true;

                this.cameraBlur.enabled = false;

                LevelController.instance.interactionOnPause = false;

                Debug.Log("Returning state to : " + this.state);
            }
        }
    }

	void FixedUpdate()
	{
        Tossable tossable = null;

        Ray cameraRay = Camera.main.ScreenPointToRay(this.currentMousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(cameraRay, out hitInfo))
        {
            tossable = hitInfo.transform.GetComponent<Tossable>();
        }


        if (tossable != null)
        {
            if (!LevelController.instance.interactionOnPause && Input.GetMouseButtonDown(0))
            {
                Hat hat = tossable.GetComponent<Hat>();
                if (hat != null && hat.owner != null)
                {
                    hat.owner.GetComponent<Posable>().DropEquippedHat();
                }
                else
                {
                    this.PickUp(tossable);
                }
            }
            //Cursor.SetCursor(this.cursorCanGrab,new Vector2(0, 0), CursorMode.Auto);
        }
        else// if (!this.cursorDoingGrab)
        {
            //Cursor.SetCursor(this.cursorDefault, new Vector2(0, 0), CursorMode.Auto);
        }

        if (!LevelController.instance.interactionOnPause)
        {
            if (Input.GetMouseButtonUp(0) && this.heldTossable != null)
            {
                this.TossHeldObject();
            }
            else if (Input.GetMouseButton(0) && this.heldTossable != null)
            {
                this.heldTossable.GetComponent<Rigidbody>().velocity = Vector3.zero;

                Vector3 previousMouseWorldPos = this.GetMouseWorldPosition(this.previousMousePosition);
                Vector3 currentMouseWorldPos = this.GetMouseWorldPosition(this.currentMousePosition);

                this.heldTossable.rigidbody.AddForce((currentMouseWorldPos - previousMouseWorldPos) * 10000);
                this.heldTossable.rigidbody.position = this.heldTossable.rigidbody.position.SetY(this.holdHeight);
                this.heldTossable.rigidbody.velocity.Clamp(20);

                this.previousHeldPosition = this.currentHeldPosition;
                this.currentHeldPosition = this.heldTossable.transform.position;
            }
        }
	}

    private Vector3 GetMouseWorldPosition(Vector3 mousePosition)
    {
        Vector3 mouseViewportPos = Camera.main.ScreenToViewportPoint(mousePosition).Clamp01();

        float xMin = this.screenToWorldMap.transform.position.x + this.screenToWorldMap.bounds.extents.x;
        float xMax = this.screenToWorldMap.transform.position.x - this.screenToWorldMap.bounds.extents.x;
        float zMin = this.screenToWorldMap.transform.position.z - this.screenToWorldMap.bounds.extents.z;
        float zMax = this.screenToWorldMap.transform.position.z + this.screenToWorldMap.bounds.extents.z;

        return new Vector3(
            Mathf.Lerp(xMin, xMax, mouseViewportPos.y),
            this.screenToWorldMap.transform.position.y, 
            Mathf.Lerp(zMin, zMax, mouseViewportPos.x));
    }

    private void PickUp(Tossable tossable)
    {
        this.heldTossable = tossable;
        this.heldTossable.GetPickedUp(Input.mousePosition);

        Cursor.visible = false;
    }

    public void TossHeldObject()
    {
        if (this.heldTossable != null)
        {
            this.heldTossable.GetTossed(this.currentHeldPosition - this.previousHeldPosition);
            this.heldTossable = null;
        }
    }

    private void SnapPhoto()
    {
        //Do the flash.
        foreach (Lamp lamp in this.lamps) {
            lamp.light.enabled = true;
        }

        this.takePhotoAnimator.gameObject.SetActive(true);

        // take the photo just before the flash goes off
        Timer.Register(this.lengthOfFlash - 0.09f, () =>
        {
            this.snapCamera.enabled = false;
        });

        this.flashTimer = Timer.Register(this.lengthOfFlash, () =>
        {
            this.cameraBlur.enabled = true;

            foreach (Lamp lamp in this.lamps)
            {
                lamp.light.enabled = false;
            }
            LevelController.instance.AddPhoto(this.snapCamera.targetTexture);

            this.flashTimer = null;

            this.state = PlayerState.ReviewingPhoto;

            SoundManager.PlaySound(SoundManager.instance.cameraShutter, this.transform.position);

        });

        LevelController.instance.interactionOnPause = true;

        this.state = PlayerState.TakingPhoto;
    }
}
