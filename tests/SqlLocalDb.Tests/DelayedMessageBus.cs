﻿// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MartinCostello.SqlLocalDb
{
    internal sealed class DelayedMessageBus : IMessageBus
    {
        private readonly IMessageBus _inner;
        private readonly List<IMessageSinkMessage> _messages = new List<IMessageSinkMessage>();

        internal DelayedMessageBus(IMessageBus inner)
        {
            _inner = inner;
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            lock (_messages)
            {
                _messages.Add(message);
            }

            // No way to ask the inner bus if they want to cancel without sending them the message, so
            // we just go ahead and continue always.
            return true;
        }

        public void Dispose()
        {
            foreach (var message in _messages)
            {
                _inner.QueueMessage(message);
            }
        }
    }
}
