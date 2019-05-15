using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splineBuilder : MonoBehaviour {
    private GameObject selectedObject;
    [SerializeField] private Material nodeMat;
    private GameObject currentSpline;
    private GameObject splineCenterUI;
    [SerializeField] private LineRenderer splinePath;

    void Awake() {
        splineCenterUI = GameObject.CreatePrimitive(PrimitiveType.Capsule);
    }

    void Update() {
        createNewSpline();
        selectObject();
        dropNode();
    }

    void createNewSpline() {
        if (Input.GetKeyDown(KeyCode.S)) {
            currentSpline = new GameObject("Spline");
            currentSpline.AddComponent<spline>();
            currentSpline.AddComponent<BoxCollider>();
        }
    }

    void selectObject() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider != null && hit.collider.gameObject.GetComponent<asset>()) {
                    selectedObject = hit.collider.gameObject;
                }
            }
        }
    }

    void dropNode() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(currentSpline != null) {
                if (selectedObject != null) {
                    createSplineNode();
                }
            } else {
                print("Choose to edit or create a spline first");
            }
        }
    }

    void createSplineNode() {
        //Add splineNode class
        var newSplineNodeObj = Instantiate(selectedObject);
        Destroy(newSplineNodeObj.GetComponent<asset>());
        newSplineNodeObj.AddComponent<splineNode>();
        print(newSplineNodeObj.GetComponent<Renderer>().material.color);
        newSplineNodeObj.GetComponent<Renderer>().material = nodeMat;
        newSplineNodeObj.transform.parent = currentSpline.transform;
        newSplineNodeObj.name = "Node_" + newSplineNodeObj.transform.GetSiblingIndex();

        resetSplineCenter(getSplineCenter(currentSpline.transform.GetComponentsInChildren<Transform>()));
        generatePath(currentSpline.transform.GetComponentsInChildren<Transform>());
        
    }

    //Resets the center point of spline by offseting the box collider
    void resetSplineCenter(Vector3 newCenter) {
        print("Reminder, place some UI at " + newCenter + " to spline center");
        splineCenterUI.transform.position = newCenter;
        //To center the box collider we have to translate it from local space
        var boxColliderCentered = currentSpline.transform.InverseTransformPoint(newCenter);
        currentSpline.GetComponent<BoxCollider>().center = boxColliderCentered;
    }

    //Gets the average center of the spline nodes
    Vector3 getSplineCenter(Transform[] childrenNodes) {
        Vector3 summatedNodePositions = Vector3.zero;
        foreach(Transform child in childrenNodes) {
            summatedNodePositions += child.position;
        }
        Vector3 averageNodePositions = summatedNodePositions / childrenNodes.Length; 
        return averageNodePositions;
    }

    LineRenderer generatePath(Transform[] splineNodes) {
        var path = splinePath;
        path.positionCount = splineNodes.Length;
        for(int i = 0; i < splineNodes.Length; i ++) {
            path.SetPosition(i, splineNodes[i].position);
        }
        return path;
    }
}

