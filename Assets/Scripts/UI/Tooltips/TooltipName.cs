using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipName : MonoBehaviour
{
    private TextMeshProUGUI AbilityNameLabel;
    private Tooltip tooltip;
    void Start()
    {
        tooltip = GetComponentInParent<Tooltip>();
        AbilityNameLabel = GetComponent<TextMeshProUGUI>();
        AbilityNameLabel.text = tooltip.AbilityName;
    }
}
