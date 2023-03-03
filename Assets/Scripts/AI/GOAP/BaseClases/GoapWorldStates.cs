using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapWorldStates : MonoBehaviour
{
    public abstract Dictionary<string, bool> GetCurrentWorldState(); //returns the state of the world
}
