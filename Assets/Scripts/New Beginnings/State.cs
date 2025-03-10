using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public abstract class State
    {
        protected Dictionary<Transition, FSMStateID> map = new Dictionary<Transition, FSMStateID>();
        protected FSMStateID stateID;
        public FSMStateID ID {get {return stateID; } }

        public void AddTransition(Transition transition, FSMStateID id)
        {
            // Check if anyone of the args is invallid
            if (transition == Transition.None || id == FSMStateID.None)
            {
                Debug.LogWarning("FSMState : Null transition not allowed");
                return;
            }

            //Since this is a Deterministc FSM,
            //Check if the current transition was already inside the map
            if (map.ContainsKey(transition))
            {
                Debug.LogWarning("FSMState ERROR: transition is already inside the map");
                return;
            }

            map.Add(transition, id);
            Debug.Log("Added : " + transition + " with ID : " + id);
        }

        /// <summary>
        /// This method deletes a pair transition-state from this state?s map.
        /// If the transition was not inside the state?s map, an ERROR message is printed.
        /// </summary>
        public void DeleteTransition(Transition trans)
        {
            // Check for NullTransition
            if (trans == Transition.None)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed");
                return;
            }

            // Check if the pair is inside the map before deleting
            if (map.ContainsKey(trans))
            {
                map.Remove(trans);
                return;
            }
            Debug.LogError("FSMState ERROR: Transition passed was not on this State?s List");
        }


        /// <summary>
        /// This method returns the new state the FSM should be if
        ///    this state receives a transition  
        /// </summary>
        public FSMStateID GetOutputState(Transition trans)
        {
            // Check for NullTransition
            if (trans == Transition.None)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed");
                return FSMStateID.None;
            }

            // Check if the map has this transition
            if (map.ContainsKey(trans))
            {
                return map[trans];
            }

            Debug.LogError("FSMState ERROR: " + trans+ " Transition passed to the State was not on the list");
            return FSMStateID.None;
        }

        /// <summary>
        /// Decides if the state should transition to another on its list
        /// NPC is a reference to the npc tha is controlled by this class
        /// </summary>
        public abstract void CheckTransitionRules(Transform player, Transform npc);

        /// <summary>
        /// This method controls the behavior of the NPC in the game World.
        /// Every action, movement or communication the NPC does should be placed here
        /// NPC is a reference to the npc tha is controlled by this class
        /// </summary>
        public abstract void RunState(Transform player, Transform npc);


        /// <summary>
        /// Check whether the next random position is the same as current tank position
        /// </summary>
        /// <param name="pos">position to check</param>
        protected bool IsInCurrentRange(Transform trans, Vector3 pos)
        {
            float xPos = Mathf.Abs(pos.x - trans.position.x);
            float zPos = Mathf.Abs(pos.z - trans.position.z);

            if (xPos <= 50 && zPos <= 50)
                return true;

            return false;
        }
    }
}