using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectScript : MonoBehaviour
{
    public float fadeInTime;
    public float lingerTime;
    public float fadeOutTime;
    public int state;
    SpriteRenderer sp;
    GameObject parentCam;
    // Start is called before the first frame update
    void Start()
    {
        Transform parentTransform = transform.parent;
        parentCam = parentTransform.gameObject;
        sp = GetComponent<SpriteRenderer>();
        sp.color = new Color(0, 0, 0, 0);
        sp.enabled = true;
        StartCoroutine(FadeObject());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(parentCam.transform.position.x, parentCam.transform.position.y, 0);
    }

    IEnumerator FadeObject()
    {
        yield return new WaitForSeconds(0.01f);
        int i = 0;
        while (i < 30)
        {
            yield return new WaitForSeconds(1f/30f * fadeInTime);

            sp.color = new Color(0, 0, 0, i/30f);
            i++;
        }

        sp.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(lingerTime);

        int j = 0;

        while (j < 30)
        {
            yield return new WaitForSeconds(1f / 30f * fadeOutTime);

            sp.color = new Color(0, 0, 0, 1-(j / 30f));

            j++;
        }

        Destroy(gameObject);
    }

}
