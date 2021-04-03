using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour, IDestructible
{
    public Ragdoll RagdollObject;
    public float Force;
    public float Lift;

    public void OnDestruction(GameObject destroyer)
    {
        var ragdollObj = PhotonNetwork.Instantiate("YBot-Ragdoll", transform.position, transform.rotation);

        var vectorFromDestroyer = transform.position - destroyer.transform.position;
        vectorFromDestroyer.Normalize();
        vectorFromDestroyer.y += Lift;

        ragdollObj.GetComponent<Ragdoll>().ApplyForce(vectorFromDestroyer * Force);
    }
}
