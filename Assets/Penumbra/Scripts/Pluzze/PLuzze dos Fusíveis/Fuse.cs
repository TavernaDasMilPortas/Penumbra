using UnityEngine;
public enum FuseColor { Red, Blue, Green, Yellow, White }

[CreateAssetMenu(menuName = "Items/Fuse")]
public class Fuse : Item
{
    public FuseColor color;
}