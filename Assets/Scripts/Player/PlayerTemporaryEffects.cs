using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStats))]
public class PlayerTemporaryEffects : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    public void ApplySpeedBoost(float percentIncrease, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(percentIncrease, duration));
    }

    private IEnumerator SpeedBoostRoutine(float percentIncrease, float duration)
    {
        playerStats.AddMoveSpeed(percentIncrease);
        yield return new WaitForSeconds(duration);
        playerStats.AddMoveSpeed(-percentIncrease); // remove the bonus
    }

    public void ApplyDamageBoost(float percentIncrease, float duration)
    {
        StartCoroutine(DamageBoostRoutine(percentIncrease, duration));
    }

    private IEnumerator DamageBoostRoutine(float percentIncrease, float duration)
    {
        playerStats.AddDamageBonus(percentIncrease);
        yield return new WaitForSeconds(duration);
        playerStats.AddDamageBonus(-percentIncrease);
    }

    // Add other temporary effects the same way
    //TODO add shield effect. 
}
