using System;
using Sharper.C.Data;
using static Sharper.C.Data.Unit;

namespace Sharper.C.Control.Optics
{
    public static class Instances
    {
        public static Lens<A, A> IdentityLens<A>()
        =>  Lens.Mk_<A, A>(a => a, f => a => f(a));

        public static Iso<A, A> IdentityIso<A>()
        =>  Iso.Mk_<A, A>(a => a, a => a);

        public static Prism<Maybe<A>, Maybe<B>, A, B> _Just<A, B>()
        =>  Prism.Mk<Maybe<A>, Maybe<B>, A, B>
            ( ma =>
                  ma.Cata
                    ( () => Or.Left<Maybe<B>, A>(Maybe.Nothing<B>())
                    , Or.Right<Maybe<B>, A>
                    )
            , Maybe.Just
            , f => s => s.Map(f)
            );

        public static Prism<Maybe<A>, Unit> _Nothing<A>()
        =>  Prism.Mk_<Maybe<A>, Unit>
            ( ma =>
                  ma.Cata
                    ( () => Or.Right<Maybe<A>, Unit>(UNIT)
                    , _ => Or.Left<Maybe<A>, Unit>(Maybe.Nothing<A>())
                    )
            , _ => Maybe.Nothing<A>()
            , f => ma => ma
            );

        public static Prism<Or<A, X>, Or<B, X>, A, B> _Left<A, B, X>()
        =>  Prism.Mk<Or<A, X>, Or<B, X>, A, B>
            ( eax =>
                  eax.Cata
                    ( Or.Right<Or<B, X>, A>
                    , x => Or.Left<Or<B, X>, A>(Or.Right<B, X>(x))
                    )
            , Or.Left<B, X>
            , f => eax => eax.Cata(a => Or.Left<B, X>(f(a)), Or.Right<B, X>)
            );

        public static Prism<Or<X, A>, Or<X, B>, A, B> _Right<X, A, B>()
        =>  Prism.Mk<Or<X, A>, Or<X, B>, A, B>
            ( eax =>
                  eax.Cata
                    ( x => Or.Left<Or<X, B>, A>(Or.Left<X, B>(x))
                    , Or.Right<Or<X, B>, A>
                    )
            , Or.Right<X, B>
            , f => eax => eax.Map(f)
            );

        public static Prism<A, A> PrismFilter<A>(Func<A, bool> f)
        =>  Prism.Mk_<A, A>
              ( a => f(a) ? Or.Right<A, A>(a) : Or.Left<A, A>(a)
              , a => a
              );

        public static Iso<Maybe<A>, Maybe<B>, Or<E, A>, Or<E, B>>
        IsoMaybeSum<A, B, E>(Func<E> e)
        =>  Iso.Mk<Maybe<A>, Maybe<B>, Or<E, A>, Or<E, B>>
            ( ma => ma.Cata(() => Or.Left<E, A>(e()), Or.Right<E, A>)
            , eb => eb.Cata(_ => Maybe.Nothing<B>(), Maybe.Just)
            );

        public static Iso<Or<E, A>, Or<E, B>, Maybe<A>, Maybe<B>>
        IsoSumMaybe<A, B, E>(Func<E> e)
        =>  IsoMaybeSum<B, A, E>(e).From;

        public static Iso<And<A, B>, And<C, D>, And<B, A>, And<D, C>>
        IsoProductSwap<A, B, C, D>()
        =>  Iso.Mk<And<A, B>, And<C, D>, And<B, A>, And<D, C>>
              ( x => x.Swap
              , x => x.Swap
              );

        public static Iso<And<A, B>, And<B, A>> IsoProductSwap_<A, B>()
        =>  IsoProductSwap<A, B, A, B>().ToSimple();

        public static Iso<Or<A, B>, Or<C, D>, Or<B, A>, Or<D, C>>
        IsoSumSwap<A, B, C, D>()
        =>  Iso.Mk<Or<A, B>, Or<C, D>, Or<B, A>, Or<D, C>>
              ( x => x.Swap
              , x => x.Swap
              );

        public static Iso<Or<A, B>, Or<B, A>> IsoSumSwap_<A, B>()
        =>  IsoSumSwap<A, B, A, B>().ToSimple();
    }
}
