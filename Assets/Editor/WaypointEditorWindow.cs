using System.Collections;
using System.Collections.Generic;
using Movement;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEngine;

public class WaypointEditorWindow : EditorWindow 
{
    // Creates a new menu item in the tools menu
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointEditorWindow>();
    }

    // Creates a new root transform for the waypoints, which the waypoints will be children of
    public Transform waypointRoot;

    // Draws the editor window
    private void OnGUI()
    {
        // Creates a serialized object of the current object
        SerializedObject obj = new SerializedObject(this);

        // Draws the waypoint root field
        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        // If the root transform is not selected, it will show a warning
        if(waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        // Applies the changes to the object
        obj.ApplyModifiedProperties();
    }

    // Draws the buttons in the editor window
    void DrawButtons(){
        if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if(GUILayout.Button("Add Branch"))
            {
                CreateBranch();
            }
            if(GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if(GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if(GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
            if(GUILayout.Button("Assign as Traffic Light"))
            {
                //Selection.activeGameObject.GetComponent<Waypoint>().waypointType = Waypoint.WaypointType.TrafficLight;
            }
            //if(GUILayout.Button("Assign as Turn"))
            //{
                //Selection.activeGameObject.GetComponent<Waypoint>().waypointType = Waypoint.WaypointType.Turn;
            //}
            if(GUILayout.Button("Assign as Bus Stop")){
                //Selection.activeGameObject.GetComponent<Waypoint>().waypointType = Waypoint.WaypointType.BusStop;
            }
        }
    }

    // Creates a new waypoint
    void CreateWaypoint(){
        // Creates the waypoint underneath the root transform, with an incremented counter
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        // Gets the waypoint component and sets the previous waypoint to the last waypoint
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        if(waypointRoot.childCount > 1){
            waypoint.previousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;

            // Place the waypoint at the last position
            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward; 
        }

        // Selects the waypoint
        Selection.activeGameObject = waypoint.gameObject;
    }
    void CreateBranch(){
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        branchedFrom.branches.Add(waypoint);

        waypoint.transform.position = branchedFrom.transform.position;
        waypoint.transform.forward = branchedFrom.transform.forward;

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CreateWaypointBefore(){
        // Creates the waypoint underneath the root transform, with an incremented counter
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        // Gets the waypoint component and sets the previous waypoint to the last waypoint
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        // Take the selected object position and forward, and save it in the new object
        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward; 

        // if the selected waypoint does not have a previous waypoint, it will be the first one
        if(selectedWaypoint.previousWaypoint != null)
        {
            newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
        }

        // Set the next waypoint of the new waypoint to the selected waypoint
        newWaypoint.nextWaypoint = selectedWaypoint;

        // Set the previous waypoint of the selected waypoint to the new waypoint
        selectedWaypoint.previousWaypoint = newWaypoint;

        // Set the new waypoint to the same index as the selected waypoint
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        
        // Selects the waypoint
        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CreateWaypointAfter(){
        // Creates the waypoint underneath the root transform, with an incremented counter
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        // Gets the waypoint component and sets the previous waypoint to the last waypoint
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        // Take the selected object position and forward, and save it in the new object
        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward; 

        // If the selected waypoint does not have a next waypoint, the new one will be the next one
        if(selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
        }

        // Assigns the new waypoint as the next waypoint of the selected waypoint
        selectedWaypoint.nextWaypoint = newWaypoint;

        // Updates the sibling index of the new waypoint
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        // Selects the waypoint
        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void RemoveWaypoint(){
        // Gets the selected waypoint
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        // If the selected waypoint has a next waypoint, it will set the previous waypoint of the next waypoint to the previous waypoint of the selected waypoint
        if(selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
        }
        // if the selected waypoint has a previous waypoint, it will set the next waypoint of the previous waypoint to the next waypoint of the selected waypoint
        if(selectedWaypoint.previousWaypoint != null)
        {
            selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
        }

        // Destroys the gameobject
        DestroyImmediate(selectedWaypoint.gameObject);
    }
}
