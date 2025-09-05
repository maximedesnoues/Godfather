using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [Header("Source des vies")]
    [SerializeField] private TentLives tent;

    [Header("Cœurs à afficher")]
    [SerializeField] private List<Image> hearts = new List<Image>();

    private int lastLives = -1;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (tent == null) return;
        if (tent.Lives != lastLives)
            Refresh();
    }

    private void Refresh()
    {
        if (tent == null || hearts == null || hearts.Count == 0) return;

        lastLives = Mathf.Max(0, tent.Lives);

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].enabled = (i < lastLives);
        }
    }

    public void Bind(TentLives source, IList<Image> heartImages)
    {
        tent = source;
        hearts = new List<Image>(heartImages);
        Refresh();
    }
}