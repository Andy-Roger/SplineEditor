using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splineSegment : MonoBehaviour {
    [SerializeField] private LineRenderer splineLine;
    [SerializeField] private LineRenderer controlPointLine_0;
    [SerializeField] private LineRenderer controlPointLine_1;

    public Transform point0, point1, point2, point3;
    public int numPoints = 50;

    private Vector3[] positions = new Vector3[50];

    void Start() {
        splineLine.positionCount = numPoints;
        drawCubicCurve();
    }

    void Update() {
        //Todo: only do drawCubicCurve if currently being edited
        drawCubicCurve();
        updateControlPointLine();
    }

    void drawCubicCurve() {
        for (int i = 1; i < numPoints + 1; i++) {
            float t = i / (float)numPoints;
            positions[i - 1] = calculateCubicBezierPoint(t, point0.position, point1.position, point2.position, point3.position);
        }
        splineLine.SetPositions(positions);
    }

    private void updateControlPointLine() {
        //Translate control point line ending to worldspace and set to cooresponding waypoint
        controlPointLine_0.SetPosition(1, controlPointLine_0.transform.InverseTransformPoint(point0.position));
        controlPointLine_1.SetPosition(1, controlPointLine_1.transform.InverseTransformPoint(point3.position));
    }

    private Vector3 calculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
}

