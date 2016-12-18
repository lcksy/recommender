using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using MongoDB.Driver;

namespace CQSS.Mongo.Client
{
    public interface ICQSSMongoClient
    {
        OperateResult InsertOne<TDocument>(string collectionName, TDocument document);
        OperateResult InsertMany<TDocument>(string collectionName, IEnumerable<TDocument> documents);
        OperateResult Delete<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, bool deleteMany = false);
        OperateResult Update<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, TDocument document);
        OperateResult UpdatePartial<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, string partialDocument);
        FindResult<ToutDocument> Find<TinDocument, ToutDocument>(string collectionName, Func<TinDocument, bool> filter, Func<TinDocument, ToutDocument> selectField);
        IAsyncCursor<TDocument> FindSync<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, int batchSize = 100);
    }
}