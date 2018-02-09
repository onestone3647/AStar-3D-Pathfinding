using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> movingUnits = new List<GameObject>(); 

    public static UnitManager instance; 

    private void Awake()
    {

       instance = this; 
    }

}