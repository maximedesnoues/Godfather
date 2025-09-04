using UnityEngine;

public class TentLives : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0;
    [SerializeField] private int lives = 3;

    public int PlayerIndex => playerIndex;
    public int Lives => lives;
    public bool IsDepleted => lives <= 0;

    public void ResetLives(int value)
    {
        lives = Mathf.Max(0, value);
    }

    public void LoseOneLife()
    {
        if (lives > 0) lives--;
    }
}