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

using Gauge.Dotnet.Models;
using Gauge.Dotnet.Processors;
using Gauge.Dotnet.Wrappers;
using Gauge.Messages;
using NUnit.Framework;

namespace Gauge.Dotnet.IntegrationTests
{
    public class ExternalReferenceTests : IntegrationTestsBase
    {
        private readonly string _testProjectPath = TestUtils.GetIntegrationTestSampleDirectory();

        [Test]
        [TestCase("Dll Reference: Vowels in English language are {}.", "Dll Reference: Vowels in English language are <vowelString>.", "Dll Reference: Vowels in English language are \"aeiou\".")]
        [TestCase("Project Reference: Vowels in English language are {}.", "Project Reference: Vowels in English language are <vowelString>.", "Project Reference: Vowels in English language are \"aeiou\".")]
        public void ShouldGetStepsFromReference(string stepText, string stepValue, string parameterizedStepValue)
        {
            var assemblies = new AssemblyLocater(new DirectoryWrapper(), new FileWrapper()).GetAllAssemblies();
            var assemblyLoader = new AssemblyLoader(new AssemblyWrapper(),
                assemblies, new ReflectionWrapper(), new ActivatorWrapper(), new StepRegistry());

            var stepValidationProcessor = new StepValidationProcessor(assemblyLoader.GetStepRegistry());
            var message = new StepValidateRequest
                {
                    StepText = stepText,
                    StepValue = new ProtoStepValue {StepValue = stepValue, ParameterizedStepValue = parameterizedStepValue},
                    NumberOfParameters  = 1,
                };
            var result = stepValidationProcessor.Process(message);

            Assert.IsTrue(result.IsValid, $"Expected valid step text, got error: {result.ErrorMessage}");
        }

        [Test]
        [TestCase("Take Screenshot in reference Project", "ReferenceProject-IDoNotExist.png")]
        [TestCase("Take Screenshot in reference DLL", "ReferenceDll-IDoNotExist.png")]
        public void ShouldRegisterScreenshotWriterFromReference(string stepText, string expected) {
            var reflectionWrapper = new ReflectionWrapper();
            var activatorWrapper = new ActivatorWrapper();
            var assemblyLoader = new AssemblyLoader(new AssemblyWrapper(),
                new AssemblyLocater(new DirectoryWrapper(), new FileWrapper()).GetAllAssemblies(), reflectionWrapper, activatorWrapper, new StepRegistry());
            var classInstanceManager = assemblyLoader.GetClassInstanceManager();

            var mockOrchestrator = new ExecutionOrchestrator(reflectionWrapper, assemblyLoader, activatorWrapper,
                classInstanceManager,
                new HookExecutor(assemblyLoader, reflectionWrapper, classInstanceManager),
                new StepExecutor(assemblyLoader, reflectionWrapper, classInstanceManager));

            var executeStepProcessor = new ExecuteStepProcessor(assemblyLoader.GetStepRegistry(),
                mockOrchestrator, new TableFormatter(assemblyLoader, activatorWrapper));

            var message = new ExecuteStepRequest
            {
                ParsedStepText = stepText,
                ActualStepText = stepText
            };

            var result = executeStepProcessor.Process(message);
            var protoExecutionResult = result.ExecutionResult;

            Assert.IsNotNull(protoExecutionResult);
            Assert.AreEqual(protoExecutionResult.ScreenshotFiles[0], expected);
        }
    }
}