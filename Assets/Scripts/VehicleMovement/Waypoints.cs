using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Movement{
    public class Waypoints : MonoBehaviour
    {
        [SerializeField] protected List<Waypoint> m_Waypoints = new List<Waypoint>();

        public virtual List<Waypoint> nodes {
            get{return m_Waypoints;}
        }
        
        public virtual Path GetShortestPath ( Waypoint start, Waypoint end )
        {
            
            // We don't accept null arguments
            if ( start == null || end == null )
            {
                throw new ArgumentNullException ();
            }
            
            // The final path
            Path path = new Path ();

            // If the start and end are same Waypoints, we can return the start Waypoints
            if ( start == end )
            {
                path.waypoints.Add ( start );
                return path;
            }
            
            // The list of unvisited Waypoint
            List<Waypoint> unvisited = new List<Waypoint> ();
            
            // Previous Waypoint in optimal path from source
            Dictionary<Waypoint, Waypoint> previous = new Dictionary<Waypoint, Waypoint> ();
            
            // The calculated distances, set all to Infinity at start, except the start Waypoint
            Dictionary<Waypoint, float> distances = new Dictionary<Waypoint, float> ();
            
            for ( int i = 0; i < m_Waypoints.Count; i++ )
            {
                Waypoint Waypoint = m_Waypoints [ i ];
                unvisited.Add ( Waypoint );
                
                // Setting the Waypoint distance to Infinity
                distances.Add ( Waypoint, float.MaxValue );
            }
            
            // Set the starting Waypoint distance to zero
            distances [ start ] = 0f;
            while ( unvisited.Count != 0 )
            {
                
                // Ordering the unvisited list by distance, smallest distance at start and largest at end
                unvisited = unvisited.OrderBy ( Waypoint => distances [ Waypoint ] ).ToList ();
                
                // Getting the Waypoint with smallest distance
                Waypoint current = unvisited [ 0 ];
                
                // Remove the current Waypoint from unvisisted list
                unvisited.Remove ( current );
                
                // When the current Waypoint is equal to the end Waypoint, then we can break and return the path
                if ( current == end )
                {
                    
                    // Construct the shortest path
                    while ( previous.ContainsKey ( current ) )
                    {
                        
                        // Insert the Waypoint onto the final result
                        path.waypoints.Insert ( 0, current );
                        
                        // Traverse from start to end
                        current = previous [ current ];
                    }
                    
                    // Insert the source onto the final result
                    path.waypoints.Insert ( 0, current );
                    break;
                }
                
                // Looping through the Waypoint connections (neighbors) and where the connection (neighbor) is available at unvisited list
                for ( int i = 0; i < current.connections.Count; i++ )
                {
                    Waypoint neighbor = current.connections [ i ];
                    
                    // Getting the distance between the current Waypoints and the connection (neighbor)
                    float length = Vector3.Distance ( current.transform.position, neighbor.transform.position );
                    
                    // The distance from start Waypoints to this connection (neighbor) of current Waypoints
                    float alt = distances [ current ] + length;
                    
                    // A shorter path to the connection (neighbor) has been found
                    if ( alt < distances [ neighbor ] )
                    {
                        distances [ neighbor ] = alt;
                        previous [ neighbor ] = current;
                    }
                }
            }
            path.Bake ();
            return path;
        }
    }
}