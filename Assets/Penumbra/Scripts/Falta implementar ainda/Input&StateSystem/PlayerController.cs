using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public GameObject Player;


    private void Awake()
    {
        Instance = this;
        
    }

    private void Update()
    {
        
    }

    public void Move(float h, float v)
    {

    }

    public void Stop()
    {
       
    }

    public void Interagir()
    {
        Stop();
        InteractionHandler.Instance.nearestInteractable.Interact();
    }
}
