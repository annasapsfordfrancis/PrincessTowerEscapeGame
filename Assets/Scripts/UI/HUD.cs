using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI HPText;
    public GameObject TempHPIcon;
    public TextMeshProUGUI TempHPText;
    public GameObject MessageBox;
    public GameObject StartText;
    public GameObject WinText;
    public GameObject LoseText;
    public GameObject PauseText;
    public TextMeshProUGUI CrossbowBoltNumberText;
    public TextMeshProUGUI FireballScrollNumberText;
    public TextMeshProUGUI LightningScrollNumberText;
    public TextMeshProUGUI ShieldScrollNumberText;
    public TextMeshProUGUI HealthPotionNumberText;

    public Button CrossbowBoltButton;
    public Button FireballScrollButton;
    public Button LightningScrollButton;
    public Button ShieldScrollButton;
    public Button HealthPotionButton;
    public GameObject TargetSquarePrefab;
    public GameObject TargetGridPrefab;

    public GameObject FloatingDamageTakenPrefab;
    public GameObject FloatingDamageDealtPrefab;
    public GameObject FloatingHealingReceivedPrefab;
    public GameObject FloatingMessagePrefab;
    public GameObject TurnSystemObject;
    private List<Button> actionBarButtonList;
    private bool aimingCursorActive;
    private TurnSystem turnSystem;
    private GameObject playerObject;
    private PlayerStats playerStats;
    private GameObject abilityTargetInstance;

    void Start()
    {
        turnSystem = TurnSystemObject.GetComponent<TurnSystem>();
        turnSystem.onWinCallback += ShowMessage;
        turnSystem.onLoseCallback += ShowMessage;
        turnSystem.onPauseCallback += ShowMessage;
        playerObject = GameObject.FindWithTag("Player");
        playerStats = playerObject.GetComponent<PlayerStats>();

        actionBarButtonList = new List<Button>{CrossbowBoltButton, FireballScrollButton, LightningScrollButton, ShieldScrollButton, HealthPotionButton};
        aimingCursorActive = false;
        UpdateHPText();
        UpdateActionBarText();

    }

    public void DismissMessage()
    {
        if (TurnSystem.state == TurnState.START)
        {
            StartText.SetActive(false);
            MessageBox.SetActive(false);
            turnSystem.StartPlayerTurn();
        }
        else if (TurnSystem.state == TurnState.PAUSE)
        {
            PauseText.SetActive(false);
            MessageBox.SetActive(false);
            turnSystem.ResumeGame();
        }
    }

    public void ShowMessage()
    {
        if (TurnSystem.state == TurnState.START)
        {
            MessageBox.SetActive(true);
            StartText.SetActive(true);
        }
        else if (TurnSystem.state == TurnState.WIN)
        {
            MessageBox.SetActive(true);
            WinText.SetActive(true);

        }
        else if (TurnSystem.state == TurnState.LOSE)
        {
            MessageBox.SetActive(true);
            LoseText.SetActive(true);
        }
        else if (TurnSystem.state == TurnState.PAUSE)
        {
            MessageBox.SetActive(true);
            PauseText.SetActive(true);
        }
        else
        {
            Debug.Log("Error: no message exists for this state");
            return;
        }
    }


    public void ActivateAimingReticule(string ability)
    {
        if (!aimingCursorActive)
        {
            Cursor.visible = false;
            aimingCursorActive = true;

            if (ability == "crossbow")
            {
                abilityTargetInstance = Instantiate(TargetSquarePrefab, Vector3.zero, Quaternion.identity);
            }
            else if (ability == "fireball")
            {
                abilityTargetInstance = Instantiate(TargetGridPrefab, Vector3.zero, Quaternion.identity);
            }
            else if (ability == "lightning")
            {
                abilityTargetInstance = Instantiate(TargetSquarePrefab, Vector3.zero, Quaternion.identity);
            }
        }

    }

    public void DeactivateAimingReticule()
    {
        if (aimingCursorActive)
        {
            Destroy(abilityTargetInstance);
            Cursor.visible = true;
            aimingCursorActive = false;
        }
    }

    public Vector3 GetAbilityTargetPosition()
    {
        return abilityTargetInstance.transform.position;
    }


    public void UpdateHPText()
    {
        HPText.text = $"HP: {PlayerStats.CurrentHP} / {playerStats.MaxHP}";
    }

    public void UpdateTempHPText()
    {
        TempHPText.text = PlayerStats.TempHP.ToString();

        // Hide TempHP icon if TempHP is 0
        if (PlayerStats.TempHP == 0 && TempHPIcon.activeSelf)
        {
            TempHPIcon.SetActive(false);
        }
        // Show TempHP icon if TempHP above 0 and icon is hidden
        else if (PlayerStats.TempHP > 0 && !TempHPIcon.activeSelf)
        {
            TempHPIcon.SetActive(true);
        }
    }

    public void DisableActionBarButtons()
    {
        foreach (Button button in actionBarButtonList)
        {
            button.interactable = false;
        }

    }

    public void DisableActionButtonIfItemsZero(Button button, int inventoryItem)
    {
        if (inventoryItem == 0 && button.interactable)
        {
            button.interactable = false;
        }
    }

    public void EnableActionButtonIfItemsLeft(Button button, int inventoryItem)
    {
        if (inventoryItem > 0 && !button.interactable)
        {
            button.interactable = true;
        }
    }

    // Enables action bar buttons but skips buttons at 0
    public void EnableActionBarButtons()
    {
        EnableActionButtonIfItemsLeft(CrossbowBoltButton, Inventory.CrossbowBolts);
        EnableActionButtonIfItemsLeft(FireballScrollButton, Inventory.FireballScrolls);
        EnableActionButtonIfItemsLeft(LightningScrollButton, Inventory.LightningScrolls);
        EnableActionButtonIfItemsLeft(ShieldScrollButton, Inventory.ShieldScrolls);
        EnableActionButtonIfItemsLeft(HealthPotionButton, Inventory.HealthPotions);
    }


    public void UpdateCrossbowBoltNumberText()
    {
        CrossbowBoltNumberText.text = Inventory.CrossbowBolts.ToString();
        DisableActionButtonIfItemsZero(CrossbowBoltButton, Inventory.CrossbowBolts);
        EnableActionButtonIfItemsLeft(CrossbowBoltButton, Inventory.CrossbowBolts);
    }

    public void UpdateFireballScrollNumberText()
    {
        FireballScrollNumberText.text = Inventory.FireballScrolls.ToString();

        EnableActionButtonIfItemsLeft(FireballScrollButton, Inventory.FireballScrolls);
        DisableActionButtonIfItemsZero(FireballScrollButton, Inventory.FireballScrolls);
    }

    public void UpdateLightningScrollNumberText()
    {
        LightningScrollNumberText.text = Inventory.LightningScrolls.ToString();

        EnableActionButtonIfItemsLeft(LightningScrollButton, Inventory.LightningScrolls);
        DisableActionButtonIfItemsZero(LightningScrollButton, Inventory.LightningScrolls);
    }

    public void UpdateShieldScrollNumberText()
    {
        ShieldScrollNumberText.text = Inventory.ShieldScrolls.ToString();

        EnableActionButtonIfItemsLeft(ShieldScrollButton, Inventory.ShieldScrolls);
        DisableActionButtonIfItemsZero(ShieldScrollButton, Inventory.ShieldScrolls);
    }

    public void UpdateHealthPotionNumberText()
    {
        HealthPotionNumberText.text = Inventory.HealthPotions.ToString();

        EnableActionButtonIfItemsLeft(HealthPotionButton, Inventory.HealthPotions);
        DisableActionButtonIfItemsZero(HealthPotionButton, Inventory.HealthPotions);
    }

    // Calls all of the text update functions
    public void UpdateActionBarText()
    {
        UpdateCrossbowBoltNumberText();
        UpdateFireballScrollNumberText();
        UpdateLightningScrollNumberText();
        UpdateShieldScrollNumberText();
        UpdateHealthPotionNumberText();
    }

    public void CreateFloatingDamageTakenNumber(int amount, Transform parent)
    {
        GameObject floatingDamageNumberText = Instantiate(FloatingDamageTakenPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        floatingDamageNumberText.GetComponent<TextMeshPro>().text = amount.ToString();
        floatingDamageNumberText.transform.SetParent(parent, false);
        Object.Destroy(floatingDamageNumberText, 1f);
    }

    public void CreateFloatingDamageDealtNumber(int amount, Transform target)
    {
        GameObject floatingDamageNumberText = Instantiate(FloatingDamageDealtPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        floatingDamageNumberText.GetComponent<TextMeshPro>().text = amount.ToString();
        
        floatingDamageNumberText.transform.position = target.position;
        Object.Destroy(floatingDamageNumberText, 1f);
    }

    public void CreateFloatingHealNumber(int amount, Transform parent)
    {
        GameObject floatingHealNumberText = Instantiate(FloatingHealingReceivedPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        floatingHealNumberText.GetComponent<TextMeshPro>().text = $"+{amount}";
        floatingHealNumberText.transform.SetParent(parent, false);
        Object.Destroy(floatingHealNumberText, 1f);
    }

    public void CreateFloatingMessage(string message, Transform parent)
    {
        GameObject floatingMessage = Instantiate(FloatingMessagePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        floatingMessage.GetComponent<TextMeshPro>().text = message;
        floatingMessage.transform.SetParent(parent, false);
        Object.Destroy(floatingMessage, 1f);
    }
}
