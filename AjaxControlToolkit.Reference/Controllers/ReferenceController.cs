﻿using AjaxControlToolkit.Reference.Core;
using AjaxControlToolkit.Reference.Core.Parsing;
using AjaxControlToolkit.Reference.Core.Razor;
using AjaxControlToolkit.Reference.Core.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Web.UI.WebControls;

namespace AjaxControlToolkit.Reference.Controllers {

    public class ReferenceController : Controller {
        const string ActNamespace = "AjaxControlToolkit";

        static Lazy<PageTemplateBase<TypeDoc>> _templateLazy = new Lazy<PageTemplateBase<TypeDoc>>(CreateTemplate);
        static PageTemplateBase<TypeDoc> Template {
            get { return _templateLazy.Value; }
        }

        static PageTemplateBase<TypeDoc> CreateTemplate() {
            var context = System.Web.HttpContext.Current;

            var rootDir = Path.Combine(
                Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName,
                "AjaxControlToolkit.Reference"
            );

            var engine = new Engine(rootDir);
#warning hardcoded!
            return engine.CreateTemplateInstance<TypeDoc>("~/Views/Reference/Type.cshtml");
        }

        public ActionResult Index() {
            var typeNames = new string[] {
                "AccordionExtender",
                "AjaxFileUpload",
                "AlwaysVisibleControlExtender",
                "Animation",
                "AreaChart",
                "AsyncFileUpload",
                "AutoCompleteExtender",
                "BalloonPopupExtender",
                "BarChart",
                "BubbleChart",
                "CalendarExtender",
                "CascadingDropDown",
                "CollapsiblePanelExtender",
                "ColorPickerExtender",
                "ComboBox",
                "ConfirmButtonExtender",
                "DragPanelExtender",
                "DropDownExtender",
                "DropShadowExtender",
                "DynamicPopulateExtender",
                "FilteredTextBoxExtender",
                "Gravatar",
                "HoverMenuExtender",
                "HtmlEditorExtender",
                "LineChart",
                "ListSearchExtender",
                "MaskedEditExtender",
                "ModalPopupExtender",
                "MultiHandleSliderExtender",
                "MutuallyExclusiveCheckBoxExtender",
                "NoBot",
                "NumericUpDownExtender",
                "PagingBulletedListExtender",
                "PasswordStrength",
                "PieChart",
                "PopupControlExtender",
                "Rating",
                "ReorderList",
                "ResizableControlExtender",
                "RoundedCornersExtender",
                "Seadragon",
                "SliderExtender",
                "SlideShowExtender",
                "TabPanel",
                "TextBoxWatermarkExtender",
                "ToggleButtonExtender",
                "Twitter",
                "UpdatePanelAnimationExtender",
                "ValidatorCalloutExtender"
            };
            var doc = GetDoc();

            var types = doc.Types.Where(t => typeNames.Contains(t.Name)).Select(t => t.Name);

            return View(types);
        }

        public ContentResult Type(string typeName) {
            var doc = GetDoc();

            var typeFullName = ActNamespace + "." + typeName;
            FillClientMembers(doc, typeFullName);

            var types = doc.Types.FirstOrDefault(t => t.Name == typeName);

            return Content(Template.Render(types));
        }

        public ContentResult Markup() {
            var doc = GetDoc();

            foreach(var docType in doc.Types) {
                var typeFullName = docType.Namespace + "." + docType.Name;
                FillClientMembers(doc, typeFullName);
            }

            var codeplexDocRenderer = new CodePlexDocRenderer();
            var extenderDoc = new ExtenderDoc(codeplexDocRenderer);
            var markup = extenderDoc.BuildDoc(doc.Types);
            var wikiEngine = new WikiPlex.WikiEngine();

            return Content(wikiEngine.Render(markup));
        }

        void FillClientMembers(Documentation doc, string typeFullName) {
            var actAssembly = typeof(ToolkitResourceManager).Assembly;
            var type = actAssembly.GetType(typeFullName, true);

            if(type.BaseType == typeof(ExtenderControlBase)) {
                var clientScriptName = type
                    .CustomAttributes
                    .First(a => a.AttributeType.Name == "ClientScriptResourceAttribute")
                    .ConstructorArguments[1];
                var jsFileName = clientScriptName.Value + ".js";

                var jsLines = System.IO.File.ReadAllLines(Server.MapPath("~/bin/Scripts/" + jsFileName));
                var commentParser = new CommentParser();
                var clientMembers = commentParser.ParseFile(jsLines);

                doc.Add(clientMembers);
            }
        }

        Documentation GetDoc() {
            var doc = new Documentation();
            var xml = LoadXml(Server.MapPath("~/bin/AjaxControltoolkit.xml"));

            var members = xml.Root.Element("members").Elements().Select(el => new RawDoc(el.Attribute("name").Value) {
                Elements = el.Elements()
            }).OrderBy(el => el.TargetFullName);

            doc.Add(members);
            return doc;
        }

        static XDocument LoadXml(string fileName) {
            XDocument xml;
            if(!System.IO.File.Exists(fileName))
                throw new ArgumentException(String.Format("File '{0}' not found", fileName), "fileName");

            xml = XDocument.Load(fileName);
            if(xml == null)
                throw new ArgumentException(String.Format("Unable to load XML from '{0}'", fileName), "fileName");

            return xml;
        }
    }

}
