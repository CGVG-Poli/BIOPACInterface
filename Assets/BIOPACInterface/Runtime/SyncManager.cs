using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using UnityEngine;

//The sync algorithm was taken from 
//https://gamedev.stackexchange.com/questions/93477/how-to-keep-server-client-clocks-in-sync-for-precision-networked-games-like-quak

public class SyncManager : Singleton<SyncManager>
{
    public event Action<double> ClocksDesyncComputed; 

    private readonly int SYNC_CYCLES = 8;
    private readonly float CYCLES_DELAY = 0.05f;
    
    private Coroutine _syncCoroutine;
    private int _cyclesToComplete;
    private List<double> _calculatedDelays;

    private double _lastComputedDelta = 0;

    public double LastComputedDelta => _lastComputedDelta;

    void Start()
    {
        BIOPACInterfaceClient.Instance.ReceivedClientServerSyncMessage += OnReceivedClientServerSyncMessage;
        BIOPACInterfaceServer.Instance.ReceivedClientServerSyncMessage += OnReceivedClientServerSyncMessage;
    }

    private void OnReceivedClientServerSyncMessage(ClientServerSyncMessage message, bool isServer)
    {
        if (!isServer)
        {
            message.ClientTime = new SimpleTime(DateTime.Now);
            Debug.Log($"Client Received Server Time: {message.ServerTime}. Now sending its time.");
            message.ClientTimeSending = new SimpleTime(DateTime.Now);
            BIOPACInterfaceClient.Instance.SendMessageToServer<ClientServerSyncMessage>(message);
            return;
        }

        //SERVER SIDE MESSAGE
        SimpleTime serverNewTime = new SimpleTime(DateTime.Now);
        double rountTripTime = (serverNewTime - message.ServerTime).TotalMilliseconds;
        double networkLatency = rountTripTime / 2f;
            
        //double clientDelta = (message.ClientTime - message.ServerTime).TotalMilliseconds + networkLatency;
        double clientDelta = (serverNewTime - message.ClientTime).TotalMilliseconds - networkLatency;
        
        //NEW FORMULATION FROM https://en.wikipedia.org/wiki/Network_Time_Protocol
        // double clientDeltaNew = ((message.ClientTime - message.ServerTime).TotalMilliseconds +
        //                       (message.ClientTimeSending - serverNewTime).TotalMilliseconds) / 2;

        _cyclesToComplete--;
        _calculatedDelays.Add(clientDelta);
        //Debug.Log($"Roundtrip Time: {rountTripTime} // Latency: {networkLatency}// Client Time: {message.ClientTime} // Server Time: {message.ServerTime}");
        Debug.Log($"Current Sync Delta {clientDelta} // Remaining Sync CyclesClocks Sync cycle {_cyclesToComplete}");
        
        if(_cyclesToComplete > 0)
            return;
        
        double finalDelta = CalculateDelta();

        TimeSpan ts = TimeSpan.FromMilliseconds(finalDelta);
        Debug.Log($"Final clock sync delta:{finalDelta} ms -> {ts}");

        _lastComputedDelta = finalDelta;
        _syncCoroutine = null;
        ClocksDesyncComputed?.Invoke(finalDelta);
    }

    private double CalculateDelta()
    {
        double standardDeviation = _calculatedDelays.StandardDeviation();
        double median = _calculatedDelays.Median();
        
        List<double> deltasToKeep = new List<double>();
        foreach (double delay in _calculatedDelays)
        {
            if( (delay <  median + standardDeviation) && (delay > median - standardDeviation))
                deltasToKeep.Add(delay);
        }

        return deltasToKeep.Average();

    }

    private IEnumerator CalculateClientServerSyncTimeCoroutine(int cycles, float cyclesDelay)
    {
        _cyclesToComplete = cycles;
        _calculatedDelays = new List<double>();
        
        for (int i = 0; i < cycles; i++)
        {
            ClientServerSyncMessage syncMessage = new ClientServerSyncMessage();
            syncMessage.ServerTime = new SimpleTime(DateTime.Now);
            syncMessage.ClientTime = new SimpleTime();
            syncMessage.ClientTimeSending = new SimpleTime();
            
            BIOPACInterfaceServer.Instance.SendMessageToClient<ClientServerSyncMessage>(syncMessage);

            yield return new WaitForSeconds(cyclesDelay);
        }
    }

    public void CalculateClientServerSyncTime()
    {
        CalculateClientServerSyncTime(SYNC_CYCLES, CYCLES_DELAY);
    }
    public void CalculateClientServerSyncTime(int cycles, float cyclesDelay)
    {
        if (_syncCoroutine != null)
            return;

        _syncCoroutine = StartCoroutine(CalculateClientServerSyncTimeCoroutine(cycles, cyclesDelay));

    }

}
