using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftWandController : SteamVR_TrackedController
{

    //Variables
    protected LineRenderer lineRenderer;
    protected Vector3[] lineRendererVertices;
    protected RaycastHit hit;
    protected bool teleportActive;
    protected Vector3 previousPosition;
    protected int rewindCharge = 0;
    protected bool charging;

    //Properties
    public SteamVR_Controller.Device controller
    {
        get
        {
            return SteamVR_Controller.Input((int)(controllerIndex));
        }
    }
    public Vector3 velocity
    {
        get
        {
            return controller.velocity;
        }
    }
    public Vector3 angularVelocity
    {
        get
        {
            return controller.angularVelocity;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.numPositions = 2;

        lineRendererVertices = new Vector3[2];
    }

    public float GetTriggerAxis()
    {
        if (controller == null)
            return 0;

        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x;
    }

    public Vector2 GetTouchpadAxis()
    {
        if (controller == null)
            return new Vector2();

        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
    }

    public override void OnPadClicked(ClickedEventArgs e)
    {
        base.OnPadClicked(e);

        teleportActive = true;
    }

    public override void OnPadUnclicked(ClickedEventArgs e)
    {
        base.OnPadUnclicked(e);

        if (transform.parent == null)
            return;

        Vector3 startPos = transform.position;

        if (Physics.Raycast(startPos, transform.forward, out hit, 40.0f))
        {
            if (hit.collider.tag == "moveable" && hit.normal.y != -1)
            {
                previousPosition = transform.parent.position;
                transform.parent.position = hit.transform.position + new Vector3(0, hit.transform.localScale.y / 2, 0);
            }
            else if (hit.normal.y == 1)
            {
                previousPosition = transform.parent.position;
                transform.parent.position = hit.point;
            }
            else if(hit.collider.tag == "portal")
            {
                if(hit.transform.localScale.x == 2)
                {
                    GameObject.Find("GameManager").GetComponent<GameManager>().LoadNextLevel(hit.collider.GetComponent<Portal>().connectedLevel);
                }
            }
        }

        teleportActive = false;
    }

    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);

        charging = true;
    }

    public override void OnTriggerUnclicked(ClickedEventArgs e)
    {
        base.OnTriggerUnclicked(e);

        if(rewindCharge >= 50)
        {
            transform.parent.position = previousPosition;
        }

        rewindCharge = 0;
        charging = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (teleportActive)
        {
            if (lineRenderer && lineRenderer.enabled)
            {
                Vector3 startPos = transform.position;

                if (Physics.Raycast(startPos, transform.forward, out hit, 40.0f) && (hit.normal.y == 1 || (hit.collider.tag == "moveable" && hit.normal.y != -1)
                    || hit.collider.tag == "portal"))
                {
                    lineRendererVertices[1] = hit.point;
                    lineRenderer.startColor = lineRenderer.endColor = Color.cyan;
                }
                else
                {
                    lineRendererVertices[1] = startPos + transform.forward * 40.0f;
                    lineRenderer.startColor = lineRenderer.endColor = Color.red;
                }

                lineRendererVertices[0] = transform.position;
                lineRenderer.SetPositions(lineRendererVertices);
            }
        }
        else
        {
            lineRendererVertices[0] = lineRendererVertices[1];
            lineRenderer.SetPositions(lineRendererVertices);
        }

        if (charging)
        {
            rewindCharge++;
        }
    }
}