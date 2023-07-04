using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareImageWithOutline : MonoBehaviour
{
    public Color outlineColor = Color.red;
    public Vector2 outlineDistance = new Vector2(5f, 5f);
    public float moveInterval = 7f;

    private Image image;
    private Outline outline;
    private RectTransform rectTransform;

    private Vector3[] positions;
    private int currentPositionIndex = 0;

    private void Start()
    {
        image = GetComponent<Image>();

        // Disable the image component's default outline effect
        image.material = new Material(Shader.Find("UI/Default"));

        outline = gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;

        rectTransform = GetComponent<RectTransform>();

        SetImageAlpha(0.5f);

        positions = new Vector3[]
        {
            new Vector3(0f, -250f, 0f),
            new Vector3(-250f, 0f, 0f),
            new Vector3(250f, 0f, 0f),
            new Vector3(0f, -250f, 0f)
        };

        StartCoroutine(MoveOutline());
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private IEnumerator MoveOutline()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            currentPositionIndex = (currentPositionIndex + 1) % positions.Length;
            Vector3 targetPosition = positions[currentPositionIndex];

            // Move the outline to the target position
            rectTransform.anchoredPosition = targetPosition;
        }
    }
}
