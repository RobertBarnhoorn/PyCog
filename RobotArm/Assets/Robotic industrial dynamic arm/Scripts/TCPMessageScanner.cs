using System.Collections.Generic;
using System;
using UnityEngine;

/**
 * Used to interpret the TCPMessage Objects that are sent across the network between the testbed and the agent module
 */
public class TCPMessageScanner {

	public RoboticArm armController;

    /* Given a string input, this method outputs a new TCPMessage object */
    public TCPMessage BuildTCPMessage(string input) {
        List<string> message_break = BreakUpData(input);
        int id = Int32.Parse(message_break[0]);
        message_break.RemoveAt(0);
        return new TCPMessage(id, message_break);
    }

    /* Given a protocol-adhearing string, an arraylist is output containing
        * separate data contained in the string */
    private List<string> BreakUpData(string data) {
        List<string> data_break = new List<string>();

        string current_data = "";

        for (int i = 0; i < data.Length; i++)
        {
            char ch = data[i];

            if (ch == '$')
            {
                data_break.Add(current_data);
                current_data = "";
            }
            else if (i == data.Length - 1)
            {
                current_data += ch;
                data_break.Add(current_data);
            }
            else
            {
                current_data += ch;
            }
        }
        return data_break;
    }
}

