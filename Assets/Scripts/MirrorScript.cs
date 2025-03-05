using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorScript : MonoBehaviour
{
    private Transform trans;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        trans = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        offset = trans.rotation.eulerAngles - transform.rotation.eulerAngles;  
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = Quaternion.Euler(trans.rotation.eulerAngles - offset * -1f);
        gameObject.transform.rotation = rot; 
    }
}
