using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 startCamPos, startMousePos, currentMousePos, difference;
    public Camera cam;
    public bool isDragging = false;
    public float initialDragSpeed = 1, currentDragSpeed, scrollInput, scrollMultiplier = 5, camMoveSmoothness = .2f;
    private float initialCamSize, currentCamSize;

    // Start is called before the first frame update
    void Start()
    {
        initialCamSize = cam.orthographicSize;
        currentCamSize = initialCamSize;
        currentDragSpeed = initialDragSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            startCamPos = transform.position;
            startMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(1))
            isDragging = false;

        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (isDragging)
            MoveCamera();

        ManageScrollInput();

    }

    private void MoveCamera()
    {
        currentMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        difference = currentMousePos - startMousePos;
        transform.position = startCamPos - difference * currentDragSpeed;
    }

    public void MoveCamera(Vector3 input)
    {
        transform.position -= input * camMoveSmoothness;
    }

    private void ManageScrollInput()
    {
        if (isDragging)
        {
            currentDragSpeed -= scrollInput * scrollMultiplier * -1;
            if (currentDragSpeed < .5f)
                currentDragSpeed = .5f;
        }
        else
        {
            currentCamSize -= scrollInput * scrollMultiplier;
            if (currentCamSize < 2)
                currentCamSize = 2;
            cam.orthographicSize = currentCamSize;
        }

    }
}
