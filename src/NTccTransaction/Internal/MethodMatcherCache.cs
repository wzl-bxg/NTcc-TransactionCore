// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NTccTransaction.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NTccTransaction
{
    public class MethodMatcherCache
    {
        private readonly ITransactionServiceSelector _selector;

        public MethodMatcherCache(ITransactionServiceSelector selector)
        {
            _selector = selector;
            Entries = new ConcurrentDictionary<string, CacheExecutorDescriptor>();
        }

        private ConcurrentDictionary<string, CacheExecutorDescriptor> Entries { get; }


        /// <summary>
        /// Get a dictionary of candidates.In the dictionary,
        /// the Key is the HybridCacheAttribute Group, the Value for the current Group of candidates
        /// </summary>
        public ConcurrentDictionary<string, CacheExecutorDescriptor> GetTccCandidatesMethods()
        {
            if (Entries.Count != 0)
            {
                return Entries;
            }

            var executorCollection = _selector.SelectCandidates();

            foreach (var item in executorCollection)
            {
                Entries.TryAdd(item.ImplTypeInfo.FullName + "." + item.MethodInfo.Name, item);
            }

            return Entries;
        }

      
        public bool TryGetTccTransactionExecutor(string key,
            out CacheExecutorDescriptor matchExcutor)
        {
            if (Entries == null)
            {
                throw new ArgumentNullException(nameof(Entries));
            }

            matchExcutor = null;

            if (Entries.TryGetValue(key, out matchExcutor))
            {
                return matchExcutor != null;
            }

            return false;
        }
    }
}