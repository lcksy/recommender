using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQSS.Mongo.Client
{
    public abstract class CQSSMongoClientBase : ICQSSMongoClient
    {
        protected IMongoClient client = null;
        protected IMongoDatabase database = null;

        protected CQSSMongoClientBase(string server, int port, string database)
        {
            this.InnerConnect(server, port, database);
        }

        protected virtual void InnerConnect(string server, int port, string database)
        {
            var settings = this.CreateMongoClientSettings(server, port);

            this.client = new MongoClient(settings);

            this.database = this.client.GetDatabase(database);
        }

        protected virtual MongoClientSettings CreateMongoClientSettings(string server, int port)
        {
            var setting = new MongoClientSettings();
            setting.ConnectionMode = ConnectionMode.Standalone;
            setting.ConnectTimeout = TimeSpan.FromSeconds(1);
            setting.Server = new MongoServerAddress(server, port);
            setting.WriteEncoding = new UTF8Encoding();

            return setting;
        }

        protected virtual IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            var collection = database.GetCollection<TDocument>(collectionName);

            return collection;
        }

        protected virtual BulkWriteResult<TDocument> BulkWrite<TDocument>(IMongoCollection<TDocument> collection, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null)
        {
            var result = collection.BulkWrite(requests, options);

            return result;
        }

        public virtual string Serializer<TDocument>(TDocument document)
        {
            var encoding = new UTF8Encoding(true, true); // emit UTF8 identifier

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, encoding))
                using (var streamReader = new StreamReader(memoryStream))
                using (var jsonWriter = new JsonWriter(streamWriter, JsonWriterSettings.Defaults))
                {
                    BsonSerializer.Serialize(jsonWriter, document);
                }
                var bytes = memoryStream.ToArray();

                return encoding.GetString(bytes);
            }
        }

        public virtual OperateResult Delete<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, bool deleteMany = false)
        {
            throw new NotImplementedException();
        }

        public virtual FindResult<ToutDocument> Find<TinDocument, ToutDocument>(string collectionName, Func<TinDocument, bool> filter, Func<TinDocument, ToutDocument> selectField)
        {
            throw new NotImplementedException();
        }

        public virtual OperateResult InsertMany<TDocument>(string collectionName, IEnumerable<TDocument> documents)
        {
            throw new NotImplementedException();
        }

        public virtual OperateResult InsertOne<TDocument>(string collectionName, TDocument document)
        {
            throw new NotImplementedException();
        }

        public virtual OperateResult Update<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, TDocument document)
        {
            throw new NotImplementedException();
        }

        public virtual OperateResult UpdatePartial<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, string partialDocument)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncCursor<TDocument> FindSync<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, int batchSize = 100)
        {
            throw new NotImplementedException();
        }
    }
}