using System.Collections.Generic;

/**
 * Represents the network message that is sent across between the Simulation Testbed and the Agent Module.
 */
 public class TCPMessage {
    public int id;
    private List<string> data;

    public TCPMessage(int id)
    {
        this.id = id;
        data = new List<string>();
    }

    public TCPMessage(int id, List<string> data)
    {
        this.id = id;
        this.data = data;
    }

    public void AddData(string new_data)
    {
        data.Add(new_data);
    }

    public List<string> getData()
    {
        return data;
    }

    public override string ToString()
    {
        string output = "id: " + id.ToString() + " data: " + data;
        return output;
    }

} 
