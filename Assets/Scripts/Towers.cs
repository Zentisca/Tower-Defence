using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour
{
    public int towerCost = 15;
    public int level = 1;
    public int maxLevel = 5;
    public int upgradeCost;
    public float fireRate = 1f;
    public float range = 5f;
    public GameObject bulletPrefab;
    public GameObject nextUpgradePrefab;
    [SerializeField] public string[] buildableTerrainTypes;
    
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject gameObjectToMoveOrNull;
    [SerializeField] private float rotationAngle = 145f;
    [SerializeField] private float rotationTime = 1f;
    private float fireCountdown = 0f;
    private bool isRotating = false;
    private Quaternion initialRotation;
    private TowerPlacement towerPlacement;
    private GameObject _gameObject;

    public static int GetTowerCost(GameObject towerPrefab)
    {
        Towers towers = towerPrefab.GetComponent<Towers>();
        if (towers != null)
        {
            return towers.towerCost;
        }
        return -1;
    }
    
    
    private void Start()
    {
        _gameObject = GameObject.Find("Player Elements");
        towerPlacement = _gameObject.GetComponent<TowerPlacement>();
    }

    public GameObject GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            return nearestEnemy;
        }

        return null;
    }

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f) //&& !isPlacing)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (isRotating)
        {
            return;
        }

        GameObject nearestEnemy = GetNearestEnemy();
        if (nearestEnemy != null)
        {
            StartCoroutine(RotateObject());
            Vector3 direction = (nearestEnemy.transform.position - firePoint.position).normalized;
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position,
                Quaternion.LookRotation(direction));
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if (bullet != null)
                bullet.Seek(nearestEnemy.transform);
        }
    }

    IEnumerator RotateObject()
    {
        if (gameObjectToMoveOrNull == null)
        {
            yield break;
        }
        
        isRotating = true;
        Vector3 currentRotation = gameObjectToMoveOrNull.transform.localEulerAngles;
        Vector3 targetRotation = currentRotation + new Vector3(rotationAngle, 0f, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            gameObjectToMoveOrNull.transform.localEulerAngles =
                Vector3.Lerp(currentRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        currentRotation = gameObjectToMoveOrNull.transform.localEulerAngles;
        targetRotation = initialRotation.eulerAngles;
        elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            gameObjectToMoveOrNull.transform.localEulerAngles =
                Vector3.Lerp(currentRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isRotating = false;
    }
}

