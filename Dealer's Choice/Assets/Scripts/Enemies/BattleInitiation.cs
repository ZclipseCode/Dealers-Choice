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
            Transform player = hit.transform;

            PlayerMovement playerMovement = player.GetComponentInParent<PlayerMovement>();
            playerMovement.DisableInput();

            PlayerCamera playerCamera = player.parent.GetComponentInChildren<PlayerCamera>();
            playerCamera.DisableInput();

            inBattle = true;

            Camera cam = player.parent.GetComponentInChildren<Camera>();
            StartCoroutine(FaceMe(cam));
        }
    }

    IEnumerator FaceMe(Camera cam)
    {
        float lastRotationMagnitude = 360f;
        float rotationSpeed = 1f;

        while (Mathf.Abs(cam.transform.rotation.eulerAngles.magnitude - lastRotationMagnitude) > 0.001f)
        {
            Vector3 targetDirection = transform.position - cam.transform.position;
            targetDirection = new Vector3(targetDirection.x, 0f, targetDirection.z);

            Vector3 newDirection = Vector3.RotateTowards(cam.transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);

            lastRotationMagnitude = cam.transform.rotation.eulerAngles.magnitude;

            cam.transform.rotation = Quaternion.LookRotation(newDirection);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
    }
}
