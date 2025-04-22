using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class Hearts : MonoBehaviour
{
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Transform heartParent;

    private int startingHealth;
    private List<Image> heartImages;

    void Start()
    {
        if (GameData.Instance != null)
        {
            startingHealth = GameData.Instance.HasItem(CollectableType.GrandmasCookies) ? 6 : 3;
        }
        else startingHealth = 3;

        GameManager.Instance.playerHearts = startingHealth;

        heartImages = new();
        for (int i = 0; i < startingHealth; i++)
        {
            Image heart = Instantiate(heartPrefab, heartParent);
            heartImages.Add(heart);
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.onPlayerDamage += Damage;
    }

    private void OnDisable()
    {
        GameManager.Instance.onPlayerDamage -= Damage;
    }

    private void Damage()
    {
        heartImages[GameManager.Instance.playerHearts].sprite = emptyHeartSprite;

        if (GameManager.Instance.playerHearts <= 0)
        {
            if (GameData.Instance != null)
            SceneManager.LoadScene(4);
        }
    }
}