using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
        bool Active { get; }
        object UnderlyingTransaction { get; }
    }

    public class DummyTransaction : ITransaction
    {

        bool active = true;
        public void Commit()
        {
            active = false;
        }

        public void Rollback()
        {
            active = false;
        }

        public bool Active
        {
            get { return active; }
        }

        public object UnderlyingTransaction => null;

        public void Dispose()
        {

        }
    }

    public class DbTransactionWrapper : ITransaction
    {
        IDbTransaction transaction;
        public bool Disposed { get; private set; }

        object ITransaction.UnderlyingTransaction => transaction;

        public DbTransaction UnderlyingTransaction => transaction as DbTransaction;

        bool active = true;
        public bool Active { get { return active && transaction != null && transaction.Connection != null; } }
        public DbTransactionWrapper(IDbTransaction Transaction)
        {
            transaction = Transaction;
        }

        

        public void Commit()
        {
            if (!Active) throw new Exception("Transaction is closed");
            if (Disposed) throw new Exception("Transaction has been disposed");


            try
            {
                transaction.Commit();
            }
            finally
            {
                try
                {
                    transaction.Dispose();
                }
                finally
                {
                    transaction = null;
                    active = false;
                    Disposed = true;
                }
            }
        }

        public void Rollback()
        {
            if (!Active) throw new Exception("Transaction is closed");
            if (Disposed) throw new Exception("Transaction has been disposed");

            try
            {
                transaction.Rollback();
            }
            finally
            {
                try
                {
                    transaction.Dispose();
                }
                finally
                {
                    transaction = null;
                    active = false;
                    Disposed = true;
                }
            }
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            if (Active)
            {
                try
                {
                    transaction.Rollback();
                }
                finally
                {
                    active = false;
                }
            }

            try
            {
                transaction.Dispose();
            }
            finally
            {
                Disposed = true;
            }
        }
    }
}
