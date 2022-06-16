using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSquare : MonoBehaviour
{
    public Color ValidTargetColor;
    public Color InvalidTargetColor;
    public Color LineRendererValid;
    public Color LineRendererInvalid;
    private Vector3 positionNormalizer;
    private SpriteRenderer spriteRenderer;
    private PlayerManager playerManager;
    private Vector3 playerPosition;
    private Vector3[] lineRendererPositions;
    private Vector3 lineRendererNormalizer;
    private LineRenderer lineRenderer;
    void Start()
    {
        positionNormalizer = new Vector3(0.5f, 0.5f, 10f);
        playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
        playerPosition = GameObject.FindWithTag("Player").transform.position;
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = InvalidTargetColor;
        lineRendererNormalizer = new Vector3(0f, 0f, -0.5f);
        lineRendererPositions = new Vector3[2] {playerPosition + lineRendererNormalizer, playerPosition + lineRendererNormalizer};
        lineRenderer.SetPositions(lineRendererPositions);
        lineRenderer.material.color = LineRendererInvalid;

    }

    void Update()
    {
        transform.position = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + positionNormalizer;
        bool targetValid = playerManager.AbilityTargetIsValid(transform.position);

        // Update second point to target location
        lineRendererPositions[1] = transform.position + lineRendererNormalizer;
        lineRenderer.SetPosition(1, lineRendererPositions[1]);

        if (targetValid && spriteRenderer.color == InvalidTargetColor)
        {
            spriteRenderer.color = ValidTargetColor;
            lineRenderer.material.color = LineRendererValid;

        }
        else if (!targetValid && spriteRenderer.color == ValidTargetColor)
        {
            spriteRenderer.color = InvalidTargetColor;
            lineRenderer.material.color = LineRendererInvalid;

        }
    }
}
