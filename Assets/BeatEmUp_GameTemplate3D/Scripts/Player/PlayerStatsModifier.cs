using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatsModifier : MonoBehaviour
{
    public float allModifier = 1f;

    [Header("Per Level Modifications")]
    public float punchMult = .1f;
    public float kickMult = .1f;
    public float healthAdd = 1;
    public float critAdd;

    private void Start()
    {
        Modify();
    }

    private void Modify()
    {
        var pCombat = GetComponent<PlayerCombat>();
        var health = GetComponent<HealthSystem>();
        var save = SavableSettings.instance;


        if (SceneManager.GetActiveScene().name == "04_TrainingRoom")
        {
            // Super Punch
                Array.Resize(ref pCombat.PunchCombo, pCombat.PunchCombo.Length + 1);
                pCombat.PunchCombo[pCombat.PunchCombo.Length - 1] = pCombat.SuperPunch;
            
            // Super Kick
                Array.Resize(ref pCombat.KickCombo, pCombat.KickCombo.Length + 1);
                pCombat.KickCombo[pCombat.KickCombo.Length - 1] = pCombat.SuperKick;
            
            // Super Punch Combo
            pCombat.superPunchAllowed = true;

            // Super Kick Combo
            pCombat.superKickAllowed = true;

            // Jump Kick Combo
            pCombat.JumpKickAllowed = true;
            
            
            return;
        }


        // Health Add
        health.MaxHp *= allModifier; // allModifier
        health.MaxHp += save.currentCharacter.infiniteUpgrades[SavableSettings.UpgradeType.Health] * healthAdd;
        health.CurrentHp = health.MaxHp;

        // Punch Damage Mult
        ModifyCombo(ref pCombat.PunchCombo, allModifier, 0); // allModifier
        float punchMultiplyer = 1 + save.currentCharacter.infiniteUpgrades[SavableSettings.UpgradeType.PunchDamage] *
                                punchMult;
        ModifyCombo(
            ref pCombat.PunchCombo,
            multDamage: punchMultiplyer,
            addDamage: 0);
        pCombat.SuperPunch.damage *= punchMultiplyer;

        // Kick Damage Mult
        ModifyCombo(ref pCombat.KickCombo, allModifier, 0); // allModifier
        float kickMultiplyer =
            1 + save.currentCharacter.infiniteUpgrades[SavableSettings.UpgradeType.KickDamage] * kickMult;
        ModifyCombo(
            ref pCombat.KickCombo,
            multDamage: kickMultiplyer,
            addDamage: 0);
        pCombat.SuperKick.damage *= kickMultiplyer;
        pCombat.JumpKickData.damage *= kickMultiplyer;

        // Critical Add
        pCombat.critChance *= allModifier; // allModifier
        pCombat.critChance +=
            save.currentCharacter.infiniteUpgrades[SavableSettings.UpgradeType.CriticalChance] * critAdd;

        // Super Punch
        if (save.currentCharacter.boolUpgrades[SavableSettings.UpgradeType.PunchCombo])
        {
            Array.Resize(ref pCombat.PunchCombo, pCombat.PunchCombo.Length + 1);
            pCombat.PunchCombo[pCombat.PunchCombo.Length - 1] = pCombat.SuperPunch;
        }

        // Super Kick
        if (save.currentCharacter.boolUpgrades[SavableSettings.UpgradeType.KickCombo])
        {
            Array.Resize(ref pCombat.KickCombo, pCombat.KickCombo.Length + 1);
            pCombat.KickCombo[pCombat.KickCombo.Length - 1] = pCombat.SuperKick;
        }

        // Super Punch Combo
        pCombat.superPunchAllowed = save.currentCharacter.boolUpgrades[SavableSettings.UpgradeType.SuperPunch];

        // Super Kick Combo
        pCombat.superKickAllowed = save.currentCharacter.boolUpgrades[SavableSettings.UpgradeType.SuperKick];

        // Jump Kick Combo
        pCombat.JumpKickAllowed = save.currentCharacter.boolUpgrades[SavableSettings.UpgradeType.JumpKick];
    }

    private static void ModifyCombo(ref DamageObject[] combo, float multDamage, float addDamage)
    {
        foreach (var damageObject in combo)
        {
            damageObject.damage *= multDamage;
            damageObject.damage += addDamage;
        }
    }
}