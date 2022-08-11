using HtmlAgilityPack;

public class Program
{
    static void Main() => new Program().MainAsync().GetAwaiter().GetResult();
    public async Task MainAsync()
    {
        const string faqLocation = @"http://www.sjgames.com/gurps/books/";
        using var client = new HttpClient();
        var response = client.GetAsync(faqLocation).Result;
        var pageDocument = new HtmlDocument();
        pageDocument.LoadHtml(await response.Content.ReadAsStringAsync());
        if (pageDocument?.DocumentNode != null)
        {
            var things = pageDocument.DocumentNode.
                SelectNodes(@"//td[@class=""pagemainpane""]/div[@class=""wblist""]").
                FirstOrDefault().
                ChildNodes.
                Where(cn => cn.Name.ToUpperInvariant() != "H3" && !(string.IsNullOrWhiteSpace(cn.InnerText) && cn.Name.ToUpperInvariant() != "BR")).
                ToArray();
            var breakDown = new List<List<HtmlNode>>();
            List<HtmlNode> currentList = null;
            foreach (var thing in things)
            {
                
                if(thing.Name.ToUpperInvariant() != "BR")
                {
                    if (currentList == null) currentList = new List<HtmlNode>();
                    currentList.Add(thing);
                }
                else if(currentList != null)
                {
                    breakDown.Add(currentList);
                    currentList = null;
                }
            }
            foreach(var thing in breakDown)
            {
                Console.WriteLine(thing.Aggregate<HtmlNode, string>(String.Empty, (s, h) => s + h.InnerText + " "));
            }
            var first = breakDown.FirstOrDefault();
        }


    }
}