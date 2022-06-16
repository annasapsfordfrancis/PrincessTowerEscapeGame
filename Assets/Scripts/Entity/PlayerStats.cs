using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
   public string EntityName;
   public int AttackDamage;
   public int MaxHP;
   public int ActionsPerTurn = 1;
   public int ActionsRemainingThisTurn = 1;
   public static int CurrentHP;
   public static int TempHP = 0;
   private HUD HUD;
   private TurnSystem turnSystem;
   private SoundManager soundManager;
   

   void Start()
   {
      turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
      HUD = GameObject.Find("UI").GetComponent<HUD>();
      soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

      // Initializes CurrentHP in first scene
      // Will also set CurrentHP if testing scenes by jumping straight to them
      if (SceneManager.GetActiveScene().buildIndex == 1 || CurrentHP == 0)
      {
         CurrentHP = MaxHP;
         HUD.UpdateHPText();
      }
   }


   // damage is negative number
   public void TakeDamage(int damage)
   {
      // Use temporary HP from shield if we have it
      if (TempHP > 0)
      {
         // Save original damage number
         int originalDamage = damage;

         // Reduce damage by temporary HP
         damage = Mathf.Abs(damage) - TempHP;

         // Don't reduce damage below 0
         if (damage < 0)
         {
            damage = 0;
         }

         // Reduce temporary HP by the amount of damage it prevented
         TempHP -= Mathf.Abs(originalDamage);

         // Don't reduce temporary HP below 0
         if (TempHP < 0)
         {
            TempHP = 0;
         }

         HUD.UpdateTempHPText();
      }

      UpdateHP(damage);
      HUD.UpdateHPText();
      HUD.CreateFloatingDamageTakenNumber(damage, transform);
      if (CurrentHP == 0)
      {
         turnSystem.PlayerDefeated();
      }
   }

   public void UpdateTempHP(float percentage)
   {
      int amount = (int)(MaxHP * (percentage / 100));
      TempHP += amount;
      HUD.UpdateTempHPText();
   }

   public void Heal(float percentage)
   {
      int originalHP = CurrentHP;
      int amount = (int)(MaxHP * (percentage / 100));
      UpdateHP(amount);
      HUD.UpdateHPText();
      int amountHealed = CurrentHP - originalHP;
      HUD.CreateFloatingHealNumber(amountHealed, transform);
   }

   public void DealDamage(int damage, Transform target)
   {
      EnemyStats targetStats = target.GetComponent<EnemyStats>();

      // Create floating number before damage is dealt to target
      HUD.CreateFloatingDamageDealtNumber(damage, target);
      targetStats.UpdateHP(-damage);
   }

   public void UseAction()
   {
      ActionsRemainingThisTurn--;

      if (ActionsRemainingThisTurn <= 0)
      {
         ActionsRemainingThisTurn = 0;
         HUD.DisableActionBarButtons();
      }
   }

   public void ReplenishActions()
   {
      ActionsRemainingThisTurn = ActionsPerTurn;
      HUD.EnableActionBarButtons();
   }

   public void UseAllActions()
   {
      ActionsRemainingThisTurn = 0;
      HUD.DisableActionBarButtons();
   }

   private void UpdateHP(int amount)
   {
      int oldHP = CurrentHP;
      CurrentHP += amount;
        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }
        else if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            transform.gameObject.SetActive(false);
        }
   }
}
