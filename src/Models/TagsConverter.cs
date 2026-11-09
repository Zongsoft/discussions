/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (c) Zongsoft Studio. All rights reserved.
 *
 * Licensed under the MIT License (see LICENSE).
 */

using System;
using System.Globalization;
using System.ComponentModel;

namespace Zongsoft.Discussions.Models;

public class TagsConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if(sourceType == typeof(string))
			return true;

		return base.CanConvertFrom(context, sourceType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if(destinationType == typeof(string))
			return true;

		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if(value == null)
			return null;

		if(value is string text)
			return Utility.GetTags(text);

		if(value is string[] items)
			return items;

		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if(value == null)
			return null;

		if(value is string text)
			return string.IsNullOrEmpty(text) ? null : value;

		if(value is string[] items)
		{
			if(items == null || items.Length == 0)
				return null;
			else
				return string.Join(",", items);
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}
}
