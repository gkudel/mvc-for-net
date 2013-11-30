using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;

namespace MVCEngine
{
    public sealed class TryCatchStatment
    {
        #region Exception
        private Exception Exception { get; set; }
        #endregion Exception

        #region Constructor
        private TryCatchStatment()
        {
        }
        #endregion Constructor

        #region Methods
        public static TryCatchStatment Try()
        {
            return new TryCatchStatment();          
        }

        public TryCatchStatment Invoke(Action action)
        {
            if (Exception.IsNull())
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Exception = e;
                }
            }
            return this;
        }

        public TryCatchStatment Catch<T>(Action<T> catchStatment = null) 
            where T : Exception
        {
            if (Exception.IsNotNull())
            {
                if (Exception.IsTypeOf<T>())
                {
                    if (catchStatment.IsNotNull())
                    {
                        catchStatment(Exception.CastToType<T>());
                    }
                    Exception = null;
                }
            }
            return this;
        }

        public TryCatchStatment Catch<T1, T2>(Action<T1> catchStatmentT1 = null, Action<T2> catchStatmentT2 = null) 
            where T1 : Exception 
            where T2 : Exception                                                                                          
        {
            Catch<T1>(catchStatmentT1);
            Catch<T2>(catchStatmentT2);
            return this;
        }

        public TryCatchStatment Catch<T1, T2, T3>(Action<T1> catchStatmentT1 = null, Action<T2> catchStatmentT2 = null, 
                                                    Action<T3> catchStatmentT3 = null)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
        {
            Catch<T1>(catchStatmentT1);
            Catch<T2>(catchStatmentT2);
            Catch<T3>(catchStatmentT3);
            return this;
        }

        public TryCatchStatment Catch<T1, T2, T3, T4>(Action<T1> catchStatmentT1 = null, Action<T2> catchStatmentT2 = null,
                                                        Action<T3> catchStatmentT3 = null, Action<T4> catchStatmentT4 = null)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
            where T4 : Exception
        {
            Catch<T1>(catchStatmentT1);
            Catch<T2>(catchStatmentT2);
            Catch<T3>(catchStatmentT3);
            Catch<T4>(catchStatmentT4);
            return this;
        }

        public TryCatchStatment Catch<T>(Action<string, string, string, Exception> catchStatment)
            where T : Exception
        {
            if (Exception.IsNotNull())
            {
                if (Exception.IsTypeOf<T>())
                {
                    if (catchStatment.IsNotNull())
                    {
                        catchStatment(Exception.Message, Exception.Source, Exception.StackTrace, Exception);
                    }
                    Exception = null;
                }
            }
            return this;
        }

        public TryCatchStatment Catch<T1, T2>(Action<string, string, string, Exception> catchStatment)
            where T1 : Exception
            where T2 : Exception
        {
            Catch<T1>(catchStatment);
            Catch<T2>(catchStatment);
            return this;
        }
        
        public TryCatchStatment Catch<T1, T2, T3>(Action<string, string, string, Exception> catchStatment)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
        {
            Catch<T1, T2>(catchStatment);
            Catch<T3>(catchStatment);
            return this;
        }

        public TryCatchStatment Catch<T1, T2, T3, T4>(Action<string, string, string, Exception> catchStatment)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
            where T4 : Exception
        {
            Catch<T1, T2>(catchStatment);
            Catch<T3, T4>(catchStatment);
            return this;
        }

        public TryCatchStatment Catch<T>(Action catchStatment)
            where T : Exception
        {
            if (Exception.IsNotNull())
            {
                if (Exception.IsTypeOf<T>())
                {
                    catchStatment();
                    Exception = null;
                }
            }
            return this;
        }

        public TryCatchStatment Catch<T1, T2>(Action catchStatment)
            where T1 : Exception
            where T2 : Exception
        {
            Catch<T1>(catchStatment);
            Catch<T2>(catchStatment);
            return this;
        }

        public TryCatchStatment Catch<T1, T2, T3>(Action catchStatment)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
        {
            Catch<T1, T2>(catchStatment);
            Catch<T3>(catchStatment);
            return this;
        }

        public TryCatchStatment Catch<T1, T2, T3, T4>(Action catchStatment)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
            where T4 : Exception
        {
            Catch<T1, T2>(catchStatment);
            Catch<T3, T4>(catchStatment);
            return this;
        }

        public TryCatchStatment Catch(Action<string, string, string, Exception> catchStatment)
        {
            if (Exception.IsNotNull())
            {
                if (catchStatment.IsNotNull())
                {
                    catchStatment(Exception.Message, Exception.Source, Exception.StackTrace, Exception);
                }
                Exception = null;
            }
            return this;
        }


        public void Throw()
        {
            if (Exception.IsNotNull())
            {
                throw Exception;
            }
        }

        public TryCatchStatment Finally(Action action)
        {
            action();
            return this;
        }
        #endregion Methods
    }
}
