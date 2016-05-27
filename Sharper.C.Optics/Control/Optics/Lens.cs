using System;

namespace Sharper.C.Control.Optics
{
    public interface Lens<S, T, A, B>
      : Getter<S, A>
      , Setter<S, T, A, B>
    {
    }

    public interface Lens<S, A>
      : Lens<S, S, A, A>
      , Setter<S, A>
    {
    }

    public static class Lens
    {
        public static Lens<S, A> ToSimple<S, A>(this Lens<S, S, A, A> l)
        =>  Mk_<S, A>(l.View, l.Update);

        public static Lens<S, T, X, Y> CompLL<S, T, A, B, X, Y>
          ( this Lens<S, T, A, B> l0
          , Lens<A, B, X, Y> l1
          )
        =>  Mk<S, T, X, Y>
              ( s => l1.View(l0.View(s))
              , f => l0.Update(l1.Update(f))
              );

        public static Lens<S, X> CompLL_<S, A, X>
          ( this Lens<S, A> l0
          , Lens<A, X> l1
          )
        =>  l0.CompLL(l1).ToSimple();

        public static Lens<S, T, X, Y> CompIL<S, T, A, B, X, Y>
          ( this Iso<S, T, A, B> i
          , Lens<A, B, X, Y> l
          )
        =>  Mk<S, T, X, Y>
              ( s => l.View(i.View(s))
              , xy => s => i.Back(l.Update(xy)(i.There(s)))
              );

        public static Lens<S, X> CompIL_<S, A, X>
          ( this Iso<S, A> i
          , Lens<A, X> l
          )
        => i.CompIL(l).ToSimple();

        public static Lens<S, T, A, B> Mk<S, T, A, B>
          ( Func<S, A> view
          , Func<Func<A, B>, Func<S, T>> update
          )
        =>  new ALens<S, T, A, B>(view, update);

        public static Lens<S, A> Mk_<S, A>
          ( Func<S, A> view
          , Func<Func<A, A>, Func<S, S>> update
          )
        =>  new ALens<S, A>(view, update);

        private struct ALens<S, T, A, B>
          : Lens<S, T, A, B>
        {
            private readonly Func<S, A> view;
            private readonly Func<Func<A, B>, Func<S, T>> update;

            internal ALens
              ( Func<S, A> view
              , Func<Func<A, B>, Func<S, T>> update
              )
            {   this.view = view;
                this.update = update;
            }

            public A View(S s)
            =>  view(s);

            public Func<S, T> Update(Func<A, B> f)
            =>  update(f);
        }

        private struct ALens<S, A>
          : Lens<S, A>
        {
            private readonly Func<S, A> view;
            private readonly Func<Func<A, A>, Func<S, S>> update;

            internal ALens
              ( Func<S, A> view
              , Func<Func<A, A>, Func<S, S>> update
              )
            {   this.view = view;
                this.update = update;
            }

            public A View(S s)
            =>  view(s);

            public Func<S, S> Update(Func<A, A> f)
            =>  update(f);
        }
    }

}
