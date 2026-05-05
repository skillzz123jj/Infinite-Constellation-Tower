using UnityEngine;

public class ControlAttackTiming : MonoBehaviour
{
    [SerializeField] PlayerCombat playerCombat;

   public void ControlAttack()
   {
       playerCombat.EndAttack();
  
   }
}
