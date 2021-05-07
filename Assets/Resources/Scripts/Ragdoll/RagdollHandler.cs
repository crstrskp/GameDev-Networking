using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class RagdollHandler : MonoBehaviour, IDestructible
{
    public Ragdoll RagdollObject;
    public float Force;
    public float Lift;

    private bool m_ragdollOnCooldown = false;

    public void OnDestruction(GameObject destroyer)
    {
        if (m_ragdollOnCooldown) return;

        var ragdollObj = PhotonNetwork.Instantiate("YBot-Ragdoll", transform.position, transform.rotation);

        var vectorFromDestroyer = transform.position - destroyer.transform.position;
        vectorFromDestroyer.Normalize();
        vectorFromDestroyer.y += Lift;

        ragdollObj.GetComponent<Ragdoll>().ApplyForce(vectorFromDestroyer * Force);

        StartCoroutine(RagdollCooldown());
    }

    IEnumerator RagdollCooldown()
    {
        m_ragdollOnCooldown = true;
        yield return new WaitForSeconds(0.5f);
        m_ragdollOnCooldown = false;
    }
}
