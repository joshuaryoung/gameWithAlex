using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentlyVisableObjects : MonoBehaviour
{
    public List<GameObject> visibleObjects = new List<GameObject>();
    public Camera cam;
    public GameObject playerGameObject;
    public GameObject lockedOnEnemyObj = null;
    public GameObject lockOnEnemyCircleObj = null;
    public bool isLockedOn = false;
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) {
            cam = GetComponent<Camera>();
        }
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
        if (gameObj.Equals(lockedOnEnemyObj)) {
            lockedOnEnemyObj = null;
            lockOnEnemyCircleObj.SetActive(false);
            lockOnEnemyCircleObj = null;
            isLockedOn = false;
        }
        visibleObjects.Remove(gameObj);
    }

    public void toggleLockOn() {
        if (isLockedOn) {
            endLockOn();
        } else {
            initiateLockOn();
        }
    }

    public void endLockOn() {

        isLockedOn = false;
    }

    public void initiateLockOn() {
        if (playerGameObject == null) {
            Debug.LogError("playerGameObject is null!");
            return;
        }
        if (isLockedOn) {
            lockOnEnemyCircleObj.SetActive(false);
            isLockedOn = false;
            lockedOnEnemyObj = null;
            lockOnEnemyCircleObj = null;

            return;
        }
        if (visibleObjects.Count == 0) {
            return;
        } else if (visibleObjects.Count == 1) {
            bool playerFacingEnemy = (playerGameObject.transform.localPosition.x - visibleObjects[0].transform.localPosition.x) * playerGameObject.transform.localScale.x < 0;

            if (playerFacingEnemy) {
                lockedOnEnemyObj = visibleObjects[0];
            } else {
                return;
            }
        } else {
            GameObject closestEnemyObj = new GameObject();
            foreach (GameObject el in visibleObjects) {
                float distanceToElFromPlayer = Vector3.Distance(el.transform.localPosition, playerGameObject.transform.localPosition);
                float distanceToClosestEnemyFromPlayer = Vector3.Distance(closestEnemyObj.transform.localPosition, playerGameObject.transform.localPosition);

                // Check: is player facing enemy?
                bool playerFacingEnemy = (playerGameObject.transform.localPosition.x - el.transform.localPosition.x) * playerGameObject.transform.localScale.x < 0;
                
                // Find closest Obj
                if (distanceToElFromPlayer < distanceToClosestEnemyFromPlayer && playerFacingEnemy) {
                    closestEnemyObj = el;
                }
            }
            if (closestEnemyObj.Equals(new GameObject())) {
                return;
            }

            lockedOnEnemyObj = closestEnemyObj;
        }

        if (lockedOnEnemyObj == null) {
            return;
        }

        Transform lockOnCircleTransform = lockedOnEnemyObj.transform.Find("LockOnCircle");

        if (lockOnCircleTransform == null) {
            Debug.LogWarning($"LockOnCircle not found in {lockedOnEnemyObj.name}");
            return;
        }

        lockOnEnemyCircleObj = lockOnCircleTransform.gameObject;
        lockedOnEnemyObj = lockOnCircleTransform.parent.gameObject;

        lockOnEnemyCircleObj.SetActive(!lockOnEnemyCircleObj.activeSelf);
        isLockedOn = true;
    }
}
