using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AdminPanelController : MonoBehaviour
{
    public static AdminPanelController instance;
    private AdminPanelUser adminPanelUser;

    [SerializeField] private Image solutionImage;
    public Sprite testSprite;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        Screen.SetResolution(1920, 300, FullScreenMode.Windowed);
        NetworkManager.Singleton.StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoint()
    {
        if (adminPanelUser != null)
        {
            adminPanelUser.AddPointServerRpc();
        }
    }

    public void RegisterAdminPanelUser(AdminPanelUser user)
    {
        adminPanelUser = user;
    }

    public void SetSolutionImage(Sprite sprite)
    {
        if (sprite != null)
        {
            solutionImage.color = Color.white;
        } else
        {
            solutionImage.color = Color.clear;
        }
        solutionImage.sprite = sprite;
    }

    public void SetSolutionImageTest()
    {
        SetSolutionImage(testSprite);
    }
}
