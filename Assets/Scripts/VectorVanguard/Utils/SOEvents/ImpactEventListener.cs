using LiteNinja.SOA.Events;
using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Utils.SOEvents
{
  [AddComponentMenu("VectorVanguard/Event Listeners/Impact Event Listener")]
  public class ImpactEventListener : ASOEventListener<ImpactInfo>
  {
    [SerializeField] private EventResponse[] _eventResponses;
    protected override EventResponse<ImpactInfo>[] EventResponses => _eventResponses;

    [System.Serializable]
    public class EventResponse : EventResponse<ImpactInfo>
    {
      [SerializeField] private ImpactEvent _soEvent;
      public override ASOEvent<ImpactInfo> SOEvent => _soEvent;

      [SerializeField] private UnityEventImpact _response;
      public override UnityEvent<ImpactInfo> Response => _response;
    }
  }
}