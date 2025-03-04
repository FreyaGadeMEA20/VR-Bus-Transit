using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public enum Transition
    {
        None = 0,
        SawPlayer,
        ReachPlayer,
        LostPlayer,
        NoHealth,
    }

    public enum FSMStateID
    {
        None = 0,
        Patrolling,
        Chasing,
        Attacking,
        Dead,
    }

    public class StateMachine : FSM
    {

    }
}