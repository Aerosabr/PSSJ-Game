using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    public Image image;
    public BoxCollider2D box;
    public Region region;
    public bool isDropped;
    public bool isMarked = false;
    public int xSize;
    public int ySize;
    public int spawnDuration;
    public abstract bool CheckSlot(string Pos);
    public abstract bool PickupItem();
    public abstract void ItemDropped(GameObject Character);
    public abstract void Highlight(bool toggle);
}
