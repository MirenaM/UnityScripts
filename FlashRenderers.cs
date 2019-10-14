using UnityEngine;

public class FlashRenderers : MonoBehaviour
{
    [SerializeField]
    private Renderer[] _Renderers = default;

    private Material[][] _FlashMaterials = default;
    private Material[][] _OriginalMaterials = default;

    [SerializeField]
    private Material _FlashMaterial = default;

    [SerializeField]
    private float _FlashDuration = 4f;
    [SerializeField]
    private float _FlashSpeed = 0.2f;
    private float _CurrentFlashSpeed = 0f;
    private float _CurrentTime = 0f;

    private bool _IsFlashing = false;
    private bool _InFlashMat = false;
    [SerializeField]
    private bool _Flickers = true;

    private void Awake()
    {
        _OriginalMaterials = SaveRenderersMaterialArray(_Renderers);
        _FlashMaterials = SaveRenderersMaterialArray(_Renderers);

        for (int i = 0; i < _FlashMaterials.Length; i++)
        {
            var matArr = CreateMaterialArrayOf(_FlashMaterial, _FlashMaterials[i].Length);
            _FlashMaterials[i] = matArr;
        }

    }

    private void OnDisable()
    {
        ResetMaterials();
    }

    private void Update()
    {
        if (_IsFlashing == false) return;
        _CurrentTime += Time.deltaTime;

        if (_CurrentTime >= _FlashDuration)
        {
            ResetMaterials();
            _IsFlashing = false;
        }
        else
        {
            if (_CurrentFlashSpeed >= _FlashSpeed)
            {
                if (_Flickers == true)
                {
                    if (_InFlashMat == true)
                    {
                        ResetMaterials();
                    }
                    else
                    {
                        FlashMaterials();
                    }
                }
                _CurrentFlashSpeed = 0f;
            }
            else
            {
                _CurrentFlashSpeed += Time.deltaTime;
            }
        }
    }

    public void Flash()
    {
        FlashMaterials();
        _CurrentTime = 0f;
        _CurrentFlashSpeed = 0f;
        _IsFlashing = true;
    }

    private void FlashMaterials()
    {
        for (int i = 0; i < _Renderers.Length; i++)
        {
            _Renderers[i].materials = _FlashMaterials[i];
        }
        _InFlashMat = true;
    }

    private void ResetMaterials()
    {
        for (int i = 0; i < _Renderers.Length; i++)
        {
            _Renderers[i].materials = _OriginalMaterials[i];
        }
        _InFlashMat = false;
    }

    private Material[] CreateMaterialArrayOf(Material material, int count)
    {
        Material[] ret = new Material[count];
        for (int i = 0; i < count; i++)
        {
            ret[i] = material;
        }
        return ret;
    }

    private Material[][] SaveRenderersMaterialArray(Renderer[] _Rend)
    {
        var ret = new Material[_Rend.Length][];
        for (int i = 0; i < _Rend.Length; i++)
        {
            ret[i] = _Rend[i].materials;
        }
        return ret;
    }

}
