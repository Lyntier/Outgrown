using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (transform.position.x < -10 || transform.position.x > 10
            || transform.position.y < -10 || transform.position.y > 10) {
            Destroy(gameObject);
        }
    }
}
