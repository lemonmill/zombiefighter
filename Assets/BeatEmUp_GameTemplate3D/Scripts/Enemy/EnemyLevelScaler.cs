using UnityEngine;

public class EnemyLevelScaler : MonoBehaviour
{
    public bool scaleAllowed = true;
    public EnemyTypesManager enemyTypesManager;

    public EnemyLevelScaler()
    {
        savedLevel = 1;
    }

    public int savedLevel { get; private set; }
    
    public void ScaleLevel(int levelModifier)
    {
        savedLevel *= levelModifier;
        levelModifier--;

        if (scaleAllowed)
        {
            //Applying Scale
            transform.localScale *= enemyTypesManager.GetSizeValue(levelModifier);

            // Modifiing with level
            var enemyAI = GetComponent<EnemyAI>();
            // Attack
            foreach (var attack in enemyAI.AttackList)
            {
                attack.damage += (int)(attack.damage * enemyTypesManager.attackModifier * levelModifier);
            }
            enemyAI.attackSpeedModifier += enemyAI.attackSpeedModifier * enemyTypesManager.attackSpeedModifier * levelModifier;
            // Walk Speed
            enemyAI.walkSpeed += enemyAI.walkSpeed * enemyTypesManager.walkSpeedModifier * levelModifier;
            enemyAI.walkBackwardSpeed += enemyAI.walkBackwardSpeed * enemyTypesManager.walkSpeedModifier * levelModifier;
            enemyAI.walkSpeedModifier += enemyAI.walkSpeedModifier * enemyTypesManager.walkSpeedModifier * levelModifier;
            // Health
            var enemyHP = GetComponent<HealthSystem>();
            enemyHP.MaxHp += (int)(enemyHP.MaxHp * enemyTypesManager.hpModifier * levelModifier);
            enemyHP.CurrentHp = enemyHP.MaxHp; //restoring HP to max
        }

        scaleAllowed = false;
    }
}