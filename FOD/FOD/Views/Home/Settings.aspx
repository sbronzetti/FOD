<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FOD.Models.SettingsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Flight Offers Daily - Settings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Settings</h2>
    



    <% using (Html.BeginForm()) { %>
        
         <div class="editor-label">
                    <%: Html.LabelFor(m => m.Airports) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Airports) %>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.DateMin) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.DateMin) %>
                   
                </div>
        
        <div class="editor-label">
                    <%: Html.LabelFor(m => m.DateRange) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.DateRange) %>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ForceSearch) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.ForceSearch) %>
                   
                </div>


                <div class="editor-label">
                    <%: Html.LabelFor(m => m.MaxTrip) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.MaxTrip)%>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.MinTrip) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.MinTrip)%>
                   
                </div>
        
        <div class="editor-label">
                    <%: Html.LabelFor(m => m.Price) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Price)%>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.RefreshAirports) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.RefreshAirports)%>
                   
                </div>
          <div class="editor-label">
                    <%: Html.LabelFor(m => m.SearchCountries) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.SearchCountries)%>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.SendDailyReport) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.SendDailyReport)%>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.StepProcess) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.StepProcess)%>
                   
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.gpl) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.gpl)%>
                   
                </div>

        <input type="submit" value="Save" />
    <% } %>

</asp:Content>
