using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitiation : MonoBehaviour
{
    [SerializeField] float maxDistance = 1f;
    [SerializeField] LayerMask playerLayer;
    bool inBattle;

    private void Update()
    {
        RaycastHit hit;

        if (!inBattle && Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, playerLayer))
        {
            PlayerMovement playerMovement = hit.transform.GetComponentInParent<PlayerMovement>();
            playerMovement.DisableInput();

            PlayerCamera playerCamera = hit.transform.parent.GetComponentInChildren<PlayerCamera>();
            playerCamera.DisableInput();

            inBattle = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
    }
}
