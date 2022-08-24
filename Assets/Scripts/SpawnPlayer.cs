using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPackagePrefab;
    void Start()
    {
        GameObject playerPackage = Instantiate(playerPackagePrefab, Vector3.zero, Quaternion.identity);
    }

}
