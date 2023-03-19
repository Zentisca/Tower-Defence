using System.Collections;
                         using System.Collections.Generic;
                         using UnityEngine;
                         
                         public class TowerAnimationAim : MonoBehaviour
                         {
                             [SerializeField] private float rotationSpeed = 5.0f;
                             [SerializeField] private GameObject bracketRotator;
                             [SerializeField] private GameObject towerTurretRotator;
                             [SerializeField] private int lowerRotationLimit = -20;
                             [SerializeField] private int upperRotationLimit = 20;
                             [SerializeField] private int range = 20;
                         
                             void Update()
                             {
                                 TrackTarget();
                             }
                         
                             private void TrackTarget()
                             {
                                 GameObject nearestEnemy = GetNearestEnemy();
                                 if (nearestEnemy != null)
                                 {
                                     if (bracketRotator != null)
                                     {
                                         Vector3 direction = (nearestEnemy.transform.position - bracketRotator.transform.position).normalized;
                                         Vector3 targetDirection = new Vector3(direction.x, 0f, direction.z);
                                         Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                                         bracketRotator.transform.rotation = Quaternion.Lerp(bracketRotator.transform.rotation, targetRotation,
                                             rotationSpeed * Time.deltaTime);
                                     }
                         
                                     if (towerTurretRotator != null)
                                     {
                                         Vector3 directionTowerRotator =
                                             (nearestEnemy.transform.position - towerTurretRotator.transform.position).normalized;
                                         Vector3 targetDirectionTowerRotator = new Vector3(directionTowerRotator.x, directionTowerRotator.y, directionTowerRotator.z);

                Quaternion targetRotationTowerRotator =
                    Quaternion.LookRotation(targetDirectionTowerRotator, Vector3.up);

                // Get the current rotation and clamp it within the minimum and maximum rotation values
                Quaternion currentRotation = towerTurretRotator.transform.rotation;
                float clampedAngle = Mathf.Clamp(currentRotation.eulerAngles.x, lowerRotationLimit, upperRotationLimit);
                Quaternion clampedRotation = Quaternion.Euler(clampedAngle, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

                // Lerp between the current rotation and the target rotation
                towerTurretRotator.transform.rotation = Quaternion.Lerp(clampedRotation, targetRotationTowerRotator,
                    rotationSpeed * Time.deltaTime);
            }
        }
    }

    private GameObject GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(bracketRotator.transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}