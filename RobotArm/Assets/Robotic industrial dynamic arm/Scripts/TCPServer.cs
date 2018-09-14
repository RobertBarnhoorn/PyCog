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
 public class TCPServer : MonoBehaviour {
    private StreamReader reader;
    private StreamWriter writer;
    private Queue<TCPMessage> recv_buffer;
	private Queue<TCPMessage> send_buffer;
    private TCPMessageScanner message_scanner;
    private TcpListener listener;
    private TcpClient client;

    private int networkPort = 1302; //port used for TCP connection to the agent module


    void Awake() {
		DontDestroyOnLoad (this);
		if (FindObjectsOfType(GetType()).Length > 1) {
			Destroy(gameObject);
			return;
		}

        recv_buffer = new Queue<TCPMessage>();
        send_buffer = new Queue<TCPMessage>();
        message_scanner = new TCPMessageScanner();
        InitializeServer();
    }

    void Update() {
        if (client != null && !client.Connected) //if the client disconnects
        {
            Debug.LogError("Client not connected.");
            CleanUp();
            Application.Quit();
        }
    }

    private void InitializeServer() {
        StartServer();
		new Thread(receive_messages).Start();
		new Thread(send_messages).Start();
    }

    private void receive_messages() {
        while (true)
        {
			string raw_input = reader.ReadLine();
			if (raw_input.Equals ("")) continue;
            TCPMessage msg_recv = message_scanner.BuildTCPMessage(raw_input);
			recv_buffer.Enqueue(msg_recv);

			//print ("recv: " + raw_input);
        }
    }

	private void send_messages() {
		while (true) {
			if (send_buffer.Count > 0) {
				TCPMessage msg = send_buffer.Dequeue ();

				int id = msg.id;
				String data = "";
				for (int i = 0; i < msg.getData ().Count; i++) {
					data += "$" + msg.getData () [i];
				}

				writer.WriteLine (id + data);
				writer.Flush ();

				//print ("sent: " + id + data);
			}
		}

	}

    public TCPMessage Receive() {
		if (recv_buffer.Count == 0) return null;

		TCPMessage message = recv_buffer.Dequeue();
        return message;
    }

	public TCPMessage BlockingReceive() {
		TCPMessage msg = Receive ();
		while (msg == null) {
			msg = Receive ();
		}
		return msg;
	}

    public void SendMessage(TCPMessage msg) {
		send_buffer.Enqueue (msg);
    }

    void StartServer()
    {
        Debug.Log("Connecting to agent...");
        try
        {
            listener = new TcpListener(System.Net.IPAddress.Any, networkPort);
            listener.Start();
            client = listener.AcceptTcpClient();
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            Debug.Log("Client connection successful");
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
        if (client != null)
        {
            client.Close();
        }
        listener.Stop();

    }

}

