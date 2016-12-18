using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CQSS.Mongo.Client
{
    public class CQSSMongoClient : CQSSMongoClientBase
    {
        protected DateTime BeginTime { get; set; }

        public CQSSMongoClient(string server, int port, string database)
            : base(server, port, database)
        {
        }

        public override OperateResult InsertOne<TDocument>(string collectionName, TDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            var documents = new List<TDocument>() { document };

            var result = this.InsertMany<TDocument>(collectionName, documents);

            return result;
        }

        public override OperateResult InsertMany<TDocument>(string collectionName, IEnumerable<TDocument> documents)
        {
            this.BeginTime = DateTime.Now;
            OperateResult insertResult = null;

            try
            {
                var collection = base.GetCollection<TDocument>(collectionName);

                if (documents == null || documents.Count() == 0)
                    throw new ArgumentNullException("documents");

                var models = documents.Select(d => new InsertOneModel<TDocument>(d));

                var writeResult = base.BulkWrite<TDocument>(collection, models);

                var interval = (DateTime.Now - this.BeginTime).TotalMilliseconds;

                if (writeResult.IsAcknowledged)
                    insertResult = new OperateResult() { AffectCount = writeResult.InsertedCount, Interval = interval, Status = OperateStatus.Success, Message = "success" };
            }
            catch (Exception ex)
            {
                insertResult = new OperateResult() { AffectCount = 0, Interval = 0, Status = OperateStatus.Fail, Message = ex.StackTrace };
            }

            return insertResult;
        }

        public override OperateResult Delete<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, bool deleteMany = false)
        {
            this.BeginTime = DateTime.Now;
            OperateResult deleteResult = null;

            try
            {
                var collection = base.GetCollection<TDocument>(collectionName);

                DeleteResult tmpResult = null;
                if (deleteMany)
                    tmpResult = collection.DeleteMany(filter);
                else
                    tmpResult = collection.DeleteOne(filter);

                var interval = (DateTime.Now - this.BeginTime).TotalMilliseconds;

                if (tmpResult.IsAcknowledged)
                    deleteResult = new OperateResult() { AffectCount = tmpResult.DeletedCount, Interval = interval, Status = OperateStatus.Success, Message = "success" };
            }
            catch (Exception ex)
            {
                deleteResult = new OperateResult() { AffectCount = 0, Interval = 0, Status = OperateStatus.Fail, Message = ex.StackTrace };
            }

            return deleteResult;
        }

        public override OperateResult Update<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, TDocument document)
        {
            this.BeginTime = DateTime.Now;
            OperateResult updateResult = null;

            try
            {
                var collection = base.GetCollection<TDocument>(collectionName);

                var bsonDocument = document.ToBsonDocument<TDocument>();

                var updateDocument = new UpdateDocument { { "$set", bsonDocument } };

                var result = collection.UpdateOne<TDocument>(filter, updateDocument);

                var interval = (DateTime.Now - this.BeginTime).TotalMilliseconds;

                if (result.IsAcknowledged)
                    updateResult = new OperateResult() { AffectCount = result.ModifiedCount, Interval = interval, Status = OperateStatus.Success, Message = "success" };
            }
            catch (Exception ex)
            {
                updateResult = new OperateResult() { AffectCount = 0, Interval = 0, Status = OperateStatus.Fail, Message = ex.StackTrace };
            }

            return updateResult;
        }

        public override OperateResult UpdatePartial<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, string partialDocument)
        {
            this.BeginTime = DateTime.Now;
            OperateResult updateResult = null;

            try
            {
                if (string.IsNullOrEmpty(partialDocument))
                    throw new ArgumentNullException("partialDocument");

                var collection = base.GetCollection<TDocument>(collectionName);

                var updateVlues = BsonDocument.Parse(partialDocument);

                var result = collection.UpdateOne(filter, updateVlues);

                var interval = (DateTime.Now - this.BeginTime).TotalMilliseconds;

                if (result.IsAcknowledged)
                    updateResult = new OperateResult() { AffectCount = result.ModifiedCount, Interval = interval, Status = OperateStatus.Success, Message = "success" };
            }
            catch (Exception ex)
            {
                updateResult = new OperateResult() { AffectCount = 0, Interval = 0, Status = OperateStatus.Fail, Message = ex.StackTrace };
            }

            return updateResult;
        }

        public override FindResult<ToutDocument> Find<TinDocument, ToutDocument>(string collectionName, Func<TinDocument, bool> filter, Func<TinDocument, ToutDocument> selectField)
        {
            this.BeginTime = DateTime.Now;
            FindResult<ToutDocument> findResult = null;

            try
            {
                var collection = base.GetCollection<TinDocument>(collectionName);

                var documents = collection.AsQueryable<TinDocument>().Where(filter).Select(selectField).ToList();

                var interval = (DateTime.Now - this.BeginTime).TotalMilliseconds;

                findResult = new FindResult<ToutDocument>() { Interval = interval, Status = OperateStatus.Success, Message = "success", Documents = documents };
            }
            catch (Exception ex)
            {
                findResult = new FindResult<ToutDocument>() { Interval = 0, Status = OperateStatus.Fail, Message = ex.StackTrace, Documents = Enumerable.Empty<ToutDocument>() };
            }

            return findResult;
        }

        public override IAsyncCursor<TDocument> FindSync<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, int batchSize = 100)
        {
            try
            {
                var collection = base.GetCollection<TDocument>(collectionName);

                var options = new FindOptions<TDocument>() { BatchSize = batchSize };

                var cursor = collection.FindSync<TDocument>(filter, options);

                return cursor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}