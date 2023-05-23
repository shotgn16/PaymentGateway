using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.exceptions
{
    public class DnsException : Exception
    {
        public DnsException() : base() { }
        public DnsException(string message) : base(message) { }
        public DnsException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ActiveDirectoryException : Exception
    {
        public ActiveDirectoryException() : base() { }
        public ActiveDirectoryException(string message) : base(message) { }
        public ActiveDirectoryException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CertificateException : Exception
    {
        public CertificateException() : base() { }
        public CertificateException(string message) : base(message) { }
        public CertificateException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidDatabaseConnection : Exception
    {
        public InvalidDatabaseConnection() : base() { }
        public InvalidDatabaseConnection(string message) : base(message) { }
        public InvalidDatabaseConnection(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseBuildException : Exception
    {
        public DatabaseBuildException() : base() { }
        public DatabaseBuildException(string message) : base(message) { }
        public DatabaseBuildException(string message, Exception innerException) : base(message, innerException) { }
    }
    public class QueryException : Exception
    {
        public QueryException() : base() { }
        public QueryException(string message) : base(message) { }
        public QueryException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class OperationException : Exception
    {
        public OperationException() : base() { }
        public OperationException(string message) : base(message) { }
        public OperationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ApplicationConfiguration : Exception
    {
        public ApplicationConfiguration() : base() { }
        public ApplicationConfiguration(string message) : base(message) { }
        public ApplicationConfiguration(string message, Exception innerException) : base(message, innerException) { }
    }

    public class WhitelistException : Exception
    {
        public WhitelistException() : base() { }
        public WhitelistException(string message) : base(message) { }

        public WhitelistException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DeserializationException : Exception
    {
        public DeserializationException() : base() { }
        public DeserializationException(string message) : base(message) { }
        public DeserializationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class RequestException : Exception
    {
        public RequestException(Flurl.Http.FlurlHttpException ex) : base() { }
        public RequestException(string message) : base(message) { }
        public RequestException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DataValidationException : Exception
    {
        public DataValidationException() : base() { }
        public DataValidationException(string message) : base(message) { }
        public DataValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class RechargeException : Exception
    {
        public RechargeException() : base() { }
        public RechargeException(string message) : base(message) { }
        public RechargeException(string message, Exception innerException) : base(message, innerException) { }
    }
}