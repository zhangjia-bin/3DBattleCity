using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSource : MonoBehaviour
{
    public Toggle toggle;
    public AudioSource source;
    void Start()
    {
        toggle.onValueChanged.AddListener((i) => {

            if (i)
            {
                source.Play();
            }
            else
            {
                source.Stop();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
