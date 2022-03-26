using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseWheelMode
{
    _Size = 0,
    _Distance = 1
}

public enum PaintMode
{
    _Add = 0,
    _Subtract = 1
}

public class SphereBrush : MonoBehaviour
{
    [SerializeField]
    private float
        currentScale = 1f,
        currentDistance = 2f;

    [SerializeField]
    private Material
        unused,
        add,
        subtract;

    [SerializeField]
    private MouseWheelMode mouseWheelMode;

    [SerializeField]
    private PaintMode paintMode;

    private Renderer rnd;

    private const string mouseWheelAxis = "Mouse ScrollWheel";

    private void Awake()
    {
        rnd = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("m"))
            mouseWheelMode = (MouseWheelMode)((int)mouseWheelMode ^ 1);

        if (Input.GetKeyDown("p"))
            paintMode = (PaintMode)((int)paintMode ^ 1);

        float deltaMouseWheel = Input.GetAxis(mouseWheelAxis);

        switch(mouseWheelMode)
        {
            case MouseWheelMode._Size:
                currentScale += deltaMouseWheel;
                break;
            case MouseWheelMode._Distance:
                currentDistance += deltaMouseWheel * 10f;
                break;
        }

        transform.localScale = Vector3.one * currentScale;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, currentDistance));

        if(Input.GetMouseButton(0))
        {
            rnd.material = (paintMode == PaintMode._Add) ? add : subtract;
            float color = (paintMode == PaintMode._Add) ? -1f : 1f;

            ExampleVolume.Instance.SpherePaint(transform.position, currentScale * currentScale, color);
        }
        else
        {
            rnd.material = unused;
        }
    }

    private void Paint()
    {

    }
}
