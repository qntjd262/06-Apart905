using UnityEngine;

public class MoveChecker : MonoBehaviour
{
    private MoveLevel moveLevel;

    public bool isDown, isMid, isUp;

    private void Awake()
    {
        moveLevel = FindFirstObjectByType<MoveLevel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (!moveLevel.check)
            {
                if (isDown)
                    moveLevel.state = MoveLevel.PlayerState.MoveDown;
                else if (isMid)
                    moveLevel.check = true;
                else if (isUp)
                        moveLevel.state = MoveLevel.PlayerState.MoveUp;
            }
            else
            {
                if (isDown)
                {
                    if (moveLevel.state == MoveLevel.PlayerState.MoveDown)
                    {
                        moveLevel.check = false;
                        return;
                    }
                    else if (moveLevel.state == MoveLevel.PlayerState.MoveUp)
                    {
                        moveLevel.check = false;
                        moveLevel.MoveUp();
                    }
                }
                else if (isUp)
                {
                    if (moveLevel.state == MoveLevel.PlayerState.MoveUp)
                    {
                        moveLevel.check = false;
                        return;
                    }
                    else if (moveLevel.state == MoveLevel.PlayerState.MoveDown)
                    {
                        moveLevel.check = false;
                        moveLevel.MoveDown();
                    }
                }
            }
        }
    }
}
