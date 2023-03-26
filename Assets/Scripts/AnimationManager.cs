using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnimationManager : Singleton<AnimationManager>
{
    [SerializeField] public Image screen;
    //private Color fadeToColour;
    private bool transitioning = false;
    private double startTime;
    private readonly double fadeTime = 0.5;

    // Start is called before the first frame update
    void Start()
    {
        //Sets the colour to be black and completely transparent
        screen.color = new Color(0, 0, 0, 0);
    }

    public bool FadeToColour(Color fadeToColour, Action completionCallback)
    {
        if (transitioning)
        {
            return false;
        }
        else
        {
            StartCoroutine(FadeTo(fadeToColour, completionCallback));
            return true;
        }
    }

    public bool FadeFromColour(Action completionCallback)
    {
        if (transitioning)
        {
            return false;
        }
        else
        {
            StartCoroutine(FadeIn(completionCallback));
            return true;
        }
    }

    public IEnumerator FadeTo(Color fadeToColour, Action completionCallback = null)
    {
        transitioning = true;
        screen.color = new Color(fadeToColour.r, fadeToColour.g, fadeToColour.b, 0);
        startTime = Time.unscaledTimeAsDouble;

        double timePassed = 0;
        while (timePassed < fadeTime)
        {
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, (float)(timePassed / fadeTime));
            yield return null;
            timePassed = Time.unscaledTimeAsDouble - startTime;
        }

        screen.color = fadeToColour;

        transitioning = false;

        if (completionCallback != null)
            completionCallback();
    }

    public IEnumerator FadeIn(Action completionCallback = null)
    {
        transitioning = true;
        startTime = Time.unscaledTimeAsDouble;

        double timePassed = 0;
        while (timePassed < fadeTime)
        {
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, (float)(1 - (timePassed / fadeTime)));
            yield return null;
            timePassed = Time.unscaledTimeAsDouble - startTime;
        }

        screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, 0);

        transitioning = false;

        if (completionCallback != null)
            completionCallback();
    }
}
