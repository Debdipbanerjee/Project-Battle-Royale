using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    public bool isAutomatic;
    public float timeBetweenShots = 0.1f, heatPerShot = 1f;
    public GameObject muzzleFlash;
}
