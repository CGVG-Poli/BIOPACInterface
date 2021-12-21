using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedClientUI : MonoBehaviour
{
    [SerializeField] private Text _clientName;
    [SerializeField] private Text _desyncTime;
    [SerializeField] private Button _syncBtn;

    public Text ClientName => _clientName;

    public Text DesyncTime => _desyncTime;

    void Start()
    {
        _syncBtn.onClick.AddListener((() =>
        {
            SyncManager.Instance.CalculateClientServerSyncTime();
            _syncBtn.interactable = false;
        }));

        SyncManager.Instance.ClocksDesyncComputed += OnClientClockDesyncComputed;
    }

    private void OnDestroy()
    {
        if(SyncManager.Instance != null)
            SyncManager.Instance.ClocksDesyncComputed -= OnClientClockDesyncComputed;
    }

    private void OnClientClockDesyncComputed(double delta)
    {
        _syncBtn.interactable = true;
        TimeSpan ts = TimeSpan.FromMilliseconds(delta);
        _desyncTime.text = ts.ToString();
    }
}
