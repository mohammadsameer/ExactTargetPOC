﻿using System.Linq;
using DataExtractor.Core.Configuration;
using DataExtractor.ETService;
using System;

namespace DataExtractor.Core.RequestClients.Shared
{
    public class SharedCoreRequestClient : ISharedCoreRequestClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public SharedCoreRequestClient(IExactTargetConfiguration config)
        {
            _client = SoapClientFactory.Manufacture(config);
            _config = config;
        }

        public bool DoesObjectExist(string propertyName, string value, string objectType)
        {
            string[] Properties;
            if (objectType != "Email")
                Properties = new[] { "Name", "ObjectID", "CustomerKey" };
            else
                Properties = new[] { "ID", "Name" };
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = objectType,
                Properties = Properties,

                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            return results != null && results.Any();
        }

        public string RetrieveObjectId(string propertyName, string value, string objectType)
        {
            string[] Properties;
            if (objectType != "Email")
                Properties = new[] { "Name", "ObjectID", "CustomerKey" };
            else
                Properties = new[] { "ID", "Name" };

            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = objectType,
                Properties = Properties,
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            if (results != null && results.Any())
            {
                if (objectType == "Email")
                    return Convert.ToString( results.First().ID);

                return results.First().ObjectID;
            }

            return string.Empty;
        }

        public T RetrieveObject<T>(string propertyName, string value, string objectType)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = objectType,
                Properties = new[] { "Name", "ObjectID", "CustomerKey" },
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            if (results != null && results.Any())
            {
                return (T)(object)results.First();
            }
            return default(T);
        }
    }
}
