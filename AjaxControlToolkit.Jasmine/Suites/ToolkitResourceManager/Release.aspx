﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Release.aspx.cs" Inherits="AjaxControlToolkit.Jasmine.Suites.ToolkitResourceManager.ToolkitResourceManager" %>

<%@ Register Assembly="AjaxControlToolkit.Jasmine" Namespace="AjaxControlToolkit.Jasmine.Suites.ToolkitResourceManager" TagPrefix="test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body onload="onLoad()">
    <form runat="server">
        <asp:ScriptManager runat="server" ScriptMode="Release" EnableCdn="true" />

        <asp:TextBox runat="server" ID="Target" />
        <test:TestExtenderControl runat="server" TargetControlID="Target" />
    </form>
</body>
</html>

<script>
    function onLoad() {
        parent.Testing.TestExtenderBehavior = Sys.Extended.UI.TestExtenderBehavior;
        parent.Testing.LoadSpecCallback();
    }
</script>

