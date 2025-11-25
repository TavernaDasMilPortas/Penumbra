using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioMorse : MonoBehaviour
{
    [Header("Night necessária para ativar o rádio")]
    public NightData requiredNight;

    [Header("Configuração do Morse")]
    public string message = "SOS";
    public float dotDuration = 0.1f;
    public float dashDuration = 0.3f;
    public float symbolSpacing = 0.1f;
    public float letterSpacing = 0.3f;
    public float wordSpacing = 0.7f;

    [Header("Pausa após a mensagem inteira")]
    public float messagePause = 2f;

    [Header("Áudio")]
    public AudioSource source;
    public AudioClip dotClip;
    public AudioClip dashClip;
    public AudioClip staticClip;

    [Header("Efeito de rádio instável")]
    [Range(0f, 1f)] public float staticChance = 0.2f;
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    [Header("Item Holder para ativar Morse")]
    public ItemHolder holder;

    private Dictionary<char, string> morseMap;
    private bool isPlayingMorse = false;


    private void Awake()
    {
        // Se existe uma Night configurada, valida
        if (requiredNight != null)
        {
            if (NightManager.Instance == null)
            {
                Debug.LogWarning("[RadioMorse] NightManager não existe ainda. O objeto permanecerá ativo.");
            }
            else if (NightManager.Instance.CurrentNight != requiredNight)
            {
                Debug.Log($"[RadioMorse] Desativado. Só deve funcionar na noite: {requiredNight.nightName}");
                gameObject.SetActive(false);
                return;
            }
        }
    }


    private void Start()
    {
        if (!gameObject.activeSelf)
            return; // já foi desativado no Awake

        CreateMorseDictionary();
        StartCoroutine(StaticLoop());
    }

    private void CreateMorseDictionary()
    {
        morseMap = new Dictionary<char, string>()
        {
            {'A', ".-"}, {'B', "-..."}, {'C', "-.-."},
            {'D', "-.."}, {'E', "."}, {'F', "..-."},
            {'G', "--."}, {'H', "...."}, {'I', ".."},
            {'J', ".---"}, {'K', "-.-"}, {'L', ".-.."},
            {'M', "--"}, {'N', "-."}, {'O', "---"},
            {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."},
            {'S', "..."}, {'T', "-"}, {'U', "..-"},
            {'V', "...-"}, {'W', ".--"}, {'X', "-..-"},
            {'Y', "-.--"}, {'Z', "--.."},
            {'0', "-----"}, {'1', ".----"}, {'2', "..---"},
            {'3', "...--"}, {'4', "....-"}, {'5', "....."},
            {'6', "-...."}, {'7', "--..."}, {'8', "---.."},
            {'9', "----."}
        };
    }

    private IEnumerator StaticLoop()
    {
        while (true)
        {
            if (holder != null && holder.currentItem != null && !isPlayingMorse)
            {
                StartCoroutine(PlayMorseLoop());
                yield break;
            }

            PlayStatic();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator PlayMorseLoop()
    {
        isPlayingMorse = true;

        while (holder != null && holder.currentItem != null)
        {
            yield return PlayMessage(message);
            yield return new WaitForSeconds(messagePause);
        }

        isPlayingMorse = false;
        StartCoroutine(StaticLoop());
    }

    private IEnumerator PlayMessage(string msg)
    {
        msg = msg.ToUpper();

        foreach (char c in msg)
        {
            if (holder.currentItem == null)
                yield break;

            if (c == ' ')
            {
                yield return new WaitForSeconds(wordSpacing);
                continue;
            }

            if (!morseMap.ContainsKey(c))
                continue;

            string code = morseMap[c];

            foreach (char symbol in code)
            {
                PlaySymbol(symbol);

                float dur = (symbol == '.') ? dotDuration : dashDuration;
                yield return new WaitForSeconds(dur);
                yield return new WaitForSeconds(symbolSpacing);

                if (holder.currentItem == null)
                    yield break;
            }

            yield return new WaitForSeconds(letterSpacing);
        }
    }

    private void PlaySymbol(char symbol)
    {
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Stop();

        source.clip = (symbol == '.') ? dotClip : dashClip;
        source.Play();
    }

    private void PlayStatic()
    {
        if (staticClip == null) return;

        source.Stop();
        source.clip = staticClip;
        source.pitch = 1f;
        source.Play();
    }
}
