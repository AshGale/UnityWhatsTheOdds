using System.Collections;
using UnityEngine;

public class CameraConrol : MonoBehaviour
{
    public GameObject centerOfWorld;
    public PlayerControl playerControl;
    private Vector3 dragOrigin;
    private Vector3 rotateTo;
    private float width;
    private float depth;

    private void Start()
    {
        width = ( GlobalVariables.data.WIDTH -1 ) / 2;
        depth = ( GlobalVariables.data.DEPTH -1 ) / 2;
        //centerOfWorld = GameObject.Find("CenterOfWorld");
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(MoveCameraByMouse());
        }
    }

    public IEnumerator MoveCameraByMouse()
    {
        int rotationSpeed = GlobalVariables.data.CAMERA_ROTATION_SPEED;
        dragOrigin = Input.mousePosition;
        while (Input.GetMouseButton(1))
        {
            rotateTo = Input.mousePosition - dragOrigin;
            rotateTo.x = 0f;
            rotateTo.z = 0f;
            transform.RotateAround(new Vector3(width, 1, depth), rotateTo, rotationSpeed * .01f);
            yield return null;
        }
        playerControl.activePlayer.cameraPosition = transform.rotation.eulerAngles;
    }

    public IEnumerator GlidePosition(Vector3 playerCameraPostion)
    {
        float distance = playerCameraPostion.y > transform.rotation.eulerAngles.y ? playerCameraPostion.y - transform.rotation.eulerAngles.y : playerCameraPostion.y - transform.rotation.eulerAngles.y;

        Vector3 direction;
        if(distance > 0)
        {
            direction = Vector3.up;
        } else
        {
            direction = Vector3.down;
            distance = -distance;
        }
        distance = Mathf.Round(distance);
        //Debug.Log($"{transform.rotation.eulerAngles} -> {playerCameraPostion} going {direction}: {distance}");
        for (float i = 0; i < distance; i+=5)
        {
            transform.RotateAround(centerOfWorld.transform.position, direction, 5);
            //transform.RotateAround(centerOfWorld.transform.position, direction, 5 * Time.deltaTime);

            yield return null;
        }
        transform.rotation.Normalize();
    }
}
// 3.5 7 0      -> 65 0   0 F
// 0   7 3.5    -> 65 90  0 L
// 3.5 7 7      -> 65 180 0 B
// 7   7 3.5    -> 65 270 0 R