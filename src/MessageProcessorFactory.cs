﻿// Copyright 2015 ThoughtWorks, Inc.
//
// This file is part of Gauge-CSharp.
//
// Gauge-CSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Gauge-CSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Gauge-CSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Gauge.Dotnet.Models;
using Gauge.Dotnet.Processors;
using Gauge.Dotnet.Wrappers;
using Gauge.Messages;

namespace Gauge.Dotnet
{
    public class MessageProcessorFactory
    {
        private Dictionary<Message.Types.MessageType, IMessageProcessor> _messageProcessorsDictionary;

        public MessageProcessorFactory()
        {
            StaticLoader.Instance.LoadImplementations();
            _messageProcessorsDictionary = InitializeMessageHandlers();

        }
        public IMessageProcessor GetProcessor(Message.Types.MessageType messageType)
        {
            if (messageType == Message.Types.MessageType.SuiteDataStoreInit)
            {
                StepRegistry.Instance.Clear();
                InitializeExecutionMessageHandlers();
            }
            if (!_messageProcessorsDictionary.ContainsKey(messageType)) return new DefaultProcessor();
            return _messageProcessorsDictionary[messageType];
        }

        private void InitializeExecutionMessageHandlers()
        {
            var reflectionWrapper = new ReflectionWrapper();
            var assemblyLoader = new AssemblyLoader(new AssemblyWrapper(), new AssemblyLocater(new DirectoryWrapper(), new FileWrapper()).GetAllAssemblies(), reflectionWrapper);
            var activatorWrapper = new ActivatorWrapper();
            var tableFormatter = new TableFormatter(assemblyLoader, activatorWrapper);
            var sandbox = new Sandbox(assemblyLoader, new HookRegistry(assemblyLoader), activatorWrapper, reflectionWrapper);
            var methodExecutor = new MethodExecutor(sandbox);
            var handlers = new Dictionary<Message.Types.MessageType, IMessageProcessor>{
                {Message.Types.MessageType.ExecutionStarting, new ExecutionStartingProcessor(methodExecutor, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.ExecutionEnding, new ExecutionEndingProcessor(methodExecutor, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.SpecExecutionStarting, new SpecExecutionStartingProcessor(methodExecutor, sandbox, assemblyLoader, reflectionWrapper) },
                {Message.Types.MessageType.SpecExecutionEnding,new SpecExecutionEndingProcessor(methodExecutor, sandbox, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.ScenarioExecutionStarting,new ScenarioExecutionStartingProcessor(methodExecutor, sandbox, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.ScenarioExecutionEnding,new ScenarioExecutionEndingProcessor(methodExecutor, sandbox, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.StepExecutionStarting, new StepExecutionStartingProcessor(methodExecutor, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.StepExecutionEnding, new StepExecutionEndingProcessor(methodExecutor, assemblyLoader, reflectionWrapper)},
                {Message.Types.MessageType.ExecuteStep, new ExecuteStepProcessor(methodExecutor, tableFormatter)},
                {Message.Types.MessageType.KillProcessRequest, new KillProcessProcessor()},
                {Message.Types.MessageType.ScenarioDataStoreInit, new ScenarioDataStoreInitProcessor(assemblyLoader)},
                {Message.Types.MessageType.SpecDataStoreInit, new SpecDataStoreInitProcessor(assemblyLoader)},
                {Message.Types.MessageType.SuiteDataStoreInit, new SuiteDataStoreInitProcessor(assemblyLoader)},
            };
            handlers.ToList().ForEach(x => _messageProcessorsDictionary.Add(x.Key, x.Value));

        }

        private Dictionary<Message.Types.MessageType, IMessageProcessor> InitializeMessageHandlers()
        {
            var messageHandlers = new Dictionary<Message.Types.MessageType, IMessageProcessor>
            {
                {Message.Types.MessageType.StepNamesRequest, new StepNamesProcessor()},
                {Message.Types.MessageType.StepValidateRequest, new StepValidationProcessor()},
                {Message.Types.MessageType.StepNameRequest, new StepNameProcessor()},
                {Message.Types.MessageType.RefactorRequest, new RefactorProcessor()}
            };
            return messageHandlers;
        }
    }
}