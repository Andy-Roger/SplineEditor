using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splineManager : MonoBehaviour
{
    //Instantiate segment into spline parent
    [SerializeField] private Transform splineParent;
    [SerializeField] private GameObject segment;
    private List<splineSegment> segments = new List<splineSegment>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            createSegment(splineParent);
        }

        //Update next point when node edited
        updateSegmentPoints();
    }

    public void createSegment(Transform splineParent) {
        var segment = Instantiate(this.segment, splineParent);
        segments.Add(segment.GetComponent<splineSegment>());
        //if first segment place at spline initial position
        if (segment.transform.GetSiblingIndex() == 0)
            segment.transform.localPosition = Vector3.zero;
        else {
            //if not first segment place segment at last segments waypoint_1 (point3)
            var prevSegmentIndex = segment.transform.GetSiblingIndex() - 1;
            var prevSegment = splineParent.GetChild(prevSegmentIndex).GetComponent<splineSegment>();
            segment.transform.position = prevSegment.point3.position;
        }
    }

    //On waypoint1 move (TODO), if there is a next segment, move the next segment's waypoint0 with this waypoint1
    void updateSegmentPoints() {
        if(segments.Count > 0) {
            for (int i = 0; i < segments.Count - 1; i++) {
                segments[i + 1].point0.transform.position = segments[i].point3.transform.position;
            }
        }
    }
}
