using System.IO;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;

namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>Provides a file-based persistent store. </summary>
    public class FilePersistenceStrategy : IPersistenceStrategy
    {
        private string file;

        private static Logger log = LoggerFactory.GetLogger(typeof(FilePersistenceStrategy));

        /// @param file the file to use for storage. If the file does not exist it will be created when required.
        public FilePersistenceStrategy(string file)
        {
            this.file = file; // Preconditions.checkNotNull(file);
        }

        public Factorization Load()
        {
            if (!File.Exists(file))
            {
                log.Info("{0} does not yet exist, no factorization found", file);
                return null;
            }
            Stream inFile = null;
            try
            {
                log.Info("Reading factorization from {0}...", file);
                inFile = new FileStream(file, FileMode.Open, FileAccess.Read);
                return ReadBinary(inFile);
            }
            finally
            {
                inFile.Close();
            }
        }

        public void MaybePersist(Factorization factorization)
        {
            Stream outFile = null;
            try
            {
                log.Info("Writing factorization to {0}...", file);
                outFile = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
                WriteBinary(factorization, outFile);
            }
            finally
            {
                outFile.Close();
            }
        }

        protected static void WriteBinary(Factorization factorization, Stream outFile)
        {
            var binWr = new BinaryWriter(outFile);
            binWr.Write(factorization.NumFeatures());
            binWr.Write(factorization.NumUsers());
            binWr.Write(factorization.NumItems());

            foreach (var mappingEntry in factorization.GetUserIDMappings())
            {
                if (!mappingEntry.Value.HasValue)
                    continue; //?correct?

                long userID = mappingEntry.Key;
                binWr.Write(mappingEntry.Value.Value);
                binWr.Write(userID);
                try
                {
                    double[] userFeatures = factorization.GetUserFeatures(userID);
                    for (int feature = 0; feature < factorization.NumFeatures(); feature++)
                    {
                        binWr.Write(userFeatures[feature]);
                    }
                }
                catch (NoSuchUserException e)
                {
                    throw new IOException("Unable to persist factorization", e);
                }
            }

            foreach (var entry in factorization.GetItemIDMappings())
            {
                if (!entry.Value.HasValue)
                    continue; //?correct?

                long itemID = entry.Key;
                binWr.Write(entry.Value.Value);
                binWr.Write(itemID);
                try
                {
                    double[] itemFeatures = factorization.GetItemFeatures(itemID);
                    for (int feature = 0; feature < factorization.NumFeatures(); feature++)
                    {
                        binWr.Write(itemFeatures[feature]);
                    }
                }
                catch (NoSuchItemException e)
                {
                    throw new IOException("Unable to persist factorization", e);
                }
            }
        }

        public static Factorization ReadBinary(Stream inFile)
        {
            var binRdr = new BinaryReader(inFile);

            int numFeatures = binRdr.ReadInt32();
            int numUsers = binRdr.ReadInt32();
            int numItems = binRdr.ReadInt32();

            FastByIDMap<int?> userIDMapping = new FastByIDMap<int?>(numUsers);
            double[][] userFeatures = new double[numUsers][];

            for (int n = 0; n < numUsers; n++)
            {
                int userIndex = binRdr.ReadInt32();
                long userID = binRdr.ReadInt64();

                userFeatures[userIndex] = new double[numFeatures];

                userIDMapping.Put(userID, userIndex);
                for (int feature = 0; feature < numFeatures; feature++)
                {
                    userFeatures[userIndex][feature] = binRdr.ReadDouble();
                }
            }

            FastByIDMap<int?> itemIDMapping = new FastByIDMap<int?>(numItems);
            double[][] itemFeatures = new double[numItems][];

            for (int n = 0; n < numItems; n++)
            {
                int itemIndex = binRdr.ReadInt32();
                long itemID = binRdr.ReadInt64();

                itemFeatures[itemIndex] = new double[numFeatures];

                itemIDMapping.Put(itemID, itemIndex);
                for (int feature = 0; feature < numFeatures; feature++)
                {
                    itemFeatures[itemIndex][feature] = binRdr.ReadDouble();
                }
            }

            return new Factorization(userIDMapping, itemIDMapping, userFeatures, itemFeatures);
        }
    }
}