using System.Collections.ObjectModel;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Hierarchic {
    public interface IGuardian<out TGuardian, TDependant>
        where TGuardian : IGuardian<TGuardian, TDependant>
        where TDependant : IDependant<TGuardian, TDependant> {
        /// <summary>
        /// All of this <typeparamref name="TGuardian"/>'s <typeparamref name="TDependant"/>s.
        /// </summary>
        /// <remarks>
        /// This is a <see cref="ReadOnlyCollection{T}"/>, which (I think?) is a <i>copy</i> of the backing <see cref="List{T}"/>, <see cref="_dependants"/>.
        /// </remarks>
        ReadOnlyCollection<TDependant> Dependants { get; }

        /// <summary>
        /// Creates non-transferable <a href="https://en.wikipedia.org/wiki/Filiation">filiation</a> between an <see cref="orphan"/> and a <see cref="Guardian{TGuardian,TDependant}"/>.
        /// </summary>
        /// <param name="orphan">a starving British child</param>
        /// <returns>this <see cref="Guardian{TGuardian,TDependant}"/>, with inversely proportional self-worth and taxes</returns>
        /// <exception cref="BrandonException">if the <see cref="orphan"/> has already been <see cref="Guardian{TGuardian,TDependant}.Adopt">adopted</see> by a different <see cref="Dependant{TGuardian,TDependant}.Guardian"/></exception>
        TGuardian Adopt(TDependant orphan);

        /// <summary>
        /// Exiles a <see cref="pariah"/> from Athens <a href="https://en.wikipedia.org/wiki/Ostracism">for 10 years</a>.
        /// </summary>
        /// <param name="pariah"></param>
        /// <returns></returns>
        /// <exception cref="BrandonException">if the <see cref="pariah"/> isn't one of the <see cref="Guardian{TGuardian,TDependant}.Dependants"/></exception>
        TGuardian Disown(TDependant pariah);

        BrandonException ValidateGuardianship(TDependant dependant, string prefix = default, string suffix = default);
    }
}