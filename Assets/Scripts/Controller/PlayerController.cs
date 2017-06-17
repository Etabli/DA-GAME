using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Vector3 PrevPosition;
    Vector3 CurrentPosition;
    public float movementSpeed = 2.0f;

	// Use this for initialization
	void Start () {
        UpdateRotation();
	}
	
	// Update is called once per frame
	void Update () {

        UpdateMovement();
        UpdateRotation();
    }

    /// <summary>
    /// updates player movment
    /// </summary>
    void UpdateMovement()
    {
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        transform.Translate(inputDir * Time.deltaTime * movementSpeed,Space.World);
        Camera.main.transform.position = new Vector3(transform.position.x,transform.position.y,-10f);
    }

    /// <summary>
    /// Updates the player rotation
    /// </summary>
    void UpdateRotation()
    {
        Vector3 mouse= Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotationTowards = mouse - transform.position;


        float angle = Mathf.Atan2(rotationTowards.x, rotationTowards.y) * Mathf.Rad2Deg;

        transform.eulerAngles = Vector3.back * angle;
    }

}
