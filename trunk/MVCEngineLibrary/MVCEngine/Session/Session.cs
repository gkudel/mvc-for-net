using MVCEngine.Session.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Session
{
    public class Session
    {
        #region Members
        private static Lazy<List<Session>> _sessions;
        private Lazy<List<KeyValuePair<string, object>>> _values;
        private static object _threadLock;
        #endregion Members
        
        #region Constructor
        static Session()
        {
            _threadLock = new object();
            _sessions = new Lazy<List<Session>>(() => { return new List<Session>(); }, true);
        }

        private Session()
        {
            _values = new Lazy<List<KeyValuePair<string, object>>>(() => { return new List<KeyValuePair<string, object>>(); }, true);
        }
        #endregion Constructor

        #region Properties
        public string SessionId { get; private set; }
        #endregion Properties

        #region Methods
        public static string CreateSession()
        {
            Session session = new Session() { SessionId = Guid.NewGuid().ToString() };
            _sessions.Value.Add(session);
            return session.SessionId;
        }

        public static bool IsSessionExists(string sessionId)
        {
            lock (_threadLock)
            {
                return _sessions.Value.Exists(s => s.SessionId == sessionId);
            }
        }

        public static void ReleaseSession(string sessionId)
        {
            lock (_threadLock)
            {
                Session session = _sessions.Value.FirstOrDefault(s => s.SessionId == sessionId);
                if (session.IsNotNull())
                {
                    foreach (KeyValuePair<string, object> value in session._values.Value)
                    {
                        if (value.Value.IsTypeOf<IDisposable>())
                        {
                            value.Value.CastToType<IDisposable>().Dispose();
                        }
                    }
                    _sessions.Value.Remove(session);
                }
            }
        }
        #endregion Methods

        #region Data Methods
        public static object GetSessionData(string sessionId, string key)
        {
            lock (_threadLock)
            {
                return GetSessionData<object>(sessionId, key);
            }
        }

        public static T GetSessionData<T>(string sessionId, string key) where T : class
        {
            lock (_threadLock)
            {
                T value = default(T);
                Session session = _sessions.Value.FirstOrDefault(s => s.SessionId == sessionId);
                if (session.IsNotNull())
                {
                    KeyValuePair<string, object> kv = session._values.Value.FirstOrDefault(v => v.Key == key);
                    if (kv.IsNotNull() && kv.Value.IsTypeOf<T>())
                    {
                        value = kv.Value.CastToType<T>();
                    }
                }
                return value;
            }
        }

        public static void SetSessionData(string sessionId, string key, object value)
        {
            lock (_threadLock)
            {
                Session session = _sessions.Value.FirstOrDefault(s => s.SessionId == sessionId);
                if (session.IsNotNull())
                {
                    KeyValuePair<string, object> kv = session._values.Value.FirstOrDefault(v => v.Key == key);
                    if (kv.IsNotNull())
                    {
                        session._values.Value.Remove(kv);
                    }
                    session._values.Value.Add(new KeyValuePair<string, object>(key, value));
                }
                else
                {
                    throw new InvalidSessionIdException("Session doesn't exist or was released");
                }
            }
        }
        #endregion Indexer
    }
}
