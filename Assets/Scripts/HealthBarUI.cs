
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private CharacterHealth _target;
    [SerializeField] private UnityEngine.UI.Image _fillImage;

    private void Update()
    {
        if (_target == null || _fillImage == null)
            return;

        if (_target.MaxHealth <= 0f)
            return;

        float healthPercentage =
            _target.CurrentHealth / _target.MaxHealth;

        _fillImage.fillAmount =
            Mathf.Clamp01(healthPercentage);
    }
}
