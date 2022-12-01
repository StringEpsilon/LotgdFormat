using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace LotgdFormat.Tests;

public static class IHtmlContentExtension {
	public static string GetString(this IHtmlContent content) {
		using var writer = new StringWriter();
		content.WriteTo(writer, HtmlEncoder.Default);
		return writer.ToString();
	}
}
