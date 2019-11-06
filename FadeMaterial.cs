using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterial : MonoBehaviour
{
    [SerializeField]
    private GameObject _Model = default;

    [SerializeField]
    private float _FadeTime = 4f;

    [SerializeField]
    private bool _FadeOnEnable = true;

    private List<Material> _Materials = new List<Material>();

    private void Awake()
    {
        var transforms = _Model.gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {

            var rend = transforms[i].GetComponent<Renderer>();

            if (rend != null)
            {
                _Materials.Add(rend.material);
            }

        }

    }

    private void OnEnable()
    {
        if (_FadeOnEnable)
        {
            StartFadeOut();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _Materials.Count; i++)
        {
            _Materials[i].SetFloat("_Opacity", 1f);
            _Materials[i].SetFloat("_OutlineAlpha", 1f);
        }
    }

    public void StartFadeOut()
    {
        for (int i = 0; i < _Materials.Count; i++)
        {
            _Materials[i].SetFloat("_Opacity", 1f);
            _Materials[i].SetFloat("_OutlineAlpha", 1f);
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float currentTime = 0f;

        while (currentTime < _FadeTime)
        {
            float lerpAmount = Mathf.Lerp(1, 0, currentTime);
            float lerpAmountOutline = Mathf.Lerp(1, 0, currentTime * 2f);

            for (int i = 0; i < _Materials.Count; i++)
            {
                _Materials[i].SetFloat("_Opacity", lerpAmount);
            }

            currentTime += Time.deltaTime / _FadeTime;
            yield return null;
        }
    }
}
