﻿//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if FEATURE_COM
using static System.Runtime.InteropServices.Marshal;
#endif

namespace Moq
{
	/// <include file='It.xdoc' path='docs/doc[@for="It"]/*'/>
	public static class It
	{
		/// <summary>
		/// Contains matchers for <see langword="ref"/> (C#) / <see langword="ByRef"/> (VB.NET) parameters of type <typeparamref name="TValue"/>.
		/// </summary>
		/// <typeparam name="TValue">The parameter type.</typeparam>
		public static class Ref<TValue>
		{
			/// <summary>
			/// Matches any value that is assignment-compatible with type <typeparamref name="TValue"/>.
			/// </summary>
			public static TValue IsAny;
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsAny"]/*'/>
		public static TValue IsAny<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value == null || (typeof(TValue).IsAssignableFrom(value.GetType())
				                           || (IsComObject(value) && value is TValue)),
#else
				value => value == null || typeof(TValue).IsAssignableFrom(value.GetType()),
#endif
				() => It.IsAny<TValue>());
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotNull"]/*'/>
		public static TValue IsNotNull<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value != null && (typeof(TValue).IsAssignableFrom(value.GetType())
				                           || (IsComObject(value) && value is TValue)),
#else
				value => value != null && typeof(TValue).IsAssignableFrom(value.GetType()),
#endif
				() => It.IsNotNull<TValue>());
		}


		/// <include file='It.xdoc' path='docs/doc[@for="It.Is"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static TValue Is<TValue>(Expression<Func<TValue, bool>> match)
		{
			return Match<TValue>.Create(
				value => match.Compile().Invoke(value),
				() => It.Is<TValue>(match));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsInRange"]/*'/>
		public static TValue IsInRange<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			return Match<TValue>.Create(value =>
			{
				if (value == null)
				{
					return false;
				}

				if (rangeKind == Range.Exclusive)
				{
					return value.CompareTo(from) > 0 && value.CompareTo(to) < 0;
				}

				return value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
			},
			() => It.IsInRange(from, to, rangeKind));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsIn(enumerable)"]/*'/>
		public static TValue IsIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsIn(params)"]/*'/>
		public static TValue IsIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotIn(enumerable)"]/*'/>
		public static TValue IsNotIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotIn(params)"]/*'/>
		public static TValue IsNotIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex)"]/*'/>
		public static string IsRegex(string regex)
		{
			Guard.NotNull(regex, nameof(regex));

			// The regex is constructed only once.
			var re = new Regex(regex);

			// But evaluated every time :)
			return Match<string>.Create(value => value != null && re.IsMatch(value), () => It.IsRegex(regex));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex,options)"]/*'/>
		public static string IsRegex(string regex, RegexOptions options)
		{
			Guard.NotNull(regex, nameof(regex));

			// The regex is constructed only once.
			var re = new Regex(regex, options);

			// But evaluated every time :)
			return Match<string>.Create(value => value != null && re.IsMatch(value), () => It.IsRegex(regex, options));
		}
	}
}
