using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] Transform anchorPoint;
    [SerializeField] GameObject bulletboy;
    [SerializeField] Transform shootFromWhere;
    [Tooltip("CALM DOWN HORSEMAN")]
    [SerializeField] float shootyBoyCoolDownInSeconds;

    Vector2 mousePos;

    float timeSinceAShootyBoyHasPassed;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceAShootyBoyHasPassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceAShootyBoyHasPassed += Time.deltaTime;
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        diff.Normalize();

        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        if (Input.GetAxisRaw("Fire1") > 0.5f && shootyBoyCoolDownInSeconds < timeSinceAShootyBoyHasPassed) DoAShootyBoy();
    }

    void DoAShootyBoy()
    {
        print("DOING A SHOOTY BOY");
        timeSinceAShootyBoyHasPassed = 0;
        GameObject newBulletboy = Instantiate(bulletboy);
        newBulletboy.transform.position = shootFromWhere.position;
        newBulletboy.transform.position.Set(newBulletboy.transform.position.x, newBulletboy.transform.position.y, 0);
        Rigidbody2D rb = newBulletboy.GetComponent<Rigidbody2D>();
        newBulletboy.transform.rotation = transform.rotation;
        rb.velocity = transform.right * 15f;
    }
}
