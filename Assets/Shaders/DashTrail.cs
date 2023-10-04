using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashTrail : MonoBehaviour
{
    [SerializeField] private int maxTrailExtension;
    [SerializeField] private Material dashShader;
    [SerializeField] private float intervalInSeconds;
    [SerializeField] private float fadeDuration;
    [SerializeField] private int particleSpawns;
    [SerializeField] private int slowdownOffset;
    [SerializeField] private string animateStateName = "Dash";
    
    private Transform[] _trailExtensions;
    private int _localTrailIndex = 0;
    private float _fadeOffset = 0;
    

    private IEnumerator _trailAction = null;
    
    // Start is called before the first frame update
    void Start() {
        _trailExtensions = new Transform[maxTrailExtension];
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animateStateName) && _localTrailIndex < maxTrailExtension) {
            CreateAfterimage();
        } else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Refresh")) {
            _localTrailIndex = 0;
            _fadeOffset = 0;
        }
    }

    private void CreateAfterimage() {
        if (_trailAction == null) {
            if (slowdownOffset <= _localTrailIndex || _localTrailIndex % 2 == 0) {
                _trailAction = AfterimageAction();
                StartCoroutine(_trailAction);
                _localTrailIndex++;
            }
        }
    }

    private IEnumerator AfterimageAction() {
        _fadeOffset += 0.01f;
        GameObject afterimage = Instantiate(gameObject, transform.GetComponent<Animator>().pivotPosition, transform.rotation);
        Destroy(afterimage.GetComponent<DashTrail>());
        Destroy(afterimage.GetComponent<Animator>());
        if (_localTrailIndex > particleSpawns) {
            Destroy(afterimage.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
        }
        
        afterimage.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = dashShader;
        afterimage.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>().material = dashShader;
        afterimage.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.DOFade(0, fadeDuration + _fadeOffset);
        afterimage.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>().material.DOFade(0, fadeDuration + _fadeOffset);
        Destroy(afterimage, fadeDuration + _fadeOffset);
        yield return new WaitForSeconds(intervalInSeconds);
        _trailAction = null;
        yield return null;
    }
}
