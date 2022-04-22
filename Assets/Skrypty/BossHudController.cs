using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossHudController : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Image hpBar;
    private Boss1Controller bossController;
    private Boss2Controller boss2Controller;
    private string sceneName;
    private float hpBarWidthRate;
    private RectTransform _hpRectTransform;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "Level 3")
        {
            boss2Controller = boss.GetComponent<Boss2Controller>();
            _hpRectTransform = hpBar.gameObject.GetComponent<RectTransform>();
        }
        else if (sceneName == "Level 5")
        {
            bossController = boss.GetComponent<Boss1Controller>();
            _hpRectTransform = hpBar.gameObject.GetComponent<RectTransform>();
        }
        if (_hpRectTransform != null)
        {
            if (sceneName == "Level 3")
            {
                hpBarWidthRate = _hpRectTransform.rect.width / Boss2Controller.MaxHealth;
            }
            else if (sceneName == "Level 5")
            {
                hpBarWidthRate = _hpRectTransform.rect.width / Boss1Controller.MaxHealth;
            }
        }
        if (bossController != null)
        {
            bossController.OnDamageTaken += ChangeHpBar;
        }
        if (boss2Controller != null)
        {
            boss2Controller.OnDamageTaken += ChangeHpBar;
        }
    }

    private void ChangeHpBar(float value)
    {
        if (_hpRectTransform != null)
        {
            var rect = _hpRectTransform.rect;
            var newWidth = rect.width - (value * hpBarWidthRate) >= 0 ? rect.width - (value * hpBarWidthRate) : 0;
            _hpRectTransform.sizeDelta = new Vector2(newWidth, rect.height);
        }
    }
}
