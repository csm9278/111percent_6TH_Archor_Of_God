using UnityEngine;

public interface IAnimEventReceiver { void OnAnimEvent(string evt); }

public class AnimEventRelay : MonoBehaviour
{
    public void AnimEvent(string evt)
    {
        foreach (var r in GetComponentsInParent<IAnimEventReceiver>())
            r.OnAnimEvent(evt);
        foreach (var r in GetComponentsInChildren<IAnimEventReceiver>())
            r.OnAnimEvent(evt);
    }
}
// �ִϸ��̼� Ŭ�� �̺�Ʈ �Լ� �̸�: AnimEventRelay.AnimEvent(string evt)
