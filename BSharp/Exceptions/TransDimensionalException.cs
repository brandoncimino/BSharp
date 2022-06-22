using System;

namespace FowlFever.BSharp.Exceptions {
    /// <summary>
    /// An <see cref="System.Exception"/> to be thrown at corporeal ontology, which will hit it from behind.
    /// </summary>
    /// <remarks>
    /// Throw a <see cref="TransDimensionalException"/> when an incorrect <see cref="UnityEngine.RectTransform.Axis"/> is accessed, or when the volume of a <see cref="UnityEngine.Collider"/> is negative, etc.
    /// </remarks>
    public class TransDimensionalException : BrandonException {
        protected override string BaseMessage { get; } = "You will kick Descartes in the junk; and Euclid said 'ow'.";
        public TransDimensionalException() : base() { }
        public TransDimensionalException(string message, Exception? innerException = null) : base(message, innerException) { }
    }
}