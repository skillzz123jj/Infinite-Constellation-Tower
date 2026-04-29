using UnityEngine;

public class ManageCheckpoints : MonoBehaviour
{
    [SerializeField] private Checkpoint[] checkpoints;
    
    void Start()
    {
        ActivateCheckpoint(Gamedata.Instance.checkPointNum + 1);
    }
    public void ActivateCheckpoint(int index)
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (i < index)
            {
                checkpoints[i].gameObject.GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                checkpoints[i].gameObject.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}
