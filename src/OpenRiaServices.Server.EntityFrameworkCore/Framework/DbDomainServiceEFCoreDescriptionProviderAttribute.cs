﻿using System;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using OpenRiaServices.Server;
using OpenRiaServices.Server.EntityFrameworkCore;

namespace OpenRiaServices.EntityFrameworkCore
{
    /// <summary>
    /// Attribute applied to a <see cref="DbDomainServiceEFCore{DbContext}"/> that exposes LINQ to Entities mapped
    /// Types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DbDomainServiceEFCoreDescriptionProviderAttribute : DomainServiceDescriptionProviderAttribute
    {
        private Type _dbContextType;

        /// <summary>
        /// Default constructor. Using this constructor, the Type of the LINQ To Entities
        /// DbContext will be inferred from the <see cref="DomainService"/> the
        /// attribute is applied to.
        /// </summary>
        public DbDomainServiceEFCoreDescriptionProviderAttribute()
            : base(typeof(LinqToEntitiesDomainServiceEFCoreDescriptionProvider))
        {
        }

        /// <summary>
        /// Constructs an attribute for the specified LINQ To Entities
        /// DbContext Type.
        /// </summary>
        /// <param name="dbContextType">The LINQ To Entities ObjectContext Type.</param>
        public DbDomainServiceEFCoreDescriptionProviderAttribute(Type dbContextType)
            : base(typeof(LinqToEntitiesDomainServiceEFCoreDescriptionProvider))
        {
            this._dbContextType = dbContextType;
        }

        /// <summary>
        /// The Linq To Entities DbContext Type.
        /// </summary>
        public Type DbContextType
        {
            get
            {
                return this._dbContextType;
            }
        }

        /// <summary>
        /// This method creates an instance of the <see cref="TypeDescriptionProvider"/>.
        /// </summary>
        /// <param name="domainServiceType">The <see cref="DomainService"/> Type to create a description provider for.</param>
        /// <param name="parent">The existing parent description provider.</param>
        /// <returns>The description provider.</returns>
        public override DomainServiceDescriptionProvider CreateProvider(Type domainServiceType, DomainServiceDescriptionProvider parent)
        {
            if (domainServiceType == null)
            {
                throw new ArgumentNullException(nameof(domainServiceType));
            }

            if (this._dbContextType == null)
            {
                this._dbContextType = GetContextType(domainServiceType);
            }

            if (!typeof(DbContext).IsAssignableFrom(this._dbContextType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    DbResource.InvalidDbDomainServiceDescriptionProviderSpecification,
                    this._dbContextType));
            }

            return new LinqToEntitiesDomainServiceEFCoreDescriptionProvider(domainServiceType, this._dbContextType, parent);
        }

        /// <summary>
        /// Extracts the context type from the specified <paramref name="domainServiceType"/>.
        /// </summary>
        /// <param name="domainServiceType">A LINQ to Entities domain service type.</param>
        /// <returns>The type of the object context.</returns>
        private static Type GetContextType(Type domainServiceType)
        {
            Type efDomainServiceType = domainServiceType.BaseType;
            while (!efDomainServiceType.IsGenericType || efDomainServiceType.GetGenericTypeDefinition() != typeof(DbDomainServiceEFCore<>))
            {
                if (efDomainServiceType == typeof(object))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    ResourceEFCore.InvalidMetadataProviderSpecification,
                    typeof(DbDomainServiceEFCoreDescriptionProviderAttribute).Name, domainServiceType.Name, typeof(DbDomainServiceEFCore<>).Name));
                }
                efDomainServiceType = efDomainServiceType.BaseType;
            }

            return efDomainServiceType.GetGenericArguments()[0];
        }
    }
}
