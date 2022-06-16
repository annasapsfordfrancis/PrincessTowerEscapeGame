using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public static int CrossbowBolts;
    public static int FireballScrolls;
    public static int LightningScrolls;
    public static int ShieldScrolls;
    public static int HealthPotions;
    public GameObject UI;
    private HUD HUD;
    private GameObject playerObject;
    private PlayerStats playerStats;
    private PlayerManager playerManager;
    private SoundManager soundManager;

    void Start()
    {
        HUD = UI.GetComponent<HUD>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        // Reset inventory at start of game to prevent bugs when player restarts without quitting.
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            ResetInventory();
        }
    }

    private void GetPlayerReferences()
    {
        playerObject = GameObject.FindWithTag("Player");
        playerStats = playerObject.GetComponent<PlayerStats>();
        playerManager = playerObject.GetComponent<PlayerManager>();
    }

    private void ResetInventory()
    {
        HealthPotions = 0;
        CrossbowBolts = 0;
        FireballScrolls = 0;
        LightningScrolls = 0;
        ShieldScrolls = 0;

        HUD.UpdateHealthPotionNumberText();
        HUD.UpdateCrossbowBoltNumberText();
        HUD.UpdateFireballScrollNumberText();
        HUD.UpdateLightningScrollNumberText();
        HUD.UpdateShieldScrollNumberText();
    }

    public void AddItem(string type)
    {
        if (!playerObject) GetPlayerReferences();

        if (type == "HealthPotion")
        {
            HealthPotions++;
            HUD.UpdateHealthPotionNumberText();
            HUD.CreateFloatingMessage("+1 potion", playerObject.transform);
        }
        else if (type == "CrossbowBolt")
        {
            int bolts = Random.Range(1,4);
            CrossbowBolts += bolts;
            HUD.UpdateCrossbowBoltNumberText();
            string message = $"+{bolts} bolts";
            HUD.CreateFloatingMessage(message, playerObject.transform);
        }
        else if (type == "FireballScroll")
        {
            FireballScrolls++;
            HUD.UpdateFireballScrollNumberText();
            HUD.CreateFloatingMessage("+1 scroll", playerObject.transform);
        }
        else if (type == "LightningScroll")
        {
            LightningScrolls++;
            HUD.UpdateLightningScrollNumberText();
            HUD.CreateFloatingMessage("+1 scroll", playerObject.transform);
        }
        else if (type == "ShieldScroll")
        {
            ShieldScrolls++;
            HUD.UpdateShieldScrollNumberText();
            HUD.CreateFloatingMessage("+1 scroll", playerObject.transform);
        }
    }

    public void AimCrossbow()
    {
        // If we don't have a reference to player get it
        if (!playerObject) GetPlayerReferences();

        if (CrossbowBolts > 0 && playerManager.PlayerCanUseItem())
        {
            // Change cursor
            // Change player targetting status in player manager
            playerManager.StartAiming("crossbow");
            
        }
    }

    public void FireCrossbow(Transform target)
    {
        // Make sure player targetting status is aiming crossbow
        // Get crossbow damage
        // Deal damage to target
        playerStats.DealDamage(4, target);
        playerStats.UseAction();
        CrossbowBolts--;
        HUD.UpdateCrossbowBoltNumberText();

    }

    public void AimFireball()
    {
        // If we don't have a reference to player get it
        if (!playerObject) GetPlayerReferences();

        if (FireballScrolls > 0 && playerManager.PlayerCanUseItem())
        {
            // Change cursor
            // Change player targetting status in player manager
            playerManager.StartAiming("fireball");
        }
    }

    public void CastFireball(List<Transform> targets)
    {
        if (targets.Count == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                playerStats.DealDamage(4, targets[i]);
            }
            
            playerStats.UseAction();
            FireballScrolls--;
            HUD.UpdateFireballScrollNumberText();
        }        
    }

    public void AimLightning()
    {
        // If we don't have a reference to player get it
        if (!playerObject) GetPlayerReferences();

        if (LightningScrolls > 0 && playerManager.PlayerCanUseItem())
        {
            // Change cursor
            // Change player targetting status in player manager
            playerManager.StartAiming("lightning");
        }
    }

    public void CastLightning(List<Transform> targets)
    {

        if (targets.Count == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                playerStats.DealDamage(4, targets[i]);
            }
        playerStats.UseAction();
        LightningScrolls--;
        HUD.UpdateLightningScrollNumberText();
        }
    }

    public void UseShieldScroll()
    {
        // If we don't have a reference to player get it
        if (!playerObject) GetPlayerReferences();

        if (ShieldScrolls > 0 && playerManager.PlayerCanUseItem())
        {
            playerObject.GetComponent<PlayerStats>().UpdateTempHP(20f);
            playerStats.UseAction();
            ShieldScrolls--;
            HUD.UpdateShieldScrollNumberText();
            soundManager.PlaySound("buff");
        }
    }
    
    public void UseHealthPotion()
    {
        // If we don't have a reference to player get it
        if (!playerObject) GetPlayerReferences();

        if (HealthPotions > 0 && playerManager.PlayerCanUseItem() && PlayerStats.CurrentHP < playerStats.MaxHP)
        {
            playerObject.GetComponent<PlayerStats>().Heal(20f);
            playerStats.UseAction();
            HealthPotions--;
            HUD.UpdateHealthPotionNumberText();
            soundManager.PlaySound("buff");
        }
    }
}
