using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DungeonGate : MonoBehaviour
{

    public GameObject leftDoor;
    public GameObject rightDoor;

    public float maxAngleLeft;
    public float maxAngleRight;
    public float openTime;


    private void OnTriggerEnter(Collider other)
    {
        OpenDoors();
    }

    private void OnTriggerExit(Collider other)
    {
        CloseDoors();
    }

    async void OpenDoors()
    {
        
        var fromAngle = leftDoor.transform.localRotation;
        var toAngleLeft = Quaternion.Euler(0, leftDoor.transform.localEulerAngles.y + maxAngleLeft, 0);
        var toAngleRight = Quaternion.Euler(0, rightDoor.transform.localEulerAngles.y + maxAngleRight, 0);

        if (fromAngle.y < toAngleLeft.y)
        {
            for (var t = 0f; t < 1; t += Time.deltaTime / openTime)
            {
                leftDoor.transform.localRotation = Quaternion.Lerp(fromAngle, toAngleLeft, t);
                rightDoor.transform.localRotation = Quaternion.Lerp(fromAngle, toAngleRight, t);
                await Task.Yield();
            }
        }
        else
        {
            await Task.Yield();
        }
    }

    async void CloseDoors()
    {
        var fromAngleLeft = leftDoor.transform.localRotation;
        var fromAngleRight = rightDoor.transform.localRotation;
        var toAngle = Quaternion.Euler(0, 0, 0);
        for (var t = 0f; t < 1; t += Time.deltaTime / openTime)
        {
            leftDoor.transform.localRotation = Quaternion.Lerp(fromAngleLeft, toAngle, t);
            rightDoor.transform.localRotation = Quaternion.Lerp(fromAngleRight, toAngle, t);
            await Task.Yield();
        }

        //while (leftDoor.transform.rotation.y > 0)
        //{
        //    leftDoor.transform.rotation = Quaternion.Lerp(leftDoor.transform.rotation, Quaternion.Euler(0, 0, 0), speed * Time.deltaTime);
        //    rightDoor.transform.rotation = Quaternion.Lerp(rightDoor.transform.rotation, Quaternion.Euler(0, 0, 0), speed * Time.deltaTime);

        //    await Task.Yield();
        //}
    }
}
