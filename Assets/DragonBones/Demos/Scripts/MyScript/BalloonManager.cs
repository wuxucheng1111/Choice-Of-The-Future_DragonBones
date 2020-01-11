using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloonManager : MonoBehaviour
{
    public GameObject balloon;
    public float invokeTime;
    public List<GameObject> allBalloon; //大玉列表
    public int life;
    public int score;
    public Text scoreText;
    public Text lifeText;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("BalloonCreat", 1f, invokeTime);
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "score:" + score.ToString();
        lifeText.text = "life:" + life.ToString();
    }

    void BalloonCreat()
    {
        GameObject b = Instantiate(balloon, new Vector3(11, Random.Range(-2.5f, 5.5f)), Quaternion.identity);
        allBalloon.Add(b);
    }
    public void RemoveBalloon(int index)
    {
        allBalloon.RemoveAt(index);
    }
    public void RemoveBalloon(GameObject balloon)
    {
        allBalloon.Remove(balloon);
    }

}
