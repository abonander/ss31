using System;

namespace SS31.Common.Network
{
	// The flag at the beginning of Data messages, telling what kind of message it is.
	public enum NetMessageType :
		byte
	{
		PlainText = 0 // Testing message for now
	}
}