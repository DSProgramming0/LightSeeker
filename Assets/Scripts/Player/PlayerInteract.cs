using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject objectInSight;
    [SerializeField] private GameObject previousObj;

    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactDistance;

    private void InteractWithObj()
    {
        if (objectInSight != null)
        {
            objectInSight.GetComponent<InteractableObj>().Interact();
        }
    }

    public Vector3 returnMousePos()
    {
        RaycastHit cameraHit;
        Ray cameraAim = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Physics.Raycast(cameraAim, out cameraHit, 1000f);
        Debug.DrawLine(cameraAim.origin, cameraAim.GetPoint(20f), Color.green);
        float cameraDistance = cameraHit.distance;

        if (Physics.Raycast(cameraAim, out cameraHit))
        {
            Vector3 cameraHitPoint = cameraAim.GetPoint(20f);

            // something was hit
            RaycastHit playerHit;
            Physics.Raycast(transform.position + new Vector3(0f, 1.8f, 0f), cameraHitPoint - transform.position - new Vector3(0f, 1.8f, 0f), out playerHit, 100f, interactableLayer);
            Debug.DrawRay(transform.position + new Vector3(0f, 1.8f, 0f), cameraHitPoint - transform.position - new Vector3(0f, 1.8f, 0f), Color.red);

            float playerDistance = playerHit.distance;

            if (playerHit.transform.GetComponent<InteractableObj>()) //An interactable objects has been targets and can now be interacted with
            {
                objectInSight = playerHit.transform.gameObject; //Setting object as currentObject in sight
                previousObj = objectInSight; //Keeping track of object so that the material can be swapped after currentObj becomes null

                objectInSight.GetComponent<InteractableObj>().HighlightObject(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    InteractWithObj();
                }
                return cameraHitPoint;
            }
            else //If we are still looking at something but it is not an interactable Object
            {
                if(previousObj != null)
                {
                    previousObj.GetComponent<InteractableObj>().HighlightObject(false); // switch to standard material
                    objectInSight = null;
                }
            }

            return cameraHitPoint;
        }
        else
        {
            Vector3 cameraHitPoint = cameraAim.GetPoint(20f);

            Physics.Raycast(transform.position + new Vector3(0f, 1.8f, 0f), cameraHitPoint - transform.position - new Vector3(0f, 1.8f, 0f), 100f);
            objectInSight = null;
            if(previousObj != null)
            {
                previousObj.GetComponent<InteractableObj>().HighlightObject(false);
            }
            return cameraHitPoint;
        }
    }
}
