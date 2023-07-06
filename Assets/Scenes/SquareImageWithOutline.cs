using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareImageWithOutline : MonoBehaviour
{
    public Color outlineColor = Color.red;
    public Vector2 outlineDistance = new Vector2(5f, 5f);
    public float moveInterval = 4f;

    private Image image;
    private Outline outline;
    private RectTransform rectTransform;

    private Vector3[] positions;
    public List<int> currentPositionIndex;
    public static int currentPosition;

    private void Start()
    {
        image = GetComponent<Image>();

        // Disable the image component's default outline effect
        image.material = new Material(Shader.Find("UI/Default"));

        outline = gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;

        rectTransform = GetComponent<RectTransform>();

        SetImageAlpha(1f);

        currentPositionIndex = new List<int>() {4, 0, 4, 1, 4, 1, 4, 1, 4, 2, 4, 3, 4, 0, 4, 3, 4, 1, 4, 2, 4, 0, 4, 3, 4};

        positions = new Vector3[]
        {
            new Vector3(0f, -250f, 0f),
            new Vector3(-250f, 0f, 0f),
            new Vector3(250f, 0f, 0f),
            new Vector3(0f, 220f, 0f),
            new Vector3(1000f, 1000f, 0f)
        };

        StartCoroutine(MoveOutline());
    }

    private void SetImageAlpha(float alpha)
    {
        // Set the alpha value of the image's material color
        Color materialColor = image.material.color;
        materialColor.a = alpha;
        image.material.color = materialColor;

        // Set the alpha value of the outline's effect color
        Color effectColor = outline.effectColor;
        effectColor.a = 1f; // Ensure the outline color remains fully opaque
        outline.effectColor = effectColor;
    }

    private IEnumerator MoveOutline()
    {
        for(int i=0; i < currentPositionIndex.Count; i++)
        {
            yield return new WaitForSeconds(moveInterval);

            currentPosition = currentPositionIndex[i];            
            Vector3 targetPosition = positions[currentPosition];

            // Move the outline to the target position
            rectTransform.anchoredPosition = targetPosition;
            // UnityEngine.Debug.Log("currentPositionIndex: " + currentPositionIndex[i]);
        };
    }
}