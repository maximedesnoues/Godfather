using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedbackManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private Coroutine _screenshakeCoroutine;

    #region Singleton
    private static FeedbackManager instance = null;

    public static FeedbackManager Instance => instance;

    private void InitSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Awake()
    {
        InitSingleton();
        _audioSource = GetComponent<AudioSource>();
    }

    #region Feedback Methods
    public void PlaySFX(Clip audioClip)
    {
        _audioSource.PlayOneShot(audioClip.Audioclip, audioClip.Volume);
    }
    public void PlayRandomSFX(List<Clip> audioClips)
    {
        int randIndex = UnityEngine.Random.Range(0, audioClips.Count);
        _audioSource.PlayOneShot(audioClips[randIndex].Audioclip, audioClips[randIndex].Volume);
    }

    public void ScreenShake(float intensity, float duration)
    {
        if (_screenshakeCoroutine != null)
            return;
        Camera.main.transform.DOShakePosition(duration, intensity);
        _screenshakeCoroutine = StartCoroutine(WaitForScreenShake(duration));
    }
    private IEnumerator WaitForScreenShake(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Camera.main.transform.position = new Vector3(0,0,-10); //reset cam
        _screenshakeCoroutine = null;
    }

    public void VibrateAllControllers(float seconds) => StartCoroutine(WaitForVibrate(seconds));

    private IEnumerator WaitForVibrate(float seconds)
    {
        foreach( var gamepad in Gamepad.all)
        {
            gamepad.SetMotorSpeeds(.2f, .2f);
        }
        yield return new WaitForSeconds(seconds);
        foreach (var gamepad in Gamepad.all)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
    #endregion

    [Serializable]
    public class Clip
    {
        public AudioClip Audioclip
        {
            get { return audioclip; }
            set { audioclip = value; }
        }
        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }
        [SerializeField]
        AudioClip audioclip;
        [SerializeField]
        float volume = 1;
    }
}
