using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IDropHandler,IPointerClickHandler
{
    private Vector3 originalPos;
    private int id;
    private SpriteRenderer sr;
    static private bool drag = false;
    static private int dragId=Main.NULL;

    // Use this for initialization
    void Start()
    {
        originalPos = transform.position;
        id = int.Parse(new Regex(@"\((?<value>.*?)\)").Match(transform.name).Groups["value"].Value);
        sr = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Player.IsPlaying()&&sr.sprite!=null)
        {
            drag = true;
            dragId = this.id;
            Main.players[0].ShowHandAlpha(id);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Player.IsPlaying() && sr.sprite != null)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            pos.z = -1f;
            this.transform.position = pos;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Player.IsPlaying() && sr.sprite != null)
        {
            Main.players[0].SelfSort(dragId, id);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Player.IsPlaying() && sr.sprite != null)
        {
            drag = false;
            transform.position = originalPos;
            dragId = Main.NULL;
            Main.players[0].ShowHandResetAlpha();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Player.IsPlaying() && sr.sprite != null)
        {
            if (drag == false)
            {
                StateObjects.waitInput_object.Discard(id);
            }
        }
    }
}
