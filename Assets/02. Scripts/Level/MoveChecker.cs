using UnityEngine;

public class MoveChecker : MonoBehaviour
{
    [SerializeField] private bool _isMoveUp;
    [SerializeField] private MoveLevel moveLevel;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {/*
            if (_isMoveUp)
                other.GetComponent<Player>().state = PlayerState.   ;
            else
                other.GetComponent<Player>().MoveDown();*/
        }
    }
}
