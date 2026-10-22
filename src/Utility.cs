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
 * Copyright (C) 2015-2025 Zongsoft Corporation. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.IO;
using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Security;
using Zongsoft.Configuration;

using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions;

internal static class Utility
{
	#region 常量定义
	private const string CONTENT_TYPE_EMBEDDED_SUFFIX = "+embedded";
	#endregion

	#region 内容处理
	public static bool IsContentEmbedded(string contentType)
	{
		if(string.IsNullOrEmpty(contentType))
			return true;

		return contentType.TrimEnd().EndsWith(CONTENT_TYPE_EMBEDDED_SUFFIX, StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsContentFile(string contentType)
	{
		if(string.IsNullOrEmpty(contentType))
			return false;

		return !contentType.TrimEnd().EndsWith(CONTENT_TYPE_EMBEDDED_SUFFIX, StringComparison.OrdinalIgnoreCase);
	}

	public static string GetContentType(string contentType, bool embedded)
	{
		if(string.IsNullOrWhiteSpace(contentType))
			contentType = "text/plain";
		else
			contentType = contentType.Trim();

		if(embedded)
		{
			if(contentType.EndsWith(CONTENT_TYPE_EMBEDDED_SUFFIX, StringComparison.OrdinalIgnoreCase))
				return contentType;
			else
				return contentType + CONTENT_TYPE_EMBEDDED_SUFFIX;
		}
		else
		{
			if(contentType.EndsWith(CONTENT_TYPE_EMBEDDED_SUFFIX, StringComparison.OrdinalIgnoreCase))
				return contentType.Substring(0, contentType.Length - CONTENT_TYPE_EMBEDDED_SUFFIX.Length);
			else
				return contentType;
		}
	}

	public static async ValueTask<string> SetContentAsync(IDataDictionary data, Func<string> getFilePath, CancellationToken cancellation)
	{
		string content = null;
		if(!data.TryGetValue<string>("Content", value => content = value))
			return string.Empty;

		var rawType = data.GetValue<string>("ContentType", null);

		if(string.IsNullOrEmpty(content) || content.Length < 500)
		{
			data.TrySetValue("ContentType", GetContentType(rawType, true));
			return string.Empty;
		}

		var filePath = getFilePath();

		try
		{
			await WriteTextFileAsync(filePath, content, cancellation);
		}
		catch
		{
			await DeleteFileAsync(filePath, CancellationToken.None);
			throw;
		}

		data.SetValue("Content", filePath);
		data.SetValue("ContentType", GetContentType(rawType, false));
		return filePath;
	}

	public static async ValueTask<int> MutateContentAsync(IDataDictionary data, Func<string> getFilePath, Func<ValueTask<int>> mutate, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		var filePath = await SetContentAsync(data, getFilePath, cancellation);

		try
		{
			var count = await mutate();
			if(count > 0)
				filePath = null;
			return count;
		}
		finally
		{
			if(!string.IsNullOrEmpty(filePath))
				await DeleteFileAsync(filePath, CancellationToken.None);
		}
	}

	public static async ValueTask<bool> DeleteContentFileAsync(IDataDictionary data, CancellationToken cancellation)
	{
		if(data.TryGetValue<string>("ContentType", out var contentType) && IsContentFile(contentType) &&
		   data.TryGetValue<string>("Content", out var content))
		{
			return await DeleteFileAsync(content, cancellation);
		}

		return false;
	}

	public static string[] GetTags(string text)
	{
		if(string.IsNullOrEmpty(text))
			return Array.Empty<string>();

		int position = -1;
		var list = new List<string>();

		for(int i = 0; i < text.Length; i++)
		{
			if(text[i] == ',' || text[i] == ';')
			{
				if(i - position > 1)
					list.Add(text.Substring(position + 1, i - position - 1));

				position = i;
			}
		}

		if(position < text.Length - 1)
			list.Add(text.Substring(position + 1));

		if(list.Count > 0)
			return list.ToArray();
		else
			return Array.Empty<string>();
	}
	#endregion

	#region 文件操作
	public static async ValueTask<bool> DeleteFileAsync(string path, CancellationToken cancellation)
	{
		if(string.IsNullOrWhiteSpace(path))
			return false;

		try
		{
			return await FileSystem.File.DeleteAsync(path, cancellation);
		}
		catch
		{
			return false;
		}
	}

	public static async ValueTask<string> ReadTextFileAsync(string path, CancellationToken cancellation)
	{
		if(string.IsNullOrWhiteSpace(path))
			throw new ArgumentNullException(nameof(path));

		try
		{
			if(!await FileSystem.File.ExistsAsync(path, cancellation))
				return string.Empty;

			await using var stream = await FileSystem.File.OpenAsync(path, FileMode.Open, FileAccess.Read, cancellation);
			using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true, 1024, true);
			return await reader.ReadToEndAsync(cancellation);
		}
		catch(OperationCanceledException) when(cancellation.IsCancellationRequested)
		{
			throw;
		}
		catch
		{
			return string.Empty;
		}
	}

	public static async ValueTask<bool> WriteTextFileAsync(string path, string content, CancellationToken cancellation)
	{
		if(string.IsNullOrWhiteSpace(path))
			throw new ArgumentNullException(nameof(path));

		if(string.IsNullOrWhiteSpace(content))
			return false;

		await using var stream = await FileSystem.File.OpenAsync(path, FileMode.Create, FileAccess.Write, cancellation);
		await using var writer = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, true);
		await writer.WriteAsync(content.AsMemory(), cancellation);
		return true;
	}

	public static string GetFilePath(string relativePath = null) => GetFilePath(0, 0, relativePath);
	public static string GetFilePath(uint siteId, string relativePath = null) => GetFilePath(siteId, 0, relativePath);

	/// <summary>获取特定规则的文件存储路径。</summary>
	/// <param name="siteId">指定的站点编号，零表示当前用户所在的站点。</param>
	/// <param name="userId">指定的用户编号，零表示当前用户。</param>
	/// <param name="relativePath">相对路径，如果以斜杠符“/”打头表示忽略编号为零的层级。</param>
	/// <returns>返回的文件存储路径。</returns>
	/// <remarks>
	///		<list type="list">
	///			<item>根目录下的指定子目录：<code>GetFilePath(0, 0, "/apps");</code></item>
	///
	///			<item>指定站点下的子目录：<code>GetFilePath(1, 0, "/sub");</code></item>
	///			<item>指定站点下的当前用户子目录：<code>GetFilePath(1, 0, "sub");</code></item>
	///			<item>指定站点下的指定用户子目录：<code>GetFilePath(1, 100, "sub");</code></item>
	///
	///			<item>指定用户下的子目录：<code>GetFilePath(0, 100, "/sub");</code></item>
	///			<item>当前站点下的当前用户子目录：<code>GetFilePath(0, 0, "sub");</code></item>
	///			<item>当前站点下的指定用户子目录：<code>GetFilePath(0, 100, "sub");</code></item>
	///		</list>
	/// </remarks>
	public static string GetFilePath(uint siteId, uint userId, string relativePath = null)
	{
		var basePath = ApplicationContext.Current.Configuration.GetOptionValue<string>("/Discussions/General:BasePath");

		if(string.IsNullOrWhiteSpace(basePath))
			return string.Empty;

		if(relativePath != null && relativePath.StartsWith("/"))
		{
			relativePath = relativePath.TrimStart('/');

			if(siteId == 0)
			{
				if(userId == 0)
					return Zongsoft.IO.Path.Combine(basePath, relativePath);
				else
					return Zongsoft.IO.Path.Combine(basePath, "user-" + userId.ToString(), relativePath);
			}
			else
			{
				if(userId == 0)
					return Zongsoft.IO.Path.Combine(basePath, "site-" + siteId.ToString(), relativePath);
				else
					return Zongsoft.IO.Path.Combine(basePath, "site-" + siteId.ToString(), "user-" + userId.ToString(), relativePath);
			}
		}

		if(siteId == 0 || userId == 0)
		{
			var principal = ApplicationContext.Current.Principal as Zongsoft.Security.CredentialPrincipal;

			if(principal != null && principal.Identity.IsAuthenticated)
			{
				if(userId == 0)
					userId = principal.Identity.GetIdentifier<uint>();

				if(siteId == 0)
				{
					if(principal.Identity.TryGetClaim<uint>(nameof(UserProfile.SiteId), out var value))
						siteId = value;
				}
			}
		}

		if(siteId == 0)
		{
			if(userId == 0)
				return Zongsoft.IO.Path.Combine(basePath, "anonymous", relativePath);
			else
				return Zongsoft.IO.Path.Combine(basePath, "user-" + userId.ToString(), relativePath);
		}
		else
		{
			if(userId == 0)
				return Zongsoft.IO.Path.Combine(basePath, "site-" + siteId.ToString(), relativePath);
			else
				return Zongsoft.IO.Path.Combine(basePath, "site-" + siteId.ToString(), "user-" + userId.ToString(), relativePath);
		}
	}
	#endregion
}
