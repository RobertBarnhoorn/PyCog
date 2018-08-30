using UnityEngine;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/**
 * Executes and manages  the TCP connection between the Q-Cog testbed and the agent module.
 * Listens for client TCP connections on the assigned network port
 */
 public class TCPServer : MonoBehaviour
 {
    private StreamReader reader;
    private StreamWriter writer;
    private List<TCPMessage> message_buffer;

    private TCPMessageScanner message_scanner;

    //Will Stuff
    private TcpListener listener;
    private TcpClient client;
    public bool serverOn = false;

    private int networkPort = 1302; //port used for TCP connection to the agent module

    void Awake()
    {
        message_buffer = new List<TCPMessage>();
        message_scanner = new TCPMessageScanner();
        InitializeServer();
    }

    void Update()
    {
        if (serverOn)
        {
            if (client != null && !client.Connected)//if the client disconnects
            {
                Debug.LogError("Client not connected.");
                CleanUp();
                Application.Quit();
            }
        }
    }

    private void InitializeServer()
    {
        StartServer();
        Thread thread = new Thread(receive_messages);
        thread.Start();
    }

    private void receive_messages()
    {
        while (true)
        {
            string raw_input = reader.ReadLine();

            TCPMessage new_tcp_message = message_scanner.BuildTCPMessage(raw_input);
            message_buffer.Add(new_tcp_message);
        }
    }

    public TCPMessage GetNextMessage()
    {
        int prev = message_buffer.Count;
        if (message_buffer.Count == 0)
        return null;

        TCPMessage message = message_buffer[0];
        message_buffer.RemoveAt(0);
        return message;
    }

    public void SendMessage(TCPMessage message)
    {
        int id = message.id;
        String data = "";

        for (int i = 0; i < message.getData().Count; i++)
        {
            data += "$" + message.getData()[i];
        }

        //Debug.Log("Out: id: " + id + " data: " + data);

        writer.WriteLine(id + data);
        writer.Flush();
    }

    void StartServer()
    {
        serverOn = true;
        Debug.Log("Connecting to Java backend...");
        try
        {
            listener = new TcpListener(System.Net.IPAddress.Any, networkPort);
            listener.Start();
            client = listener.AcceptTcpClient();
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            Debug.Log("Java connection successful");
        }
        catch (Exception e)
        {
            Debug.LogError("Error : " + e.GetType() + " = " + e.ToString());
            Debug.LogError("Listener Endpoint = " + listener.LocalEndpoint.ToString());
            if (client != null)
            {
                Debug.LogError("Client Endpoint = " + client.Client.LocalEndPoint.ToString());
            }
            CleanUp();
            Application.Quit();
        }
    }


    public void CleanUp()
    {
        serverOn = false;
        if (client != null)
        {
            client.Close();
        }
        listener.Stop();

    }

    public void SetNetworkPort(int port)
    {
        networkPort = port;
    }

}

