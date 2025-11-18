using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioMorse : MonoBehaviour
{
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
    [Range(0f, 1f)]
    public float staticChance = 0.2f;
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    [Header("Item Holder para ativar Morse")]
    public ItemHolder holder; // <<< atribua o holder que fica como filho do rádio

    private Dictionary<char, string> morseMap;

    private bool isPlayingMorse = false;

    private void Start()
    {
        CreateMorseDictionary();

        // Começa com estática constante
        StartCoroutine(StaticLoop());
    }

    // -------------------------------------------------------------
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

    // -------------------------------------------------------------
    private IEnumerator StaticLoop()
    {
        while (true)
        {
            // Se item foi colocado → iniciar Morse
            if (holder != null && holder.currentItem != null && !isPlayingMorse)
            {
                StartCoroutine(PlayMorseLoop());
                yield break;
            }

            PlayStatic();
            yield return new WaitForSeconds(0.2f);
        }
    }

    // -------------------------------------------------------------
    private IEnumerator PlayMorseLoop()
    {
        isPlayingMorse = true;

        while (holder != null && holder.currentItem != null)
        {
            yield return StartCoroutine(PlayMessage(message));
            yield return new WaitForSeconds(messagePause);
        }

        isPlayingMorse = false;

        // Volta para estática contínua
        StartCoroutine(StaticLoop());
    }

    // -------------------------------------------------------------
    private IEnumerator PlayMessage(string msg)
    {
        msg = msg.ToUpper();

        foreach (char c in msg)
        {
            if (holder.currentItem == null)
                yield break; // item removido → parar no meio

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
                    yield break; // item removido → parar
            }

            yield return new WaitForSeconds(letterSpacing);
        }
    }

    // -------------------------------------------------------------
    private void PlaySymbol(char symbol)
    {
        source.pitch = Random.Range(minPitch, maxPitch);

        // chance de tocar estática no lugar do bip
        if (staticClip != null && Random.value < staticChance)
        {
            PlayStatic();
            return;
        }

        if (symbol == '.')
            source.PlayOneShot(dotClip);
        else
            source.PlayOneShot(dashClip);
    }

    private void PlayStatic()
    {
        if (staticClip != null)
            source.PlayOneShot(staticClip, 0.7f);
    }
}
