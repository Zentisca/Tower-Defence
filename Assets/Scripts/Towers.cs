using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToMove;
    public int damage = 10; // the amount of damage the tower deals to enemies
    public float fireRate = 1f; // the rate at which the tower fires
    public float range = 5f; // the range of the tower's attack
    private float fireCountdown = 0f; // the time remaining before the tower can fire again
    public int buildPrice = 0;
    public int upgradePrice = 0;
    public int sellPrice = 0;
    public int towerCost = 15;
    [SerializeField] public string[] buildableTerrainTypes;
    [SerializeField] private float rotationAngle = 145f; // the angle the hatch rotates when firing
    [SerializeField ]private float rotationTime = 1f; // the time it takes for the hatch to rotate
    private bool isRotating = false; // flag to prevent multiple coroutines running simultaneously
    private Quaternion initialRotation;
    public GameObject nextUpgradePrefab;
    public GameObject bulletPrefab; // the prefab for the bullet the tower fires
    public Transform firePoint; // the point at which the bullet is instantiated
    private TowerPlacement towerPlacement;
    public int level = 1; // The current level of the tower
    public int maxLevel = 5; // The maximum level the tower can be upgraded to
    public int upgradeCost; // The cost to upgrade the tower to the next level
    public GameObject prefab;

    private GameObject _gameObject;
    //private GameObject nearestEnemy;


    //public bool isPlacing = true;
    public bool isPlaced;

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
            //if(!towerPlacement.isPlaced)
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (isRotating)
        {
            return; // exit the function if the object is already rotating
        }

        GameObject nearestEnemy = GetNearestEnemy();
        if (nearestEnemy != null)
        {
            // start coroutine to rotate object
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
        if (gameObjectToMove == null)
        {
            yield break; // exit the coroutine if gameObjectToMove is null
        }
        isRotating = true;

        // rotate object open
        Vector3 currentRotation = gameObjectToMove.transform.localEulerAngles;
        Vector3 targetRotation = currentRotation + new Vector3(rotationAngle, 0f, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            gameObjectToMove.transform.localEulerAngles =
                Vector3.Lerp(currentRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // rotate object closed
        currentRotation = gameObjectToMove.transform.localEulerAngles;
        targetRotation = initialRotation.eulerAngles;
        elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            gameObjectToMove.transform.localEulerAngles =
                Vector3.Lerp(currentRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isRotating = false;
    }
}

