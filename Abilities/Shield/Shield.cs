using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public bool _IsDissolving { get; private set; }

    [SerializeField]
    private GameObject _Model = default;
    private Material _Material;

    private Coroutine _DissolveIn;
    private Coroutine _DissolveOut;

    [SerializeField]
    private float _DissolveSpeed = 5f;

    [HideInInspector]
    public ShieldAbility _ShieldAbility;

    private void Awake()
    {
        _Material = _Model.gameObject.GetComponent<Renderer>().material;
        _Material.SetFloat("_DissolveAmount", 1f);
    }

    private void OnEnable()
    {
        _ShieldAbility.HasShield = true;
        StartDissolveIn();
    }

    private void Update()
    {
        if (_ShieldAbility is null) return;
        transform.position = _ShieldAbility.transform.position;
    }

    public void StartDissolveIn()
    {
        _IsDissolving = true;

        if (_DissolveIn != null)
        {
            StopCoroutine(_DissolveIn);
        }

        _Material.SetFloat("_DissolveAmount", 1f);

        _DissolveIn = StartCoroutine(ShieldDissolveIn());
    }

    private IEnumerator ShieldDissolveIn()
    {
        var currentTime = 0f;

        while (currentTime < 1f)
        {
            var lerpAmount = Mathf.Lerp(1, 0, currentTime);
            _Material.SetFloat("_DissolveAmount", lerpAmount);
            currentTime += Time.deltaTime * _DissolveSpeed;
            yield return null;
        }

        _Material.SetFloat("_DissolveAmount", 0f);


        _IsDissolving = false;
        yield return null;
    }

    public void StartDissolveOut()
    {
        _IsDissolving = true;

        if (_DissolveOut != null)
        {
            StopCoroutine(_DissolveOut);
        }
        if (_DissolveIn != null)
        {
            StopCoroutine(_DissolveIn);
        }

        _Material.SetFloat("_DissolveAmount", 0f);

        _DissolveOut = StartCoroutine(ShieldDissolveOut());
    }

    private IEnumerator ShieldDissolveOut()
    {
        var currentTime = 0f;

        while (currentTime < 1f)
        {
            var lerpAmount = Mathf.Lerp(0, 1, currentTime);
            _Material.SetFloat("_DissolveAmount", lerpAmount);
            currentTime += Time.deltaTime * _DissolveSpeed;
            yield return null;
        }

        _Material.SetFloat("_DissolveAmount", 1f);
        _IsDissolving = false;
        gameObject.SetActive(false);

        yield return null;
    }

    private void OnDisable()
    {
        _ShieldAbility.HasShield = false;
    }
}
