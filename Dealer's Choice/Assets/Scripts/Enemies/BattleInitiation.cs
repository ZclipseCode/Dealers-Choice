using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class BattleInitiation : MonoBehaviour
{
    [SerializeField] float maxDistance = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] LayerMask playerLayer;
    bool inBattle;

    private void Update()
    {
        if (!inBattle)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, playerLayer))
            {
                StartBattle(hit);
            }
        }
    }

    void StartBattle(RaycastHit hit)
    {
        Transform player = hit.transform;

        PlayerMovement playerMovement = player.GetComponentInParent<PlayerMovement>();
        playerMovement.DisableInput();

        PlayerCamera playerCamera = player.parent.GetComponentInChildren<PlayerCamera>();
        playerCamera.DisableInput();

        inBattle = true;

        Camera cam = player.parent.GetComponentInChildren<Camera>();
        StartCoroutine(FaceMe(cam));

        CardBattleMode cardBattleMode = player.GetComponent<CardBattleMode>();
        
        EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.SetCardBattleMode(cardBattleMode);

        cardBattleMode.StartBattle(enemyHealth);
    }

    IEnumerator FaceMe(Camera cam)
    {
        float lastRotationMagnitude = 360f;

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
