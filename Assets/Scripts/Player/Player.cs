using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour, IMove
{
    [SerializeField] private float speed;
    [SerializeField] private Transform camera;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private Transform spawnPoint;
    private Rigidbody rigidbody;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private int lives;
    public int Lives => lives; 

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform.position = spawnPoint.position;
        lives = 3;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            transform.eulerAngles += new Vector3(0, horizontalSpeed * Input.GetAxis("Mouse X"), 0);
        }
    }
    public void Move(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            rigidbody.velocity = (transform.forward * direction.z * speed) + (transform.right * direction.x * speed) + 
                                 new Vector3(0, rigidbody.velocity.y, 0);
            camera.position += (camera.right * direction.x * speed);
        }
        else
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") 
        {
            transform.position = spawnPoint.position;
            lives--;
        }
        if (collision.gameObject.tag == "Key") 
        {
            collision.gameObject.SetActive(false);
        }
    }
}
