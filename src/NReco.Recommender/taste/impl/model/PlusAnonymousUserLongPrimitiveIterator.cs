using System.Collections;
using System.Collections.Generic;

namespace NReco.CF.Taste.Impl.Model
{
    public sealed class PlusAnonymousUserlongPrimitiveIterator : IEnumerator<long>
    {
        private IEnumerator<long> enumerator;
        private long extraDatum;
        private bool datumConsumed;
        private bool currentDatum = false;
        private bool prevMoveNext = false;

        public PlusAnonymousUserlongPrimitiveIterator(IEnumerator<long> enumerator, long extraDatum)
        {
            this.enumerator = enumerator;
            this.extraDatum = extraDatum;
            datumConsumed = false;
        }

        /*public void skip(int n) {
          for (int i = 0; i < n; i++) {
            nextlong();
          }
        }*/

        public long Current
        {
            get
            {
                return currentDatum ? extraDatum : enumerator.Current;
            }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (currentDatum)
            {
                currentDatum = false;
                return prevMoveNext;
            }

            prevMoveNext = enumerator.MoveNext();

            if (prevMoveNext && !datumConsumed && extraDatum <= Current)
            {
                datumConsumed = true;
                currentDatum = true;
                return true;
            }

            if (!prevMoveNext && !datumConsumed)
            {
                datumConsumed = true;
                currentDatum = true;
                return true;
            }
            return prevMoveNext;
        }

        public void Reset()
        {
            datumConsumed = false;
            enumerator.Reset();
        }
    }
}