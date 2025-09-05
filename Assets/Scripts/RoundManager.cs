using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [Header("Players & Spawns")]
    [SerializeField] private PlayerBehaviour player1;
    [SerializeField] private PlayerBehaviour player2;
    [SerializeField] private Transform spawnP1;
    [SerializeField] private Transform spawnP2;

    [Header("Tents (lives are on tents)")]
    [SerializeField] private TentLives tentP1;
    [SerializeField] private TentLives tentP2;
    [SerializeField] private int startingLives = 3;

    [Header("Round Rules")]
    [SerializeField] private float roundDurationSeconds = 60f;
    [SerializeField] private float postRoundDelay = 2f;
    [SerializeField] private float simultaneousWindow = 0.12f;

    [Header("Water")]
    [SerializeField] private RisingWater risingWater;
    [SerializeField] private float waterStartYOffset = -6f;
    [SerializeField] private float waterRiseSpeed = 2.5f;

    [Header("HUD (Timer only)")]
    [SerializeField] private Text timerText;
    // [SerializeField] private Text centerMessage;

    private float roundTimeLeft;
    private bool roundRunning;
    private bool resolvingDrown;
    private bool matchOver;
    private int roundIndex = 0;

    private readonly HashSet<PlayerBehaviour> drownedThisRound = new();

    private Dictionary<PlayerBehaviour, TentLives> tentByPlayer;

    private void Start()
    {
        // Sécurité références
        if (player1 == null || player2 == null || spawnP1 == null || spawnP2 == null ||
            tentP1 == null || tentP2 == null || risingWater == null)
        {
            Debug.LogError("[RoundManager] Références manquantes dans l'Inspector.");
            enabled = false;
            return;
        }

        // Réinitialise les vies (internes aux tentes)
        tentP1.ResetLives(startingLives);
        tentP2.ResetLives(startingLives);

        tentByPlayer = new Dictionary<PlayerBehaviour, TentLives>
        {
            { player1, tentP1 },
            { player2, tentP2 }
        };

        // Initialise et place l'eau
        risingWater.Initialize(this, GetWaterStartY());

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return null;

        while (!matchOver)
        {
            roundIndex++;
            yield return StartCoroutine(StartRound());
            yield return StartCoroutine(PlayRound());
            yield return StartCoroutine(EndRound());

            matchOver = tentP1.IsDepleted || tentP2.IsDepleted;
        }

        LockInputs(true);
        var winnerMsg = tentP1.IsDepleted == tentP2.IsDepleted
            ? "Match nul !"
            : (tentP2.IsDepleted ? "Joueur 1 gagne !" : "Joueur 2 gagne !");
        Debug.Log(winnerMsg);
        // ShowCenter($"Fin de partie !\n{winnerMsg}");
    }

    private IEnumerator StartRound()
    {
        drownedThisRound.Clear();
        resolvingDrown = false;

        // Respawn & reset vitesses
        ResetPlayer(player1, spawnP1);
        ResetPlayer(player2, spawnP2);

        // Eau : replacée sous la scène et à l’arrêt
        risingWater.ResetToStart(GetWaterStartY());

        yield return new WaitForSeconds(1.0f);

        roundTimeLeft = Mathf.Max(1f, roundDurationSeconds);
        roundRunning = true;
        LockInputs(false);
    }

    private IEnumerator PlayRound()
    {
        // Phase timer sans eau
        while (roundRunning && roundTimeLeft > 0f && drownedThisRound.Count == 0)
        {
            roundTimeLeft -= Time.deltaTime;
            UpdateTimer(roundTimeLeft);
            yield return null;
        }

        // Lance la montée de l’eau si personne n’est tombé pendant le timer
        if (drownedThisRound.Count == 0)
            risingWater.StartRising();

        // Phase eau → attend que noyade(s) survienne(nt) puis qu’on résolve
        while (roundRunning && !resolvingDrown) yield return null;
        while (resolvingDrown) yield return null;
    }

    private IEnumerator EndRound()
    {
        risingWater.StopRising();
        yield return new WaitForSeconds(postRoundDelay);
    }

    private void UpdateTimer(float t)
    {
        t = Mathf.Max(0, t);
        timerText.text = "Timer : " + Mathf.CeilToInt(t).ToString();
    }

    //private void ShowCenter(string msg)
    //{
    //    if (centerMessage != null) centerMessage.text = msg;
    //}

    private void LockInputs(bool locked)
    {
        if (player1?.PlayerInputs?.Map != null) player1.PlayerInputs.Map.enabled = !locked;
        if (player2?.PlayerInputs?.Map != null) player2.PlayerInputs.Map.enabled = !locked;
    }

    private void ResetPlayer(PlayerBehaviour p, Transform spawn)
    {
        var rb = p.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = spawn.position;
        }
        else
        {
            p.transform.position = spawn.position;
        }
    }

    private float GetWaterStartY()
    {
        var minY = Mathf.Min(spawnP1.position.y, spawnP2.position.y);
        return minY + waterStartYOffset;
    }

    // Appelé par WaterKillZone quand un joueur touche la surface
    public void RegisterDrown(PlayerBehaviour player)
    {
        if (!roundRunning) return;

        drownedThisRound.Add(player);
        if (!resolvingDrown)
            StartCoroutine(ResolveAfterWindow());
    }

    private IEnumerator ResolveAfterWindow()
    {
        resolvingDrown = true;
        yield return new WaitForSeconds(simultaneousWindow);

        if (drownedThisRound.Count >= 2)
        {
            // Égalité : les deux tentes perdent 1 vie
            tentP1.LoseOneLife();
            tentP2.LoseOneLife();
            Debug.Log("DRAW");
            // ShowCenter("Égalité !");
        }
        else if (drownedThisRound.Count == 1)
        {
            // Un seul joueur tombé → seule sa tente perd 1 vie
            foreach (var loser in drownedThisRound)
            {
                ApplyLifeLossToLoserTent(loser);
                break;
            }
        }

        roundRunning = false;
        resolvingDrown = false;
    }

    private void ApplyLifeLossToLoserTent(PlayerBehaviour loser)
    {
        if (loser == null) return;

        if (tentByPlayer != null && tentByPlayer.TryGetValue(loser, out var tent))
        {
            tent.LoseOneLife();
        }
    }
}