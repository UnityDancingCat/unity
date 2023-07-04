using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlickeringEffect3 : MonoBehaviour
{
    private bool isFlickering = false;
    private Image image;
    private Color originalColor;

    private void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;

        StartFlickering();
    }

    private void StartFlickering()
    {
        if (isFlickering)
            return;

        isFlickering = true;
        StartCoroutine(Flicker());
    }

    public void StopFlickering()
    {
        if (!isFlickering)
            return;

        isFlickering = false;
        StopCoroutine(Flicker());
        image.color = originalColor;
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            //오브젝트 비활성화
            image.color = Color.clear;
            yield return new WaitForSeconds(0.08f);
            //오브젝트 활성화
            image.color = originalColor;
            yield return new WaitForSeconds(0.08f);
        }
    }
}

