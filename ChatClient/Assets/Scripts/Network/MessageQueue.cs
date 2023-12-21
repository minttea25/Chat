using Google.Protobuf;
using System.Collections;
using System.Collections.Concurrent;

public class MessagePacket
{
    public ushort Type; // to PacketType
    public IMessage Message;
}

public class MessageQueue
{
    #region Singleton
    static readonly MessageQueue instance = new MessageQueue();
    public static MessageQueue Instance => instance;
    #endregion

    readonly ConcurrentQueue<MessagePacket> queue = new();

    public void Push(MessagePacket packet)
    {
        queue.Enqueue(packet);
    }

    public MessagePacket[] PopAll()
    {
        var arr = queue.ToArray();
        queue.Clear();
        return arr;
    }
}
