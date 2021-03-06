﻿// Copyright 2018 ThoughtWorks, Inc.
//
// This file is part of Gauge-Dotnet.
//
// Gauge-Dotnet is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Gauge-Dotnet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Gauge-Dotnet.  If not, see <http://www.gnu.org/licenses/>.

using Gauge.Dotnet.Processors;
using Gauge.Messages;
using NUnit.Framework;

namespace Gauge.Dotnet.UnitTests.Processors
{
    [TestFixture]
    public class DefaultProcessorTests
    {
        [Test]
        public void ShouldProcessMessage()
        {
            var request = new Message
            {
                MessageId = 20,
                MessageType = Message.Types.MessageType.ExecuteStep
            };

            var response = new DefaultProcessor().Process(request);
            var executionStatusResponse = response.ExecutionStatusResponse;

            Assert.AreEqual(response.MessageId, 20);
            Assert.AreEqual(response.MessageType, Message.Types.MessageType.ExecutionStatusResponse);
            Assert.AreEqual(executionStatusResponse.ExecutionResult.ExecutionTime, 0);
        }
    }
}