namespace Ops.Template
{
    internal class test
    {
        private void Main()
        {
            TemplatePage tp = new TemplatePage();
            tp.TemplateContent = "$m=item  KEY:${item.key} Note:${item.note}<br />${m.key}";
            tp.AddVariable("item", new {Key = "Key", Note = "Note"});
            var x = System.Web.HttpContext.Current.Items;
            string y = tp.ToString();
            // return y;
        }
    }
}