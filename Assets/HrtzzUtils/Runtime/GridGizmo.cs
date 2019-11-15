using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hrtzz
{
    public class GridGizmo : MonoBehaviour
    {
        public bool drawGrid = false;
        public bool drawCenterLine = false;

        [Range (2, 100)]
        public int rowCount = 10, columnCount = 10;
        public Color verticleLineColor, horizontalLineColor;

        float verticalStep, horizontalStep;
        Camera cam;
        Vector3 horStartPoint, horEndPoint, vertStartPoint, vertEndPoint;
        Vector3 startMidPointVert, endMidPointVert, startMidPointHor, endMidPointHor;

        void OnValidate ()
        {
            cam = GetComponent<Camera> ();
            verticalStep = 1 / (float) rowCount;
            horizontalStep = 1 / (float) columnCount;

            startMidPointVert = cam.ViewportToWorldPoint (new Vector3 (0, 0.5f, cam.nearClipPlane));
            endMidPointVert = cam.ViewportToWorldPoint (new Vector3 (1, 0.5f, cam.nearClipPlane));

            startMidPointHor = cam.ViewportToWorldPoint (new Vector3 (0.5f, 0f, cam.nearClipPlane));
            endMidPointHor = cam.ViewportToWorldPoint (new Vector3 (0.5f, 1f, cam.nearClipPlane));
        }

        void OnDrawGizmos ()
        {
            if (!drawGrid)
                return;

            Gizmos.color = horizontalLineColor;
            for (int i = 0; i <= rowCount; i++)
            {
                horStartPoint = cam.ViewportToWorldPoint (new Vector3 (0, i * verticalStep, cam.nearClipPlane));
                horEndPoint = cam.ViewportToWorldPoint (new Vector3 (1, i * verticalStep, cam.nearClipPlane));
                Gizmos.DrawLine (horStartPoint, horEndPoint);
            }

            Gizmos.color = verticleLineColor;
            for (int i = 0; i <= columnCount; i++)
            {
                vertStartPoint = cam.ViewportToWorldPoint (new Vector3 (i * horizontalStep, 0f, cam.nearClipPlane));
                vertEndPoint = cam.ViewportToWorldPoint (new Vector3 (i * horizontalStep, 1f, cam.nearClipPlane));
                Gizmos.DrawLine (vertStartPoint, vertEndPoint);
            }

            if (drawCenterLine)
            {
                Gizmos.color = horizontalLineColor;
                Gizmos.DrawLine (startMidPointHor, endMidPointHor);

                Gizmos.color = verticleLineColor;
                Gizmos.DrawLine (startMidPointVert, endMidPointVert);
            }

        }
    }
}
