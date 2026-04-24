using UnityEngine;

public class ControlAttackTiming : MonoBehaviour
{
    [SerializeField] PlayerCombat playerCombat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void ControlAttack()
   {
       playerCombat.EndAttack();
  
   }
}
