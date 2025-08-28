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
// 애니메이션 클립 이벤트 함수 이름: AnimEventRelay.AnimEvent(string evt)
