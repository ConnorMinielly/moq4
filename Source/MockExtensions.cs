﻿using System.ComponentModel;

namespace Moq
{

	/// <summary>
	/// Provides additional methods on mocks.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockExtensions
	{
		/// <summary>
		/// Resets all invocations recorded for this mock.
		/// </summary>
		/// <param name="mock">The mock whose recorded invocations should be reset.</param>
		public static void ResetCalls(this Mock mock)
		{
			mock.Invocations.Clear();
		}

		/// <summary>
		/// Resets this mock's state. This includes its setups, configured default return values, registered event handlers, and all recorded invocations.
		/// </summary>
		/// <param name="mock">The mock whose state should be reset.</param>
		public static void Reset(this Mock mock)
		{
			mock.ConfiguredDefaultValues.Clear();
			mock.Setups.Clear();
			mock.EventHandlers.Clear();
			mock.ResetCalls();
		}
	}
}
