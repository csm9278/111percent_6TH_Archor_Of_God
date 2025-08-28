using UnityEngine;

public class ActionGate : MonoBehaviour
{
    int lockCount; public bool Locked => lockCount > 0;
    public void Lock() => lockCount++;
    public void Release() => lockCount = Mathf.Max(0, lockCount - 1);
}
