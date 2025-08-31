using UnityEngine;

public static class TimeConverter
{
    // Recebe minutos e segundos e retorna o total em segundos
    public static int ToSeconds(int minutes, int seconds)
    {
        return minutes * 60 + seconds;
    }

    // Recebe string no formato "MM:SS" e retorna total em segundos
    public static int FromString(string time)
    {
        string[] split = time.Split(':');
        if (split.Length != 2)
        {
            Debug.LogError("Formato inválido! Use MM:SS");
            return 0;
        }

        int minutes = int.Parse(split[0]);
        int seconds = int.Parse(split[1]);
        return ToSeconds(minutes, seconds);
    }
}
