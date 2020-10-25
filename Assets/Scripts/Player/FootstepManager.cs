using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    [SerializeField] private float rayLength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkSurface();
    }

    private void checkSurface()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * rayLength), Color.red);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            Debug.Log(hit.transform.tag);
        }
    }
}
