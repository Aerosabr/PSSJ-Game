using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WolfHungerUI : Item, IBeginDragHandler, IEndDragHandler, IDragHandler, IConsumable
{
    public static WolfHungerUI instance;
    [SerializeField] private List<GameObject> Slots = new List<GameObject>();
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject HighlightObject;
    [SerializeField] private int current;
 
    public Wolf wolf;
    public Slider hungerBar;
    public Transform Icon;

    private void Start()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        hungerBar.value = wolf.currentHunger / wolf.maxHunger;

        if ((wolf.currentHunger / wolf.maxHunger) < 0.4f)
        {
            float rotationSpeedFactor = Mathf.Clamp01((wolf.currentHunger / wolf.maxHunger));
            if (wolf.currentHunger != 0)
            {
                float rotationSpeed = 10 * ((wolf.maxHunger - wolf.currentHunger) / wolf.maxHunger);

                float rotationAngle = Mathf.Sin(Time.time * rotationSpeed) * (1 - wolf.currentHunger / wolf.maxHunger) * 10f;

                Icon.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
        }
    }

    private float GetDivisors()
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        return corners[2].x - corners[1].x;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 temp = Input.mousePosition - transform.position;
        float divisor = GetDivisors() / 6;
        if (temp.y >= 0) //Top half
        {
            if (temp.x <= -divisor)
                current = 1;
            else if (temp.x <= divisor)
                current = 2;
            else
                current = 3;
        }
        else //Bottom half
        {
            if (temp.x <= -divisor)
                current = 4;
            else if (temp.x <= divisor)
                current = 5;
            else
                current = 6;
        }

        image.raycastTarget = false;
        foreach (GameObject slot in Slots)
            slot.GetComponent<InventorySlot>().Taken = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float divisor = GetDivisors() / 6;
        switch (current)
        {
            case 1:
                transform.position = Input.mousePosition - new Vector3(-(2 * divisor), divisor);
                break;
            case 2:
                transform.position = Input.mousePosition - new Vector3(0, divisor);
                break;
            case 3:
                transform.position = Input.mousePosition - new Vector3(2 * divisor, divisor);
                break;
            case 4:
                transform.position = Input.mousePosition - new Vector3(-(2 * divisor), -divisor);
                break;
            case 5:
                transform.position = Input.mousePosition - new Vector3(0, -divisor);
                break;
            case 6:
                transform.position = Input.mousePosition - new Vector3(2 * divisor, -divisor);
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDropped)
        {
            transform.position = new Vector3(((Slots[0].transform.position.x + Slots[5].transform.position.x) / 2), ((Slots[0].transform.position.y + Slots[5].transform.position.y) / 2));
            image.raycastTarget = true;
            foreach (GameObject slot in Slots)
                slot.GetComponent<InventorySlot>().Taken = true;
        }
    }

    public override bool CheckSlot(string Pos)
    {
        if (!Inventory.instance.Grid[Pos].Taken)
        {
            int x = int.Parse(Pos.Substring(0, 1));
            int y = int.Parse(Pos.Substring(1, 1));
            switch (current)
            {
                case 1:
                    break;
                case 2:
                    y -= 1;
                    break;
                case 3:
                    y -= 2;
                    break;
                case 4:
                    x -= 1;
                    break;
                case 5:
                    x -= 1;
                    y -= 1;
                    break;
                case 6:
                    x -= 1;
                    y -= 2;
                    break;
            }

            if ((x != 1 && x != 2) || y != 1)
                Debug.Log("Invalid");
            else if (CheckGrid(x, y))
            {
                Debug.Log("Valid");
                int index = 0;
                for (int i = x; i <= x + 1; i++)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        Slots[index] = Inventory.instance.Grid[i.ToString() + j.ToString()].gameObject;
                        index++;
                    }
                }
                return true;
            }
        }

        return false;
    }

    public bool CheckGrid(int x, int y)
    {
        for (int i = x; i <= x + 1; i++)
        {
            for (int j = y; j <= y + 2; j++)
            {
                if (Inventory.instance.Grid[i.ToString() + j.ToString()].Taken)
                    return false;
            }
        }

        return true;
    }

    public override bool PickupItem()
    {
        for (int i = 1; i <= 2; i++)
        {
            if (CheckSlot(i.ToString() + "1"))
            {
                isDropped = false;
                isMarked = false;
                transform.SetParent(GameObject.Find("InventoryImages").transform);
                OnEndDrag(null);
                sprite.enabled = false;
                image.enabled = true;
                box.enabled = false;
                transform.localScale = new Vector3(1, 1, 1);
                MusicManager.instance.soundSources[17].Play();
                return true;
            }
        }
        return false;
    }

    public override bool EatItem(Player player)
    {
        return false;
    }

    public override void ItemDropped(GameObject Character)
    {
        MusicManager.instance.soundSources[16].Play();
        sprite.enabled = true;
        image.raycastTarget = true;
        image.enabled = false;
        isDropped = true;
        for (int i = 0; i <= 5; i++)
            Slots[i] = null;
        current = 0;
        transform.SetParent(GameObject.Find("RegionManager").transform);
        transform.localScale = Vector3.one;
        Transform character = Character.transform;
        transform.position = character.position;
        if (character.GetComponent<Rigidbody2D>().velocity.x > 0)
            StartCoroutine(MoveToPositionCoroutine(transform.localPosition + new Vector3(2f, 0f, 0f), 0.5f));
        else
            StartCoroutine(MoveToPositionCoroutine(transform.localPosition + new Vector3(-2f, 0f, 0f), 0.5f));
    }
    private IEnumerator MoveToPositionCoroutine(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float height = 1f;
            Vector3 arcPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            arcPosition.y += Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI) * height;

            Collider2D[] hits = Physics2D.OverlapCircleAll(arcPosition, 0.5f);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Wolf"))
                    isMarked = true;
            }
            transform.position = arcPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        box.enabled = true;
    }

    public override void Highlight(bool toggle)
    {
        if (toggle)
            HighlightObject.SetActive(true);
        else
            HighlightObject.SetActive(false);
    }

    public void Consume(out float eatTime, out float foodValue, out string effect, out float effectValue)
    {
        eatTime = 1;
        foodValue = 60;
        effect = "Slow";
        effectValue = 12;
        region.numActive--;
        Destroy(gameObject);
        Debug.Log("Consume Ice");
    }
}
