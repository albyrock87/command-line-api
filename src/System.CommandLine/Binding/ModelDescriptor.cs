﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.CommandLine.Binding
{
    public class ModelDescriptor
    {
        private const BindingFlags CommonBindingFlags =
            BindingFlags.IgnoreCase
            | BindingFlags.Public
            | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, ModelDescriptor> _modelDescriptors = new ConcurrentDictionary<Type, ModelDescriptor>();

        private readonly List<PropertyDescriptor> _propertyDescriptors = new List<PropertyDescriptor>();
        private readonly List<ConstructorDescriptor> _constructorDescriptors = new List<ConstructorDescriptor>();

        protected ModelDescriptor(Type modelType)
        {
            ModelType = modelType ??
                        throw new ArgumentNullException(nameof(modelType));

            foreach (var propertyInfo in modelType.GetProperties(CommonBindingFlags).Where(p => p.CanWrite))
            {
                _propertyDescriptors.Add(new PropertyDescriptor(propertyInfo));
            }

            foreach (var constructorInfo in modelType.GetConstructors(CommonBindingFlags))
            {
                _constructorDescriptors.Add(new ConstructorDescriptor(constructorInfo));
            }
        }

        public IReadOnlyList<ConstructorDescriptor> ConstructorDescriptors => _constructorDescriptors;

        public IReadOnlyList<IValueDescriptor> PropertyDescriptors => _propertyDescriptors;

        public Type ModelType { get; }

        public static ModelDescriptor FromType<T>() =>
            _modelDescriptors.GetOrAdd(
                typeof(T),
                _ => new ModelDescriptor(typeof(T)));

        public static ModelDescriptor FromType(Type type) =>
            _modelDescriptors.GetOrAdd(
                type,
                _ => new ModelDescriptor(type));
    }
}