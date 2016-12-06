using NReco.CF;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.Recommender.Test.VS.taste.impl
{
    public abstract class TasteTestCase /*: MahoutTestCase*/
    {
        public const double EPSILON = 0.000001;

        public TasteTestCase()
        {
            RandomUtils.useTestSeed();
        }
        //[SetUp]
        //public virtual void SetUp()
        //{
        //    RandomUtils.useTestSeed();
        //}

        public static IDataModel getDataModel(long[] userIDs, double?[][] prefValues)
        {
            FastByIDMap<IPreferenceArray> result = new FastByIDMap<IPreferenceArray>();
            for (int i = 0; i < userIDs.Length; i++)
            {
                List<IPreference> prefsList = new List<IPreference>();
                for (int j = 0; j < prefValues[i].Length; j++)
                {
                    if (prefValues[i][j].HasValue)
                    {
                        prefsList.Add(new GenericPreference(userIDs[i], j, (float)prefValues[i][j].Value));
                    }
                }
                if (prefsList.Count > 0)
                {
                    result.Put(userIDs[i], new GenericUserPreferenceArray(prefsList));
                }
            }
            return new GenericDataModel(result);
        }

        public static IDataModel getBooleanDataModel(long[] userIDs, bool[][] prefs)
        {
            FastByIDMap<FastIDSet> result = new FastByIDMap<FastIDSet>();
            for (int i = 0; i < userIDs.Length; i++)
            {
                FastIDSet prefsSet = new FastIDSet();
                for (int j = 0; j < prefs[i].Length; j++)
                {
                    if (prefs[i][j])
                    {
                        prefsSet.Add(j);
                    }
                }
                if (!prefsSet.IsEmpty())
                {
                    result.Put(userIDs[i], prefsSet);
                }
            }
            return new GenericBooleanPrefDataModel(result);
        }

        /*
         *        item1    item2   item3   item4
         * user1   0.1      0.3      x       x
         * user2   0.2      0.3     0.3      x
         * user3   0.4      0.3     0.5      x
         * user4   0.7      0.3     0.8      x
         * 
         */
        protected static IDataModel getDataModel()
        {
            return getDataModel(
                new long[] { 1, 2, 3, 4 },
                new double?[][] {
                    new double?[] {0.1, 0.3},
                    new double?[] {0.2, 0.3, 0.3},
                    new double?[] {0.4, 0.3, 0.5},
                    new double?[] {0.7, 0.3, 0.8}
                }
            );
        }

        protected static IDataModel getBooleanDataModel()
        {
            return getBooleanDataModel(new long[] { 1, 2, 3, 4 },
                                       new bool[][] {
                                           new[]{false, true,  false},
                                           new[]{false, true,  true,  false},
                                           new[]{true,  false, false, true},
                                           new[]{true,  false, true,  true}
                                       });
        }

        protected static bool arrayContains(long[] array, long value)
        {
            foreach (long l in array)
            {
                if (l == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}