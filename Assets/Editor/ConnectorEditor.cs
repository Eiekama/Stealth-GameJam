using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class ConnectorEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Connector connector, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.white;
        }
        else
        {
            Gizmos.color = Color.white * 0.5f;
        }

        Gizmos.DrawCube(connector.transform.position, 0.5f * Vector3.one);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(connector.transform.position, connector.transform.position + 1f * connector.transform.forward);
    }
}
