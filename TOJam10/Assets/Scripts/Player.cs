using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public static Player instance;

    private Vector3 previousHeldPosition;
    private Vector3 currentHeldPosition;

    private Vector3 previousMousePosition;
    private Vector3 currentMousePosition;

    private Tossable heldTossable;
    public float holdHeight;

    public BoxCollider screenToWorldMap;

    void Awake()
    {
        Player.instance = this;

        this.previousMousePosition = this.currentMousePosition = Input.mousePosition;
    }

	void Update()
	{
	    this.previousMousePosition = this.currentMousePosition;
	    this.currentMousePosition = Input.mousePosition;

	    if (Input.GetMouseButtonDown(0))
	    {
	        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hitInfo;
	        if (Physics.Raycast(cameraRay, out hitInfo))
	        {
	            Tossable tossable = hitInfo.transform.GetComponent<Tossable>();

	            if (tossable != null)
	            {
	                this.PickUp(tossable);
	            }
	        }
        }
        else if (Input.GetMouseButtonUp(0) && this.heldTossable != null)
        {
            this.TossHeldObject();
        }
        else if (Input.GetMouseButton(0) && this.heldTossable != null)
        {
            this.heldTossable.GetComponent<Rigidbody>().velocity = Vector3.zero;

            Vector3 previousMouseWorldPos = this.GetMouseWorldPosition(this.previousMousePosition);
            Vector3 currentMouseWorldPos = this.GetMouseWorldPosition(this.currentMousePosition);

            this.heldTossable.transform.position += currentMouseWorldPos - previousMouseWorldPos;
            this.heldTossable.transform.position = this.heldTossable.transform.position.SetY(this.holdHeight);

            this.previousHeldPosition = this.currentHeldPosition;
            this.currentHeldPosition = this.heldTossable.transform.position;
        }
        else if (Input.GetKey(KeyCode.Space)) {
            this.snapPhoto();
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

    private void TossHeldObject()
    {
        this.heldTossable.GetTossed(this.currentHeldPosition - this.previousHeldPosition);
        this.heldTossable = null;
        Cursor.visible = true;
    }

    private void snapPhoto()
    {

    }
}
