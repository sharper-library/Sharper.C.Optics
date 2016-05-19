using System;

namespace Sharper.C.Control.Optics
{
    public interface Getter<S, A>
    {
        A View(S s);
    }

    public static class Getter
    {
        public static Getter<S, A> Mk<S, A>(Func<S, A> view)
        => new AGetter<S, A>(view);

        public static Getter<S, X> Then<S, A, X>
          ( this Getter<S, A> g0
          , Getter<A, X> g1
          )
        =>  Getter.Mk<S, X>(s => g1.View(g0.View(s)));

        private struct AGetter<S, A>
        : Getter<S, A>
        {
            private readonly Func<S, A> view;

            internal AGetter(Func<S, A> view)
            {
                this.view = view;
            }

            public A View(S s)
            =>  view(s);
        }
    }
}