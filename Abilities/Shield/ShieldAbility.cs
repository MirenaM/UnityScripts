using UnityEngine;

public class ShieldAbility : MonoBehaviour
{
    [SerializeField]
    private Shield _ShieldPrefab = default;
    private Shield _CurrentShield;

    public bool HasShield = false;

    [SerializeField]
    private KeyCode _ShieldKey = default;

    private void Awake()
    {
        var GO = Instantiate(_ShieldPrefab.gameObject, transform.position, transform.rotation);
        _CurrentShield = GO.GetComponent<Shield>();
        _CurrentShield._ShieldAbility = this;

        _CurrentShield.gameObject.SetActive(false);
    }

    private void Update()
    {
        ShieldInputs();
    }

    private void ShieldInputs()
    {
        if (_CurrentShield is null) return;
        if (HasShield == false && Input.GetKeyDown(_ShieldKey) && _CurrentShield._IsDissolving == false)
        {
            Activate();
        }

        if (Input.GetKeyUp(_ShieldKey) && HasShield == true)
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        _CurrentShield.transform.position = transform.position;
        _CurrentShield.transform.rotation = transform.rotation;
        _CurrentShield.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _CurrentShield.GetComponent<Shield>().StartDissolveOut();
    }

}
