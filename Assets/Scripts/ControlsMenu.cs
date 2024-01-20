using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;

    public void OpenControlsPanel()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);
    }
}
