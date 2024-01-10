using ServerCoreTCP.Job;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Network
{
    public class SessionManager
    {
        #region Singleton
        readonly static SessionManager _instance = new SessionManager();
        public static SessionManager Instance => _instance;
        #endregion

        // TODO : How about using Concurrent Dictionary?
        Dictionary<uint, ClientSession> _sessions = new Dictionary<uint, ClientSession>();
        uint _idIssuer = 1;
        object _lock = new object();

        public ClientSession CreateNewSession()
        {
            ClientSession session = new ClientSession();
            lock (_lock)
            {
                session.ServerSessionId = _idIssuer;
                _sessions.Add(_idIssuer, session);
                _idIssuer++;
            }
            return session;
        }

        public bool Remove(uint id)
        {
            lock (_lock)
            {
                return _sessions.Remove(id);
            }
        }

        public List<ClientSession> GetSessions()
        {
            List<ClientSession> sessions;
            lock (_lock)
            {
                sessions = _sessions.Values.ToList();
            }
            return sessions;
        }
    }
}
