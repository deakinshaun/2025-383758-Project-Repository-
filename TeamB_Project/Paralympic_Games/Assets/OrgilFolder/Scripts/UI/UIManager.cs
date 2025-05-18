using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIScreen initialScreen;
    private Stack<UIScreen> stack = new Stack<UIScreen>();

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Push(initialScreen);
    }

    public void TryPush(GameObject screen)
    {
        if (screen.GetComponent<UIScreen>())
        {
            Push(screen.GetComponent<UIScreen>());
        }
    }
    public void Push(UIScreen screen, object data = null)
    {
        if (stack.Count > 0)
        {
            stack.Peek().Hide();
        }
        stack.Push(screen);
        screen.Show(data);
    }
    public void Pop()
    {
        if (stack.Count > 0)
        {
            stack.Pop().Hide();
        }

        if (stack.Count > 0)
        {
            stack.Peek().Show();
        }
    }
}
