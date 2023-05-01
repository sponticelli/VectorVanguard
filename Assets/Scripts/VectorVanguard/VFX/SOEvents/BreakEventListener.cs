using LiteNinja.SOA.Events;
using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.VFX.SOEvents
{
  [AddComponentMenu("VectorVanguard/Event Listeners/Break Event Listener")]
  public class BreakEventListener : ASOEventListener<BreakInfo>
  {
    [SerializeField] private EventResponse[] _eventResponses;
    protected override EventResponse<BreakInfo>[] EventResponses => _eventResponses;

    [System.Serializable]
    public class EventResponse : EventResponse<BreakInfo>
    {
      [SerializeField] private BreakEvent _soEvent;
      public override ASOEvent<BreakInfo> SOEvent => _soEvent;

      [SerializeField] private UnityEventBreak _response;
      public override UnityEvent<BreakInfo> Response => _response;
    }
  }
}