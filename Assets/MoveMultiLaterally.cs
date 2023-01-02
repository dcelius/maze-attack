using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMultiLaterally : MonoBehaviour
{

    // Use this for initialization

    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {

        if (Input.GetKey("r"))

            transform.Translate(Vector3.forward * Time.deltaTime);

        if (Input.GetKey("e"))

            transform.Translate(Vector3.back * Time.deltaTime);

        if (Input.GetKey("d"))

            transform.Translate(Vector3.left * Time.deltaTime);

        if (Input.GetKey("f"))

            transform.Translate(Vector3.right * Time.deltaTime);

        if (Input.GetKey("c"))

            transform.Translate(Vector3.up * Time.deltaTime);

        if (Input.GetKey("v"))

            transform.Translate(Vector3.down * Time.deltaTime);

    }

}