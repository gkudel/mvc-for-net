﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    public static class DefaultInterceptors
    {
        public const string SecurityInterceptor = "MVCEngine.Model.Interceptors.SecurityInterceptor";
        public const string ModificationInterceptor = "MVCEngine.Model.Interceptors.ModificationInterceptor";
        public const string ValidationInterceptor = "MVCEngine.Model.Interceptors.ValidationInterceptor";
        internal const string CollectionInterceptor = "MVCEngine.Model.Interceptors.CollectionInterceptor";
        internal const string EntityInterceptor = "MVCEngine.Model.Interceptors.EntityInterceptor";        
    }
}