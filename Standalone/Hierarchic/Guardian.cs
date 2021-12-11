using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;

namespace BrandonUtils.Standalone.Hierarchic {
    [Obsolete("not thrilled")]
    public abstract class Guardian<TGuardian, TDependant> : IGuardian<TGuardian, TDependant> where TGuardian : Guardian<TGuardian, TDependant>
                                                                                             where TDependant : Dependant<TGuardian, TDependant> {
        private readonly List<TDependant> _dependants = new List<TDependant>();

        /// <summary>
        /// All of this <typeparamref name="TGuardian"/>'s <typeparamref name="TDependant"/>s.
        /// </summary>
        /// <remarks>
        /// This is a <see cref="ReadOnlyCollection{T}"/>, which (I think?) is a <i>copy</i> of the backing <see cref="List{T}"/>, <see cref="_dependants"/>.
        /// </remarks>
        public ReadOnlyCollection<TDependant> Dependants => _dependants.AsReadOnly();

        /// <summary>
        /// Creates non-transferable <a href="https://en.wikipedia.org/wiki/Filiation">filiation</a> between an <see cref="orphan"/> and a <see cref="Guardian{TGuardian,TDependant}"/>.
        /// </summary>
        /// <param name="orphan">a starving British child</param>
        /// <returns>this <see cref="Guardian{TGuardian,TDependant}"/>, with inversely proportional self-worth and taxes</returns>
        /// <exception cref="BrandonException">if the <see cref="orphan"/> has already been <see cref="Adopt">adopted</see> by a different <see cref="Dependant{TGuardian,TDependant}.Guardian"/></exception>
        public virtual TGuardian Adopt(TDependant orphan) {
            if (!ReferenceEquals(orphan.Guardian, this)) {
                ValidateGuardianship(orphan, $"Couldn't {nameof(Adopt)} the {nameof(orphan)}!");
            }

            if (!_dependants.Contains(orphan)) {
                _dependants.Add(orphan);
            }

            return (TGuardian)this;
        }

        /// <summary>
        /// Exiles a <see cref="pariah"/> from Athens <a href="https://en.wikipedia.org/wiki/Ostracism">for 10 years</a>.
        /// </summary>
        /// <param name="pariah"></param>
        /// <returns></returns>
        /// <exception cref="BrandonException">if the <see cref="pariah"/> isn't one of the <see cref="Dependants"/></exception>
        public virtual TGuardian Disown(TDependant pariah) {
            if (_dependants.Contains(pariah)) {
                _dependants.Remove(pariah);
            }
            else {
                ValidateGuardianship(pariah, $"Couldn't {nameof(Disown)} the {nameof(pariah)}!");
            }

            return (TGuardian)this;
        }

        public BrandonException ValidateGuardianship(TDependant dependant, string prefix = default, string suffix = default) {
            var msg = new[] { prefix, $"[{dependant.GetType().Name}]{dependant} isn't one of [{GetType().Name}]{this}'s {nameof(Dependants)}!", suffix, }.Where(it => !string.IsNullOrWhiteSpace(it))
                                                                                                                                                         .JoinString(" ");

            return new BrandonException(msg);
        }
    }
}