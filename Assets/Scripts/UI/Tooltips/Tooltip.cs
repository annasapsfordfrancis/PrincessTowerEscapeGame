using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public string AbilityName;
    public string AbilityText;
    public GameObject TooltipPrefab;
    private Vector3 tooltipPosition;
    private Button button;
    private GameObject tooltip;
    private CanvasGroup uiGroup;
    private bool tooltipExists;
    private float targetTooltipAlpha;
    void Start()
    {
        targetTooltipAlpha = 0;
        tooltipPosition = new Vector3(-40, 78, 0);
        tooltipExists = false;
    }

    void Update()
    {
        if (tooltipExists == true && uiGroup.alpha != targetTooltipAlpha)
        {
            uiGroup.alpha = Mathf.MoveTowards(uiGroup.alpha, targetTooltipAlpha, 6.0f * Time.deltaTime);
        }
        else if (tooltipExists == true && uiGroup.alpha == 0)
        {
            Object.Destroy(tooltip);
            tooltipExists = false;
        }
    }

    public void ShowTooltip()
    {
        button = GetComponent<Button>();
        tooltip = GameObject.Instantiate(TooltipPrefab, tooltipPosition, Quaternion.identity);
        uiGroup = tooltip.GetComponent<CanvasGroup>();
        uiGroup.alpha = 0;
        targetTooltipAlpha = 1;
        tooltipExists = true;
        tooltip.transform.SetParent(button.transform, false);
    }

    public void HideTooltip()
    {
        targetTooltipAlpha = 0;
    }
}
