using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWandController : SteamVR_TrackedController
{

    //Variables
    protected LineRenderer lineRenderer;
    protected Vector3[] lineRendererVertices;
    RaycastHit hit;
    string objName;
    bool objGrabbed;
    bool objTriggeredUp;
    bool objTriggeredDown;

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

    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);

        Vector3 startPos = transform.position;

        if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f))
        {
            if (hit.collider.tag == "moveable")
            {
                objName = hit.collider.name;
                objTriggeredUp = true;
            }
            else if (hit.collider.tag == "portal")
                OpenTeleport();
        }
    }

    public override void OnTriggerUnclicked(ClickedEventArgs e)
    {
        base.OnTriggerUnclicked(e);

        objTriggeredUp = false;
    }

    public override void OnPadClicked(ClickedEventArgs e)
    {
        base.OnPadClicked(e);

        Vector3 startPos = transform.position;

        if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f) && hit.collider.tag == "moveable")
        {
            objName = hit.collider.name;
            objTriggeredDown = true;
        }
        else
            objTriggeredDown = false;
    }

    public override void OnPadUnclicked(ClickedEventArgs e)
    {
        base.OnPadUnclicked(e);

        objTriggeredDown = false;
    }
    
    public override void OnGripped(ClickedEventArgs e)
    {
        base.OnGripped(e);

        Vector3 startPos = transform.position;

        if(Physics.Raycast(startPos, transform.forward, out hit, 1000.0f) && hit.collider.tag == "moveable")
        {
            objName = hit.collider.name;
            objGrabbed = true;
        }
    }

    public override void OnUngripped(ClickedEventArgs e)
    {
        base.OnUngripped(e);

        objGrabbed = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (lineRenderer && lineRenderer.enabled)
        {
            Vector3 startPos = transform.position;

            if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f) && ((hit.collider.tag == "moveable" && !StandingOn(GameObject.Find(hit.collider.name))) || 
                (hit.collider.tag == "portal")))
            {
                lineRendererVertices[1] = hit.point;
                lineRenderer.startColor = lineRenderer.endColor = Color.cyan;
            }
            else
            {
                lineRendererVertices[1] = startPos + transform.forward * 1000.0f;
                lineRenderer.startColor = lineRenderer.endColor = Color.red;
            }

            lineRendererVertices[0] = transform.position;
            lineRenderer.SetPositions(lineRendererVertices);
        }

        GameObject obj = null;
        if(objName != null)
        {
            obj = GameObject.Find(objName);
        }

        if(objGrabbed && hit.normal.y == 1)
        {
            Vector3 offset = new Vector3();
            if(hit.normal.x == 1)
            {
                offset.x = -1 * obj.transform.localScale.x / 2;
            }
            else if(hit.normal.x == -1)
            {
                offset.x = obj.transform.localScale.x / 2;
            }
            else if(hit.normal.z == 1)
            {
                offset.z = -1 * obj.transform.localScale.z / 2;
            }
            else if(hit.normal.z == -1)
            {
                offset.z = obj.transform.localScale.z / 2;
            }

            if(hit.point.y == 0)
            {
                offset.y = obj.transform.position.y;
            }

            obj.transform.position = hit.point + offset;
        }
        
        if (objTriggeredUp && !StandingOn(obj))
        {
            float yVal = obj.transform.position.y + .1f;
            Vector3 newPos = new Vector3(obj.transform.position.x, yVal, obj.transform.position.z);
            obj.transform.position = newPos;
        }
        if (objTriggeredDown && !StandingOn(obj))
        {
            float yVal = obj.transform.position.y - .1f;
            Vector3 newPos = new Vector3(obj.transform.position.x, yVal, obj.transform.position.z);
            obj.transform.position = newPos;
        }
    }

    //Helper Methods
    public bool StandingOn(GameObject obj)
    {
        if (this.transform.position.x >= obj.transform.position.x - obj.transform.localScale.x && this.transform.position.x <= obj.transform.position.x + obj.transform.localScale.x)
            if (this.transform.position.z >= obj.transform.position.z - obj.transform.localScale.z && this.transform.position.z <= obj.transform.position.z + obj.transform.localScale.z)
                return true;
        return false;
    }

    public void OpenTeleport()
    {
        Portal portal = GameObject.FindGameObjectWithTag("portal").GetComponent<Portal>();
        if(!portal.locked && !portal.open)
        {
            GameObject.FindGameObjectWithTag("portal").transform.localScale = new Vector3(2, 2, 2);
            portal.open = true;
        }
    }
}
