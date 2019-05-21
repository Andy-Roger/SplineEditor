using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class splineManager : MonoBehaviour
{
    //Instantiate segment into spline parent
    [SerializeField] private Transform splineParent;
    [SerializeField] private GameObject segment;
    [SerializeField] private List<splineSegment> segments = new List<splineSegment>();

    private bool _isTransforming = false;

    public delegate void onLinesNeedUpdate();
    public static event onLinesNeedUpdate onLinesNeedUpdateEvent;

    void Awake() {
        RuntimeGizmos.TransformGizmo.onGizmoInteractionEvent += updateIsTransforming;
    }

    void OnApplicationQuit() {
        RuntimeGizmos.TransformGizmo.onGizmoInteractionEvent -= updateIsTransforming;
    }

    void updateIsTransforming(bool value) {
        _isTransforming = value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            addSegment(splineParent);
        }

        if (Input.GetKeyDown(KeyCode.Delete)) {
            removeSegment();
        }

        //Update next point when node edited
        if (_isTransforming) {
            updateSegmentPoints();
        }
    }

    public void addSegment(Transform splineParent) {
        var segment = Instantiate(this.segment, splineParent);

        //if(FindObjectOfType<RuntimeGizmos.TransformGizmo>().mainTargetRoot != null) {
        //    //Get selected segment index in segments list
        //    int selectedSegmentIndex = segments.IndexOf(FindObjectOfType<RuntimeGizmos.TransformGizmo>().mainTargetRoot.parent.GetComponent<splineSegment>());
        //    //Insert the new segment at index
        //    segments.Insert(selectedSegmentIndex, segment.GetComponent<splineSegment>());
        //} else {
        segments.Add(segment.GetComponent<splineSegment>());

        //if first segment place at spline initial position
        if (segment.transform.GetSiblingIndex() == 0)
            segment.transform.localPosition = Vector3.zero;
        else {
            //if not first segment place segment at last segments waypoint_1 (point3)
            var prevSegmentIndex = segment.transform.GetSiblingIndex() - 1;
            var prevSegment = splineParent.GetChild(prevSegmentIndex).GetComponent<splineSegment>();
            segment.transform.position = prevSegment.point3.position;
            //make previous segment's waypoint 1 unselectable
            prevSegment.point3.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void removeSegment() {
        //Delete the seleced waypoint, do nothing if "getSplineWaypoint" returns null
        if (getSplineWaypoint() != null) {
            segments.RemoveAt(getSplineWaypoint().transform.parent.GetSiblingIndex());
            Destroy(getSplineWaypoint().transform.parent.GetComponent<splineSegment>());
            Destroy(getSplineWaypoint().transform.parent.gameObject);
            updateSegmentPoints();
            onLinesNeedUpdateEvent();
            //always make last node selectable
            segments[segments.Count - 1].point3.GetComponent<BoxCollider>().enabled = true;          
        }
        //Feature TODO : pressing delete on control point will reset it
    }

    void updateSegmentPoints() {
        if(segments.Count > 0) {            
            for (int i = 0; i < segments.Count - 1; i++) {
                segments[i].point3.transform.position = segments[i + 1].point0.transform.position;
            }
        }
    }

    //Gets the currently selected spline waypoint
    GameObject getSplineWaypoint() {
        Transform currentSelection = FindObjectOfType<RuntimeGizmos.TransformGizmo>().mainTargetRoot;
        if (currentSelection.parent.GetComponent<splineSegment>())
            return currentSelection.gameObject;
        else
            return null;
    }
}
