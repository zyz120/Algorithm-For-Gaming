using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockState : MonoBehaviour {

    public enum State { road, wall, start, end }

    public State currentState;

    public bool showPathing;

    private void Awake()
    {
        showPathing = false;
    }

    public void AddState()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        currentState = (State)((int)(currentState + 1) % 4);
        if(currentState == State.road)
        {
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        else if(currentState == State.wall)
        {
            GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
        else if(currentState == State.start)
        {
            GetComponent<Image>().color = new Color(0.2f, 0.9f, 0.2f, 1f);
        }
        else if(currentState == State.end)
        {
            GetComponent<Image>().color = new Color(1f, 0.7f, 0.1f, 1f);
        }
    }

    public void Empty()
    {
        currentState = State.road;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void ShowPath()
    {
        showPathing = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0.5f, 0.5f, 1f);
    }

    public void HidePath()
    {
        showPathing = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowOpen()
    {
        showPathing = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 1f, 1f, 1f);
    }

    public void ShowClose()
    {
        showPathing = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 0.5f, 1f);
    }

}
