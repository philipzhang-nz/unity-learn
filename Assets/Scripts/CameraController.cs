// this script is for the camera controller
/*
If Main Camera is child of Player, then it would follow Player's 
POSITION and ROTATION, which is not what we want in this example.
this script makes the camera follow the player only in POSITION, not in ROTATION
This is useful for 3D games where you want the camera to follow the player, 
but you also want the camera to be able to rotate independently of the player
*/

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    // LateUpdate is called after all Update functions have been called in that frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
