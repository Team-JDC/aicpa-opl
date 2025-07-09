#region

using System;

#endregion

namespace AICPA.Destroyer.Shared
{
    /// <summary>
    ///   Summary description for D_Exception.
    /// </summary>
    public class D_Exception : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D_Exception" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public D_Exception()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D_Exception" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public D_Exception(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D_Exception" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public D_Exception(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class SecurityException : D_Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public SecurityException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public SecurityException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public SecurityException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class UserNotAuthenticated : SecurityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotAuthenticated" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public UserNotAuthenticated()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotAuthenticated" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public UserNotAuthenticated(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotAuthenticated" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public UserNotAuthenticated(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class DocumentNotAuthorized : SecurityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotAuthorized" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public DocumentNotAuthorized()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotAuthorized" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public DocumentNotAuthorized(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotAuthorized" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public DocumentNotAuthorized(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class DatabaseException : D_Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public DatabaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public DatabaseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public DatabaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class BusinessRuleException : D_Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public BusinessRuleException() : base("A business rule or constraint was violated")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public BusinessRuleException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public BusinessRuleException(string message, Exception inner) :
            base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class SubscriptionException : SecurityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionException" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public SubscriptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public SubscriptionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public SubscriptionException(string message, Exception inner) :
            base(message, inner)
        {
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public class UserCapExceededException : SubscriptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCapExceededException" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public UserCapExceededException()
            : base("The subscription was not allowed because the user cap would be exceeded.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCapExceededException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public UserCapExceededException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCapExceededException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public UserCapExceededException(string message, Exception inner) :
            base(message, inner)
        {
        }
    }
}