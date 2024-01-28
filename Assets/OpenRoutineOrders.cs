using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenRoutineOrders : MonoBehaviour
{
    public GameObject routineOrder;

    public void OpenOrders()
    {
        AudioManager.PlayOneShot("ScrollOpen");
        routineOrder.SetActive(true);
    }
}
