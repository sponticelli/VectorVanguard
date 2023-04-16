using TMPro;
using UnityEngine;
using VectorVanguard.Actors;
using VectorVanguard.Actors.Abilities;

namespace VectorVanguard.Levels
{
  /// <summary>
  /// A pointer that checks if the player has a radar ability and if the target is in the radar range.
  /// </summary>
  public class OutOfScreenPointerRadar : OutOfScreenPointer
  {
    [SerializeField] private TextMeshPro _distanceText;
    
    private bool _hasRadarAbility;
    private ActorRadarAbility _radarAbility;
    private ActorAbilities _abilities;
    private bool _showDistance;
    
    private float _distance;
    
    public override void Initialization(Actor actor, Actor player, Camera camera)
    {
      base.Initialization(actor, player, camera);
      _abilities = player.ActorAbilities;
      _radarAbility = _abilities.GetAbility<ActorRadarAbility>();
      _hasRadarAbility = _radarAbility != null;
      
      _abilities.OnAbilityAdded.AddListener(OnAbilityAdded);
      _abilities.OnAbilityRemoved.AddListener(OnAbilityRemoved);
      
      _showDistance = _distanceText != null;
    }

    public void OnDisable()
    {
      _abilities?.OnAbilityAdded.RemoveListener(OnAbilityAdded);
      _abilities?.OnAbilityRemoved.RemoveListener(OnAbilityRemoved);
    }

    private void OnAbilityAdded(AActorAbility ability)
    {
      if (ability is not ActorRadarAbility radarAbility) return;
      _radarAbility = radarAbility;
      _hasRadarAbility = true;
    }
    
    private void OnAbilityRemoved(AActorAbility ability)
    {
      if (ability is not ActorRadarAbility) return;
      _radarAbility = null;
      _hasRadarAbility = false;
    }
    
    protected override bool CanDisplay()
    {
      if (!_hasRadarAbility) return false;
      if (_radarAbility.CanExecute == false) return false;
      _distance = Vector3.Distance(_player.transform.position, _target.transform.position);
      return (_radarAbility.Range >= _distance);
    }
    
    protected override void DisplayPointer()
    {
      base.DisplayPointer();
      if (_showDistance)
      {
        _distanceText.gameObject.SetActive(true);
        _distanceText.text = $"{_distance:F0}";
      }
    }
    
    protected override void HidePointer()
    {
      base.HidePointer();
      if (_showDistance)
      {
        _distanceText.gameObject.SetActive(false);
      }
    }
  }
}