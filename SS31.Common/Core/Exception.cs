using System;

namespace SS31.Common
{
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

	public sealed class InvalidConstructorParameterException : Exception
	{
		public InvalidConstructorParameterException(string message) :
			base(message)
		{

		}
	}
}