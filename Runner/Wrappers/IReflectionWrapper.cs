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
using System.Reflection;

namespace Gauge.CSharp.Runner.Wrappers
{
    public interface IReflectionWrapper
    {
        MethodInfo GetMethod(Type type, string methodName);
        object Invoke(MethodInfo method, object obj, params object[] args);
        object InvokeMethod(Type type, object instance, string methodName, params object[] args);
    }
}