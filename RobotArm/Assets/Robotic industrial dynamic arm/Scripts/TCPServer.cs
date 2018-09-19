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
 * Executes and manages  the TCP connection between the Q-Cog Simulation Testbed and the Agent Module.
 * Listens for client TCP connections on the assigned network port
 */
 public class TCPServer : MonoBehaviour {
    private StreamReader reader;                // Read from the stream
    private StreamWriter writer;                // Write to the stream
    private Queue<TCPMessage> recv_buffer;      // Incoming messages are buffered here
	private Queue<TCPMessage> send_buffer;      // Outgoing messages are buffered here
    private TCPMessageScanner message_scanner;  // Convert between bytes and messages
    private TcpListener listener;               // Used to listen for potential clients
	private TcpClient client;                   // This is our client (make list if more than 1)
    private int networkPort = 1302; 			// Network port of connection


    void Awake() {
		DontDestroyOnLoad (this);  // Preserve this server between simulation iterations
		if (FindObjectsOfType(GetType()).Length > 1) {
			Destroy(gameObject);   // Don't ever have more than 1 server
			return;
		}

        recv_buffer = new Queue<TCPMessage>();
        send_buffer = new Queue<TCPMessage>();
        message_scanner = new TCPMessageScanner();
        InitializeServer();
    }

    void Update() {
        if (client != null && !client.Connected) {  // If the client disconnects
            Debug.LogError("Client not connected.");
            CleanUp();
            Application.Quit();
        }
    }

    private void InitializeServer() {
        StartServer();  // Handle connections
		new Thread(receive_messages).Start();  // Start listening
		new Thread(send_messages).Start();     // Start sending
    }

	// Run this as a new thread
    private void receive_messages() {
        while (true) {
			string raw_input = reader.ReadLine();
			if (raw_input.Equals ("")) continue;
            TCPMessage msg_recv = message_scanner.BuildTCPMessage(raw_input);
			recv_buffer.Enqueue(msg_recv);
			//print ("recv: " + raw_input);
        }
    }

	// Run this as a new thread
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

	// Check if there is a message
    public TCPMessage Receive() {
		if (recv_buffer.Count == 0) return null;
		TCPMessage message = recv_buffer.Dequeue();
        return message;
    }

	// Wait until there is a message
	public TCPMessage BlockingReceive() {
		TCPMessage msg = Receive ();
		while (msg == null) {
			msg = Receive ();
		}
		return msg;
	}

	// Send a message
    public void SendMessage(TCPMessage msg) {
		send_buffer.Enqueue (msg);
    }

	// Wait for a connection on the port
    void StartServer() {
        Debug.Log("Connecting to agent...");
        try {
            listener = new TcpListener(System.Net.IPAddress.Any, networkPort);
            listener.Start();
            client = listener.AcceptTcpClient();
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            Debug.Log("Client connection successful");
        }
        catch (Exception e) {
            Debug.LogError("Error : " + e.GetType() + " = " + e.ToString());
            Debug.LogError("Listener Endpoint = " + listener.LocalEndpoint.ToString());
            if (client != null) {
                Debug.LogError("Client Endpoint = " + client.Client.LocalEndPoint.ToString());
            }
            CleanUp();
            Application.Quit();
        }
    }

	//  Session finished
    public void CleanUp() {
        if (client != null) {
            client.Close();
        }
        listener.Stop();
    }

}
