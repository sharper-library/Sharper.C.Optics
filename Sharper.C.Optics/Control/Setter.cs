using System;

namespace Sharper.C.Control.Optics
{
    public interface Setter<S, T, A, B>
    {
        Func<S, T> Update(Func<A, B> f);
    }

    public interface Setter<S, A>
      : Setter<S, S, A, A>
    {
    }

    public static class Setter
    {
        public static Func<S, T> Set<S, T, A, B>(this Setter<S, T, A, B> x, B b)
        =>  x.Update(_ => b);

        public static Func<S, S> Set<S, A>(this Setter<S, A> x, A a)
        =>  x.Update(_ => a);

        public static Setter<S, T, X, Y> Then<S, T, A, B, X, Y>
        ( this Setter<S, T, A, B> s0
        , Setter<A, B, X, Y> s1
        )
        =>  new ASetter<S, T, X, Y>(f => s0.Update(s1.Update(f)));

        public static Setter<S, T, A, B> Mk<S, T, A, B>
          ( Func<Func<A, B>, Func<S, T>> update
          )
        => new ASetter<S, T, A, B>(update);

        public static Setter<S, A> Mk<S, A>
          ( Func<Func<A, A>, Func<S, S>> update
          )
        => new ASetter<S, A>(update);

        private struct ASetter<S, T, A, B>
          : Setter<S, T, A, B>
        {
            private readonly Func<Func<A, B>, Func<S, T>> update;

            internal ASetter(Func<Func<A, B>, Func<S, T>> update)
            {
               this.update = update;
            }

            public Func<S, T> Update(Func<A, B> f)
            =>  update(f);
        }

        private struct ASetter<S, A>
          : Setter<S, A>
        {
            private readonly Func<Func<A, A>, Func<S, S>> update;

            internal ASetter(Func<Func<A, A>, Func<S, S>> update)
            {   this.update = update;
            }

            public Func<S, S> Update(Func<A, A> f)
            =>   update(f);
        }
    }
}