using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverTextScript : MonoBehaviour
{
    public TMP_Text hoverTextTMP;
  //  public GameObject background;


    public string message;
    public float displayRadius;
    public float fadeInDistance;
    public float xOffset;
    public float yOffset;
    private bool isDisplaying;
    private GameObject player;

    public bool notInteracting;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (hoverTextTMP != null)
        {
            hoverTextTMP.gameObject.SetActive(false);
          //  background.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && hoverTextTMP != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance <= displayRadius + fadeInDistance && (notInteracting == false))
            {
                if (!isDisplaying) //turn on text 
                {
                    isDisplaying = true;
                    hoverTextTMP.gameObject.SetActive(true);
                  //  background.gameObject.SetActive(true);
                    hoverTextTMP.text = message;
                }

                
                if (fadeInDistance > 0 && distance > displayRadius) //if close enough to start fading in but not enough to fully display text
                {
                    
                    float alpha = Mathf.Clamp01(1f - ((distance - displayRadius) / fadeInDistance)); //locks alpha between 0 and 1

                    Color textColor = hoverTextTMP.color; 
                    textColor.a = alpha;
                    hoverTextTMP.color = textColor;

                    
                }
                else if (distance <= displayRadius) //if close enough to fully display text
                {
                    
                    Color textColor = hoverTextTMP.color;
                    textColor.a = 1f;
                    hoverTextTMP.color = textColor;
                }
            }
            else //turn off text
            {
                isDisplaying = false;
                hoverTextTMP.gameObject.SetActive(false);
              //  background.gameObject.SetActive(false);
            }

            if (isDisplaying) //keep text on correct part of screen
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(xOffset, yOffset, 0));
                hoverTextTMP.rectTransform.position = screenPos; // Adjust offset as needed
            }
        }
    }
}
