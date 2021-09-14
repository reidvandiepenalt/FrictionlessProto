using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    float timer = 0f;
    bool dismissable = false;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer >= 5) { dismissable = true; }
    }

    public void OnDismiss()
    {
        if (dismissable) { gameObject.SetActive(false); }
    }

    private void OnEnable()
    {
        timer = 0f;
    }
}
