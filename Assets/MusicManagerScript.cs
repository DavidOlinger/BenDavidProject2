using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    AudioSource musicSource;
    public float maxVolume = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeMusic(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = maxVolume;
        musicSource.Play();
        //StartCoroutine(FadeOut(2f, clip));
    }

    private IEnumerator FadeOut( float duration, AudioClip clip)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(duration / 100);
          
            if (musicSource.volume > 0.01)
            {
                musicSource.volume -= 0.01f;
            }

            elapsedTime += Time.deltaTime;
            
        }
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = maxVolume;
        musicSource.Play();
    }
}
