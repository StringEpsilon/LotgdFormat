using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

public interface INode {
	public IHtmlContent Render();
}
