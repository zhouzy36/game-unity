using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Check : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerMainBody")
        {
            //Debug.Log("enter");
            GameObject obj = this.gameObject;
            obj.GetComponent<NavMeshAgent>().enabled = true;
            obj.GetComponent<AIroute>().enabled = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerMainBody")
        {
            //Debug.Log("exit");
            GameObject obj = this.gameObject;
            obj.GetComponent<NavMeshAgent>().enabled = false;
            obj.GetComponent<AIroute>().enabled = false;
        }
    }
}
