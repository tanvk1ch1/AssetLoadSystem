using System;
using UnityEngine;

public class InputObserver : MonoBehaviour
{
    public static InputObserver Instance { get; } = new InputObserver();
    public event Action OnAttackLeft;
    public event Action OnAttackRight;
    public int HitDown
    {
        get
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    return 1;
                }
                return 0;
            }
            return 0;
        }
    }
    
    public int GetHit()
    {
        return HitDown;
    }
    
    public void ClearEvents()
    {
        OnAttackLeft = null;
        OnAttackRight = null;
    }
}
