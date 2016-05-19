using Sharper.C.Data;
using static Sharper.C.Data.UnitModule;

namespace Sharper.C.Control.Optics
{
    public static class Instances
    {
        public static Lens<A, A> IdentityLens<A>()
        =>  Lens.Mk<A, A>(a => a, f => a => f(a));

        public static Iso<A, A> IdentityIso<A>()
        =>  Iso.Mk<A, A>(a => a, a => a);

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

        public static Prism<Maybe<A>, Maybe<A>, Unit, Unit> _Nothing<A>()
        =>  Prism.Mk<Maybe<A>, Maybe<A>, Unit, Unit>
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

        public static Iso<Maybe<A>, Maybe<B>, Or<Unit, A>, Or<Unit, B>>
        IsoMaybeSum<A, B>()
        =>  Iso.Mk<Maybe<A>, Maybe<B>, Or<Unit, A>, Or<Unit, B>>
            ( ma => ma.Cata(() => Or.Left<Unit, A>(UNIT), Or.Right<Unit, A>)
            , eb => eb.Cata(_ => Maybe.Nothing<B>(), Maybe.Just)
            );

        public static Iso<Or<Unit, A>, Or<Unit, B>, Maybe<A>, Maybe<B>>
        IsoSumMaybe<A, B>()
        =>  IsoMaybeSum<B, A>().From;
    }
}
