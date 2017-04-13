using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct Node
{
    public int x;
    public int y;

    public float G;
    public float H;

    public int parent;

    public Node(int _x, int _y)
    {
        x = _x;
        y = _y;
        G = 0;
        H = 999999;
        parent = 0;
    }

    public void SetCoor(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

}

public class UIEventManager : MonoBehaviour
{
    public Text showText;
    public GameObject errorText;
    public GameObject errorText2;
    public GameObject resText;
    public int blockNum;
    public GameObject[] blocks;

    public int row;
    public int col;

    private float lastTime;


    public void OnBoundarySliderChanged(Slider slider)
    {
        showText.text = (slider.value / slider.maxValue).ToString();
    }

    public void OnBlockClicked(GameObject block)
    {
        block.GetComponent<BlockState>().AddState();
    }

    public void OnBuildMapClicked()
    {
        InitWhenClicked();
        bool[] temp = new bool[blocks.Length];
        for(int i = 0; i < temp.Length; i++)
        {
            temp[i] = false;
        }

        for(int i = 0; i < float.Parse(showText.text) * blocks.Length; i++)
        {
            temp[i] = true;
        }

        for(int i = 0;i < 1000;i++)
        {
            bool t = false;
            int t1 = Random.Range(0, blocks.Length);
            int t2 = Random.Range(0, blocks.Length);
            t = temp[t1];
            temp[t1] = temp[t2];
            temp[t2] = t;
        }

        for(int i = 0; i < blocks.Length; i++)
        {
            blocks[i].GetComponent<BlockState>().Empty();
            if (temp[i])
                blocks[i].GetComponent<BlockState>().AddState();
        }

    }

    void AddNode(List<Node> list, Node n)
    {
        if (list.Count == 0)
        {
            list.Add(n);
            return;
        }

        int length = list.Count;
        for(int i = 0; i < length; i++)
        {
            if(n.G + n.H < list[i].G + list[i].H)
            {
                list.Insert(i, n);
                return;
            }
        }
        list.Add(n);
    }

    int CheckIfHave(List<Node> list, Node n)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i].x == n.x && list[i].y == n.y)
            {
                return i;
            }
        }
        return -1;
    }

    int GetH(Node n, Node e)
    {
        return Mathf.Abs(n.x - e.x) + Mathf.Abs(n.y - e.y);
    }

    public void OnBuildPathClicked()
    {
        // 初始化
        InitWhenClicked();
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].GetComponent<BlockState>().HidePath();
        }

        Node startNode = new Node(-1, -1);
        Node endNode = new Node(-1, -1);

        // 获得起点和终点
        for (int i = 0; i < blocks.Length; i++)
        {
            // 获得起点
            if(blocks[i].GetComponent<BlockState>().currentState == BlockState.State.start)
            {
                if(startNode.x != -1)
                {
                    errorText.SetActive(true);
                    return;
                }
                startNode.SetCoor(i / 14, i % 14);
            }

            // 获得终点
            if(blocks[i].GetComponent<BlockState>().currentState == BlockState.State.end)
            {
                if(endNode.x != -1)
                {
                    errorText.SetActive(true);
                    return;
                }
                endNode.SetCoor(i / 14, i % 14);
            }
        }
        // 判断是否至少存在1个起点和终点
        {
            if (startNode.x == -1 || endNode.x == -1)
            {
                errorText.SetActive(true);
                return;
            }
            else
            {
                errorText.SetActive(false);
            }
        }

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        Node current = new Node(-1, -1);
        bool hasAnser = false;

        AddNode(openList, startNode);
        
        while(openList.Count != 0)
        {
            current = openList[0];
            openList.RemoveAt(0);
            AddNode(closeList, current);

            //Debug.Log(current.x + "    " + current.y + "    ----->   " + (current.G + current.H));

            if(current.x == endNode.x && current.y == endNode.y)
            {
                hasAnser = true;
                break;
            }

            // 该点的上面
            if (current.x > 0)
            {
                Node n = new Node(current.x - 1, current.y);
                if (blocks[(current.x - 1) * col + current.y].GetComponent<BlockState>().currentState != BlockState.State.wall && CheckIfHave(closeList, n) == -1)
                {
                    n.G = current.G + 1;
                    n.H = GetH(n, endNode);
                    n.parent = 3;
                    int index = CheckIfHave(openList, n);
                    if (index == -1)
                    {
                        AddNode(openList, n);
                    }
                    else
                    {
                        if (current.G + current.H < openList[index].G + openList[index].H)
                            openList[index] = n;
                    }
                }
            }
            // 该点的下面
            if (current.x < row-1)
            {
                Node n = new Node(current.x + 1, current.y);
                if (blocks[(current.x + 1) * col + current.y].GetComponent<BlockState>().currentState != BlockState.State.wall && CheckIfHave(closeList, n) == -1)
                {
                    n.G = current.G + 1;
                    n.H = GetH(n, endNode);
                    n.parent = 1;
                    int index = CheckIfHave(openList, n);
                    if (index == -1)
                    {
                        AddNode(openList, n);
                    }
                    else
                    {
                        if (current.G + current.H < openList[index].G + openList[index].H)
                            openList[index] = n;
                    }
                }
            }
            // 该点的左面
            if (current.y > 0)
            {
                Node n = new Node(current.x, current.y - 1);
                if (blocks[(current.x) * col + current.y - 1].GetComponent<BlockState>().currentState != BlockState.State.wall && CheckIfHave(closeList, n) == -1)
                {
                    n.G = current.G + 1;
                    n.H = GetH(n, endNode);
                    n.parent = 2;
                    int index = CheckIfHave(openList, n);
                    if (index == -1)
                    {
                        AddNode(openList, n);
                    }
                    else
                    {
                        if (current.G + current.H < openList[index].G + openList[index].H)
                            openList[index] = n;
                    }
                }
            }
            // 该点的右面
            if (current.y < col - 1)
            {
                Node n = new Node(current.x, current.y + 1);
                if (blocks[(current.x) * col + current.y + 1].GetComponent<BlockState>().currentState != BlockState.State.wall && CheckIfHave(closeList, n) == -1)
                {
                    n.G = current.G + 1;
                    n.H = GetH(n, endNode);
                    n.parent = 4;
                    int index = CheckIfHave(openList, n);
                    if (index == -1)
                    {
                        AddNode(openList, n);
                    }
                    else
                    {
                        if(current.G + current.H < openList[index].G + openList[index].H)
                            openList[index] = n;
                    }
                }
            }
        }

        for (int i = 0; i < openList.Count; i++)
        {
            blocks[openList[i].x * col + openList[i].y].GetComponent<BlockState>().ShowOpen();
        }
        for (int i = 0; i < closeList.Count; i++)
        {
            blocks[closeList[i].x * col + closeList[i].y].GetComponent<BlockState>().ShowClose();
        }

        if (hasAnser)
        {
            while(current.x != startNode.x || current.y != startNode.y)
            {
                blocks[current.x * col + current.y].GetComponent<BlockState>().ShowPath();
                switch (current.parent)
                {
                    case 1:
                        current = closeList[CheckIfHave(closeList, new Node(current.x-1, current.y))];
                        break;
                    case 2:
                        current = closeList[CheckIfHave(closeList, new Node(current.x, current.y+1))];
                        break;
                    case 3:
                        current = closeList[CheckIfHave(closeList, new Node(current.x+1, current.y))];
                        break;
                    case 4:
                        current = closeList[CheckIfHave(closeList, new Node(current.x, current.y-1))];
                        break;
                    default:
                        break;
                }
                blocks[current.x * col + current.y].GetComponent<BlockState>().ShowPath();
            }
            resText.SetActive(true);
            resText.GetComponent<Text>().text = "找到路径！耗时" + (System.DateTime.Now.Millisecond - lastTime) + "ms";
        }
        else
        {
            errorText2.SetActive(true);
            errorText2.GetComponent<Text>().text = "找不到路径！耗时" + (System.DateTime.Now.Millisecond - lastTime) + "ms";
        }

    }

    void InitWhenClicked()
    {
        lastTime = System.DateTime.Now.Millisecond;
        errorText.SetActive(false);
        errorText2.SetActive(false);
        resText.SetActive(false);
    }

}
