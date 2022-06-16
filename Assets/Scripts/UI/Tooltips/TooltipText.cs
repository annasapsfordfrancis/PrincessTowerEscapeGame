using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipText : MonoBehaviour
{
    private TextMeshProUGUI AbilityTextLabel;
    private Tooltip tooltip;
    void Start()
    {
        tooltip = GetComponentInParent<Tooltip>();
        AbilityTextLabel = GetComponent<TextMeshProUGUI>();
        AbilityTextLabel.text = tooltip.AbilityText;
    }
}
