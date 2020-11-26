using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentlyVisableObjects : MonoBehaviour
{
    public List<GameObject> visibleObjects = new List<GameObject>();
    public Camera cam;
    public GameObject playerGameObject;
    public GameObject closestEnemyObj;
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) {
            cam = GetComponent<Camera>();
        }
        closestEnemyObj = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addObject(GameObject gameObj) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        BoxCollider2D col2D = gameObj.GetComponentInChildren<BoxCollider2D>();
        bool isInMainCamera = GeometryUtility.TestPlanesAABB(planes, col2D.bounds);
        if (visibleObjects.Contains(gameObj) || !isInMainCamera) {
            return;
        }
        visibleObjects.Add(gameObj);
    }
    
    public void removeObject(GameObject gameObj) {
        if (!visibleObjects.Contains(gameObj)) {
            return;
        }
        visibleObjects.Remove(gameObj);
    }

    public void initiateLockOn() {
        if (playerGameObject == null) {
            Debug.LogError("playerGameObject is null!");
            return;
        }
        if (visibleObjects.Count == 0) {
            return;
        } else if (visibleObjects.Count == 1) {
            closestEnemyObj = visibleObjects[0];
        } else {
            foreach (GameObject el in visibleObjects) {
                float distanceToElFromPlayer = Vector3.Distance(el.transform.localPosition, playerGameObject.transform.localPosition);
                float distanceToClosestEnemyFromPlayer = Vector3.Distance(closestEnemyObj.transform.localPosition, playerGameObject.transform.localPosition);

                if (distanceToElFromPlayer < distanceToClosestEnemyFromPlayer) {
                    closestEnemyObj = el;
                }
            }
        }

        Transform lockOnCircleTransform = closestEnemyObj.transform.Find("LockOnCircle");

        if (lockOnCircleTransform == null) {
            Debug.LogWarning($"LockOnCircle not found in {closestEnemyObj.name}");
            return;
        }

        GameObject lockOnCircleObj = lockOnCircleTransform.gameObject;

        lockOnCircleObj.SetActive(!lockOnCircleObj.activeSelf);
        Debug.Log("initiate LockOn!");
    }
}
