using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animation Package/Camera Animation Package")]
public class CameraAnimationPackage : ScriptableObject {
    public List<CameraAnimation> animationList;
}
