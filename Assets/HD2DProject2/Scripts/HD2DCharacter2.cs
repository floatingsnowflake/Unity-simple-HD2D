using HD2DProject.Scripts.Characters;
using UnityEngine;

namespace HD2DProject2.Scripts
{
  public class HD2DCharacter2 : HD2DCharacter
  {
    public SwitchPerspective switchPerspective;
    
    protected override void ApplyMoveDelta(Vector3 delta)
    {
      var moveDelta = Quaternion.AngleAxis(switchPerspective.CurrentAngle, Vector3.up) * delta;
      transform.position += moveDelta;
    }
  }
}