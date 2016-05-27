using System;
using System.Collections.Generic;
using Sharper.C.Data;

namespace Sharper.C.Control.Optics
{
    public interface MaybeTraversal<S, T, A, B>
    {
        Func<S, Maybe<T>> Traverse(Func<A, Maybe<B>> f);
    }

    public static class Traversal
    {
        public static Func<S, Maybe<T>> Sequence<S, T, B>
          ( this MaybeTraversal<S, T, Maybe<B>, B> t
          , Func<Maybe<B>, Maybe<B>> f
          )
        =>  t.Traverse(f);

        public interface SeqTraversal<S, T, A, B>
        {
            Func<S, IEnumerable<T>> Traverse(Func<A, IEnumerable<B>> f);
        }

        public static Func<S, IEnumerable<T>> Sequence<S, T, B>
          ( this SeqTraversal<S, T, IEnumerable<B>, B> t
          , Func<IEnumerable<B>, IEnumerable<B>> f
          )
        =>  t.Traverse(f);
    }
}
