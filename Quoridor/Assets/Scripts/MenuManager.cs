using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject RoomManagerPrefab;
    public GameObject UserLoginPage;
    public InputField UserNameField;
    public InputField PasswordField;
    public Button GoButton;
    public GameObject GameBoard;

    // Start is called before the first frame update
    void Start()
    {
        GoButton.onClick.AddListener(OnGoClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGoClick()
    {
        var configuration = Configuration.GetInstance();
        configuration.UserName = UserNameField.text;
        configuration.UserPassword = PasswordField.text;
        UserLoginPage.SetActive(false);
        RoomManager.Create(RoomManagerPrefab, GameBoard);
    }
}
