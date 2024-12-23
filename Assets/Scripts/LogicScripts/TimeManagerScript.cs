using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TimeStop(float realtimeDuration, float delay)
    {
        StartCoroutine(TimeStopCoroutine(realtimeDuration, delay));
    }

    IEnumerator TimeStopCoroutine(float realtimeDuration, float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("Time Stopped");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(realtimeDuration);
        Time.timeScale = 1;
        //Debug.Log("Time Unstopped");
    }
}