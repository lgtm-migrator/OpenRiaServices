﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace OpenRiaServices.Server.Authentication
{
    /// <summary>
    /// <see cref="CodeProcessor"/> implementation that sets the base class of both the
    /// context and entity types generated by a provider implementing
    /// <see cref="IAuthentication{T}"/>.
    /// </summary>
    internal sealed class AuthenticationCodeProcessor : CodeProcessor
    {
        #region Constants

        private const string AuthenticationDomainContextBaseName =
            "OpenRiaServices.Client.Authentication.AuthenticationDomainContextBase";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the AuthenticationCodeProcessor class.
        /// </summary>
        /// <param name="codeDomProvider">The <see cref="CodeDomProvider"/> used during <see cref="DomainService"/> code generation.</param>
        public AuthenticationCodeProcessor(CodeDomProvider codeDomProvider)
            : base(codeDomProvider)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// See <see cref="CodeProcessor.ProcessGeneratedCode"/>.
        /// </summary>
        /// <param name="domainServiceDescription">The domainServiceDescription</param>
        /// <param name="codeCompileUnit">The codeCompileUnit</param>
        /// <param name="typeMapping">The typeMapping</param>
        public override void ProcessGeneratedCode(DomainServiceDescription domainServiceDescription, CodeCompileUnit codeCompileUnit, IDictionary<Type, CodeTypeDeclaration> typeMapping)
        {
            // Make sure the provider extends IAuthentication<T>
            Type genericDomainServiceType;
            CheckIAuthentication(domainServiceDescription, out genericDomainServiceType);

            Type userEntityType = genericDomainServiceType.GetGenericArguments()[0];
            CheckIUser(userEntityType);

            // Implement IPrincipal and IIdentity in the user type
            CodeTypeDeclaration entityTypeDeclaration;
            typeMapping.TryGetValue(userEntityType, out entityTypeDeclaration);

            if (entityTypeDeclaration != null)
            {
                var identityInterfaceTypeReference =
                    new CodeTypeReference(typeof(IIdentity)) { Options = CodeTypeReferenceOptions.GlobalReference };
                var principalInterfaceTypeReference =
                    new CodeTypeReference(typeof(IPrincipal)) { Options = CodeTypeReferenceOptions.GlobalReference };

                entityTypeDeclaration.BaseTypes.Add(identityInterfaceTypeReference);
                entityTypeDeclaration.BaseTypes.Add(principalInterfaceTypeReference);

                ////
                //// private string IIdentity.AuthenticationType
                ////
                var authenticationTypeProperty = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Final,
                    HasGet = true,
                    Name = "AuthenticationType",
                    Type = new CodeTypeReference(typeof(string))
                };

                // get { return string.Empty; }
                authenticationTypeProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodePropertyReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(string)),
                        "Empty")));

                authenticationTypeProperty.PrivateImplementationType = identityInterfaceTypeReference;
                entityTypeDeclaration.Members.Add(authenticationTypeProperty);

                ////
                //// public bool IsAuthenticated
                ////
                var isAuthenticatedProperty = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    HasGet = true,
                    Name = "IsAuthenticated",
                    Type = new CodeTypeReference(typeof(bool))
                };

                // get { return (true != string.IsNullOrEmpty(this.Name)); }
                isAuthenticatedProperty.GetStatements.Add(
                    new CodeMethodReturnStatement(
                        new CodeBinaryOperatorExpression(
                            new CodePrimitiveExpression(true),
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodeMethodInvokeExpression(
                                new CodeTypeReferenceExpression(typeof(string)),
                                "IsNullOrEmpty",
                                new CodePropertyReferenceExpression(
                                    new CodeThisReferenceExpression(),
                                    "Name")))));

                isAuthenticatedProperty.Comments.AddRange(
                    GetDocComments(Resources.ApplicationServices_CommentIsAuth));
                isAuthenticatedProperty.ImplementationTypes.Add(identityInterfaceTypeReference);
                entityTypeDeclaration.Members.Add(isAuthenticatedProperty);

                ////
                //// private string IIdentity.Name
                ////
                // VB Codegen requires us to implement a ReadOnly version of Name as well
                var namePropertyExp = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Final,
                    HasGet = true,
                    Name = "Name",
                    Type = new CodeTypeReference(typeof(string))
                };

                // get { return this.Name; }
                namePropertyExp.GetStatements.Add(
                    new CodeMethodReturnStatement(
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(),
                            "Name")));

                namePropertyExp.PrivateImplementationType = identityInterfaceTypeReference;
                entityTypeDeclaration.Members.Add(namePropertyExp);

                ////
                //// private IIdentity IPrincipal.Identity
                ////
                var identityProperty = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Final,
                    HasGet = true,
                    Name = "Identity",
                    Type = identityInterfaceTypeReference,
                };

                // get { return this; }
                identityProperty.GetStatements.Add(
                    new CodeMethodReturnStatement(
                        new CodeThisReferenceExpression()));

                identityProperty.PrivateImplementationType = principalInterfaceTypeReference;
                entityTypeDeclaration.Members.Add(identityProperty);

                ////
                //// public bool IsInRole(string role)
                ////
                var isInRoleMethod = new CodeMemberMethod()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = "IsInRole",
                    ReturnType = new CodeTypeReference(typeof(bool))
                };
                isInRoleMethod.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference(typeof(string)),
                        "role"));

                // if (this.Roles == null)
                // {
                //     return false;
                // }
                // return this.Roles.Contains(role);
                var ifRolesNullStatement = new CodeConditionStatement();
                ifRolesNullStatement.Condition = new CodeBinaryOperatorExpression(
                    new CodePropertyReferenceExpression(
                        new CodeThisReferenceExpression(),
                        "Roles"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodePrimitiveExpression(null));
                ifRolesNullStatement.TrueStatements.Add(
                    new CodeMethodReturnStatement(new CodePrimitiveExpression(false)));

                isInRoleMethod.Statements.Add(ifRolesNullStatement);
                isInRoleMethod.Statements.Add(
                    new CodeMethodReturnStatement(
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(
                                new CodeTypeReference(typeof(Enumerable))
                                {
                                    Options = CodeTypeReferenceOptions.GlobalReference
                                }),
                            "Contains",
                            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Roles"),
                            new CodeVariableReferenceExpression("role"))));

                isInRoleMethod.Comments.AddRange(
                    GetDocComments(Resources.ApplicationServices_CommentIsInRole));
                isInRoleMethod.ImplementationTypes.Add(principalInterfaceTypeReference);
                entityTypeDeclaration.Members.Add(isInRoleMethod);

                // Changes to Name need to raise change notification for IsAuthenticated. To accomplish this,
                // we'll insert a change event at the end of the "if (this._name != value)" block.
                //
                // >> this.RaisePropertyChanged(nameof(IsAuthenticated));
                CodeMemberProperty nameProperty = entityTypeDeclaration.Members.OfType<CodeMemberProperty>().Where(c => c.Name == "Name").First();
                nameProperty.SetStatements.OfType<CodeConditionStatement>().First().TrueStatements.Add(
                    new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(
                            new CodeThisReferenceExpression(),
                            "RaisePropertyChanged",
                            new CodePrimitiveExpression("IsAuthenticated"))));

                // Name should be set to string.Empty by default
                CodeMemberField nameField = entityTypeDeclaration.Members.OfType<CodeMemberField>().Where(c => c.Name == "_name").Single();
                nameField.InitExpression =
                    new CodePropertyReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(string)),
                        "Empty");
            }

            // Set context base type           
            CodeTypeDeclaration providerTypeDeclaration;
            typeMapping.TryGetValue(domainServiceDescription.DomainServiceType, out providerTypeDeclaration);

            if (providerTypeDeclaration != null)
            {
                providerTypeDeclaration.BaseTypes.Clear();
                providerTypeDeclaration.BaseTypes.Add(
                    new CodeTypeReference(AuthenticationDomainContextBaseName)
                    {
                        Options = CodeTypeReferenceOptions.GlobalReference
                    });
            }
        }

        /// <summary>
        /// Takes a multi-line comment defined in a resource file and correctly formats it as a doc comment
        /// for use in code-dom.
        /// </summary>
        /// <param name="resourceComment">The comment to format as a doc comment. This cannot be null.</param>
        /// <returns>A collection of comment statements that matches the input resource</returns>
        internal static CodeCommentStatementCollection GetDocComments(string resourceComment)
        {
            if (resourceComment == null)
            {
                throw new ArgumentNullException(nameof(resourceComment));
            }

            var commentCollection = new CodeCommentStatementCollection();
            foreach (string comment in resourceComment.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                commentCollection.Add(new CodeCommentStatement(comment, true));
            }
            return commentCollection;
        }

        /// <summary>
        /// Validates that the authentication service implements the <see cref="IAuthentication{T}"/> interface 
        /// naturally for use in codegen.
        /// </summary>
        /// <remarks>
        /// This check ensures no part of the interface was implemented explicitly.
        /// </remarks>
        /// <param name="authenticationServiceDescription">The domain service description for the type that implemented 
        /// the <see cref="IAuthentication{T}"/> interface.
        /// </param>
        /// <param name="genericIAuthenticationType">The generic version of <see cref="IAuthentication{T}"/> implemented
        /// by the service type of the <paramref name="authenticationServiceDescription"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"> is thrown if the <see cref="IAuthentication{T}"/> interface
        /// is not correctly implemented.
        /// </exception>
        private static void CheckIAuthentication(DomainServiceDescription authenticationServiceDescription, out Type genericIAuthenticationType)
        {
            bool implementsLogin = false;
            bool implementsLogout = false;
            bool implementsGetUser = false;
            bool implementsUpdateUser = false;

            if (!typeof(IAuthentication<>).DefinitionIsAssignableFrom(authenticationServiceDescription.DomainServiceType, out genericIAuthenticationType))
            {
                throw new InvalidOperationException(Resources.ApplicationServices_MustBeIAuth);
            }

            Type userType = genericIAuthenticationType.GetGenericArguments()[0];

            foreach (DomainOperationEntry doe in authenticationServiceDescription.DomainOperationEntries)
            {
                switch (doe.Name)
                {
                    case "Login":
                        implementsLogin = CheckIAuthenticationLogin(doe, userType);
                        break;
                    case "Logout":
                        implementsLogout = CheckIAuthenticationLogout(doe, userType);
                        break;
                    case "GetUser":
                        implementsGetUser = CheckIAuthenticationGetUser(doe, userType);
                        break;
                    case "UpdateUser":
                        implementsUpdateUser = CheckIAuthenticationUpdateUser(doe, userType);
                        break;
                    default:
                        break;
                }
            }

            if (!implementsLogin || !implementsLogout || !implementsGetUser || !implementsUpdateUser)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.InstalledUICulture,
                    Resources.ApplicationServices_MustBeIAuthImpl,
                    authenticationServiceDescription.DomainServiceType.Name));
            }
        }

        /// <summary>
        /// Validates that the operation entry represents <see cref="IAuthentication{T}.Login"/> for use in codegen.
        /// </summary>
        /// <param name="doe">The entry to validate</param>
        /// <param name="userType">The user type. <c>T</c> in <see cref="IAuthentication{T}"/>.</param>
        /// <returns>Whether the operation entry represents Login</returns>
        private static bool CheckIAuthenticationLogin(DomainOperationEntry doe, Type userType)
        {
            bool implementsLogin = true;

            // [Query]
            // public T Login(string userName, string password, bool isPersistent, string customData)
            if (doe.Operation != DomainOperation.Query)
            {
                implementsLogin = false;
            }
            if (doe.ReturnType != userType)
            {
                implementsLogin = false;
            }
            if (doe.Parameters.Count() != 4 ||
                doe.Parameters[0].ParameterType != typeof(string) ||
                doe.Parameters[1].ParameterType != typeof(string) ||
                doe.Parameters[2].ParameterType != typeof(bool) ||
                doe.Parameters[3].ParameterType != typeof(string))
            {
                implementsLogin = false;
            }

            return implementsLogin;
        }

        /// <summary>
        /// Validates that the operation entry represents <see cref="IAuthentication{T}.Logout"/> for use in codegen.
        /// </summary>
        /// <param name="doe">The entry to validate</param>
        /// <param name="userType">The user type. <c>T</c> in <see cref="IAuthentication{T}"/>.</param>
        /// <returns>Whether the operation entry represents Logout</returns>
        private static bool CheckIAuthenticationLogout(DomainOperationEntry doe, Type userType)
        {
            bool implementsLogout = true;

            // [Query]
            // public T Logout()
            if (doe.Operation != DomainOperation.Query)
            {
                implementsLogout = false;
            }
            if (doe.ReturnType != userType)
            {
                implementsLogout = false;
            }
            if (doe.Parameters.Any())
            {
                implementsLogout = false;
            }

            return implementsLogout;
        }

        /// <summary>
        /// Validates that the operation entry represents <see cref="IAuthentication{T}.GetUser"/> for use in codegen.
        /// </summary>
        /// <param name="doe">The entry to validate</param>
        /// <param name="userType">The user type. <c>T</c> in <see cref="IAuthentication{T}"/>.</param>
        /// <returns>Whether the operation entry represents GetUser</returns>
        private static bool CheckIAuthenticationGetUser(DomainOperationEntry doe, Type userType)
        {
            bool implementsGetUser = true;

            // [Query]
            // public T GetUser()
            if (doe.Operation != DomainOperation.Query)
            {
                implementsGetUser = false;
            }
            if (doe.ReturnType != userType)
            {
                implementsGetUser = false;
            }
            if (doe.Parameters.Any())
            {
                implementsGetUser = false;
            }

            return implementsGetUser;
        }

        /// <summary>
        /// Validates that the operation entry represents <see cref="IAuthentication{T}.UpdateUser"/> for use in codegen.
        /// </summary>
        /// <param name="doe">The entry to validate</param>
        /// <param name="userType">The user type. <c>T</c> in <see cref="IAuthentication{T}"/>.</param>
        /// <returns>Whether the operation entry represents UpdateUser</returns>
        private static bool CheckIAuthenticationUpdateUser(DomainOperationEntry doe, Type userType)
        {
            bool implementsUpdateUser = true;

            // [Update]
            // public void UpdateUser(T user)
            if (doe.Operation != DomainOperation.Update)
            {
                implementsUpdateUser = false;
            }
            if (doe.ReturnType != typeof(void))
            {
                implementsUpdateUser = false;
            }
            if (doe.Parameters.Count() != 1 ||
                doe.Parameters[0].ParameterType != userType)
            {
                implementsUpdateUser = false;
            }

            return implementsUpdateUser;
        }

        /// <summary>
        /// Validates that the user type implements the <see cref="IUser"/> interface naturally for use
        /// in codegen.
        /// </summary>
        /// <remarks>
        /// This check ensures no part of the interface was implemented explicitly and the <c>Name</c>
        /// property was marked as a <c>[Key]</c>.
        /// </remarks>
        /// <param name="user">The type that implemented the <see cref="IUser"/> interface.</param>
        /// <exception cref="InvalidOperationException"> is thrown if the <see cref="IUser"/> interface
        /// is not correctly implemented.
        /// </exception>
        private static void CheckIUser(Type user)
        {
            bool implementsName = false;
            bool implementsRoles = false;

            System.Diagnostics.Debug.Assert(typeof(IUser).IsAssignableFrom(user),
                "user should always be of type IUser.");

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(user))
            {
                switch (property.Name)
                {
                    case "Name":
                        {
                            // [Key]
                            // public string Name { get; set; }
                            if (typeof(string) != property.PropertyType)
                            {
                                break;
                            }
                            if (!SerializationUtility.IsSerializableDataMember(property))
                            {
                                throw new InvalidOperationException(string.Format(
                                    CultureInfo.InstalledUICulture,
                                    Resources.ApplicationServices_MustBeSerializable,
                                    property.Name, user.Name));
                            }
                            if (property.Attributes[typeof(KeyAttribute)] == null)
                            {
                                throw new InvalidOperationException(string.Format(
                                    CultureInfo.InstalledUICulture,
                                    Resources.ApplicationServices_NameMustBeAKey,
                                    user.Name));
                            }
                            PropertyInfo namePropertyInfo = user.GetProperty("Name");
                            if (namePropertyInfo != null && namePropertyInfo.GetSetMethod() == null)
                            {
                                break;
                            }
                            implementsName = true;
                            break;
                        }
                    case "Roles":
                        {
                            // public IEnumerable<string> Roles { get; set; }
                            if (!typeof(IEnumerable<string>).IsAssignableFrom(property.PropertyType))
                            {
                                break;
                            }
                            if (!SerializationUtility.IsSerializableDataMember(property))
                            {
                                throw new InvalidOperationException(string.Format(
                                    CultureInfo.InstalledUICulture,
                                    Resources.ApplicationServices_MustBeSerializable,
                                    property.Name, user.Name));
                            }
                            PropertyInfo rolesPropertyInfo = user.GetProperty("Roles");
                            if (rolesPropertyInfo != null && rolesPropertyInfo.GetSetMethod() == null)
                            {
                                break;
                            }
                            implementsRoles = true;
                            break;
                        }
                    default:
                        break;
                }
            }

            if (!implementsName || !implementsRoles)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.InstalledUICulture,
                    Resources.ApplicationServices_MustBeIUser,
                    user.Name));
            }
        }

        #endregion
    }
}
