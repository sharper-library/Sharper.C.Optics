using System;
using Sharper.C.Data;

namespace Sharper.C.Control.Optics
{
    public interface Prism<S, T, A, B>
      : Setter<S, T, A, B>
    {
        Or<T, A> Get(S s);
        Maybe<A> Preview(S s);
        Getter<B, T> Re { get; }
    }

    public interface Prism<S, A>
      : Prism<S, S, A, A>
    {
    }

    public static class Prism
    {
        public static Prism<S, A> ToSimple<S, A>(this Prism<S, S, A, A> p)
        =>  Mk_<S, A>(p.Get, p.Re.View, p.Update);

        public static Prism<S, T, X, Y> CompPP<S, T, A, B, X, Y>
          ( this Prism<S, T, A, B> p0
          , Prism<A, B, X, Y> p1
          )
        =>  new APrism<S, T, X, Y>
              ( s =>
                    p0.Get(s)
                    .FlatMap(a => p1.Get(a)
                    .Cata
                      ( b => Or.Left<T, X>(p0.Re.View(b))
                      , x => Or.Right<T, X>(x))
                      )
              , y => p0.Re.View(p1.Re.View(y))
              , f => p0.Update(p1.Update(f))
              );

        public static Prism<S, X> CompPP_<S, A, X>
          ( this Prism<S, A> p0
          , Prism<A, X> p1
          )
        =>  p0.CompPP(p1).ToSimple();

        public static Prism<S, T, X, Y> CompPI<S, T, A, B, X, Y>
          ( this Prism<S, T, A, B> p
          , Iso<A, B, X, Y> i
          )
        =>  Mk<S, T, X, Y>
            ( s => p.Get(s).Map(i.There)
            , y => p.Re.View(i.Back(y))
            , xy => p.Update(a => i.Back(xy(i.There(a))))
            );

        public static Prism<S, X> CompPI_<S, A, X>
          ( this Prism<S, A> p
          , Iso<A, X> i
          )
        =>  p.CompPI(i).ToSimple();

        public static Prism<S, T, X, Y> CompIP<S, T, A, B, X, Y>
          ( this Iso<S, T, A, B> i
          , Prism<A, B, X, Y> p
          )
        =>  Mk<S, T, X, Y>
            ( s =>
                  p.Get(i.There(s))
                  .Cata(b => Or.Left<T, X>(i.Back(b)), Or.Right<T, X>)
            , y => i.Back(p.Re.View(y))
            , f => s => i.Back(p.Update(f)(i.There(s)))
            );

        public static Prism<S, X> CompIP_<S, A, X>
          ( this Iso<S, A> i
          , Prism<A, X> p
          )
        =>  i.CompIP(p).ToSimple();

        public static Prism<And<E, S>, And<E, T>, And<E, A>, And<E, B>>
        Aside<S, T, A, B, E>
          ( this Prism<S, T, A, B> p
          )
        =>  Mk<And<E, S>, And<E, T>, And<E, A>, And<E, B>>
              ( es => es.Args
                  ( (e, s) =>
                        p.Get(s).Cata
                          ( t => Or.Left<And<E, T>, And<E, A>>(And.Mk(e, t))
                          , a => Or.Right<And<E, T>, And<E, A>>(And.Mk(e, a))
                          )
                  )
              , eb => eb.MapSnd(p.Re.View)
              , f => es =>
                    And.Mk
                      ( es.Fst
                      , p.Update(a => f(And.Mk(es.Fst, a)).Snd)(es.Snd)
                      )
              );

        public static Prism<And<E, S>, And<E, A>>
        Aside_<S, A, E>
          ( this Prism<S, A> p
          )
        =>  Aside<S, S, A, A, E>(p).ToSimple();

        public static Prism<S, T, A, B> Mk<S, T, A, B>
        ( Func<S, Or<T, A>> get
        , Func<B, T> review
        , Func<Func<A, B>, Func<S, T>> update = null
        )
        =>  new APrism<S, T, A, B>
            ( get
            , review
            , update ?? DefaultUpdate(get, review)
            );

        public static Prism<S, A> Mk_<S, A>
        ( Func<S, Or<S, A>> get
        , Func<A, S> review
        , Func<Func<A, A>, Func<S, S>> update = null
        )
        =>  new APrism<S, A>
            ( get
            , review
            , update ?? DefaultUpdate(get, review)
            );

        public static Prism<S, A> Mk__<S, A>
        ( Func<S, Maybe<A>> get
        , Func<A, S> review
        , Func<Func<A, A>, Func<S, S>> update = null
        )
        =>  Mk_
            ( s => get(s).ToOr(() => s)
            , review
            , update
            );

        private static Func<Func<A, B>, Func<S, T>> DefaultUpdate<S, T, A, B>
          ( Func<S, Or<T, A>> get
          , Func<B, T> review
          )
        => f => s => get(s).LeftValueOr(a => review(f(a)));

        private struct APrism<S, T, A, B>
          : Prism<S, T, A, B>
        {
            private readonly Func<S, Or<T, A>> get;
            private readonly Func<B, T> review;
            private readonly Func<Func<A, B>, Func<S, T>> update;

            internal APrism
            ( Func<S, Or<T, A>> get
            , Func<B, T> review
            , Func<Func<A, B>, Func<S, T>> update
            )
            {
                this.get = get;
                this.review = review;
                this.update = update;
            }

            public Or<T, A> Get(S s)
            =>  get(s);

            public Func<S, T> Update(Func<A, B> f)
            =>  update(f);

            public Maybe<A> Preview(S s)
            =>  Get(s).Cata(_ => Maybe.Nothing<A>(), Maybe.Just);

            public Getter<B, T> Re
            =>  Getter.Mk(review);
        }

        private struct APrism<S, A>
          : Prism<S, A>
          , Setter<S, A>
        {
            private readonly Func<S, Or<S, A>> get;
            private readonly Func<A, S> review;
            private readonly Func<Func<A, A>, Func<S, S>> update;

            internal APrism
              ( Func<S, Or<S, A>> get
              , Func<A, S> review
              , Func<Func<A, A>, Func<S, S>> update
              )
            {   this.get = get;
                this.review = review;
                this.update = update;
            }

            public Or<S, A> Get(S s)
            =>  get(s);

            public Func<S, S> Update(Func<A, A> f)
            =>  update(f);

            public Maybe<A> Preview(S s)
            =>  Get(s).Cata(_ => Maybe.Nothing<A>(), Maybe.Just);

            public Getter<A, S> Re
            =>  Getter.Mk(review);
        }
    }
}
