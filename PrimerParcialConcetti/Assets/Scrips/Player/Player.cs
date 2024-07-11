using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxSpeed;
    [SerializeField] private int health = 1;

    public int GetMaxSpeed()
    {
        return maxSpeed;
    }

}
