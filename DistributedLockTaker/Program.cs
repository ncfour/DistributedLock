﻿using Medallion.Threading;
using Medallion.Threading.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET471
using System.Data.SqlClient;
#elif NETCOREAPP3_1
using Microsoft.Data.SqlClient;
#endif

namespace DistributedLockTaker
{
    internal static class Program
    {
        private static readonly string ConnectionString = new SqlConnectionStringBuilder
        {
            DataSource = @".\SQLEXPRESS",
            InitialCatalog = "master",
            IntegratedSecurity = true
        }
        .ConnectionString;

        public static int Main(string[] args)
        {
            var type = args[0];
            var name = args[1];
            IDisposable? handle;
            switch (type)
            {
                case "SqlDistributedLock":
                    handle = new SqlDistributedLock(name, ConnectionString).Acquire();
                    break;
                case "SqlReaderWriterLockDistributedLock":
                    handle = new SqlDistributedReaderWriterLock(name, ConnectionString).AcquireWriteLock();
                    break;
                case "SqlSemaphoreDistributedLock":
                    handle = new SqlDistributedSemaphore(name, maxCount: 1, connectionString: ConnectionString).Acquire();
                    break;
                case "SqlSemaphoreDistributedLock5":
                    handle = new SqlDistributedSemaphore(name, maxCount: 5, connectionString: ConnectionString).Acquire();
                    break;
                case "SystemDistributedLock":
                    handle = new SystemDistributedLock(name).Acquire();
                    break;
                default:
                    return 123;
            }

            Console.WriteLine("Acquired");
            Console.Out.Flush();

            if (Console.ReadLine() != "abandon")
            {
                handle.Dispose();
            }

            return 0;
        }
    }
}
