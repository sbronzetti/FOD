<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Empty.Master" Inherits="System.Web.Mvc.ViewPage<FOD.Models.ProgressBarModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        setInterval('window.location.reload()', 60000) { alert("timeout") };
    </script>
    <div style="text-align:center">
    <h2 style="padding:0px 5px">Processing..</h2>
    <h2  style="padding:0px 5px"><%: Model.Done %>/<%: Model.Todo %></h2></div>
</asp:Content>
