using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DndFirebase;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject Loader;

    public GameObject ConnectMenu;
    public GameObject GamesListMenu;
    public GameObject NewGameMenu;
    public GameObject MapsListMenu;
    public GameObject NewMapMenu;

    Dictionary<MainMenuState, GameObject> menus;


    MainMenuState _state;

    public MainMenuState State
    {
        get => _state;
        set
        {
            _state = value;

            foreach (var (menuState, menu) in menus)
            {
                if (!menu) continue;
                menu.SetActive(menuState == _state);
            }
        }
    }

    void Start()
    {
        State = MainMenuState.Connect;
    }

    void Awake()
    {
        menus = new() {
            { MainMenuState.Connect, ConnectMenu },
            { MainMenuState.GamesList, GamesListMenu },
            { MainMenuState.NewGame, NewGameMenu },
            { MainMenuState.MapsList, MapsListMenu },
            { MainMenuState.NewMap, NewMapMenu },
        };
    }

    public void SetLoading(bool isLoading)
    {
        Loader.SetActive(isLoading);
    }
}

public enum MainMenuState
{
    Connect,
    GamesList,
    NewGame,
    MapsList,
    NewMap,
}
