using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgilityHtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WebbrowserXPathExtend
{
    /// <summary>
    /// 使用XPath查找Webbrowser元素的扩展类
    /// 基于 https://github.com/ffMathy/xpath-webbrowser
    /// 要保证页面加载完毕使用，防止因Html元素动态加载造成结果错误
    /// 因为使用了innerHtml，所以XPath表达式不需要查询body
    /// </summary>
    public static class WebbrowserXPathExtend
    {
        /// <summary>
        /// 使用XPath表达式查找元素
        /// </summary>
        /// <param name="htmlElement"></param>
        /// <param name="xPathExpress">XPath表达式</param>
        /// <returns>查询结果集合</returns>
        public static IEnumerable<HtmlElement> FindElements(this HtmlElement htmlElement, string xPathExpress)
        {
            var htmlDocument = new AgilityHtmlDocument();
            htmlDocument.LoadHtml(htmlElement.InnerHtml);

            var documentNode = htmlDocument.DocumentNode;
            if (documentNode != null)
            {
                var nodes = documentNode.SelectNodes(xPathExpress);
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        var equivalent = htmlElement.FindFromNode(node);
                        yield return equivalent;
                    }
                }
            }
        }

        /// <summary>
        /// 查找HtmlNode对应的HtmlElemnet
        /// </summary>
        /// <param name="htmlElement"></param>
        /// <param name="node">要查找元素的HtmlNode</param>
        /// <returns></returns>
        private static HtmlElement FindFromNode(this HtmlElement htmlElement, HtmlNode node)
        {
            var parent = node.ParentNode;

            var childNodesWithSameType =
                parent.ChildNodes.Where(n => string.Equals(n.Name, node.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            var parentOffset = childNodesWithSameType.IndexOf(node);

            HtmlElement parentResult;
            if (string.Equals(parent.Name, "#document", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(htmlElement.Document != null, "Document != null");
                parentResult = htmlElement;
            }
            else
            {
                parentResult = FindFromNode(htmlElement, parent);
            }

            Debug.Assert(parentResult != null, "parentResult != null");

            var childElementsWithSameType = parentResult
                .Children
                .Cast<HtmlElement>()
                .Where(e => string.Equals(e.TagName, node.Name, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            return childElementsWithSameType[parentOffset];
        }
    }
}
