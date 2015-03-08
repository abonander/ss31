using System;

// Contains the specialized exceptions for this library.
namespace SS31.Common
{
	// There is no public constructor for the given type, and it needs one to be activated.
	public sealed class NoPublicConstructorException : Exception
	{
		public readonly Type Type;

		public override string Message
		{
			get
			{
				return "The type " + Type + " does not have an invokable public constructor.";
			}
		}

		public NoPublicConstructorException(Type type)
		{
			Type = type;
		}
	}

	// There is no public constructor that matches the argument list, in most cases this will be thrown for
	// classes that dont have an empty constructor.
	public sealed class InvalidConstructorParameterException : Exception
	{
		public InvalidConstructorParameterException(string message) :
			base(message)
		{

		}
	}
}