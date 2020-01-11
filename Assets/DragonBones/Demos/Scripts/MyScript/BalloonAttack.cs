using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonAttack : MonoBehaviour
{
    public float moveSpeed;
    public float life;
    public BalloonManager balloonManager;
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        balloonManager = GameObject.Find("BalloonManager").GetComponent<BalloonManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
        if ((Time.time - startTime) > life)
        {
            balloonManager.RemoveBalloon(gameObject);
            balloonManager.life -= 1;
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        Destroy(gameObject);
    }
}
