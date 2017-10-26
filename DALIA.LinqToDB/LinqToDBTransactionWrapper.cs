using Dalia;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db
{
  

    internal class LinqToDBTransactionWrapper : ITransaction
    {
        LinqToDB.Data.DataConnectionTransaction transaction;
        public bool Disposed { get; private set; }

        object ITransaction.UnderlyingTransaction => transaction;

        public DbTransaction UnderlyingTransaction { get { return transaction?.DataConnection?.Transaction as DbTransaction; } }

        bool active = true;
        public bool Active { get { return active && transaction != null && transaction.DataConnection != null && transaction.DataConnection.Connection != null; } }
        public LinqToDBTransactionWrapper(LinqToDB.Data.DataConnectionTransaction transaction)
        {
            this.transaction = transaction;
        }

        //~DbTransactionWrapper()
        //{
        //    Dispose();
        //}

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