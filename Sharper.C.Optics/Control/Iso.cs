using System;

namespace Sharper.C.Control.Optics
{
    public interface Iso<S, T, A, B>
      : Getter<S, A>
    {
        A There(S s);
        T Back(B b);
        Iso<B, A, T, S> From { get; }
    }

    public interface Iso<S, A>
      : Iso<S, S, A, A>
    {
        new Iso<A, S> From { get; }
    }

    public static class Iso
    {
        public static Func<S, T> SwapThere<S, T, A, B>
          ( this Iso<S, T, A, B> i
          , Func<A, B> f
          )
        =>  s => i.Back(f(i.There(s)));

        public static Func<B, A> SwapBack<S, T, A, B>
          ( this Iso<S, T, A, B> i
          , Func<T, S> f
          )
        =>  b => i.There(f(i.Back(b)));

        public static Iso<S, T, A, B> Mk<S, T, A, B>
          ( Func<S, A> there
          , Func<B, T> back
          )
        =>  new AnIso<S, T, A, B>(there, back);

        public static Iso<S, A> Mk_<S, A>(Func<S, A> there, Func<A, S> back)
        =>  new AnIso<S, A>(there, back);

        private struct AnIso<S, T, A, B>
          : Iso<S, T, A, B>
        {
            private readonly Func<S, A> there;
            private readonly Func<B, T> back;

            internal AnIso(Func<S, A> there, Func<B, T> back)
            {
                this.there = there;
                this.back = back;
            }

            public A There(S s)
            =>  there(s);

            public T Back(B b)
            =>  back(b);

            public A View(S s)
            =>  there(s);

            public Iso<B, A, T, S> From
            =>  new AnIso<B, A, T, S>(Back, There);
        }

        private struct AnIso<S, A>
          : Iso<S, A>
        {
            private readonly Func<S, A> there;
            private readonly Func<A, S> back;

            internal AnIso(Func<S, A> there, Func<A, S> back)
            {
                this.there = there;
                this.back = back;
            }

            public A There(S s)
            =>  there(s);

            public S Back(A b)
            =>  back(b);

            public A View(S s)
            =>  there(s);

            Iso<A, A, S, S> Iso<S, S, A, A>.From
            =>  new AnIso<A, A, S, S>(back, there);

            public Iso<A, S> From
            => new AnIso<A, S>(back, there);
        }
    }
}
